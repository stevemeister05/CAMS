(() => {
	"use strict";


	const page =
		document.getElementById(
			"admin-dashboard-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const dashboardDate =
		document.getElementById(
			"dashboard-date");

	const activeMemberCount =
		document.getElementById(
			"active-member-count");

	const todayAttendanceCount =
		document.getElementById(
			"today-attendance-count");

	const todayAttendanceRate =
		document.getElementById(
			"today-attendance-rate");

	const pendingRegistrationCount =
		document.getElementById(
			"pending-registration-count");

	const todayEventCount =
		document.getElementById(
			"today-event-count");

	const todayEventSummary =
		document.getElementById(
			"today-event-summary");

	const todayEventsBody =
		document.getElementById(
			"today-events-body");

	const recentAttendanceBody =
		document.getElementById(
			"recent-attendance-body");

	const upcomingEvents =
		document.getElementById(
			"upcoming-events");

	const refreshButton =
		document.getElementById(
			"dashboard-refresh-button");


	let attendanceChart =
		null;


	// =========================================================
	// INITIALIZE
	// =========================================================

	initialize();


	function initialize() {

		refreshButton.addEventListener(
			"click",
			loadDashboard);


		loadDashboard();
	}


	// =========================================================
	// LOAD
	// =========================================================

	async function loadDashboard() {

		try {

			setLoadingState();


			const response =
				await camsApi.get(
					"/api/v1/dashboard");


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to load dashboard."));
			}


			const data =
				payload.data ??
				payload;


			renderDashboard(
				data);
		}
		catch (error) {

			console.error(
				"Unable to load dashboard.",
				error);


			camsTable.showError(
				todayEventsBody,
				error.message ??
				"Unable to load dashboard.",
				{
					columnCount:
						5
				});


			camsTable.showError(
				recentAttendanceBody,
				error.message ??
				"Unable to load dashboard.",
				{
					columnCount:
						4
				});
		}
	}


	function setLoadingState() {

		camsTable.showLoading(
			todayEventsBody,
			{
				columnCount:
					5,

				message:
					"Loading today's events..."
			});


		camsTable.showLoading(
			recentAttendanceBody,
			{
				columnCount:
					4,

				message:
					"Loading attendance..."
			});
	}


	// =========================================================
	// MAIN RENDER
	// =========================================================

	function renderDashboard(
		data) {

		dashboardDate.textContent =
			camsUtils.formatDate(
				data.date);


		activeMemberCount.textContent =
			formatNumber(
				data.activeMemberCount);


		todayAttendanceCount.textContent =
			formatNumber(
				data.todayAttendanceCount);


		todayAttendanceRate.textContent =
			`${data.todayAttendanceRate ?? 0}% of active members`;


		pendingRegistrationCount.textContent =
			formatNumber(
				data.pendingRegistrationCount);


		const eventSummary =
			data.todayEventSummary ??
			{};


		todayEventCount.textContent =
			formatNumber(
				eventSummary.total);


		todayEventSummary.textContent =
			`${eventSummary.ongoing ?? 0} ongoing · ` +
			`${eventSummary.scheduled ?? 0} scheduled · ` +
			`${eventSummary.completed ?? 0} completed`;


		renderTodayEvents(
			data.todayEvents ??
			[]);


		renderAttendanceTrend(
			data.attendanceTrend ??
			[]);


		renderRecentAttendance(
			data.recentAttendances ??
			[]);


		renderUpcomingEvents(
			data.upcomingEvents ??
			[]);
	}


	// =========================================================
	// TODAY'S EVENTS
	// =========================================================

	function renderTodayEvents(
		events) {

		camsTable.renderRows(
			todayEventsBody,
			events,
			renderTodayEventRow,
			{
				columnCount:
					5,

				title:
					"No events today",

				message:
					"There are no events scheduled for today.",

				icon:
					"ri-calendar-event-line"
			});
	}


	function renderTodayEventRow(
		event) {

		return `
			<tr>

				<td class="ps-3">

					<div class="fw-medium">
						${camsUtils.escapeHtml(
			event.name)}
					</div>

					<div class="text-muted fs-sm">
						${formatEventType(
				event.type)}
					</div>

				</td>


				<td>

					${camsUtils.formatTimeOnly(
					event.startTime)}

					<span class="text-muted">
						–
					</span>

					${camsUtils.formatTimeOnly(
						event.endTime)}

				</td>


				<td>
					${renderStatusBadge(
						event.status)}
				</td>
				<td>

					${renderAttendanceAvailability(
							event)}
				</td>


				<td>

					<span class="fw-medium">
						${formatNumber(
								event.attendanceCount)}
					</span>

				</td>


				<td class="text-end pe-3">

					<a href="/admin/events/${event.id}/attendance"
					   class="btn
							  btn-sm
							  ${event.status === "Ongoing"
				? "btn-primary"
				: "btn-soft-primary"}">

						<i class="ri-qr-code-line
								  align-middle me-1">
						</i>

						Open Attendance

					</a>

				</td>

			</tr>
		`;
	}


	// =========================================================
	// RECENT ATTENDANCE
	// =========================================================

	function renderRecentAttendance(
		attendances) {

		camsTable.renderRows(
			recentAttendanceBody,
			attendances,
			renderAttendanceRow,
			{
				columnCount:
					4,

				title:
					"No attendance yet",

				message:
					"Recent attendance records will appear here.",

				icon:
					"ri-user-follow-line"
			});
	}


	function renderAttendanceRow(
		attendance) {

		return `
			<tr>

				<td class="ps-3">

					<div class="fw-medium">

						${camsUtils.escapeHtml(
			attendance.memberName)}

					</div>

				</td>


				<td>

					${camsUtils.escapeHtml(
				attendance.eventName)}

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
				: `<span class="text-muted">—</span>`
			}

				</td>

			</tr>
		`;
	}


	// =========================================================
	// UPCOMING EVENTS
	// =========================================================

	function renderUpcomingEvents(
		events) {

		if (!events.length) {

			upcomingEvents.innerHTML = `
				<div class="text-center
							text-muted py-4">

					<i class="ri-calendar-line fs-2">
					</i>

					<div class="mt-2">
						No upcoming events.
					</div>

				</div>
			`;

			return;
		}


		upcomingEvents.innerHTML =
			events
				.map(
					event => `
						<div class="d-flex
								 align-items-start
								 gap-3
								 py-3
								 border-bottom">

							<div class="avatar-sm
									 flex-shrink-0">

								<div class="avatar-title
										  bg-primary-subtle
										  text-primary
										  rounded">

									<i class="ri-calendar-event-line
											  fs-4">
									</i>

								</div>

							</div>


							<div class="flex-grow-1">

								<div class="fw-medium">

									${camsUtils.escapeHtml(
						event.name)}

								</div>


								<div class="text-muted fs-sm mt-1">

									${camsUtils.formatDate(
							event.eventDate)}

									·

									${camsUtils.formatTimeOnly(
								event.startTime)}

								</div>


								<div class="mt-2">

									${renderStatusBadge(
									event.status)}

								</div>

							</div>

						</div>
					`)
				.join("");
	}


	// =========================================================
	// CHART
	// =========================================================

	function renderAttendanceTrend(
		trend) {

		const chartElement =
			document.getElementById(
				"attendance-trend-chart");


		if (!chartElement) {
			return;
		}


		const categories =
			trend.map(
				item =>
					formatShortDate(
						item.date));


		const values =
			trend.map(
				item =>
					item.attendanceCount ??
					0);


		if (attendanceChart) {

			attendanceChart.destroy();

			attendanceChart =
				null;
		}


		attendanceChart =
			new ApexCharts(
				chartElement,
				{
					chart:
					{
						type:
							"area",

						height:
							310,

						toolbar:
						{
							show:
								false
						}
					},

					series:
						[
							{
								name:
									"Attendance",

								data:
									values
							}
						],

					xaxis:
					{
						categories:
							categories
					},

					dataLabels:
					{
						enabled:
							false
					},

					stroke:
					{
						curve:
							"smooth"
					},

					yaxis:
					{
						min:
							0,

						forceNiceScale:
							true
					},

					noData:
					{
						text:
							"No attendance data"
					}
				});


		attendanceChart.render();
	}


	// =========================================================
	// STATUS
	// =========================================================

	function renderStatusBadge(
		status) {

		switch (status) {

			case "Ongoing":

				return `
					<span class="badge
								bg-success-subtle
								text-success">

						Ongoing

					</span>
				`;


			case "Completed":

				return `
					<span class="badge
								bg-secondary-subtle
								text-secondary">

						Completed

					</span>
				`;


			default:

				return `
					<span class="badge
								bg-info-subtle
								text-info">

						Scheduled

					</span>
				`;
		}
	}


	// =========================================================
	// FORMATTERS
	// =========================================================

	function formatNumber(
		value) {

		return Number(
			value ??
			0)
			.toLocaleString(
				"en-PH");
	}


	function formatDateTime(
		value) {

		if (!value) {
			return "—";
		}


		return camsUtils.formatDateTime(
			value);
	}


	function formatShortDate(
		value) {

		if (!value) {
			return "";
		}


		const date =
			camsUtils.parseDateTime(
				`${value}T00:00:00`);


		if (!date) {
			return value;
		}


		return date.toLocaleDateString(
			"en-PH",
			{
				month:
					"short",

				day:
					"numeric"
			});
	}


	function formatEventType(
		type) {

		switch (type) {

			case "SundayMass":
				return "Sunday Mass";

			case "MisaDeGallo":
				return "Misa de Gallo";

			case "SpecialEvent":
				return "Special Event";

			default:
				return type ?? "";
		}
	}

	function getAttendanceAvailability(
		event,
		now = new Date()) {

		if (
			!event ||
			!event.eventDate
		) {
			return "NotOpen";
		}


		const today =
			now.toLocaleDateString(
				"en-CA");


		if (
			event.eventDate <
			today
		) {
			return "Closed";
		}


		if (
			event.eventDate >
			today
		) {
			return "NotOpen";
		}


		const currentMinutes =
			now.getHours() * 60 +
			now.getMinutes();


		const timeInStart =
			toMinutes(
				event.attendanceTimeInStart);

		const timeInEnd =
			toMinutes(
				event.attendanceTimeInEnd);

		const timeOutStart =
			toMinutes(
				event.attendanceTimeOutStart);

		const timeOutEnd =
			toMinutes(
				event.attendanceTimeOutEnd);


		const timeInOpen =
			currentMinutes >= timeInStart &&
			currentMinutes <= timeInEnd;


		const timeOutOpen =
			currentMinutes >= timeOutStart &&
			currentMinutes <= timeOutEnd;


		if (
			timeInOpen &&
			timeOutOpen
		) {
			return "TimeInAndTimeOutOpen";
		}


		if (timeInOpen) {
			return "TimeInOpen";
		}


		if (timeOutOpen) {
			return "TimeOutOpen";
		}


		if (
			currentMinutes <
			timeInStart
		) {
			return "NotOpen";
		}


		return "Closed";
	}


	function toMinutes(
		value) {

		if (!value) {
			return 0;
		}


		const parts =
			value
				.toString()
				.split(":");


		return (
			Number(
				parts[0]) *
			60 +
			Number(
				parts[1])
		);
	}

	function renderAttendanceAvailability(
		event) {

		if (
			!event ||
			event.status ===
			"Cancelled"
		) {
			return "";
		}


		const availability =
			getAttendanceAvailability(
				event);


		switch (availability) {

			case "TimeInOpen":

				return `
				<div class="text-primary fs-sm mt-1">

					<i class="ri-login-box-line
							  me-1">
					</i>

					Time-In Open

				</div>
			`;


			case "TimeInAndTimeOutOpen":

				return `
				<div class="text-warning fs-sm mt-1">

					<i class="ri-arrow-left-right-line
							  me-1">
					</i>

					Time-In / Time-Out Open

				</div>
			`;


			case "TimeOutOpen":

				return `
				<div class="text-warning fs-sm mt-1">

					<i class="ri-logout-box-line
							  me-1">
					</i>

					Time-Out Open

				</div>
			`;


			case "Closed":

				return `
				<div class="text-muted fs-sm mt-1">

					<i class="ri-lock-line
							  me-1">
					</i>

					Attendance Closed

				</div>
			`;


			default:

				return "";
		}
	}

})();