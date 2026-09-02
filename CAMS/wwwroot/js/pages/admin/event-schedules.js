(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"event-schedules-page");


	if (!page) {
		return;
	}


	const EventType =
		Object.freeze({

			SundayMass:
				Number(
					page.dataset.eventTypeSundayMass),

			MisaDeGallo:
				Number(
					page.dataset.eventTypeMisaDeGallo)

		});


	const Sunday =
		Number(
			page.dataset.sunday);


	// =========================================================
	// STATE
	// =========================================================

	let currentPage =
		1;

	let totalCount =
		0;

	let searchTimer =
		null;


	// =========================================================
	// ELEMENTS
	// =========================================================

	const tableBody =
		document.getElementById(
			"schedule-table-body");

	const summary =
		document.getElementById(
			"schedule-summary");

	const pagination =
		document.getElementById(
			"schedule-pagination");


	const searchInput =
		document.getElementById(
			"schedule-search");

	const typeFilter =
		document.getElementById(
			"schedule-type-filter");

	const statusFilter =
		document.getElementById(
			"schedule-status-filter");

	const pageSizeSelect =
		document.getElementById(
			"schedule-page-size");


	const createButton =
		document.getElementById(
			"create-schedule-button");

	const regenerateButton =
		document.getElementById(
			"regenerate-events-button");


	// =========================================================
	// MODAL
	// =========================================================

	const modalElement =
		document.getElementById(
			"schedule-modal");

	const scheduleModal =
		bootstrap.Modal.getOrCreateInstance(
			modalElement);


	const modalTitle =
		document.getElementById(
			"schedule-modal-title");

	const form =
		document.getElementById(
			"schedule-form");

	const formError =
		document.getElementById(
			"schedule-form-error");

	const saveButton =
		document.getElementById(
			"save-schedule-button");


	const idInput =
		document.getElementById(
			"schedule-id");

	const nameInput =
		document.getElementById(
			"schedule-name");

	const eventTypeInput =
		document.getElementById(
			"schedule-event-type");


	const startTimeInput =
		document.getElementById(
			"schedule-start-time");

	const endTimeInput =
		document.getElementById(
			"schedule-end-time");


	const timeInStartInput =
		document.getElementById(
			"schedule-time-in-start");

	const timeInEndInput =
		document.getElementById(
			"schedule-time-in-end");


	const timeOutStartInput =
		document.getElementById(
			"schedule-time-out-start");

	const timeOutEndInput =
		document.getElementById(
			"schedule-time-out-end");


	const sundaySection =
		document.getElementById(
			"sunday-schedule-section");

	const seasonalSection =
		document.getElementById(
			"seasonal-schedule-section");


	const startMonthInput =
		document.getElementById(
			"schedule-start-month");

	const startDayInput =
		document.getElementById(
			"schedule-start-day");

	const endMonthInput =
		document.getElementById(
			"schedule-end-month");

	const endDayInput =
		document.getElementById(
			"schedule-end-day");


	const activeInput =
		document.getElementById(
			"schedule-is-active");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		initializeEvents();

		await loadSchedules();
	}


	function initializeEvents() {

		createButton?.addEventListener(
			"click",
			openCreateModal);


		eventTypeInput?.addEventListener(
			"change",
			() => {

				updateScheduleTypeFields();

				updateScheduleName();
			});

		startTimeInput?.addEventListener(
			"input",
			updateScheduleName);

		form?.addEventListener(
			"submit",
			saveSchedule);


		regenerateButton?.addEventListener(
			"click",
			regenerateEvents);


		searchInput?.addEventListener(
			"input",
			() => {

				window.clearTimeout(
					searchTimer);


				searchTimer =
					window.setTimeout(
						async () => {

							currentPage =
								1;

							await loadSchedules();

						},
						350);
			});


		typeFilter?.addEventListener(
			"change",
			async () => {

				currentPage =
					1;

				await loadSchedules();
			});


		statusFilter?.addEventListener(
			"change",
			async () => {

				currentPage =
					1;

				await loadSchedules();
			});


		pageSizeSelect?.addEventListener(
			"change",
			async () => {

				currentPage =
					1;

				await loadSchedules();
			});


		tableBody?.addEventListener(
			"click",
			handleTableAction);
	}


	// =========================================================
	// LOAD SCHEDULES
	// =========================================================

	async function loadSchedules() {

		camsTable.showLoading(
			tableBody,
			{
				columnCount:
					7,

				message:
					"Loading event schedules..."
			});


		try {

			const params =
				new URLSearchParams();


			params.set(
				"Page",
				currentPage.toString());


			params.set(
				"PageSize",
				getPageSize()
					.toString());


			const search =
				camsUtils.toNullableString(
					searchInput.value);


			if (search) {

				params.set(
					"Filter.Search",
					search);
			}


			if (typeFilter.value) {

				params.set(
					"Filter.EventType",
					typeFilter.value);
			}


			if (statusFilter.value) {

				params.set(
					"Filter.IsActive",
					statusFilter.value);
			}

			params.set("SortBy[0].Name", "EventType");
			params.set("SortBy[1].Name", "StartTime");

			const response =
				await camsApi.get(
					"/api/v1/event-schedules?" +
					params.toString());

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
						"Unable to load event schedules."));
			}


			const result =
				payload.data ??
				payload;


			const items =
				result.items ??
				[];


			totalCount =
				result.totalCount ??
				0;


			currentPage =
				result.page ??
				currentPage;


			camsTable.renderRows(
				tableBody,
				items,
				createScheduleRow,
				{
					columnCount:
						7,

					title:
						"No event schedules found",

					message:
						"No schedules match the selected filters.",

					icon:
						"ri-calendar-event-line"
				});


			renderSummary(
				result);


			renderPagination(
				result);
		}
		catch (error) {

			console.error(
				"Unable to load event schedules.",
				error);


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to load event schedules.",
				{
					columnCount:
						7
				});


			summary.textContent =
				"";


			pagination.innerHTML =
				"";
		}
	}


	// =========================================================
	// ROW
	// =========================================================

	function createScheduleRow(
		schedule) {

		const eventType =
			normalizeEventType(
				schedule.eventType);


		const eventTypeName =
			formatEventType(
				eventType);


		const recurrence =
			formatRecurrence(
				schedule,
				eventType);


		return `
			<tr>

				<td class="ps-3">

					<div class="fw-semibold">

						${camsUtils.escapeHtml(
			schedule.name ??
			"-")}

					</div>

				</td>


				<td>

					${renderEventTypeBadge(
				eventType,
				eventTypeName)}

				</td>


				<td>

					<div class="fw-medium">

						${camsUtils.escapeHtml(
					camsUtils.formatTimeOnly(
						schedule.startTime))}

						<span class="text-muted mx-1">
							–
						</span>

						${camsUtils.escapeHtml(
							camsUtils.formatTimeOnly(
								schedule.endTime))}

					</div>

				</td>


				<td>

					${camsUtils.escapeHtml(
									recurrence)}

				</td>


				<td>

					<div class="fs-13">

						<div>

							<span class="text-muted">
								In:
							</span>

							${camsUtils.escapeHtml(
										camsUtils.formatTimeOnly(
											schedule.attendanceTimeInStart))}

							–

							${camsUtils.escapeHtml(
												camsUtils.formatTimeOnly(
													schedule.attendanceTimeInEnd))}

						</div>

						<div class="mt-1">

							<span class="text-muted">
								Out:
							</span>

							${camsUtils.escapeHtml(
														camsUtils.formatTimeOnly(
															schedule.attendanceTimeOutStart))}

							–

							${camsUtils.escapeHtml(
																camsUtils.formatTimeOnly(
																	schedule.attendanceTimeOutEnd))}

						</div>

					</div>

				</td>


				<td>

					${schedule.isActive
				? `
							<span class="badge bg-success-subtle text-success">
								Active
							</span>
						`
				: `
							<span class="badge bg-secondary-subtle text-secondary">
								Inactive
							</span>
						`
			}

				</td>


				<td class="text-end pe-3">

					<div class="btn-group btn-group-sm">

						<button type="button"
								class="btn btn-soft-primary schedule-edit-button"
								data-id="${camsUtils.escapeHtml(
				schedule.id)}"
								title="Edit">

							<i class="ri-pencil-line">
							</i>

						</button>

						<button type="button"
								class="btn btn-soft-danger schedule-delete-button"
								data-id="${camsUtils.escapeHtml(
					schedule.id)}"
								data-name="${camsUtils.escapeHtml(
						schedule.name)}"
								title="Delete">

							<i class="ri-delete-bin-line">
							</i>

						</button>

					</div>

				</td>

			</tr>
		`;
	}


	async function handleTableAction(
		event) {

		const editButton =
			event.target.closest(
				".schedule-edit-button");


		if (editButton) {

			await openEditModal(
				editButton.dataset.id);

			return;
		}


		const deleteButton =
			event.target.closest(
				".schedule-delete-button");


		if (deleteButton) {

			await deleteSchedule(
				deleteButton.dataset.id,
				deleteButton.dataset.name);
		}
	}


	// =========================================================
	// CREATE
	// =========================================================

	function openCreateModal() {

		resetForm();


		modalTitle.textContent =
			"Add Event Schedule";


		activeInput.checked =
			true;


		scheduleModal.show();
	}


	// =========================================================
	// EDIT
	// =========================================================

	async function openEditModal(
		id) {

		resetForm();


		try {

			const response =
				await camsApi.get(
					`/api/v1/event-schedules/${id}`);

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
						"Unable to load event schedule."));
			}


			const schedule =
				payload.data ??
				payload;


			idInput.value =
				schedule.id ??
				"";


			nameInput.value =
				schedule.name ??
				"";


			eventTypeInput.value =
				normalizeEventType(
					schedule.eventType)
					.toString();


			startTimeInput.value =
				camsUtils.toTimeInput(
					schedule.startTime);


			endTimeInput.value =
				camsUtils.toTimeInput(
					schedule.endTime);


			timeInStartInput.value =
				camsUtils.toTimeInput(
					schedule.attendanceTimeInStart);


			timeInEndInput.value =
				camsUtils.toTimeInput(
					schedule.attendanceTimeInEnd);


			timeOutStartInput.value =
				camsUtils.toTimeInput(
					schedule.attendanceTimeOutStart);


			timeOutEndInput.value =
				camsUtils.toTimeInput(
					schedule.attendanceTimeOutEnd);


			startMonthInput.value =
				schedule.startMonth ??
				"";


			startDayInput.value =
				schedule.startDay ??
				"";


			endMonthInput.value =
				schedule.endMonth ??
				"";


			endDayInput.value =
				schedule.endDay ??
				"";


			activeInput.checked =
				schedule.isActive ===
				true;


			updateScheduleTypeFields();


			modalTitle.textContent =
				"Edit Event Schedule";


			scheduleModal.show();
		}
		catch (error) {

			console.error(
				"Unable to load event schedule.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to load event schedule.",
				{
					title:
						"Event Schedule"
				});
		}
	}


	// =========================================================
	// SAVE
	// =========================================================

	async function saveSchedule(
		event) {

		event.preventDefault();


		camsForm.hideError(
			formError);


		camsForm.resetValidation(
			form);


		if (
			!camsForm.validate(
				form)
		) {
			return;
		}


		if (!validateScheduleRules()) {
			return;
		}


		const id =
			camsUtils.toNullableString(
				idInput.value);


		const isEdit =
			Boolean(
				id);


		const request =
			buildRequest();


		camsUi.setButtonLoading(
			saveButton,
			true,
			"Saving...");


		try {

			const response =
				isEdit
					? await camsApi.put(
						`/api/v1/event-schedules/${id}`,
						request)
					: await camsApi.post(
						"/api/v1/event-schedules",
						request);


			/*
			 * camsApi may return null when it has already
			 * handled an authentication redirect.
			 */
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
						isEdit
							? "Unable to update event schedule."
							: "Unable to create event schedule."));
			}


			scheduleModal.hide();


			if (!isEdit) {

				currentPage =
					1;
			}


			await loadSchedules();


			camsUi.showSuccessToast(
				isEdit
					? "Event schedule updated successfully."
					: "Event schedule created successfully.",
				{
					title:
						"Event Schedule"
				});
		}
		catch (error) {

			console.error(
				"Unable to save event schedule.",
				error);


			camsForm.showError(
				formError,
				error.message ??
				"Unable to save event schedule.");
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	function validateScheduleRules() {

		if (
			startTimeInput.value >=
			endTimeInput.value
		) {

			camsForm.showError(
				formError,
				"Event start time must be earlier than the end time.");

			return false;
		}


		if (
			timeInStartInput.value >=
			timeInEndInput.value
		) {

			camsForm.showError(
				formError,
				"Attendance time-in start must be earlier than the end.");

			return false;
		}


		if (
			timeOutStartInput.value >=
			timeOutEndInput.value
		) {

			camsForm.showError(
				formError,
				"Attendance time-out start must be earlier than the end.");

			return false;
		}


		const eventType =
			Number(
				eventTypeInput.value);


		if (
			eventType ===
			EventType.MisaDeGallo
		) {

			if (
				!validateMonthDay(
					startMonthInput,
					startDayInput)
			) {

				camsForm.showError(
					formError,
					"The seasonal start date is invalid.");

				return false;
			}


			if (
				!validateMonthDay(
					endMonthInput,
					endDayInput)
			) {

				camsForm.showError(
					formError,
					"The seasonal end date is invalid.");

				return false;
			}
		}


		return true;
	}


	function validateMonthDay(
		monthInput,
		dayInput) {

		const month =
			Number(
				monthInput.value);

		const day =
			Number(
				dayInput.value);


		if (
			month < 1 ||
			month > 12 ||
			day < 1
		) {
			return false;
		}


		const daysInMonth =
			new Date(
				2000,
				month,
				0)
				.getDate();


		return day <=
			daysInMonth;
	}


	function buildRequest() {

		const eventType =
			Number(
				eventTypeInput.value);


		const isSundayMass =
			eventType ===
			EventType.SundayMass;


		return {

			name:
				nameInput.value.trim(),

			eventType:
				eventType,

			startTime:
				camsUtils.normalizeTimeRequest(
					startTimeInput.value),

			endTime:
				camsUtils.normalizeTimeRequest(
					endTimeInput.value),

			attendanceTimeInStart:
				camsUtils.normalizeTimeRequest(
					timeInStartInput.value),

			attendanceTimeInEnd:
				camsUtils.normalizeTimeRequest(
					timeInEndInput.value),

			attendanceTimeOutStart:
				camsUtils.normalizeTimeRequest(
					timeOutStartInput.value),

			attendanceTimeOutEnd:
				camsUtils.normalizeTimeRequest(
					timeOutEndInput.value),

			dayOfWeek:
				isSundayMass
					? Sunday
					: null,

			startMonth:
				isSundayMass
					? null
					: Number(
						startMonthInput.value),

			startDay:
				isSundayMass
					? null
					: Number(
						startDayInput.value),

			endMonth:
				isSundayMass
					? null
					: Number(
						endMonthInput.value),

			endDay:
				isSundayMass
					? null
					: Number(
						endDayInput.value),

			isActive:
				activeInput.checked
		};
	}


	// =========================================================
	// DELETE
	// =========================================================

	async function deleteSchedule(
	id,
	name) {

	const confirmed =
		await camsUi.confirm(
			`Delete "${name}"? ` +
			"This will remove the event schedule configuration.",
			{
				title:
					"Delete Event Schedule",

				type:
					"danger",

				confirmText:
					"Delete",

				cancelText:
					"Cancel"
			});


	if (!confirmed) {
		return;
	}


	try {

		const response =
			await camsApi.delete(
				`/api/v1/event-schedules/${id}`);


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
					"Unable to delete event schedule."));
		}


		/*
		 * If deleting the last item on a page,
		 * moving back one page avoids displaying
		 * an empty page unnecessarily.
		 */
		if (
			tableBody.querySelectorAll(
				"tr[data-id]")
				.length === 1 &&
			currentPage > 1
		) {

			currentPage--;
		}


		await loadSchedules();


		camsUi.showSuccessToast(
			"Event schedule deleted successfully.",
			{
				title:
					"Event Schedule"
			});
	}
	catch (error) {

		console.error(
			"Unable to delete event schedule.",
			error);


		await camsUi.showErrorBox(
			error.message ??
				"Unable to delete event schedule.",
			{
				title:
					"Delete Event Schedule"
			});
	}
}


	// =========================================================
	// REGENERATE
	// =========================================================

	async function regenerateEvents() {

		const confirmed =
			await camsUi.confirm(
				"Regenerate all future events using the current active event schedules?",
				{
					title:
						"Regenerate Events",

					type:
						"warning",

					confirmText:
						"Regenerate",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		camsUi.setButtonLoading(
			regenerateButton,
			true,
			"Regenerating...");


		try {

			/*
			 * This endpoint has no request model, so using
			 * camsApi.fetch avoids sending an unnecessary
			 * JSON request body.
			 */
			const response =
				await camsApi.fetch(
					"/api/v1/eventgeneration/regenerate",
					{
						method:
							"POST"
					});


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
						"Unable to regenerate future events."));
			}


			camsUi.showSuccessToast(
				"Future events regenerated successfully.",
				{
					title:
						"Event Regeneration"
				});
		}
		catch (error) {

			console.error(
				"Unable to regenerate future events.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to regenerate future events.",
				{
					title:
						"Event Regeneration"
				});
		}
		finally {

			camsUi.setButtonLoading(
				regenerateButton,
				false);
		}
	}


	// =========================================================
	// TYPE-SPECIFIC FIELDS
	// =========================================================

	function updateScheduleTypeFields() {

		const eventType =
			Number(
				eventTypeInput.value);


		const isSundayMass =
			eventType ===
			EventType.SundayMass;


		const isMisaDeGallo =
			eventType ===
			EventType.MisaDeGallo;


		sundaySection.classList.toggle(
			"d-none",
			!isSundayMass);


		seasonalSection.classList.toggle(
			"d-none",
			!isMisaDeGallo);


		seasonalSection
			.querySelectorAll(
				".seasonal-field")
			.forEach(
				field => {

					field.required =
						isMisaDeGallo;
				});


		if (isSundayMass) {

			startMonthInput.value =
				"";

			startDayInput.value =
				"";

			endMonthInput.value =
				"";

			endDayInput.value =
				"";
		}


		camsForm.clearFieldState(
			eventTypeInput);
	}


	// =========================================================
	// FORM RESET
	// =========================================================

	function resetForm() {

		camsForm.reset(
			form);


		camsForm.resetValidation(
			form);


		camsForm.hideError(
			formError);


		idInput.value =
			"";


		activeInput.checked =
			true;


		sundaySection.classList.add(
			"d-none");


		seasonalSection.classList.add(
			"d-none");


		seasonalSection
			.querySelectorAll(
				".seasonal-field")
			.forEach(
				field => {

					field.required =
						false;
				});
	}


	// =========================================================
	// FORMAT
	// =========================================================

	function normalizeEventType(
		value) {

		if (
			value ===
			"SundayMass"
		) {
			return EventType.SundayMass;
		}


		if (
			value ===
			"MisaDeGallo"
		) {
			return EventType.MisaDeGallo;
		}


		const numericValue =
			Number(
				value);


		return Number.isNaN(
			numericValue)
			? 0
			: numericValue;
	}


	function formatEventType(
		eventType) {

		switch (eventType) {

			case EventType.SundayMass:
				return "Sunday Mass";

			case EventType.MisaDeGallo:
				return "Misa de Gallo";

			default:
				return "Unknown";
		}
	}


	function renderEventTypeBadge(
		eventType,
		name) {

		if (
			eventType ===
			EventType.SundayMass
		) {

			return `
				<span class="badge bg-primary-subtle text-primary">

					${camsUtils.escapeHtml(
				name)}

				</span>
			`;
		}


		return `
			<span class="badge bg-warning-subtle text-warning">

				${camsUtils.escapeHtml(
			name)}

			</span>
		`;
	}


	function formatRecurrence(
		schedule,
		eventType) {

		if (
			eventType ===
			EventType.SundayMass
		) {
			return "Every Sunday";
		}


		if (
			eventType ===
			EventType.MisaDeGallo
		) {

			return (
				formatMonthDay(
					schedule.startMonth,
					schedule.startDay) +
				" – " +
				formatMonthDay(
					schedule.endMonth,
					schedule.endDay)
			);
		}


		return "—";
	}


	function formatMonthDay(
		month,
		day) {

		if (
			!month ||
			!day
		) {
			return "—";
		}


		const date =
			new Date(
				2000,
				Number(month) - 1,
				Number(day));


		return date.toLocaleDateString(
			"en-PH",
			{
				month:
					"short",

				day:
					"numeric"
			});
	}


	// =========================================================
	// PAGINATION
	// =========================================================

	function renderSummary(
		result) {

		const pageSize =
			result.pageSize ??
			getPageSize();


		if (
			totalCount ===
			0
		) {

			summary.textContent =
				"No schedules";

			return;
		}


		const start =
			((currentPage - 1) *
				pageSize) +
			1;


		const end =
			Math.min(
				currentPage *
				pageSize,
				totalCount);


		summary.textContent =
			`Showing ${start}–${end} of ${totalCount}`;
	}


	function renderPagination(
		result) {

		const pageSize =
			result.pageSize ??
			getPageSize();


		const totalPages =
			Math.max(
				1,
				Math.ceil(
					totalCount /
					pageSize));


		if (
			totalCount ===
			0
		) {

			pagination.innerHTML =
				"";

			return;
		}


		pagination.innerHTML = `

			<button type="button"
					class="btn btn-sm btn-light"
					id="schedule-previous-page"
					${currentPage <= 1
				? "disabled"
				: ""}>

				<i class="ri-arrow-left-s-line">
				</i>

			</button>


			<span class="text-muted fs-13 px-2">

				Page ${currentPage} of ${totalPages}

			</span>


			<button type="button"
					class="btn btn-sm btn-light"
					id="schedule-next-page"
					${currentPage >= totalPages
				? "disabled"
				: ""}>

				<i class="ri-arrow-right-s-line">
				</i>

			</button>
		`;


		document
			.getElementById(
				"schedule-previous-page")
			?.addEventListener(
				"click",
				async () => {

					if (
						currentPage <=
						1
					) {
						return;
					}


					currentPage--;


					await loadSchedules();
				});


		document
			.getElementById(
				"schedule-next-page")
			?.addEventListener(
				"click",
				async () => {

					if (
						currentPage >=
						totalPages
					) {
						return;
					}


					currentPage++;


					await loadSchedules();
				});
	}

	function updateScheduleName() {

		const eventType =
			Number(
				eventTypeInput.value);


		const startTime =
			startTimeInput.value;


		if (
			!eventType ||
			!startTime
		) {

			nameInput.value =
				"";

			return;
		}


		let eventName;


		switch (eventType) {

			case EventType.SundayMass:

				eventName =
					"Sunday Mass";

				break;


			case EventType.MisaDeGallo:

				eventName =
					"Misa de Gallo";

				break;


			default:

				nameInput.value =
					"";

				return;
		}


		const formattedTime =
			camsUtils.formatTimeOnly(
				startTime);


		nameInput.value =
			`${eventName} - ${formattedTime}`;
	}


	function getPageSize() {

		return Number(
			pageSizeSelect.value) ||
			20;
	}
})();