(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"member-attendance-report-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const form =
		document.getElementById(
			"member-attendance-report-form");


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
			"member-attendance-report-alert");


	// =========================================================
	// INITIALIZATION
	// =========================================================

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


	// =========================================================
	// DEFAULT FILTERS
	// =========================================================

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


	// =========================================================
	// LOAD REPORT
	// =========================================================

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
					5,

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
						"/api/v1/reports/member-attendance"));


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
				"Unable to generate " +
				"member attendance report.",
				error);


			recordCount.textContent =
				"0";


			camsTable.showError(
				tableBody,
				error.message ??
					"Unable to generate report.",
				{
					columnCount:
						5
				});


			camsUi.showError(
				alertContainer,
				error.message ??
					"Unable to generate report.",
				"member-attendance-report-alert-message");
		}
		finally {

			camsUi.setButtonLoading(
				generateButton,
				false);
		}
	}


	// =========================================================
	// RENDER REPORT
	// =========================================================

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
					5,

				title:
					"No Attendance Records",

				message:
					"No attendance records were found " +
					"within the selected date range.",

				icon:
					"ri-calendar-check-line"
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

				<td>

					${camsUtils.escapeHtml(
						item.memberName ??
							"—")}

				</td>

				<td>

					${formatTime(
						item.timeIn)}

				</td>

				<td class="pe-3">

					${formatTime(
						item.timeOut)}

				</td>

			</tr>
		`;
	}


	// =========================================================
	// EXPORT
	// =========================================================

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
						"member-attendance/export/" +
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


			const fileName =
				"Member-Attendance-Report-" +
				`${dateFrom.value}-to-` +
				`${dateTo.value}.` +
				extension;


			downloadBlob(
				blob,
				fileName);
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


	// =========================================================
	// FILTERS
	// =========================================================

	function validateFilters() {

		camsUi.clearAlert(
			"member-attendance-report-alert-message");


		if (
			!dateFrom.value ||
			!dateTo.value
		) {

			camsUi.showError(
				alertContainer,
				"Date From and Date To are required.",
				"member-attendance-report-alert-message");


			return false;
		}


		if (
			dateFrom.value >
			dateTo.value
		) {

			camsUi.showError(
				alertContainer,
				"Date From cannot be later than Date To.",
				"member-attendance-report-alert-message");


			return false;
		}


		return true;
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


	// =========================================================
	// DATE / TIME
	// =========================================================

	function formatTime(
		value) {

		if (!value) {
			return "—";
		}


		const date =
			camsUtils.parseDateTime(
				value);


		if (!date) {
			return "—";
		}


		return camsUtils.escapeHtml(
			date.toLocaleTimeString(
				"en-PH",
				{
					timeZone:
						"Asia/Manila",

					hour:
						"numeric",

					minute:
						"2-digit",

					hour12:
						true
				}));
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


	// =========================================================
	// DOWNLOAD
	// =========================================================

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


	function setExportButtonsEnabled(
		enabled) {

		exportExcelButton.disabled =
			!enabled;

		exportPdfButton.disabled =
			!enabled;
	}

})();