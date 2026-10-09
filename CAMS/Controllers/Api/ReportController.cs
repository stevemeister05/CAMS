using CAMS.Application.Report;
using CAMS.Application.Report.DTOs;
using CAMS.Domain.Constants;
using CAMS.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers.Api;

[ApiController]
[Route("api/v1/reports")]
[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
public sealed class ReportController
	: ControllerBase
{
	private const string ExcelContentType =
		"application/vnd.openxmlformats-" +
		"officedocument.spreadsheetml.sheet";


	private readonly IReportService
		_reportService;

	private readonly IReportExportService
		_reportExportService;


	public ReportController(
		IReportService reportService,
		IReportExportService reportExportService)
	{
		_reportService =
			reportService;

		_reportExportService =
			reportExportService;
	}


	// =========================================================
	// MEMBER ATTENDANCE
	// =========================================================

	[HttpGet("member-attendance")]
	public async Task<IActionResult>
		GetMemberAttendance(
			[FromQuery]
			MemberAttendanceReportRequest request,
			CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceAsync(
					request,
					cancellationToken);


		return Ok(
			ApiResponse<
				IReadOnlyList<
					MemberAttendanceReportResponse>>
				.Ok(
					result,
					"Member attendance report " +
					"retrieved successfully."));
	}


	// =========================================================
	// MEMBER ATTENDANCE - EXCEL
	// =========================================================

	[HttpGet("member-attendance/export/excel")]
	public async Task<IActionResult>
		ExportMemberAttendanceExcel(
			[FromQuery]
			MemberAttendanceReportRequest request,
			CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportMemberAttendanceToExcel(
					result,
					request);


		var fileName =
			GetMemberAttendanceFileName(
				request,
				"xlsx");


		return File(
			file,
			ExcelContentType,
			fileName);
	}


	// =========================================================
	// MEMBER ATTENDANCE - PDF
	// =========================================================

	[HttpGet("member-attendance/export/pdf")]
	public async Task<IActionResult>
		ExportMemberAttendancePdf(
			[FromQuery]
			MemberAttendanceReportRequest request,
			CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportMemberAttendanceToPdf(
					result,
					request);


		var fileName =
			GetMemberAttendanceFileName(
				request,
				"pdf");


		return File(
			file,
			"application/pdf",
			fileName);
	}

	[HttpGet("member-attendance-count/export/excel")]
	public async Task<IActionResult>
	ExportMemberAttendanceCountExcel(
		[FromQuery]
		MemberAttendanceCountReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceCountAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportMemberAttendanceCountToExcel(
					result,
					request);


		var fileName =
			GetMemberAttendanceCountFileName(
				request,
				"xlsx");


		return File(
			file,
			ExcelContentType,
			fileName);
	}

	[HttpGet("member-attendance-count/export/pdf")]
	public async Task<IActionResult>
	ExportMemberAttendanceCountPdf(
		[FromQuery]
		MemberAttendanceCountReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceCountAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportMemberAttendanceCountToPdf(
					result,
					request);


		var fileName =
			GetMemberAttendanceCountFileName(
				request,
				"pdf");


		return File(
			file,
			"application/pdf",
			fileName);
	}

	[HttpGet("member-attendance-count")]
	public async Task<IActionResult>
	GetMemberAttendanceCount(
		[FromQuery]
		MemberAttendanceCountReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetMemberAttendanceCountAsync(
					request,
					cancellationToken);


		return Ok(
			ApiResponse<
				IReadOnlyList<
					MemberAttendanceCountReportResponse>>
				.Ok(
					result,
					"Member attendance count report " +
					"retrieved successfully."));
	}

	[HttpGet("event-attendance-summary")]
	public async Task<IActionResult> GetEventAttendanceSummary(
		[FromQuery]
		EventAttendanceSummaryReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetEventAttendanceSummaryAsync(
					request,
					cancellationToken);


		return Ok(
			ApiResponse<
				IReadOnlyList<
					EventAttendanceSummaryReportResponse>>
				.Ok(
					result,
					"Event attendance summary " +
					"retrieved successfully."));
	}

	[HttpGet("event-attendance-summary/export/excel")]
	public async Task<IActionResult> ExportEventAttendanceSummaryExcel(
		[FromQuery]
		EventAttendanceSummaryReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetEventAttendanceSummaryAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportEventAttendanceSummaryToExcel(
					result,
					request);


		var fileName =
			GetEventAttendanceSummaryFileName(
				request,
				"xlsx");


		return File(
			file,
			ExcelContentType,
			fileName);
	}

	[HttpGet("event-attendance-summary/export/pdf")]
	public async Task<IActionResult> ExportEventAttendanceSummaryPdf(
		[FromQuery]
		EventAttendanceSummaryReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetEventAttendanceSummaryAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportEventAttendanceSummaryToPdf(
					result,
					request);


		var fileName =
			GetEventAttendanceSummaryFileName(
				request,
				"pdf");


		return File(
			file,
			"application/pdf",
			fileName);
	}

	[HttpGet("individual-event-attendance")]
	public async Task<IActionResult> GetIndividualEventAttendance(
		[FromQuery]
		IndividualEventAttendanceReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetIndividualEventAttendanceAsync(
					request,
					cancellationToken);


		return Ok(
			ApiResponse<
				IndividualEventAttendanceReportResponse>
				.Ok(
					result,
					"Individual event attendance report " +
					"retrieved successfully."));
	}

	[HttpGet("individual-event-attendance/export/excel")]
	public async Task<IActionResult> ExportIndividualEventAttendanceExcel(
		[FromQuery]
		IndividualEventAttendanceReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetIndividualEventAttendanceAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportIndividualEventAttendanceToExcel(
					result);


		var fileName =
			GetIndividualEventAttendanceFileName(
				result,
				"xlsx");


		return File(
			file,
			ExcelContentType,
			fileName);
	}

	[HttpGet("individual-event-attendance/export/pdf")]
	public async Task<IActionResult> ExportIndividualEventAttendancePdf(
		[FromQuery]
		IndividualEventAttendanceReportRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _reportService
				.GetIndividualEventAttendanceAsync(
					request,
					cancellationToken);


		var file =
			_reportExportService
				.ExportIndividualEventAttendanceToPdf(
					result);


		var fileName =
			GetIndividualEventAttendanceFileName(
				result,
				"pdf");


		return File(
			file,
			"application/pdf",
			fileName);
	}


	// =========================================================
	// FILE NAME
	// =========================================================
	private static string GetIndividualEventAttendanceFileName(
		IndividualEventAttendanceReportResponse report,
		string extension)
	{
		return
			$"Individual-Event-Attendance-" +
			$"{report.EventDate:yyyy-MM-dd}." +
			$"{extension}";
	}

	private static string GetEventAttendanceSummaryFileName(
		EventAttendanceSummaryReportRequest request,
		string extension)
	{
		return
			$"Event-Attendance-Summary-" +
			$"{request.DateFrom:yyyy-MM-dd}-to-" +
			$"{request.DateTo:yyyy-MM-dd}." +
			$"{extension}";
	}

	private static string GetMemberAttendanceCountFileName(
		MemberAttendanceCountReportRequest request,
		string extension)
	{
		return
			$"Member-Attendance-Count-" +
			$"{request.DateFrom:yyyy-MM-dd}-to-" +
			$"{request.DateTo:yyyy-MM-dd}." +
			$"{extension}";
	}

	private static string
		GetMemberAttendanceFileName(
			MemberAttendanceReportRequest request,
			string extension)
	{
		return
			$"Member-Attendance-Report-" +
			$"{request.DateFrom:yyyy-MM-dd}-to-" +
			$"{request.DateTo:yyyy-MM-dd}." +
			$"{extension}";
	}
}