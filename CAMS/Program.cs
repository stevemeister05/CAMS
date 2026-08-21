using CAMS.Application.Authentication;
using CAMS.Application.Authorization;
using CAMS.Application.Common;
using CAMS.Application.Common.Security;
using CAMS.Application.Event;
using CAMS.Application.EventSchedule;
using CAMS.Application.Member;
using CAMS.Application.Registration;
using CAMS.Application.Settings;
using CAMS.Application.User;
using CAMS.Infrastructure.Data;
using CAMS.Infrastructure.Data.Seeders;
using CAMS.Infrastructure.Identity;
using CAMS.Infrastructure.Repositories;
using CAMS.Web.BackgroundServices;
using CAMS.Web.Extensions;
using CAMS.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RegistrationSettings>(builder.Configuration.GetSection("Registration"));
builder.Services.Configure<EventGenerationSettings>(builder.Configuration.GetSection("EventGeneration"));

builder.Services
	.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
	{
		options.Lockout.AllowedForNewUsers = true;
		options.Lockout.MaxFailedAccessAttempts = 5;
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

		options.User.RequireUniqueEmail = false;
	})
	.AddEntityFrameworkStores<CAMSDBContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(
		AuthorizationPolicies.PasswordChangeCompleted,
		policy =>
		{
			policy.RequireAuthenticatedUser();

			policy.AddRequirements(
				new PasswordChangeCompletedRequirement());
		});
});

builder.Services.AddScoped<IAuthorizationHandler, PasswordChangeCompletedHandler>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IEventScheduleService, EventScheduleService>();
builder.Services.AddScoped<IEventGenerationService, EventGenerationService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add repositories
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IRegistrationRequestRepository, RegistrationRequestRepository>();
builder.Services.AddScoped<IEventScheduleRepository, EventScheduleRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

// Add DbContext
builder.Services.AddDbContext<CAMSDBContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CAMSDBContext>());

// Add hosted services
builder.Services.AddHostedService<EventGenerationHostedService>();

// Add others
builder.Services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();

builder.Services.AddCamsRateLimiting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider
		.GetRequiredService<CAMSDBContext>();

	dbContext.Database.Migrate();
	await EventScheduleSeeder.SeedAsync(
		dbContext);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();


app.Run();
