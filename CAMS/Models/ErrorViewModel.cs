namespace CAMS.Web.Models;

public class ErrorViewModel
{
	public string? RequestId { get; set; }

	public string? Path { get; set; }

	public string? Message { get; set; }

	public bool ShowRequestId =>
		!string.IsNullOrWhiteSpace(RequestId);
}
