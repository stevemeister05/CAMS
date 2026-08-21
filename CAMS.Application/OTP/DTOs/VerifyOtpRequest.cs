using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.OTP.DTOs;

public class VerifyOtpRequest
{
	public string MobileNumber { get; set; } = string.Empty;
	public string OtpCode { get; set; } = string.Empty;
}
