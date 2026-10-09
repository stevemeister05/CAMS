(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"individual-event-attendance-report-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const form =
		document.getElementById(
			"individual-event-attendance-report-form");


	const eventSelect =
		document.getElementById(
			"event-id");


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


	const selectedEventName =
		document.getElementById(
			"selected-event-name");


	const selectedEventDate =
		document.getElementById(
			"selected-event-date");


	const alertContainer =
		document.getElementById(
			"individual-event-attendance-report-alert");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		initializeEvents();


		await loadEvents();


		initializeEventSelect();
	}


	function initializeEvents() {

		form?.addEventListener(
			"submit",
			async event => {

				event.preventDefault();


				await loadReport();
			});


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
	// EVENT SELECT
	// =========================================================

	function initializeEventSelect() {

		if (
			typeof $ ===
			"undefined" ||
			typeof $.fn.select2 ===
			"undefined"
		) {

			console.error(
				"Select2 is not available.");


			return;
		}


		$(eventSelect)
			.select2({
				placeholder:
					"Search and select an event",

				allowClear:
					true,

				width:
					"100%"
			});


		$(eventSelect)
			.on(
				"change",
				() => {

					handleEventChange();
				});
	}


	function handleEventChange() {

		setExportButtonsEnabled(
			false);


		clearReportHeader();


		recordCount.textContent =
			"0";


		camsTable.renderRows(
			tableBody,
			[],
			renderRow,
			{
				columnCount:
					3,

				title:
					"No Report Generated",

				message:
					"Generate the report to view " +
					"attendance records.",

				icon:
					"ri-file-chart-line"
			});
	}


	// =========================================================
	// LOAD EVENTS
	// =========================================================

	async function loadEvents() {

		eventSelect.disabled =
			true;


		try {

			const response =
				await camsApi.get(
					"/api/v1/events?filter.status=completed&sortby[0].name=eventDate&sortby[0].sortdescending=true");


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
						"Unable to load events."));
			}


			const events =
				payload?.data?.items ??
				payload?.items ??
				[];


			renderEvents(
				events);


			eventSelect.disabled =
				false;
		}
		catch (error) {

			console.error(
				"Unable to load events.",
				error);


			eventSelect.innerHTML =
				`<option value="">
					Unable to load events
				</option>`;


			eventSelect.disabled =
				true;


			camsUi.showError(
				alertContainer,
				error.message ??
				"Unable to load events.",
				"individual-event-attendance-report-alert-message");
		}
	}


	function renderEvents(
		events) {

		eventSelect.innerHTML =
			`<option value=""></option>`;


		for (const event of events) {

			const option =
				document.createElement(
					"option");


			option.value =
				event.id;


			const eventDate =
				camsUtils.formatDate(
					event.eventDate);


			option.textContent =
				eventDate
					? `${event.name} - ${eventDate}`
					: event.name;


			eventSelect.appendChild(
				option);
		}
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
						"individual-event-attendance"));


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


			const report =
				payload.data;


			renderReport(
				report);


			setExportButtonsEnabled(
				true);
		}
		catch (error) {

			console.error(
				"Unable to generate individual " +
				"event attendance report.",
				error);


			recordCount.textContent =
				"0";


			clearReportHeader();


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to generate report.",
				{
					columnCount:
						3
				});


			camsUi.showError(
				alertContainer,
				error.message ??
				"Unable to generate report.",
				"individual-event-attendance-report-alert-message");
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
		report) {

		const items =
			report?.attendances ??
			[];


		recordCount.textContent =
			items.length.toString();


		selectedEventName.textContent =
			report?.eventName ??
			"Attendance Records";


		selectedEventDate.textContent =
			report?.eventDate
				? camsUtils.formatDate(
					report.eventDate)
				: "";


		camsTable.renderRows(
			tableBody,
			items,
			renderRow,
			{
				columnCount:
					3,

				title:
					"No Attendance Records",

				message:
					"No members attended this event.",

				icon:
					"ri-user-follow-line"
			});
	}


	function renderRow(
		item) {

		return `
			<tr>

				<td class="ps-3">

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
						"individual-event-attendance/" +
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
				"Individual-Event-Attendance." +
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


	// =========================================================
	// VALIDATION
	// =========================================================

	function validateFilters() {

		camsUi.clearAlert(
			"individual-event-attendance-report-alert-message");


		if (!eventSelect.value) {

			camsUi.showError(
				alertContainer,
				"Please select an event.",
				"individual-event-attendance-report-alert-message");


			return false;
		}


		return true;
	}


	// =========================================================
	// URL
	// =========================================================

	function buildUrl(
		baseUrl) {

		const query =
			new URLSearchParams({
				eventId:
					eventSelect.value
			});


		return `${baseUrl}?${query.toString()}`;
	}


	// =========================================================
	// TIME
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


	// =========================================================
	// REPORT STATE
	// =========================================================

	function clearReportHeader() {

		selectedEventName.textContent =
			"Attendance Records";


		selectedEventDate.textContent =
			"";
	}


	function setExportButtonsEnabled(
		enabled) {

		exportExcelButton.disabled =
			!enabled;

		exportPdfButton.disabled =
			!enabled;
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

})();