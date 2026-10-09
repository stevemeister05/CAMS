using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

public class OtpVerification : BaseEntity
{
	public OtpVerification() : base() { }

	public string MobileNumber { get; set; } = string.Empty;

	public string OtpCode { get; set; } = string.Empty;

	public OtpPurpose Purpose { get; set; }

	public DateTime ExpiresAt { get; set; }

	public DateTime? VerifiedAt { get; set; }

	public int AttemptCount { get; set; }

	public bool IsVerified { get; set; }
}
