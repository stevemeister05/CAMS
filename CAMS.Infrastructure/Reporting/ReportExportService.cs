using CAMS.Application.Report;
using CAMS.Application.Report.DTOs;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CAMS.Infrastructure.Reporting;

public sealed class ReportExportService
	: IReportExportService
{
	private static readonly TimeZoneInfo
		PhilippineTimeZone =
			TimeZoneInfo.FindSystemTimeZoneById(
				"Asia/Manila");


	// =========================================================
	// EXCEL
	// =========================================================

	public byte[] ExportMemberAttendanceToExcel(
		IReadOnlyList<MemberAttendanceReportResponse> items,
		MemberAttendanceReportRequest request)
	{
		using var workbook =
			new XLWorkbook();


		var worksheet =
			workbook.Worksheets.Add(
				"Member Attendance");


		const int columnCount =
			5;


		// -----------------------------------------------------
		// TITLE
		// -----------------------------------------------------

		worksheet.Cell(
			1,
			1).Value =
				"Member Attendance Report";


		var titleRange =
			worksheet.Range(
				1,
				1,
				1,
				columnCount);


		titleRange.Merge();

		titleRange.Style.Font.Bold =
			true;

		titleRange.Style.Font.FontSize =
			16;

		titleRange.Style.Alignment.Horizontal =
			XLAlignmentHorizontalValues.Center;


		// -----------------------------------------------------
		// REPORTING PERIOD
		// -----------------------------------------------------

		worksheet.Cell(
			3,
			1).Value =
				"Date From:";


		worksheet.Cell(
			3,
			2).Value =
				request.DateFrom.ToDateTime(
					TimeOnly.MinValue);


		worksheet.Cell(
			3,
			2)
			.Style.DateFormat.Format =
				"MMM dd, yyyy";


		worksheet.Cell(
			3,
			4).Value =
				"Date To:";


		worksheet.Cell(
			3,
			5).Value =
				request.DateTo.ToDateTime(
					TimeOnly.MinValue);


		worksheet.Cell(
			3,
			5)
			.Style.DateFormat.Format =
				"MMM dd, yyyy";


		worksheet.Cell(
			3,
			1)
			.Style.Font.Bold =
				true;


		worksheet.Cell(
			3,
			4)
			.Style.Font.Bold =
				true;


		// -----------------------------------------------------
		// HEADERS
		// -----------------------------------------------------

		const int headerRow =
			5;


		worksheet.Cell(
			headerRow,
			1).Value =
				"Date";


		worksheet.Cell(
			headerRow,
			2).Value =
				"Event";


		worksheet.Cell(
			headerRow,
			3).Value =
				"Member";


		worksheet.Cell(
			headerRow,
			4).Value =
				"Time In";


		worksheet.Cell(
			headerRow,
			5).Value =
				"Time Out";


		var headerRange =
			worksheet.Range(
				headerRow,
				1,
				headerRow,
				columnCount);


		headerRange.Style.Font.Bold =
			true;


		// -----------------------------------------------------
		// DATA
		// -----------------------------------------------------

		var row =
			headerRow + 1;


		foreach (var item in items)
		{
			worksheet.Cell(
				row,
				1).Value =
					item.EventDate.ToDateTime(
						TimeOnly.MinValue);


			worksheet.Cell(
				row,
				1)
				.Style.DateFormat.Format =
					"MMM dd, yyyy";


			worksheet.Cell(
				row,
				2).Value =
					item.EventName;


			worksheet.Cell(
				row,
				3).Value =
					item.MemberName;


			if (item.TimeIn.HasValue)
			{
				worksheet.Cell(
					row,
					4).Value =
						ToPhilippineTime(
							item.TimeIn.Value);


				worksheet.Cell(
					row,
					4)
					.Style.DateFormat.Format =
						"h:mm AM/PM";
			}


			if (item.TimeOut.HasValue)
			{
				worksheet.Cell(
					row,
					5).Value =
						ToPhilippineTime(
							item.TimeOut.Value);


				worksheet.Cell(
					row,
					5)
					.Style.DateFormat.Format =
						"h:mm AM/PM";
			}


			row++;
		}


		// -----------------------------------------------------
		// TABLE
		// -----------------------------------------------------

		if (items.Count > 0)
		{
			worksheet.Range(
				headerRow,
				1,
				row - 1,
				columnCount)
				.CreateTable(
					"MemberAttendanceReport");
		}


		// -----------------------------------------------------
		// TOTAL
		// -----------------------------------------------------

		worksheet.Cell(
			row + 1,
			1).Value =
				"Total Records:";


		worksheet.Cell(
			row + 1,
			2).Value =
				items.Count;


		worksheet.Cell(
			row + 1,
			1)
			.Style.Font.Bold =
				true;


		// -----------------------------------------------------
		// LAYOUT
		// -----------------------------------------------------

		worksheet.Columns(
			1,
			columnCount)
			.AdjustToContents();


		worksheet.SheetView.FreezeRows(
			headerRow);


		using var stream =
			new MemoryStream();


		workbook.SaveAs(
			stream);


		return stream.ToArray();
	}


	// =========================================================
	// PDF
	// =========================================================

	public byte[] ExportMemberAttendanceToPdf(
		IReadOnlyList<MemberAttendanceReportResponse> items,
		MemberAttendanceReportRequest request)
	{
		return Document
			.Create(document =>
			{
				document.Page(page =>
				{
					page.Size(
						PageSizes.A4.Landscape());


					page.Margin(
						30);


					page.DefaultTextStyle(
						style =>
							style.FontSize(
								9));


					page.Header()
						.Element(container =>
							ComposeMemberAttendanceHeader(
								container,
								request));


					page.Content()
						.PaddingTop(
							15)
						.Element(container =>
							ComposeMemberAttendanceTable(
								container,
								items));


					page.Footer()
						.PaddingTop(
							10)
						.Row(row =>
						{
							row.RelativeItem()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(
									$"Total Records: {items.Count}");


							row.RelativeItem()
								.AlignRight()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(text =>
								{
									text.Span(
										"Page ");

									text.CurrentPageNumber();

									text.Span(
										" of ");

									text.TotalPages();
								});
						});
				});
			})
			.GeneratePdf();
	}

	public byte[] ExportMemberAttendanceCountToExcel(
	IReadOnlyList<MemberAttendanceCountReportResponse> items,
	MemberAttendanceCountReportRequest request)
	{
		using var workbook =
			new XLWorkbook();


		var worksheet =
			workbook.Worksheets.Add(
				"Attendance Count");


		const int columnCount =
			2;


		// TITLE

		worksheet.Cell(
			1,
			1).Value =
				"Member Attendance Count";


		var titleRange =
			worksheet.Range(
				1,
				1,
				1,
				columnCount);


		titleRange.Merge();

		titleRange.Style.Font.Bold =
			true;

		titleRange.Style.Font.FontSize =
			16;

		titleRange.Style.Alignment.Horizontal =
			XLAlignmentHorizontalValues.Center;


		// REPORTING PERIOD

		worksheet.Cell(
			3,
			1).Value =
				"Reporting Period:";


		worksheet.Cell(
			3,
			2).Value =
				$"{request.DateFrom:MMM dd, yyyy} - " +
				$"{request.DateTo:MMM dd, yyyy}";


		worksheet.Cell(
			3,
			1)
			.Style.Font.Bold =
				true;


		// COUNT FILTER

		worksheet.Cell(
			4,
			1).Value =
				"Attendance Count:";


		worksheet.Cell(
			4,
			2).Value =
				GetAttendanceCountFilterText(
					request);


		worksheet.Cell(
			4,
			1)
			.Style.Font.Bold =
				true;


		// HEADERS

		const int headerRow =
			6;


		worksheet.Cell(
			headerRow,
			1).Value =
				"Member";


		worksheet.Cell(
			headerRow,
			2).Value =
				"Attendance Count";


		var headerRange =
			worksheet.Range(
				headerRow,
				1,
				headerRow,
				columnCount);


		headerRange.Style.Font.Bold =
			true;


		// DATA

		var row =
			headerRow + 1;


		foreach (var item in items)
		{
			worksheet.Cell(
				row,
				1).Value =
					item.MemberName;


			worksheet.Cell(
				row,
				2).Value =
					item.AttendanceCount;


			row++;
		}


		if (items.Count > 0)
		{
			worksheet.Range(
				headerRow,
				1,
				row - 1,
				columnCount)
				.CreateTable(
					"MemberAttendanceCountReport");
		}


		worksheet.Cell(
			row + 1,
			1).Value =
				"Total Members:";


		worksheet.Cell(
			row + 1,
			2).Value =
				items.Count;


		worksheet.Cell(
			row + 1,
			1)
			.Style.Font.Bold =
				true;


		worksheet.Columns(
			1,
			columnCount)
			.AdjustToContents();


		worksheet.SheetView
			.FreezeRows(
				headerRow);


		using var stream =
			new MemoryStream();


		workbook.SaveAs(
			stream);


		return stream.ToArray();
	}

	public byte[] ExportMemberAttendanceCountToPdf(
		IReadOnlyList<MemberAttendanceCountReportResponse> items,
		MemberAttendanceCountReportRequest request)
	{
		return Document
			.Create(document =>
			{
				document.Page(page =>
				{
					page.Size(
						PageSizes.A4);


					page.Margin(
						30);


					page.DefaultTextStyle(
						style =>
							style.FontSize(
								9));


					page.Header()
						.Element(container =>
							ComposeMemberAttendanceCountHeader(
								container,
								request));


					page.Content()
						.PaddingTop(
							15)
						.Element(container =>
							ComposeMemberAttendanceCountTable(
								container,
								items));


					page.Footer()
						.PaddingTop(
							10)
						.Row(row =>
						{
							row.RelativeItem()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(
									$"Total Members: {items.Count}");


							row.RelativeItem()
								.AlignRight()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(text =>
								{
									text.Span(
										"Page ");

									text.CurrentPageNumber();

									text.Span(
										" of ");

									text.TotalPages();
								});
						});
				});
			})
			.GeneratePdf();
	}

	public byte[] ExportEventAttendanceSummaryToExcel(
	IReadOnlyList<EventAttendanceSummaryReportResponse> items,
	EventAttendanceSummaryReportRequest request)
	{
		using var workbook =
			new XLWorkbook();


		var worksheet =
			workbook.Worksheets.Add(
				"Event Attendance Summary");


		const int columnCount =
			3;


		// TITLE

		worksheet.Cell(
			1,
			1).Value =
				"Event Attendance Summary";


		var titleRange =
			worksheet.Range(
				1,
				1,
				1,
				columnCount);


		titleRange.Merge();

		titleRange.Style.Font.Bold =
			true;

		titleRange.Style.Font.FontSize =
			16;

		titleRange.Style.Alignment.Horizontal =
			XLAlignmentHorizontalValues.Center;


		// REPORTING PERIOD

		worksheet.Cell(
			3,
			1).Value =
				"Reporting Period:";


		worksheet.Cell(
			3,
			2).Value =
				$"{request.DateFrom:MMM dd, yyyy} - " +
				$"{request.DateTo:MMM dd, yyyy}";


		worksheet.Range(
				3,
				2,
				3,
				3)
			.Merge();


		worksheet.Cell(
			3,
			1)
			.Style.Font.Bold =
				true;


		// HEADERS

		const int headerRow =
			5;


		worksheet.Cell(
			headerRow,
			1).Value =
				"Date";


		worksheet.Cell(
			headerRow,
			2).Value =
				"Event";


		worksheet.Cell(
			headerRow,
			3).Value =
				"Total Attendance";


		var headerRange =
			worksheet.Range(
				headerRow,
				1,
				headerRow,
				columnCount);


		headerRange.Style.Font.Bold =
			true;


		// DATA

		var row =
			headerRow + 1;


		foreach (var item in items)
		{
			worksheet.Cell(
				row,
				1).Value =
					item.EventDate.ToDateTime(
						TimeOnly.MinValue);


			worksheet.Cell(
				row,
				1)
				.Style.DateFormat.Format =
					"MMM dd, yyyy";


			worksheet.Cell(
				row,
				2).Value =
					item.EventName;


			worksheet.Cell(
				row,
				3).Value =
					item.TotalAttendance;


			row++;
		}


		if (items.Count > 0)
		{
			worksheet.Range(
				headerRow,
				1,
				row - 1,
				columnCount)
				.CreateTable(
					"EventAttendanceSummaryReport");
		}


		// TOTAL

		worksheet.Cell(
			row + 1,
			1).Value =
				"Total Events:";


		worksheet.Cell(
			row + 1,
			2).Value =
				items.Count;


		worksheet.Cell(
			row + 2,
			1).Value =
				"Total Attendance:";


		worksheet.Cell(
			row + 2,
			2).Value =
				items.Sum(x =>
					x.TotalAttendance);


		worksheet.Cell(
			row + 1,
			1)
			.Style.Font.Bold =
				true;


		worksheet.Cell(
			row + 2,
			1)
			.Style.Font.Bold =
				true;


		worksheet.Columns(
			1,
			columnCount)
			.AdjustToContents();


		worksheet.SheetView
			.FreezeRows(
				headerRow);


		using var stream =
			new MemoryStream();


		workbook.SaveAs(
			stream);


		return stream.ToArray();
	}

	public byte[] ExportEventAttendanceSummaryToPdf(
		IReadOnlyList<EventAttendanceSummaryReportResponse> items,
		EventAttendanceSummaryReportRequest request)
	{
		return Document
			.Create(document =>
			{
				document.Page(page =>
				{
					page.Size(
						PageSizes.A4);


					page.Margin(
						30);


					page.DefaultTextStyle(
						style =>
							style.FontSize(
								9));


					page.Header()
						.Element(container =>
							ComposeEventAttendanceSummaryHeader(
								container,
								request));


					page.Content()
						.PaddingTop(
							15)
						.Element(container =>
							ComposeEventAttendanceSummaryTable(
								container,
								items));


					page.Footer()
						.PaddingTop(
							10)
						.Row(row =>
						{
							row.RelativeItem()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(
									$"Total Events: {items.Count} | " +
									$"Total Attendance: " +
									$"{items.Sum(x => x.TotalAttendance)}");


							row.RelativeItem()
								.AlignRight()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(text =>
								{
									text.Span(
										"Page ");

									text.CurrentPageNumber();

									text.Span(
										" of ");

									text.TotalPages();
								});
						});
				});
			})
			.GeneratePdf();
	}

	public byte[] ExportIndividualEventAttendanceToExcel(
	IndividualEventAttendanceReportResponse report)
	{
		using var workbook =
			new XLWorkbook();


		var worksheet =
			workbook.Worksheets.Add(
				"Event Attendance");


		const int columnCount =
			3;


		// TITLE

		worksheet.Cell(
			1,
			1).Value =
				"Individual Event Attendance";


		var titleRange =
			worksheet.Range(
				1,
				1,
				1,
				columnCount);


		titleRange.Merge();

		titleRange.Style.Font.Bold =
			true;

		titleRange.Style.Font.FontSize =
			16;

		titleRange.Style.Alignment.Horizontal =
			XLAlignmentHorizontalValues.Center;


		// EVENT

		worksheet.Cell(
			3,
			1).Value =
				"Event:";


		worksheet.Cell(
			3,
			2).Value =
				report.EventName;


		worksheet.Range(
				3,
				2,
				3,
				3)
			.Merge();


		worksheet.Cell(
			3,
			1)
			.Style.Font.Bold =
				true;


		// EVENT DATE

		worksheet.Cell(
			4,
			1).Value =
				"Date:";


		worksheet.Cell(
			4,
			2).Value =
				report.EventDate.ToDateTime(
					TimeOnly.MinValue);


		worksheet.Cell(
			4,
			2)
			.Style.DateFormat.Format =
				"MMM dd, yyyy";


		worksheet.Cell(
			4,
			1)
			.Style.Font.Bold =
				true;


		// HEADERS

		const int headerRow =
			6;


		worksheet.Cell(
			headerRow,
			1).Value =
				"Member";


		worksheet.Cell(
			headerRow,
			2).Value =
				"Time In";


		worksheet.Cell(
			headerRow,
			3).Value =
				"Time Out";


		var headerRange =
			worksheet.Range(
				headerRow,
				1,
				headerRow,
				columnCount);


		headerRange.Style.Font.Bold =
			true;


		// DATA

		var row =
			headerRow + 1;


		foreach (var item in report.Attendances)
		{
			worksheet.Cell(
				row,
				1).Value =
					item.MemberName;


			if (item.TimeIn.HasValue)
			{
				worksheet.Cell(
					row,
					2).Value =
						ToPhilippineTime(
							item.TimeIn.Value);


				worksheet.Cell(
					row,
					2)
					.Style.DateFormat.Format =
						"h:mm AM/PM";
			}


			if (item.TimeOut.HasValue)
			{
				worksheet.Cell(
					row,
					3).Value =
						ToPhilippineTime(
							item.TimeOut.Value);


				worksheet.Cell(
					row,
					3)
					.Style.DateFormat.Format =
						"h:mm AM/PM";
			}


			row++;
		}


		if (report.Attendances.Count > 0)
		{
			worksheet.Range(
				headerRow,
				1,
				row - 1,
				columnCount)
				.CreateTable(
					"IndividualEventAttendanceReport");
		}


		// TOTAL

		worksheet.Cell(
			row + 1,
			1).Value =
				"Total Attendance:";


		worksheet.Cell(
			row + 1,
			2).Value =
				report.Attendances.Count;


		worksheet.Cell(
			row + 1,
			1)
			.Style.Font.Bold =
				true;


		worksheet.Columns(
			1,
			columnCount)
			.AdjustToContents();


		worksheet.SheetView
			.FreezeRows(
				headerRow);


		using var stream =
			new MemoryStream();


		workbook.SaveAs(
			stream);


		return stream.ToArray();
	}

	public byte[] ExportIndividualEventAttendanceToPdf(
	IndividualEventAttendanceReportResponse report)
	{
		return Document
			.Create(document =>
			{
				document.Page(page =>
				{
					page.Size(
						PageSizes.A4);


					page.Margin(
						30);


					page.DefaultTextStyle(
						style =>
							style.FontSize(
								9));


					page.Header()
						.Element(container =>
							ComposeIndividualEventAttendanceHeader(
								container,
								report));


					page.Content()
						.PaddingTop(
							15)
						.Element(container =>
							ComposeIndividualEventAttendanceTable(
								container,
								report.Attendances));


					page.Footer()
						.PaddingTop(
							10)
						.Row(row =>
						{
							row.RelativeItem()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(
									$"Total Attendance: " +
									$"{report.Attendances.Count}");


							row.RelativeItem()
								.AlignRight()
								.DefaultTextStyle(
									style =>
										style.FontSize(
											8))
								.Text(text =>
								{
									text.Span(
										"Page ");

									text.CurrentPageNumber();

									text.Span(
										" of ");

									text.TotalPages();
								});
						});
				});
			})
			.GeneratePdf();
	}

	private static void ComposeIndividualEventAttendanceHeader(
	IContainer container,
	IndividualEventAttendanceReportResponse report)
	{
		container.Column(column =>
		{
			column.Item()
				.AlignCenter()
				.Text(
					"Church Attendance Management System")
				.Bold()
				.FontSize(
					14);


			column.Item()
				.PaddingTop(
					5)
				.AlignCenter()
				.Text(
					"INDIVIDUAL EVENT ATTENDANCE")
				.Bold()
				.FontSize(
					12);


			column.Item()
				.PaddingTop(
					8)
				.AlignCenter()
				.Text(
					report.EventName)
				.Bold();


			column.Item()
				.PaddingTop(
					3)
				.AlignCenter()
				.Text(
					report.EventDate.ToString(
						"MMMM dd, yyyy"));
		});
	}

	private static void ComposeIndividualEventAttendanceTable(
	IContainer container,
	IReadOnlyList<IndividualEventAttendanceReportItemResponse> items)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.RelativeColumn(
					3);

				columns.RelativeColumn(
					1);

				columns.RelativeColumn(
					1);
			});


			table.Header(header =>
			{
				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Member");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Time In");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Time Out");
			});


			foreach (var item in items)
			{
				table.Cell()
					.Element(BodyCell)
					.Text(
						item.MemberName);


				table.Cell()
					.Element(BodyCell)
					.Text(
						FormatTime(
							item.TimeIn));


				table.Cell()
					.Element(BodyCell)
					.Text(
						FormatTime(
							item.TimeOut));
			}


			if (items.Count == 0)
			{
				table.Cell()
					.ColumnSpan(
						3)
					.Padding(
						15)
					.AlignCenter()
					.Text(
						"No attendance records were found " +
						"for this event.");
			}
		});
	}

	private static void ComposeEventAttendanceSummaryHeader(
		IContainer container,
		EventAttendanceSummaryReportRequest request)
	{
		container.Column(column =>
		{
			column.Item()
				.AlignCenter()
				.Text(
					"Church Attendance Management System")
				.Bold()
				.FontSize(
					14);


			column.Item()
				.PaddingTop(
					5)
				.AlignCenter()
				.Text(
					"EVENT ATTENDANCE SUMMARY")
				.Bold()
				.FontSize(
					12);


			column.Item()
				.PaddingTop(
					8)
				.AlignCenter()
				.Text(
					$"Reporting Period: " +
					$"{request.DateFrom:MMMM dd, yyyy} - " +
					$"{request.DateTo:MMMM dd, yyyy}");
		});
	}

	private static void ComposeEventAttendanceSummaryTable(
		IContainer container,
		IReadOnlyList<EventAttendanceSummaryReportResponse> items)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.ConstantColumn(
					100);

				columns.RelativeColumn(
					4);

				columns.RelativeColumn(
					1);
			});


			table.Header(header =>
			{
				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Date");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Event");


				header.Cell()
					.Element(HeaderCell)
					.AlignCenter()
					.Text(
						"Attendance");
			});


			foreach (var item in items)
			{
				table.Cell()
					.Element(BodyCell)
					.Text(
						item.EventDate.ToString(
							"MMM dd, yyyy"));


				table.Cell()
					.Element(BodyCell)
					.Text(
						item.EventName);


				table.Cell()
					.Element(BodyCell)
					.AlignCenter()
					.Text(
						item.TotalAttendance.ToString());
			}


			if (items.Count == 0)
			{
				table.Cell()
					.ColumnSpan(
						3)
					.Padding(
						15)
					.AlignCenter()
					.Text(
						"No events were found within " +
						"the selected date range.");
			}
		});
	}

	private static string GetAttendanceCountFilterText(
		MemberAttendanceCountReportRequest request)
	{
		if (
			request.MinimumCount.HasValue &&
			request.MaximumCount.HasValue
		)
		{
			return
				$"{request.MinimumCount.Value} - " +
				$"{request.MaximumCount.Value}";
		}


		if (request.MinimumCount.HasValue)
		{
			return
				$"{request.MinimumCount.Value} and above";
		}


		if (request.MaximumCount.HasValue)
		{
			return
				$"{request.MaximumCount.Value} and below";
		}


		return "All";
	}

	private static void ComposeMemberAttendanceCountHeader(
		IContainer container,
		MemberAttendanceCountReportRequest request)
	{
		container.Column(column =>
		{
			column.Item()
				.AlignCenter()
				.Text(
					"Church Attendance Management System")
				.Bold()
				.FontSize(
					14);


			column.Item()
				.PaddingTop(
					5)
				.AlignCenter()
				.Text(
					"MEMBER ATTENDANCE COUNT")
				.Bold()
				.FontSize(
					12);


			column.Item()
				.PaddingTop(
					8)
				.AlignCenter()
				.Text(
					$"Reporting Period: " +
					$"{request.DateFrom:MMMM dd, yyyy} - " +
					$"{request.DateTo:MMMM dd, yyyy}");


			column.Item()
				.PaddingTop(
					3)
				.AlignCenter()
				.Text(
					$"Attendance Count: " +
					GetAttendanceCountFilterText(
						request))
				.FontSize(
					8);
		});
	}

	private static void ComposeMemberAttendanceCountTable(
		IContainer container,
		IReadOnlyList<MemberAttendanceCountReportResponse> items)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.RelativeColumn(
					4);

				columns.RelativeColumn(
					1);
			});


			table.Header(header =>
			{
				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Member");


				header.Cell()
					.Element(HeaderCell)
					.AlignCenter()
					.Text(
						"Attendance Count");
			});


			foreach (var item in items)
			{
				table.Cell()
					.Element(BodyCell)
					.Text(
						item.MemberName);


				table.Cell()
					.Element(BodyCell)
					.AlignCenter()
					.Text(
						item.AttendanceCount.ToString());
			}


			if (items.Count == 0)
			{
				table.Cell()
					.ColumnSpan(
						2)
					.Padding(
						15)
					.AlignCenter()
					.Text(
						"No members matched the selected filters.");
			}
		});
	}


	// =========================================================
	// PDF HEADER
	// =========================================================

	private static void ComposeMemberAttendanceHeader(
		IContainer container,
		MemberAttendanceReportRequest request)
	{
		container.Column(column =>
		{
			column.Item()
				.AlignCenter()
				.Text(
					"Church Attendance Management System")
				.Bold()
				.FontSize(
					14);


			column.Item()
				.PaddingTop(
					5)
				.AlignCenter()
				.Text(
					"MEMBER ATTENDANCE REPORT")
				.Bold()
				.FontSize(
					12);


			column.Item()
				.PaddingTop(
					8)
				.AlignCenter()
				.Text(
					$"Reporting Period: " +
					$"{request.DateFrom:MMMM dd, yyyy} - " +
					$"{request.DateTo:MMMM dd, yyyy}");
		});
	}


	// =========================================================
	// PDF TABLE
	// =========================================================

	private static void ComposeMemberAttendanceTable(
		IContainer container,
		IReadOnlyList<MemberAttendanceReportResponse> items)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.ConstantColumn(
					80);

				columns.RelativeColumn(
					2);

				columns.RelativeColumn(
					2);

				columns.ConstantColumn(
					80);

				columns.ConstantColumn(
					80);
			});


			table.Header(header =>
			{
				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Date");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Event");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Member");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Time In");


				header.Cell()
					.Element(HeaderCell)
					.Text(
						"Time Out");
			});


			foreach (var item in items)
			{
				table.Cell()
					.Element(BodyCell)
					.Text(
						item.EventDate.ToString(
							"MMM dd, yyyy"));


				table.Cell()
					.Element(BodyCell)
					.Text(
						item.EventName);


				table.Cell()
					.Element(BodyCell)
					.Text(
						item.MemberName);


				table.Cell()
					.Element(BodyCell)
					.Text(
						FormatTime(
							item.TimeIn));


				table.Cell()
					.Element(BodyCell)
					.Text(
						FormatTime(
							item.TimeOut));
			}


			if (items.Count == 0)
			{
				table.Cell()
					.ColumnSpan(
						5)
					.Padding(
						15)
					.AlignCenter()
					.Text(
						"No attendance records found.");
			}
		});
	}


	private static IContainer HeaderCell(
		IContainer container)
	{
		return container
			.Background(
				Colors.Grey.Lighten3)
			.BorderBottom(
				1)
			.BorderColor(
				Colors.Grey.Darken1)
			.Padding(
				6)
			.DefaultTextStyle(
				style =>
					style.Bold());
	}


	private static IContainer BodyCell(
		IContainer container)
	{
		return container
			.BorderBottom(
				1)
			.BorderColor(
				Colors.Grey.Lighten2)
			.Padding(
				5);
	}


	// =========================================================
	// DATE / TIME
	// =========================================================

	private static string FormatTime(
		DateTime? value)
	{
		if (!value.HasValue)
		{
			return "—";
		}


		return ToPhilippineTime(
				value.Value)
			.ToString(
				"h:mm tt");
	}


	private static DateTime ToPhilippineTime(
		DateTime value)
	{
		var utcValue =
			DateTime.SpecifyKind(
				value,
				DateTimeKind.Utc);


		return TimeZoneInfo.ConvertTimeFromUtc(
			utcValue,
			PhilippineTimeZone);
	}
}