using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Web.Models.Responses;

public class ApiError
{
	public string Code { get; set; } = string.Empty;

	public string Message { get; set; } = string.Empty;
}
