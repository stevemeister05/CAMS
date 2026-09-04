(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"event-attendance-page");


	if (!page) {
		return;
	}


	const eventId =
		page.dataset.eventId;


	const AttendanceAction =
		Object.freeze({

			TimeIn:
				Number(
					page.dataset.actionTimeIn),

			TimeOut:
				Number(
					page.dataset.actionTimeOut)
		});


	// =========================================================
	// STATE
	// =========================================================

	let selectedAction =
		"TimeIn";

	let currentEvent =
		null;

	let attendanceRecords =
		[];

	let connection =
		null;

	let qrExpirationTimer =
		null;

	let attendanceWindowTimer =
		null;

	let previousAttendanceWindowState =
		null;

	let attendanceVisible =
		true;


	// =========================================================
	// ELEMENTS
	// =========================================================

	const pageEventName =
		document.getElementById(
			"page-event-name");

	const pageEventDate =
		document.getElementById(
			"page-event-date");

	const qrEventName =
		document.getElementById(
			"qr-event-name");

	const qrEventDate =
		document.getElementById(
			"qr-event-date");

	const qrCode =
		document.getElementById(
			"qr-code");

	const qrAction =
		document.getElementById(
			"qr-action");

	const qrPeriod =
		document.getElementById(
			"qr-period");

	const qrExpiration =
		document.getElementById(
			"qr-expiration");


	const attendanceBody =
		document.getElementById(
			"attendance-table-body");

	const attendanceCount =
		document.getElementById(
			"attendance-count");


	const timeInButton =
		document.getElementById(
			"time-in-button");

	const timeOutButton =
		document.getElementById(
			"time-out-button");


	const qrPanel =
		document.getElementById(
			"qr-panel");

	const attendancePanel =
		document.getElementById(
			"attendance-panel");

	const toggleAttendanceButton =
		document.getElementById(
			"toggle-attendance-button");

	const toggleAttendanceIcon =
		document.getElementById(
			"toggle-attendance-icon");

	const toggleAttendanceText =
		document.getElementById(
			"toggle-attendance-text");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		if (!eventId) {

			await camsUi.showErrorBox(
				"Unable to determine the event.",
				{
					title:
						"Event Not Found"
				});

			return;
		}


		initializeActions();

		initializeAttendanceToggle();


		const loaded =
			await loadEvent();


		if (!loaded) {
			return;
		}


		await initializeSignalR();
	}


	function initializeActions() {

		timeInButton?.addEventListener(
			"click",
			async () => {

				await changeAction(
					"TimeIn");
			});


		timeOutButton?.addEventListener(
			"click",
			async () => {

				await changeAction(
					"TimeOut");
			});


		updateActionButtons();
	}


	// =========================================================
	// LOAD EVENT
	// =========================================================

	async function loadEvent() {

		camsTable.showLoading(
			attendanceBody,
			{
				columnCount:
					3,

				message:
					"Loading attendance..."
			});


		try {

			const response =
				await camsApi.get(
					`/api/v1/attendance/event/${eventId}`);


			const data =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						data,
						response.status,
						"Unable to load event attendance."));
			}


			currentEvent =
				data.event;


			attendanceRecords =
				data.attendances ??
				[];


			selectedAction =
				normalizeAction(
					data.qrAction ??
					"TimeIn");


			renderEvent(
				data);


			startAttendanceWindowWatcher();


			return true;
		}
		catch (error) {

			console.error(
				"Unable to load event attendance.",
				error);


			renderEventError(
				error.message ??
				"Unable to load event attendance.");


			return false;
		}
	}


	// =========================================================
	// EVENT RENDERING
	// =========================================================

	function renderEvent(
		data) {

		const event =
			data.event;


		if (!event) {
			return;
		}


		const eventDate =
			camsUtils.formatDate(
				event.eventDate);


		if (pageEventName) {

			pageEventName.textContent =
				event.name ??
				"Event Attendance";
		}


		if (pageEventDate) {

			pageEventDate.textContent =
				eventDate;
		}


		qrEventName.textContent =
			event.name ??
			"Event Attendance";


		qrEventDate.textContent =
			eventDate;


		renderQr(
			data.qr);


		renderAttendances(
			attendanceRecords);


		updateActionButtons();
	}


	function renderEventError(
		message) {

		qrEventName.textContent =
			"Unable to load event";

		qrEventDate.textContent =
			"";


		renderQrError(
			message);


		camsTable.showError(
			attendanceBody,
			message,
			{
				columnCount:
					3
			});


		attendanceCount.textContent =
			"0";
	}


	// =========================================================
	// QR CODE
	// =========================================================

	async function loadQr() {

		if (
			!eventId ||
			!currentEvent
		) {
			return;
		}


		const windowState =
			getAttendanceWindowState(
				selectedAction);


		if (
			windowState.state !==
			"Open"
		) {

			renderAttendanceWindowMessage(
				selectedAction,
				windowState.state);

			return;
		}


		renderQrLoading();


		try {

			const response =
				await camsApi.get(
					"/api/v1/attendance/qr" +
					`?eventId=${eventId}` +
					`&action=${selectedAction}`);


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to load QR code."));
			}


			const qr =
				payload.data ??
				payload;


			renderQr(
				qr);
		}
		catch (error) {

			console.error(
				"Unable to load QR code.",
				error);


			qrCode.innerHTML = `
				<div class="text-danger text-center">

					<i class="ri-error-warning-line fs-2">
					</i>

					<div class="mt-2">

						${camsUtils.escapeHtml(
				error.message)}

					</div>

				</div>
			`;
		}
	}


	function renderQr(
		qr) {

		clearQrExpirationTimer();


		const windowState =
			getAttendanceWindowState(
				selectedAction);


		if (
			windowState.state !==
			"Open"
		) {

			renderAttendanceWindowMessage(
				selectedAction,
				windowState.state);

			return;
		}


		if (
			!qr ||
			!qr.token
		) {

			qrCode.innerHTML = `
				<div class="text-muted text-center">

					<i class="ri-qr-code-line fs-1"></i>

					<div class="mt-2">
						QR code is not available.
					</div>

				</div>
			`;


			qrAction.textContent =
				"-";

			qrPeriod.textContent =
				"";

			qrExpiration.textContent =
				"";


			return;
		}


		/*
		 * Ignore SignalR QR updates for another
		 * attendance action.
		 *
		 * This is useful if the Hub still has this
		 * client subscribed to an older action group.
		 */
		const qrActionName =
			normalizeAction(
				qr.action);


		if (
			qrActionName !==
			selectedAction
		) {
			return;
		}


		qrCode.innerHTML =
			"";


		const size =
			Math.min(
				qrCode.clientWidth || 300,
				qrCode.clientHeight || 300);


		new QRCode(
			qrCode,
			{
				text:
					qr.token,

				width:
					size,

				height:
					size,

				correctLevel:
					QRCode.CorrectLevel.M
			});


		qrAction.textContent =
			formatAction(
				selectedAction);


		qrAction.className =
			selectedAction === "TimeIn"
				? "badge bg-success-subtle text-success qr-action-badge"
				: "badge bg-warning-subtle text-warning qr-action-badge";


		qrPeriod.textContent =
			getAttendancePeriod(
				selectedAction);


		startQrExpiration(
			qr.expiresAt);
	}


	function renderQrLoading() {

		qrCode.innerHTML = `
			<div class="text-muted text-center">

				<div class="spinner-border
							spinner-border-sm
							text-primary"
					 role="status">
				</div>

				<div class="mt-2">
					Loading QR Code...
				</div>

			</div>
		`;
	}


	function renderQrError(
		message) {

		clearQrExpirationTimer();


		qrCode.innerHTML = `
			<div class="text-danger text-center">

				<i class="ri-error-warning-line fs-2"></i>

				<div class="mt-2">
					${camsUtils.escapeHtml(
			message)}
				</div>

			</div>
		`;


		qrExpiration.textContent =
			"";
	}


	// =========================================================
	// QR EXPIRATION
	// =========================================================

	function startQrExpiration(
		expiresAt) {

		clearQrExpirationTimer();


		if (!expiresAt) {

			qrExpiration.textContent =
				"";

			return;
		}


		const expiration =
			camsUtils.parseDateTime(
				expiresAt);


		if (!expiration) {

			qrExpiration.textContent =
				"";

			return;
		}


		const update =
			() => {

				const remaining =
					expiration.getTime() -
					Date.now();


				if (
					remaining <= 0
				) {

					qrExpiration.textContent =
						"QR code expired.";

					clearQrExpirationTimer();

					return;
				}


				const seconds =
					Math.ceil(
						remaining /
						1000);


				qrExpiration.textContent =
					`Refreshes in ${seconds} second${seconds === 1
						? ""
						: "s"
					}.`;
			};


		update();


		qrExpirationTimer =
			window.setInterval(
				update,
				1000);
	}


	function clearQrExpirationTimer() {

		if (!qrExpirationTimer) {
			return;
		}


		window.clearInterval(
			qrExpirationTimer);


		qrExpirationTimer =
			null;
	}


	// =========================================================
	// ATTENDANCE
	// =========================================================

	function renderAttendances(
		attendances) {

		const items =
			attendances ??
			[];


		attendanceCount.textContent =
			items.length.toString();


		camsTable.renderRows(
			attendanceBody,
			items,
			createAttendanceRow,
			{
				columnCount:
					3,

				title:
					"No attendance yet",

				message:
					"Attendance records will appear here as members scan.",

				icon:
					"ri-user-follow-line"
			});
	}


	function createAttendanceRow(
		attendance) {

		return `
			<tr data-attendance-id="${camsUtils.escapeHtml(
			getAttendanceKey(
				attendance))
			}">

				<td class="ps-3">

					<div class="member-name">
						${camsUtils.escapeHtml(
				attendance.memberName ??
				"-")}
					</div>

				</td>

				<td>

					${camsUtils.formatTime(
						attendance.timeIn,
						{
							assumeUtc: true
						})}

				</td>

				<td>

					${attendance.timeOut
					? camsUtils.formatTime(
						attendance.timeOut,
						{
							assumeUtc: true
						})
				: `
							<span class="text-muted">
								—
							</span>
						`
			}

				</td>

			</tr>
		`;
	}


	// =========================================================
	// LIVE ATTENDANCE UPDATE
	// =========================================================

	function updateAttendanceList(
		attendance) {

		if (!attendance) {
			return;
		}


		const incomingKey =
			getAttendanceKey(
				attendance);


		const existingIndex =
			attendanceRecords
				.findIndex(
					item =>
						getAttendanceKey(
							item) ===
						incomingKey);


		if (
			existingIndex >= 0
		) {

			attendanceRecords[
				existingIndex
			] = {
				...attendanceRecords[
				existingIndex
				],
				...attendance
			};
		}
		else {

			attendanceRecords.unshift(
				attendance);
		}


		renderAttendances(
			attendanceRecords);
	}


	function getAttendanceKey(
		attendance) {

		return String(
			attendance.id ??
			attendance.attendanceId ??
			attendance.memberId ??
			attendance.memberName ??
			"");
	}


	// =========================================================
	// ATTENDANCE ACTION
	// =========================================================

	async function changeAction(
		action) {

		if (
			selectedAction ===
			action
		) {
			return;
		}


		selectedAction =
			action;


		updateActionButtons();


		previousAttendanceWindowState =
			getAttendanceWindowState(
				selectedAction)
				.state;


		/*
		 * The Hub previously accepted:
		 *
		 * JoinEvent(eventId, action)
		 *
		 * so rejoin using the newly selected action.
		 */
		await joinSelectedEvent();


		await loadQr();
	}


	function updateActionButtons() {

		const timeIn =
			selectedAction ===
			"TimeIn";


		timeInButton.className =
			timeIn
				? "btn btn-primary"
				: "btn btn-soft-primary";


		timeOutButton.className =
			timeIn
				? "btn btn-soft-primary"
				: "btn btn-primary";
	}


	function getAttendancePeriod(
		action) {

		if (!currentEvent) {
			return "";
		}


		if (
			action ===
			"TimeIn"
		) {

			return (
				camsUtils.formatTimeOnly(
					currentEvent.attendanceTimeInStart) +
				" – " +
				camsUtils.formatTimeOnly(
					currentEvent.attendanceTimeInEnd)
			);
		}


		return (
			camsUtils.formatTimeOnly(
				currentEvent.attendanceTimeOutStart) +
			" – " +
			camsUtils.formatTimeOnly(
				currentEvent.attendanceTimeOutEnd)
		);
	}


	function formatAction(
		action) {

		return action ===
			"TimeOut"
			? "TIME OUT"
			: "TIME IN";
	}


	function normalizeAction(
		action) {

		if (
			action ===
			"TimeOut"
		) {
			return "TimeOut";
		}


		if (
			action ===
			"TimeIn"
		) {
			return "TimeIn";
		}


		const numericValue =
			Number(
				action);


		if (
			numericValue ===
			AttendanceAction.TimeOut
		) {
			return "TimeOut";
		}


		return "TimeIn";
	}


	function getActionValue(
		action) {

		return action ===
			"TimeOut"
			? AttendanceAction.TimeOut
			: AttendanceAction.TimeIn;
	}
	// =========================================================
	// SIGNALR
	// =========================================================

	async function initializeSignalR() {

		try {

			connection =
				new signalR
					.HubConnectionBuilder()
					.withUrl(
						"/hubs/attendance")
					.withAutomaticReconnect()
					.build();


			// ---------------------------------------------
			// QR UPDATED
			// ---------------------------------------------

			connection.on(
				"QrUpdated",
				qr => {

					if (!qr) {
						return;
					}


					if (
						String(
							qr.eventId)
							.toLowerCase() !==
						String(
							eventId)
							.toLowerCase()
					) {
						return;
					}


					if (
						normalizeAction(
							qr.action) !==
						selectedAction
					) {
						return;
					}


					/*
					 * renderQr() performs another attendance
					 * window check before displaying the QR.
					 *
					 * This prevents a late SignalR update
					 * from redisplaying the QR after the
					 * selected attendance period has closed.
					 */
					renderQr(
						qr);
				});


			// ---------------------------------------------
			// ATTENDANCE RECORDED
			// ---------------------------------------------

			connection.on(
				"AttendanceRecorded",
				attendance => {

					if (!attendance) {
						return;
					}


					if (
						attendance.eventId &&
						String(
							attendance.eventId)
							.toLowerCase() !==
						String(
							eventId)
							.toLowerCase()
					) {
						return;
					}


					updateAttendanceList(
						attendance);
				});


			// ---------------------------------------------
			// RECONNECTED
			// ---------------------------------------------

			connection.onreconnected(
				async () => {

					await joinSelectedEvent();
				});


			connection.onreconnecting(
				error => {

					console.warn(
						"Attendance SignalR reconnecting...",
						error);
				});


			connection.onclose(
				error => {

					if (error) {

						console.warn(
							"Attendance SignalR connection closed.",
							error);
					}
				});


			await connection.start();


			await joinSelectedEvent();
		}
		catch (error) {

			console.error(
				"Unable to initialize attendance SignalR.",
				error);


			camsUi.showErrorBox(
				"Live attendance updates could not be started. " +
				"You can refresh the page to try again.",
				{
					title:
						"Live Updates Unavailable"
				});
		}
	}


	async function joinSelectedEvent() {

		if (
			!connection ||
			connection.state !==
			signalR.HubConnectionState.Connected
		) {
			return;
		}


		if (
			!currentEvent
		) {
			return;
		}


		const windowState =
			getAttendanceWindowState(
				selectedAction);


		/*
		 * Don't subscribe to QR refreshes while
		 * the selected attendance window is closed
		 * or has not started yet.
		 */
		if (
			windowState.state !==
			"Open"
		) {
			return;
		}


		try {

			await connection.invoke(
				"JoinEvent",
				eventId,
				getActionValue(
					selectedAction));
		}
		catch (error) {

			console.error(
				"Unable to join attendance event group.",
				error);
		}
	}


	// =========================================================
	// ATTENDANCE PANEL TOGGLE
	// =========================================================

	function initializeAttendanceToggle() {

		if (
			!toggleAttendanceButton ||
			!attendancePanel ||
			!qrPanel
		) {
			return;
		}


		toggleAttendanceButton.addEventListener(
			"click",
			toggleAttendance);
	}


	function toggleAttendance() {

		attendanceVisible =
			!attendanceVisible;


		attendancePanel.classList.toggle(
			"d-none",
			!attendanceVisible);


		qrPanel.classList.toggle(
			"col-lg-6",
			attendanceVisible);


		qrPanel.classList.toggle(
			"col-lg-12",
			!attendanceVisible);


		if (
			toggleAttendanceText
		) {

			toggleAttendanceText.textContent =
				attendanceVisible
					? "Hide Attendance"
					: "Show Attendance";
		}


		if (
			toggleAttendanceIcon
		) {

			toggleAttendanceIcon.className =
				attendanceVisible
					? "ri-layout-right-line align-middle me-1"
					: "ri-layout-left-line align-middle me-1";
		}


		toggleAttendanceButton.setAttribute(
			"aria-expanded",
			attendanceVisible.toString());
	}


	// =========================================================
	// DATE / TIME
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


		return date.toLocaleTimeString(
			"en-PH",
			{
				hour:
					"numeric",

				minute:
					"2-digit",

				hour12:
					true
			});
	}


	function getAttendanceWindowState(
		action) {

		if (
			!currentEvent ||
			!currentEvent.eventDate
		) {

			return {
				state:
					"Unavailable"
			};
		}


		/*
		 * Attendance windows are based on Philippine
		 * local time regardless of the browser/device
		 * timezone.
		 */
		const now =
			getPhilippineDateTime();


		const eventDate =
			String(
				currentEvent.eventDate)
				.substring(
					0,
					10);


		/*
		 * Event already occurred on a previous date.
		 */
		if (
			eventDate <
			now.date
		) {

			return {
				state:
					"Closed"
			};
		}


		/*
		 * Event is scheduled for a future date.
		 */
		if (
			eventDate >
			now.date
		) {

			return {
				state:
					"NotStarted"
			};
		}


		let start;
		let end;


		if (
			action ===
			"TimeOut"
		) {

			start =
				toSeconds(
					currentEvent
						.attendanceTimeOutStart);

			end =
				toSeconds(
					currentEvent
						.attendanceTimeOutEnd);
		}
		else {

			start =
				toSeconds(
					currentEvent
						.attendanceTimeInStart);

			end =
				toSeconds(
					currentEvent
						.attendanceTimeInEnd);
		}


		if (
			start === null ||
			end === null
		) {

			return {
				state:
					"Unavailable"
			};
		}


		/*
		 * Selected attendance action has not commenced.
		 */
		if (
			now.seconds <
			start
		) {

			return {
				state:
					"NotStarted"
			};
		}


		/*
		 * Selected attendance action already ended.
		 */
		if (
			now.seconds >
			end
		) {

			return {
				state:
					"Closed"
			};
		}


		return {
			state:
				"Open"
		};
	}


	function toSeconds(
		value) {

		if (!value) {
			return null;
		}


		const parts =
			value
				.toString()
				.split(":");


		const hours =
			Number(
				parts[0]);

		const minutes =
			Number(
				parts[1]);

		const seconds =
			Number(
				parts[2] ??
				0);


		if (
			Number.isNaN(
				hours) ||
			Number.isNaN(
				minutes) ||
			Number.isNaN(
				seconds)
		) {
			return null;
		}


		return (
			hours *
			3600 +
			minutes *
			60 +
			seconds
		);
	}


	function getPhilippineDateTime() {

		const parts =
			new Intl.DateTimeFormat(
				"en-US",
				{
					timeZone:
						"Asia/Manila",

					year:
						"numeric",

					month:
						"2-digit",

					day:
						"2-digit",

					hour:
						"2-digit",

					minute:
						"2-digit",

					second:
						"2-digit",

					hourCycle:
						"h23"
				})
				.formatToParts(
					new Date());


		const values =
			{};


		for (const part of parts) {

			if (
				part.type ===
				"literal"
			) {
				continue;
			}


			values[
				part.type
			] =
				part.value;
		}


		const hours =
			Number(
				values.hour);

		const minutes =
			Number(
				values.minute);

		const seconds =
			Number(
				values.second);


		return {
			date:
				`${values.year}-${values.month}-${values.day}`,

			seconds:
				hours *
				3600 +
				minutes *
				60 +
				seconds
		};
	}


	// =========================================================
	// ATTENDANCE WINDOW MESSAGE
	// =========================================================

	function renderAttendanceWindowMessage(
		action,
		state) {

		clearQrExpirationTimer();


		qrCode.innerHTML =
			"";


		const isTimeOut =
			action ===
			"TimeOut";


		let icon;
		let title;
		let message;


		if (
			state ===
			"NotStarted"
		) {

			icon =
				"ri-time-line";


			title =
				isTimeOut
					? "Time-Out Not Available"
					: "Time-In Not Available";


			message =
				isTimeOut
					? "The time-out attendance period has not started yet."
					: "The time-in attendance period has not started yet.";
		}
		else if (
			state ===
			"Closed"
		) {

			icon =
				"ri-lock-line";


			title =
				"Attendance Period Closed";


			message =
				isTimeOut
					? "The time-out attendance period has been closed."
					: "The time-in attendance period has been closed.";
		}
		else {

			icon =
				"ri-error-warning-line";


			title =
				"Attendance Unavailable";


			message =
				"The attendance period is not available.";
		}


		qrCode.innerHTML = `
			<div class="text-center px-4">

				<div class="avatar-lg mx-auto mb-3">

					<div class="avatar-title
							bg-light
							text-muted
							rounded-circle
							fs-1">

						<i class="${icon}">
						</i>

					</div>

				</div>


				<h5 class="mb-2">

					${title}

				</h5>


				<p class="text-muted mb-0">

					${message}

				</p>

			</div>
		`;


		/*
		 * Keep the selected attendance action and configured
		 * period visible even when the QR itself is hidden.
		 */
		qrAction.textContent =
			formatAction(
				action);


		qrAction.className =
			action ===
				"TimeIn"
				? "badge bg-success-subtle text-success qr-action-badge"
				: "badge bg-warning-subtle text-warning qr-action-badge";


		qrPeriod.textContent =
			getAttendancePeriod(
				action);


		qrExpiration.textContent =
			"";
	}


	// =========================================================
	// ATTENDANCE WINDOW WATCHER
	// =========================================================

	function startAttendanceWindowWatcher() {

		if (attendanceWindowTimer) {

			window.clearInterval(
				attendanceWindowTimer);
		}


		previousAttendanceWindowState =
			getAttendanceWindowState(
				selectedAction)
				.state;


		attendanceWindowTimer =
			window.setInterval(
				async () => {

					if (!currentEvent) {
						return;
					}


					const currentState =
						getAttendanceWindowState(
							selectedAction)
							.state;


					/*
					 * Only refresh the QR display when the
					 * attendance-window state changes.
					 *
					 * For example:
					 *
					 * NotStarted -> Open
					 * Open -> Closed
					 */
					if (
						currentState ===
						previousAttendanceWindowState
					) {
						return;
					}


					previousAttendanceWindowState =
						currentState;


					if (
						currentState ===
						"Open"
					) {
						await joinSelectedEvent();
					}


					await loadQr();

				},
				1000);
	}


})();