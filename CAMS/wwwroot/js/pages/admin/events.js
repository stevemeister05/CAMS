(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const pageContainer =
		document.getElementById(
			"event-management-page");


	if (!pageContainer) {
		return;
	}


	const EventType = Object.freeze({

		SundayMass:
			Number(
				pageContainer.dataset.eventTypeSundayMass),

		MisaDeGallo:
			Number(
				pageContainer.dataset.eventTypeMisaDeGallo),

		SpecialEvent:
			Number(
				pageContainer.dataset.eventTypeSpecialEvent)
	});


	const EventStatus = Object.freeze({

		Scheduled:
			Number(
				pageContainer.dataset.eventStatusScheduled),

		Ongoing:
			Number(
				pageContainer.dataset.eventStatusOngoing),

		Completed:
			Number(
				pageContainer.dataset.eventStatusCompleted)
	});


	const SPECIAL_EVENT_TYPE =
		String(
			EventType.SpecialEvent);


	const canManageEvents =
		pageContainer.dataset.canManageEvents ===
		"true";


	// =========================================================
	// STATE
	// =========================================================

	const state = {

		page:
			1,

		pageSize:
			20,

		search:
			"",

		type:
			null,

		status:
			null,

		dateFrom:
			null,

		dateTo:
			null,

		selectedEventId:
			null,

		mode:
			null
	};


	let searchTimer =
		null;


	// =========================================================
	// ELEMENTS
	// =========================================================

	const tableBody =
		document.getElementById(
			"event-table-body");

	const searchInput =
		document.getElementById(
			"event-search");

	const typeFilter =
		document.getElementById(
			"event-type-filter");

	const statusFilter =
		document.getElementById(
			"event-status-filter");

	const dateFrom =
		document.getElementById(
			"event-date-from");

	const dateTo =
		document.getElementById(
			"event-date-to");

	const pageSize =
		document.getElementById(
			"event-page-size");

	const refreshButton =
		document.getElementById(
			"event-refresh-button");

	const createButton =
		document.getElementById(
			"create-event-button");

	const resultSummary =
		document.getElementById(
			"event-result-summary");

	const paginationInfo =
		document.getElementById(
			"event-pagination-info");

	const pagination =
		document.getElementById(
			"event-pagination");


	// Modal

	const modalElement =
		document.getElementById(
			"event-modal");

	const modal =
		bootstrap.Modal.getOrCreateInstance(
			modalElement);

	const form =
		document.getElementById(
			"event-form");

	const modalTitle =
		document.getElementById(
			"event-modal-title");

	const formError =
		document.getElementById(
			"event-form-error");

	const saveButton =
		document.getElementById(
			"event-save-button");


	// Form fields

	const nameInput =
		document.getElementById(
			"event-name");

	const typeInput =
		document.getElementById(
			"event-type");

	const dateInput =
		document.getElementById(
			"event-date");

	const statusInput =
		document.getElementById(
			"event-status");

	const descriptionInput =
		document.getElementById(
			"event-description");

	const startTimeInput =
		document.getElementById(
			"event-start-time");

	const endTimeInput =
		document.getElementById(
			"event-end-time");

	const timeInStartInput =
		document.getElementById(
			"attendance-time-in-start");

	const timeInEndInput =
		document.getElementById(
			"attendance-time-in-end");

	const timeOutStartInput =
		document.getElementById(
			"attendance-time-out-start");

	const timeOutEndInput =
		document.getElementById(
			"attendance-time-out-end");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	function initialize() {

		initializeEvents();

		loadEvents();
	}


	function initializeEvents() {

		searchInput.addEventListener(
			"input",
			handleSearch);


		typeFilter.addEventListener(
			"change",
			() => {

				state.type =
					typeFilter.value ||
					null;

				state.page =
					1;

				loadEvents();
			});


		statusFilter.addEventListener(
			"change",
			() => {

				state.status =
					statusFilter.value ||
					null;

				state.page =
					1;

				loadEvents();
			});


		dateFrom.addEventListener(
			"change",
			handleDateFilter);


		dateTo.addEventListener(
			"change",
			handleDateFilter);


		pageSize.addEventListener(
			"change",
			() => {

				state.pageSize =
					Number(
						pageSize.value);

				state.page =
					1;

				loadEvents();
			});


		refreshButton.addEventListener(
			"click",
			loadEvents);


		createButton?.addEventListener(
			"click",
			openCreateModal);


		tableBody.addEventListener(
			"click",
			handleTableAction);


		form.addEventListener(
			"submit",
			handleSave);


		modalElement.addEventListener(
			"hidden.bs.modal",
			resetForm);
	}


	// =========================================================
	// FILTERING
	// =========================================================

	function handleSearch() {

		clearTimeout(
			searchTimer);


		searchTimer =
			setTimeout(
				() => {

					state.search =
						searchInput.value.trim();

					state.page =
						1;

					loadEvents();
				},
				350);
	}


	function handleDateFilter() {

		state.dateFrom =
			dateFrom.value ||
			null;

		state.dateTo =
			dateTo.value ||
			null;

		state.page =
			1;


		loadEvents();
	}


	// =========================================================
	// LOAD EVENTS
	// =========================================================

	async function loadEvents() {

		camsTable.showLoading(
			tableBody,
			{
				columnCount:
					6,

				message:
					"Loading events..."
			});


		try {

			const response =
				await camsApi.get(
					buildSearchUrl());


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to load events."));
			}


			renderEvents(
				result);
		}
		catch (error) {

			console.error(
				"Unable to load events.",
				error);


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to load events.",
				{
					columnCount:
						6
				});
		}
	}


	function buildSearchUrl() {

		const params =
			new URLSearchParams();


		params.set(
			"page",
			state.page.toString());

		params.set(
			"pageSize",
			state.pageSize.toString());


		if (state.search) {

			params.set(
				"filter.search",
				state.search);
		}


		if (state.type) {

			params.set(
				"filter.type",
				state.type);
		}


		if (state.status) {

			params.set(
				"filter.status",
				state.status);
		}


		if (state.dateFrom) {

			params.set(
				"filter.eventDateFrom",
				state.dateFrom);
		}


		if (state.dateTo) {

			params.set(
				"filter.eventDateTo",
				state.dateTo);
		}


		params.set(
			"sortBy[0].name",
			"Status");

		params.set(
			"sortBy[0].sortDescending",
			"false");


		params.set(
			"sortBy[1].name",
			"EventDate");

		params.set(
			"sortBy[1].sortDescending",
			"false");

		return (
			"/api/v1/events?" +
			params.toString()
		);
	}


	// =========================================================
	// TABLE
	// =========================================================

	function renderEvents(
		result) {

		const items =
			result?.items ??
			[];


		camsTable.renderRows(
			tableBody,
			items,
			createEventRow,
			{
				columnCount:
					6,

				title:
					"No events found",

				message:
					"Try changing the current filters.",

				icon:
					"ri-calendar-search-line"
			});


		const paginationInfo =
			camsTable.getPaginationInfo(
				result,
				{
					page:
						state.page,

					pageSize:
						state.pageSize
				});


		camsTable.renderSummary(
			resultSummary,
			document.getElementById(
				"event-pagination-info"),
			{
				totalCount:
					paginationInfo.totalCount,

				currentPage:
					paginationInfo.page,

				pageSize:
					paginationInfo.pageSize,

				singularLabel:
					"event",

				pluralLabel:
					"events"
			});


		camsTable.renderPagination(
			pagination,
			{
				currentPage:
					paginationInfo.page,

				totalPages:
					paginationInfo.totalPages,

				onPageChange:
					page => {

						state.page =
							page;

						loadEvents();
					}
			});
	}


	function createEventRow(
		event) {

		const typeValue =
			normalizeEnumValue(
				event.type,
				typeFilter);

		const specialEvent =
			typeValue ===
			SPECIAL_EVENT_TYPE;


		let managementActions =
			"";

		if (event.status !== 3 && event.status !== 4) {
			if (canManageEvents) {


				managementActions += `
				<button type="button"
						class="btn btn-soft-secondary btn-sm"
						data-event-action="edit"
						data-event-id="${event.id}"
						title="Edit Event">

					<i class="ri-edit-line"></i>

				</button>
			`;


				if (specialEvent) {

					managementActions += `
					<button type="button"
							class="btn btn-soft-danger btn-sm"
							data-event-action="delete"
							data-event-id="${event.id}"
							data-event-name="${camsUtils.escapeHtml(
						event.name)}"
							title="Delete Event">

						<i class="ri-delete-bin-line"></i>

					</button>
				`;
				}
			}
		}
		
		let scanQrAction = "";

		if (event.status !== 3 && event.status !== 4) {
			scanQrAction = `
			<a href="/admin/events/${event.id}/attendance"
				class="btn btn-soft-primary btn-sm"
				title="Attendance">

				<i class="ri-qr-scan-2-line"></i>

			</a>`
		}

		return `
			<tr>

				<td>

					<div class="fw-medium">
						${camsUtils.escapeHtml(
			event.name)}
					</div>

					${event.description
				? `
							<div class="text-muted fs-sm
										text-truncate"
								 style="max-width: 300px;">

								${camsUtils.escapeHtml(
					event.description)}

							</div>
						`
				: ""
			}

				</td>


				<td>

					${getEventTypeBadge(
						event.type)}

				</td>


				<td>

					${camsUtils.formatDate(
					event.eventDate)}

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

					${getEventStatusBadge(
						event.status)}

				</td>


				<td class="text-end">

					<div class="d-inline-flex gap-1">

						${scanQrAction}

						${managementActions}

					</div>

				</td>

			</tr>
		`;
	}


	// =========================================================
	// ACTIONS
	// =========================================================

	function handleTableAction(
		event) {

		const button =
			event.target.closest(
				"[data-event-action]");


		if (!button) {
			return;
		}


		const action =
			button.dataset.eventAction;

		const id =
			button.dataset.eventId;


		switch (action) {

			case "edit":

				openEditModal(
					id);

				break;


			case "delete":

				deleteEvent(
					id,
					button.dataset.eventName);

				break;
		}
	}


	// =========================================================
	// CREATE
	// =========================================================

	function openCreateModal() {

		resetForm();


		state.mode =
			"create";

		state.selectedEventId =
			null;


		modalTitle.textContent =
			"Create Special Event";


		/*
		 * Only Special Events may be manually created.
		 */
		typeInput.value =
			SPECIAL_EVENT_TYPE;


		saveButton.textContent =
			"Create Event";


		modal.show();
	}


	// =========================================================
	// EDIT
	// =========================================================

	async function openEditModal(
		id) {

		resetForm();


		state.mode =
			"edit";

		state.selectedEventId =
			id;


		modalTitle.textContent =
			"Edit Event";


		try {

			camsUi.setButtonLoading(
				saveButton,
				true,
				"Loading...");


			modal.show();


			const response =
				await camsApi.get(
					`/api/v1/events/${id}`);


			const event =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						event,
						response.status,
						"Unable to load the event."));
			}


			populateForm(
				event);


			saveButton.textContent =
				"Save Changes";
		}
		catch (error) {

			modal.hide();


			await camsUi.showErrorBox(
				error.message ??
				"Unable to load the event.",
				{
					title:
						"Unable to Load Event"
				});
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	function populateForm(
		event) {

		nameInput.value =
			event.name ??
			"";

		typeInput.value =
			normalizeEnumValue(
				event.type,
				typeInput);

		dateInput.value =
			event.eventDate ??
			"";

		statusInput.value =
			normalizeEnumValue(
				event.status,
				statusInput);

		descriptionInput.value =
			event.description ??
			"";

		startTimeInput.value =
			camsUtils.toTimeInput(
				event.startTime);

		endTimeInput.value =
			camsUtils.toTimeInput(
				event.endTime);

		timeInStartInput.value =
			camsUtils.toTimeInput(
				event.attendanceTimeInStart);

		timeInEndInput.value =
			camsUtils.toTimeInput(
				event.attendanceTimeInEnd);

		timeOutStartInput.value =
			camsUtils.toTimeInput(
				event.attendanceTimeOutStart);

		timeOutEndInput.value =
			camsUtils.toTimeInput(
				event.attendanceTimeOutEnd);
	}


	// =========================================================
	// SAVE
	// =========================================================

	async function handleSave(
		event) {

		event.preventDefault();


		camsForm.hideError(
			formError);


		if (!form.checkValidity()) {

			form.classList.add(
				"was-validated");

			form.reportValidity();

			return;
		}


		if (!validateTimes()) {
			return;
		}


		const request =
			buildRequest();


		try {

			camsUi.setButtonLoading(
				saveButton,
				true,
				state.mode === "create"
					? "Creating..."
					: "Saving...");


			const url =
				state.mode === "create"
					? "/api/v1/events"
					: `/api/v1/events/${state.selectedEventId}`;


			const response =
				await camsApi.fetch(
					url,
					{
						method:
							state.mode === "create"
								? "POST"
								: "PUT",

						body:
							JSON.stringify(
								request)
					});


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to save the event."));
			}


			modal.hide();


			await camsUi.showSuccessBox(
				state.mode === "create"
					? "Special event created successfully."
					: "Event updated successfully.",
				{
					title:
						state.mode === "create"
							? "Event Created"
							: "Event Updated"
				});


			await loadEvents();
		}
		catch (error) {

			camsForm.showError(
				formError,
				"Unable to save the event.");
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	function buildRequest() {

		return {

			name:
				nameInput.value.trim(),

			/*
			 * Although the select is disabled visually,
			 * we explicitly include its value.
			 */
			type:
				Number(
					typeInput.value),

			eventDate:
				dateInput.value,

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

			status:
				Number(
					statusInput.value),

			description:
				descriptionInput.value.trim() ||
				null
		};
	}


	// =========================================================
	// DELETE
	// =========================================================

	async function deleteEvent(
		id,
		name) {

		const confirmed =
			await camsUi.confirm(
				`Delete "${name}"? This action cannot be undone.`,
				{
					title:
						"Delete Special Event",

					type:
						"danger",

					confirmText:
						"Yes, Delete",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		try {

			const response =
				await camsApi.fetch(
					`/api/v1/events/${id}`,
					{
						method:
							"DELETE"
					});


			if (!response.ok) {

				const result =
					await camsApi.readJson(
						response);


				throw new Error(
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to delete the event."));
			}


			await camsUi.showSuccessBox(
				"Special event deleted successfully.",
				{
					title:
						"Event Deleted"
				});


			await loadEvents();
		}
		catch (error) {

			await camsUi.showErrorBox(
				error.message ??
				"Unable to delete the event.",
				{
					title:
						"Delete Failed"
				});
		}
	}


	// =========================================================
	// VALIDATION
	// =========================================================

	function validateTimes() {

		if (
			endTimeInput.value <=
			startTimeInput.value
		) {

			camsForm.showError(
				formError,
				"Event end time must be later than the start time.");

			return false;
		}


		if (
			timeInEndInput.value <=
			timeInStartInput.value
		) {

			camsForm.showError(
				formError,
				"Attendance time-in end must be later than its start time.");

			return false;
		}


		if (
			timeOutEndInput.value <=
			timeOutStartInput.value
		) {

			camsForm.showError(
				formError,
				"Attendance time-out end must be later than its start time.");

			return false;
		}


		return true;
	}


	// =========================================================
	// FORM
	// =========================================================

	function resetForm() {

		camsForm.reset(
			form,
			{
				errorContainer:
					formError
			});


		state.mode =
			null;

		state.selectedEventId =
			null;
	}


	// =========================================================
	// ENUM HELPERS
	// =========================================================

	function normalizeEnumValue(
		value,
		select) {

		const raw =
			String(
				value ?? "");


		const options =
			[...select.options];


		const direct =
			options.find(
				option =>
					option.value === raw);


		if (direct) {
			return direct.value;
		}


		const normalized =
			raw
				.replace(
					/\s/g,
					"")
				.toLowerCase();


		const match =
			options.find(
				option =>
					option.textContent
						.replace(
							/\s/g,
							"")
						.toLowerCase() ===
					normalized);


		return match?.value ??
			raw;
	}


	function getEnumLabel(
		value,
		select) {

		const normalized =
			normalizeEnumValue(
				value,
				select);


		const option =
			[...select.options]
				.find(
					item =>
						item.value ===
						normalized);


		return option?.textContent?.trim() ??
			String(value ?? "-");
	}

	function getEventStatusBadge(
		status) {

		const statusValue =
			Number(
				normalizeEnumValue(
					status,
					statusFilter));


		const label =
			getEnumLabel(
				status,
				statusFilter);


		let badgeClass;
		let iconClass;


		switch (statusValue) {

			case EventStatus.Scheduled:

				badgeClass =
					"bg-info-subtle text-info";

				iconClass =
					"ri-time-line";

				break;


			case EventStatus.Ongoing:

				badgeClass =
					"bg-success-subtle text-success";

				iconClass =
					"ri-play-circle-line";

				break;


			case EventStatus.Completed:

				badgeClass =
					"bg-secondary-subtle text-primary";

				iconClass =
					"ri-checkbox-circle-line";

				break;


			default:

				badgeClass =
					"bg-secondary-subtle text-danger";

				iconClass =
					"ri-question-line";

				break;
		}


		return `
		<span class="badge ${badgeClass}">

			<i class="${iconClass} me-1"></i>

			${camsUtils.escapeHtml(
			label)}

		</span>
	`;
	}

	function getEventTypeBadge(
		type) {

		const typeValue =
			Number(
				normalizeEnumValue(
					type,
					typeFilter));


		const label =
			getEnumLabel(
				type,
				typeFilter);


		let badgeClass;
		let iconClass;


		switch (typeValue) {

			case EventType.SundayMass:

				badgeClass =
					"bg-primary-subtle text-primary";

				iconClass =
					"ri-church-line";

				break;


			case EventType.MisaDeGallo:

				badgeClass =
					"bg-warning-subtle text-warning";

				iconClass =
					"ri-sun-line";

				break;


			case EventType.SpecialEvent:

				badgeClass =
					"bg-info-subtle text-info";

				iconClass =
					"ri-calendar-event-line";

				break;


			default:

				badgeClass =
					"bg-secondary-subtle text-secondary";

				iconClass =
					"ri-calendar-line";

				break;
		}


		return `
		<span class="badge ${badgeClass}">

			<i class="${iconClass} me-1"></i>

			${camsUtils.escapeHtml(
			label)}

		</span>
	`;
	}

})();