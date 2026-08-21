using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.RateLimiting;

public static class RateLimitLevel
{
	/// <summary>
	/// Low rate limit level only allows 60 requests per minute
	/// </summary>
	public const string Low = "Low";

	/// <summary>
	/// Moderate rate limit level only allows 20 requests per minute
	/// </summary>
	public const string Moderate = "Moderate";

	/// <summary>
	/// High rate limit level only allows 5 requests per minute
	/// </summary>
	public const string High = "High";
}
