using CAMS.Application.Report.DTOs;

public interface IReportExportService
{
	byte[] ExportMemberAttendanceToExcel(
		IReadOnlyList<MemberAttendanceReportResponse> items,
		MemberAttendanceReportRequest request);

	byte[] ExportMemberAttendanceToPdf(
		IReadOnlyList<MemberAttendanceReportResponse> items,
		MemberAttendanceReportRequest request);

	byte[] ExportMemberAttendanceCountToExcel(
		IReadOnlyList<MemberAttendanceCountReportResponse> items,
		MemberAttendanceCountReportRequest request);


	byte[] ExportMemberAttendanceCountToPdf(
		IReadOnlyList<MemberAttendanceCountReportResponse> items,
		MemberAttendanceCountReportRequest request);

	byte[] ExportEventAttendanceSummaryToExcel(
		IReadOnlyList<EventAttendanceSummaryReportResponse> items,
		EventAttendanceSummaryReportRequest request);


	byte[] ExportEventAttendanceSummaryToPdf(
		IReadOnlyList<EventAttendanceSummaryReportResponse> items,
		EventAttendanceSummaryReportRequest request);
	byte[] ExportIndividualEventAttendanceToExcel(
		IndividualEventAttendanceReportResponse report);


	byte[] ExportIndividualEventAttendanceToPdf(
		IndividualEventAttendanceReportResponse report);
}