using CAMS.Application.Attendance;
using CAMS.Application.Authentication;
using CAMS.Application.Authorization;
using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Realtime;
using CAMS.Application.Common.Security;
using CAMS.Application.Dashboard;
using CAMS.Application.Event;
using CAMS.Application.EventSchedule;
using CAMS.Application.Fingerprint;
using CAMS.Application.Member;
using CAMS.Application.Registration;
using CAMS.Application.Report;
using CAMS.Application.Settings;
using CAMS.Application.User;
using CAMS.Infrastructure.Common.Clocking;
using CAMS.Infrastructure.Data;
using CAMS.Infrastructure.Data.Repositories;
using CAMS.Infrastructure.Data.Seeders;
using CAMS.Infrastructure.Fingerprint;
using CAMS.Infrastructure.Identity;
using CAMS.Infrastructure.Reporting;
using CAMS.Infrastructure.Repositories;
using CAMS.Web.BackgroundServices;
using CAMS.Web.Extensions;
using CAMS.Web.Hubs;
using CAMS.Web.Middlewares;
using CAMS.Web.Realtime;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add configuration settings
builder.Services.Configure<RegistrationSettings>(builder.Configuration.GetSection("Registration"));
builder.Services.Configure<EventGenerationSettings>(builder.Configuration.GetSection("EventGeneration"));
builder.Services.Configure<AttendanceSettings>(builder.Configuration.GetSection("Attendance"));

// Licenses
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add identity services
builder.Services
	.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
	{
		options.Lockout.AllowedForNewUsers = true;
		options.Lockout.MaxFailedAccessAttempts = 5;
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireDigit = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireUppercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.User.RequireUniqueEmail = false;
	})
	.AddEntityFrameworkStores<CAMSDBContext>()
	.AddDefaultTokenProviders();

builder.Services.AddCamsAuthentication();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

// Add authentication services
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
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventScheduleService, EventScheduleService>();
builder.Services.AddScoped<IEventGenerationService, EventGenerationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttendanceQrService, AttendanceQrService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped< IEventStatusService, EventStatusService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportExportService, ReportExportService>();

// Add repositories
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IRegistrationRequestRepository, RegistrationRequestRepository>();
builder.Services.AddScoped<IEventScheduleRepository, EventScheduleRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IAttendanceQrSessionRepository, AttendanceQrSessionRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Add DbContext
builder.Services.AddDbContext<CAMSDBContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CAMSDBContext>());

// Add hosted services
builder.Services.AddHostedService<EventGenerationHostedService>();
builder.Services.AddHostedService<AttendanceQrRefreshHostedService>();
builder.Services.AddHostedService<EventStatusRefreshHostedService>();

// Add others
builder.Services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddSingleton<IApplicationClock, ApplicationClock>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAttendanceNotifier, AttendanceNotifier>();
builder.Services.AddSingleton<IAttendanceQrSubscriptionRegistry, AttendanceQrSubscriptionRegistry>();
builder.Services.AddSingleton<IAttendanceWindowService, AttendanceWindowService>();
builder.Services.AddScoped<IFingerprintTemplateProtector, FingerprintTemplateProtector>();

builder.Services.AddCamsRateLimiting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider
		.GetRequiredService<CAMSDBContext>();

	dbContext.Database.Migrate();
	await EventScheduleSeeder.SeedAsync(
		dbContext);
	await IdentitySeeder.SeedAsync(
		scope.ServiceProvider);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else 
{
	app.UseDeveloperExceptionPage();
}

app.Map("/error", (HttpContext context) =>
{
	var exceptionFeature =
		context.Features.Get<IExceptionHandlerPathFeature>();

	var exception = exceptionFeature?.Error;

	return Results.Problem(
		title: "An unexpected error occurred.",
		
		// UPDATE THIS BEFORE GOING LIVE
		//detail: exception?.Message,
		detail: exception?.ToString(),
		statusCode: StatusCodes.Status500InternalServerError);
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseMiddleware<ActiveUserMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapHub<AttendanceHub>("/hubs/attendance");

app.Run();
