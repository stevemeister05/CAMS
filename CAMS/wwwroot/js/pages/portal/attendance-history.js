(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"portal-attendance-history-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const alertContainer =
		document.getElementById(
			"attendance-history-alert");

	const tableBody =
		document.getElementById(
			"attendance-history-body");

	const attendanceCount =
		document.getElementById(
			"attendance-history-count");

	const refreshButton =
		document.getElementById(
			"refresh-attendance-history-button");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		refreshButton?.addEventListener(
			"click",
			loadAttendanceHistory);


		await loadAttendanceHistory();
	}


	// =========================================================
	// LOAD HISTORY
	// =========================================================

	async function loadAttendanceHistory() {

		camsUi.clearAlert(
			"attendance-history-alert-message");


		camsTable.showLoading(
			tableBody,
			{
				columnCount:
					5,

				message:
					"Loading attendance history..."
			});


		camsUi.setButtonLoading(
			refreshButton,
			true,
			"Refreshing...");


		try {

			const response =
				await camsApi.get(
					"/api/v1/portal/attendance-history");


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
						"Unable to load attendance history."));
			}


			const items =
				payload.data ??
				[];


			renderAttendanceHistory(
				items);
		}
		catch (error) {

			console.error(
				"Unable to load attendance history.",
				error);


			attendanceCount.textContent =
				"0";


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to load attendance history.",
				{
					columnCount:
						5
				});


			camsUi.showError(
				alertContainer,
				error.message ??
				"Unable to load attendance history.",
				"attendance-history-alert-message");
		}
		finally {

			camsUi.setButtonLoading(
				refreshButton,
				false);
		}
	}


	// =========================================================
	// RENDERING
	// =========================================================

	function renderAttendanceHistory(
		items) {

		attendanceCount.textContent =
			items.length.toString();


		camsTable.renderRows(
			tableBody,
			items,
			renderAttendanceRow,
			{
				columnCount:
					5,

				title:
					"No Attendance Records",

				message:
					"You do not have any attendance records yet.",

				icon:
					"ri-calendar-check-line"
			});
	}


	function renderAttendanceRow(
		item) {

		const eventName =
			camsUtils.escapeHtml(
				item.eventName ??
				"Event");


		const eventDate =
			item.eventDate
				? camsUtils.formatDate(
					item.eventDate)
				: "—";


		const timeIn =
			camsUtils.formatTime(
				item.timeIn,
				{
					assumeUtc: true
				});

		const timeOut =
			camsUtils.formatTime(
				item.timeOut,
				{
					assumeUtc: true
				});


		const method =
			formatAttendanceMethod(
				item);


		return `
			<tr>

				<td class="ps-3">

					<div class="fw-semibold">
						${eventName}
					</div>

				</td>


				<td>
					${camsUtils.escapeHtml(
			eventDate)}
				</td>


				<td>
					${timeIn}
				</td>


				<td>
					${timeOut}
				</td>


				<td class="pe-3">
					${method}
				</td>

			</tr>
		`;
	}


	// =========================================================
	// FORMATTING
	// =========================================================

	function formatAttendanceTime(
		value) {

		if (!value) {
			return "—";
		}


		const date =
			camsUtils.parseDateTime(
				value);


		if (!date) {

			return camsUtils.escapeHtml(
				value);
		}


		return camsUtils.escapeHtml(
			date.toLocaleTimeString(
				"en-PH",
				{
					hour:
						"numeric",

					minute:
						"2-digit",

					hour12:
						true
				}));
	}


	function formatAttendanceMethod(
		item) {

		const timeInMethod =
			formatMethod(
				item.timeInMethod);

		const timeOutMethod =
			formatMethod(
				item.timeOutMethod);


		if (
			timeInMethod ===
			timeOutMethod
		) {

			return renderMethodBadge(
				timeInMethod);
		}


		return `
			<div class="d-flex
						flex-column
						gap-1">

				<div>
					<span class="text-muted me-1">
						In:
					</span>

					${renderMethodBadge(
			timeInMethod)}
				</div>

				<div>
					<span class="text-muted me-1">
						Out:
					</span>

					${renderMethodBadge(
				timeOutMethod)}
				</div>

			</div>
		`;
	}


	function formatMethod(
		value) {

		if (
			value === null ||
			value === undefined
		) {
			return "—";
		}


		const normalized =
			String(value);


		switch (
		normalized.toLowerCase()
		) {

			case "qrcode":
			case "qr code":
				return "QR Code";

			case "fingerprint":
				return "Fingerprint";

			case "manual":
				return "Manual";

			default:
				return normalized;
		}
	}


	function renderMethodBadge(
		value) {

		if (
			!value ||
			value === "—"
		) {
			return `
				<span class="text-muted">
					—
				</span>
			`;
		}


		let badgeClass =
			"bg-secondary-subtle text-secondary";

		let icon =
			"ri-checkbox-circle-line";


		switch (value) {

			case "QR Code":

				badgeClass =
					"bg-primary-subtle text-primary";

				icon =
					"ri-qr-scan-2-line";

				break;


			case "Fingerprint":

				badgeClass =
					"bg-info-subtle text-info";

				icon =
					"ri-fingerprint-line";

				break;


			case "Manual":

				badgeClass =
					"bg-warning-subtle text-warning";

				icon =
					"ri-hand-coin-line";

				break;
		}


		return `
			<span class="badge ${badgeClass}">

				<i class="${icon}
						  align-middle me-1">
				</i>

				${camsUtils.escapeHtml(
			value)}

			</span>
		`;
	}

})();