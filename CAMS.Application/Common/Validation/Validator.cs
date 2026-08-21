using CAMS.Application.Common.Exceptions;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CAMS.Application.Common.Validation;

public static class Validator
{
	public static void ThrowIfNotValidPhoneNumber(string? phoneNumber)
	{
		if (string.IsNullOrWhiteSpace(phoneNumber))
		{
			throw new ValidationException(
				"Mobile number is required.");
		}

		if (!Regex.IsMatch(phoneNumber.Trim(), @"^09\d{9}$"))
		{
			throw new ValidationException(
				"Please provide a valid Philippine mobile number.");
		}
	}

	public static void ThrowIfNotValidEmailAddress(
		string? emailAddress)
	{
		if (string.IsNullOrWhiteSpace(emailAddress))
		{
			throw new ValidationException(
				"Email address is required.");
		}

		try
		{
			var mailAddress = new MailAddress(
				emailAddress.Trim());

			if (!mailAddress.Address.Equals(emailAddress.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				throw new ValidationException(
					"Please provide a valid email address.");
			}
		}
		catch (ValidationException)
		{
			throw;
		}
		catch
		{
			throw new ValidationException(
				"Please provide a valid email address.");
		}
	}

	public static string NormalizePhoneNumber(string phoneNumber)
	{
		var value = phoneNumber.Trim();

		if (value.StartsWith("+63"))
		{
			return value[1..];
		}

		if (value.StartsWith("09"))
		{
			return $"63{value[1..]}";
		}

		return value;
	}
}
