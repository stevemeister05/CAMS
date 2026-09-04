(() => {
	"use strict";


	const page =
		document.getElementById(
			"member-attendance-count-report-page");


	if (!page) {
		return;
	}


	const form =
		document.getElementById(
			"member-attendance-count-report-form");

	const dateFrom =
		document.getElementById(
			"date-from");

	const dateTo =
		document.getElementById(
			"date-to");

	const minimumCount =
		document.getElementById(
			"minimum-count");

	const maximumCount =
		document.getElementById(
			"maximum-count");

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
			"member-attendance-count-report-alert");


	initialize();


	function initialize() {

		setDefaultDates();


		form?.addEventListener(
			"submit",
			async event => {

				event.preventDefault();


				await loadReport();
			});


		[
			dateFrom,
			dateTo,
			minimumCount,
			maximumCount
		].forEach(element => {

			element?.addEventListener(
				"change",
				() => {

					setExportButtonsEnabled(
						false);
				});
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
					2,

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
						"member-attendance-count"));


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
				"Unable to generate member " +
				"attendance count report.",
				error);


			recordCount.textContent =
				"0";


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to generate report.",
				{
					columnCount:
						2
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
					2,

				title:
					"No Members Found",

				message:
					"No members matched the " +
					"selected report filters.",

				icon:
					"ri-group-line"
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

				<td class="text-center pe-3">

					<span class="badge
								 bg-primary-subtle
								 text-primary">

						${Number(
				item.attendanceCount ??
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
						"member-attendance-count/" +
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
				"Member-Attendance-Count-" +
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


	function buildUrl(
		baseUrl) {

		const query =
			new URLSearchParams();


		query.set(
			"dateFrom",
			dateFrom.value);

		query.set(
			"dateTo",
			dateTo.value);


		if (minimumCount.value !== "") {
			query.set(
				"minimumCount",
				minimumCount.value);
		}


		if (maximumCount.value !== "") {
			query.set(
				"maximumCount",
				maximumCount.value);
		}


		return `${baseUrl}?${query.toString()}`;
	}


	function validateFilters() {

		camsUi.clearAlert(
			"member-attendance-count-report-alert-message");


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


		const minimum =
			minimumCount.value === ""
				? null
				: Number(
					minimumCount.value);


		const maximum =
			maximumCount.value === ""
				? null
				: Number(
					maximumCount.value);


		if (
			minimum !== null &&
			minimum < 0
		) {

			showFilterError(
				"Minimum attendance count cannot be negative.");


			return false;
		}


		if (
			maximum !== null &&
			maximum < 0
		) {

			showFilterError(
				"Maximum attendance count cannot be negative.");


			return false;
		}


		if (
			minimum !== null &&
			maximum !== null &&
			minimum >
			maximum
		) {

			showFilterError(
				"Minimum attendance count cannot be " +
				"greater than maximum attendance count.");


			return false;
		}


		return true;
	}


	function showFilterError(
		message) {

		camsUi.showError(
			alertContainer,
			message,
			"member-attendance-count-report-alert-message");
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