(() => {
	"use strict";


	const page =
		document.getElementById(
			"event-attendance-summary-report-page");


	if (!page) {
		return;
	}


	const form =
		document.getElementById(
			"event-attendance-summary-report-form");

	const dateFrom =
		document.getElementById(
			"date-from");

	const dateTo =
		document.getElementById(
			"date-to");

	const generateButton =
		document.getElementById(
			"generate-report-button");

	const exportExcelButton =
		document.getElementById(
			"export-excel-button");

	const exportPdfButton =
		document.getElementById(
			"export-pdf-button");

	const tableBody =
		document.getElementById(
			"report-table-body");

	const recordCount =
		document.getElementById(
			"report-record-count");

	const alertContainer =
		document.getElementById(
			"event-attendance-summary-report-alert");


	initialize();


	function initialize() {

		setDefaultDates();


		form?.addEventListener(
			"submit",
			async event => {

				event.preventDefault();


				await loadReport();
			});


		dateFrom?.addEventListener(
			"change",
			handleFilterChange);


		dateTo?.addEventListener(
			"change",
			handleFilterChange);


		exportExcelButton?.addEventListener(
			"click",
			async () => {

				await exportReport(
					"excel");
			});


		exportPdfButton?.addEventListener(
			"click",
			async () => {

				await exportReport(
					"pdf");
			});
	}


	function setDefaultDates() {

		const today =
			new Date();


		const firstDay =
			new Date(
				today.getFullYear(),
				today.getMonth(),
				1);


		dateFrom.value =
			toDateInput(
				firstDay);


		dateTo.value =
			toDateInput(
				today);
	}


	function handleFilterChange() {

		setExportButtonsEnabled(
			false);
	}


	async function loadReport() {

		if (!validateFilters()) {
			return;
		}


		setExportButtonsEnabled(
			false);


		camsTable.showLoading(
			tableBody,
			{
				columnCount:
					3,

				message:
					"Generating report..."
			});


		camsUi.setButtonLoading(
			generateButton,
			true,
			"Generating...");


		try {

			const response =
				await camsApi.get(
					buildUrl(
						"/api/v1/reports/" +
						"event-attendance-summary"));


			if (!response) {
				return;
			}


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to generate report."));
			}


			const items =
				payload.data ??
				[];


			renderReport(
				items);


			setExportButtonsEnabled(
				true);
		}
		catch (error) {

			console.error(
				"Unable to generate event " +
				"attendance summary.",
				error);


			recordCount.textContent =
				"0";


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to generate report.",
				{
					columnCount:
						3
				});
		}
		finally {

			camsUi.setButtonLoading(
				generateButton,
				false);
		}
	}


	function renderReport(
		items) {

		recordCount.textContent =
			items.length.toString();


		camsTable.renderRows(
			tableBody,
			items,
			renderRow,
			{
				columnCount:
					3,

				title:
					"No Events Found",

				message:
					"No events were found within " +
					"the selected date range.",

				icon:
					"ri-calendar-event-line"
			});
	}


	function renderRow(
		item) {

		return `
			<tr>

				<td class="ps-3">

					${camsUtils.escapeHtml(
			camsUtils.formatDate(
				item.eventDate))}

				</td>

				<td>

					${camsUtils.escapeHtml(
					item.eventName ??
					"—")}

				</td>

				<td class="text-center pe-3">

					<span class="badge
								 bg-primary-subtle
								 text-primary">

						${Number(
						item.totalAttendance ??
						0)}

					</span>

				</td>

			</tr>
		`;
	}


	async function exportReport(
		format) {

		if (!validateFilters()) {
			return;
		}


		const button =
			format ===
				"excel"
				? exportExcelButton
				: exportPdfButton;


		camsUi.setButtonLoading(
			button,
			true,
			"Exporting...");


		try {

			const response =
				await camsApi.get(
					buildUrl(
						"/api/v1/reports/" +
						"event-attendance-summary/" +
						"export/" +
						format));


			if (!response) {
				return;
			}


			if (!response.ok) {

				const payload =
					await camsApi.readJson(
						response);


				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to export report."));
			}


			const blob =
				await response.blob();


			const extension =
				format ===
					"excel"
					? "xlsx"
					: "pdf";


			downloadBlob(
				blob,
				"Event-Attendance-Summary-" +
				`${dateFrom.value}-to-` +
				`${dateTo.value}.` +
				extension);
		}
		catch (error) {

			console.error(
				"Unable to export report.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to export report.",
				{
					title:
						"Export Failed"
				});
		}
		finally {

			camsUi.setButtonLoading(
				button,
				false);
		}
	}


	function validateFilters() {

		camsUi.clearAlert(
			"event-attendance-summary-report-alert-message");


		if (
			!dateFrom.value ||
			!dateTo.value
		) {

			showFilterError(
				"Date From and Date To are required.");


			return false;
		}


		if (
			dateFrom.value >
			dateTo.value
		) {

			showFilterError(
				"Date From cannot be later than Date To.");


			return false;
		}


		return true;
	}


	function showFilterError(
		message) {

		camsUi.showError(
			alertContainer,
			message,
			"event-attendance-summary-report-alert-message");
	}


	function buildUrl(
		baseUrl) {

		const query =
			new URLSearchParams({
				dateFrom:
					dateFrom.value,

				dateTo:
					dateTo.value
			});


		return `${baseUrl}?${query.toString()}`;
	}


	function setExportButtonsEnabled(
		enabled) {

		exportExcelButton.disabled =
			!enabled;

		exportPdfButton.disabled =
			!enabled;
	}


	function downloadBlob(
		blob,
		fileName) {

		const url =
			URL.createObjectURL(
				blob);


		const link =
			document.createElement(
				"a");


		link.href =
			url;

		link.download =
			fileName;


		document.body.appendChild(
			link);


		link.click();

		link.remove();


		URL.revokeObjectURL(
			url);
	}


	function toDateInput(
		date) {

		const year =
			date.getFullYear();

		const month =
			String(
				date.getMonth() + 1)
				.padStart(
					2,
					"0");

		const day =
			String(
				date.getDate())
				.padStart(
					2,
					"0");


		return `${year}-${month}-${day}`;
	}

})();