using CAMS.Domain.Constants;
using CAMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Data.Seeders;

public static class IdentitySeeder
{
	private const string RootAdminUserName = "admin";

	private const string RootAdminPassword =
		"abc123";

	private const string AdministratorRole = Domain.Constants.ApplicationRoles.Administrator;
	private static readonly string[] Roles =
	{
		AdministratorRole,
		Domain.Constants.ApplicationRoles.Member,
		Domain.Constants.ApplicationRoles.AttendanceStaff
	};

	public static async Task SeedAsync(
		IServiceProvider serviceProvider)
	{
		var roleManager =
			serviceProvider.GetRequiredService<
				RoleManager<IdentityRole<Guid>>>();

		var userManager =
			serviceProvider.GetRequiredService<
				UserManager<ApplicationUser>>();

		await SeedRolesAsync(
			roleManager);

		await SeedRootAdminAsync(
			userManager);
	}

	private static async Task SeedRolesAsync(
		RoleManager<IdentityRole<Guid>> roleManager)
	{
		foreach (var roleName in Roles)
		{
			if (await roleManager.RoleExistsAsync(
				roleName))
			{
				continue;
			}

			var role = new IdentityRole<Guid>
			{
				Name = roleName,
				NormalizedName =
					roleName.ToUpperInvariant()
			};

			var result =
				await roleManager.CreateAsync(role);

			if (!result.Succeeded)
			{
				var errors = string.Join(
					", ",
					result.Errors.Select(x =>
						$"{x.Code}: {x.Description}"));

				throw new InvalidOperationException(
					$"Failed to create Identity role " +
					$"'{roleName}'. {errors}");
			}
		}
	}

	private static async Task SeedRootAdminAsync(
		UserManager<ApplicationUser> userManager)
	{
		var existingUser =
			await userManager.FindByNameAsync(
				RootAdminUserName);

		if (existingUser is not null)
		{
			await EnsureAdministratorRoleAsync(
				userManager,
				existingUser);

			return;
		}

		var user = new ApplicationUser
		{
			UserName = RootAdminUserName,

			// Root administrator is not associated
			// with a Member.
			MemberId = null,

			// Force the administrator to change
			// the initial password.
			MustChangePassword = true,

			EmailConfirmed = true
		};

		var createResult =
			await userManager.CreateAsync(
				user,
				RootAdminPassword);

		if (!createResult.Succeeded)
		{
			var errors = string.Join(
				", ",
				createResult.Errors.Select(x =>
					$"{x.Code}: {x.Description}"));

			throw new InvalidOperationException(
				$"Failed to create root administrator. " +
				errors);
		}

		await EnsureAdministratorRoleAsync(
			userManager,
			user);
	}

	private static async Task EnsureAdministratorRoleAsync(
		UserManager<ApplicationUser> userManager,
		ApplicationUser user)
	{
		if (await userManager.IsInRoleAsync(
			user,
			AdministratorRole))
		{
			return;
		}

		var result =
			await userManager.AddToRoleAsync(
				user,
				AdministratorRole);

		if (!result.Succeeded)
		{
			var errors = string.Join(
				", ",
				result.Errors.Select(x =>
					$"{x.Code}: {x.Description}"));

			throw new InvalidOperationException(
				$"Failed to assign Administrator role " +
				$"to root administrator. {errors}");
		}
	}
}
