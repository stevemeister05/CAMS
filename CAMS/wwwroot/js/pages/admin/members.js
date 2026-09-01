(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"member-management-page");


	if (!page) {
		return;
	}


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

		isActive:
			"",

		gender:
			"",

		selectedMemberId:
			null
	};


	// =========================================================
	// FILTER ELEMENTS
	// =========================================================

	const searchInput =
		document.getElementById(
			"member-search");

	const statusFilter =
		document.getElementById(
			"member-status-filter");

	const genderFilter =
		document.getElementById(
			"member-gender-filter");

	const pageSizeSelect =
		document.getElementById(
			"member-page-size");

	const searchButton =
		document.getElementById(
			"member-search-button");

	const resetButton =
		document.getElementById(
			"member-reset-button");


	// =========================================================
	// TABLE
	// =========================================================

	const tableBody =
		document.getElementById(
			"member-table-body");

	const tableSummary =
		document.getElementById(
			"member-table-summary");

	const totalCount =
		document.getElementById(
			"member-total-count");

	const paginationInfo =
		document.getElementById(
			"member-pagination-info");

	const pagination =
		document.getElementById(
			"member-pagination");


	// =========================================================
	// MODAL / FORM
	// =========================================================

	const modalElement =
		document.getElementById(
			"member-modal");

	const modal =
		new bootstrap.Modal(
			modalElement);


	const form =
		document.getElementById(
			"member-form");

	const formError =
		document.getElementById(
			"member-form-error");

	const modalName =
		document.getElementById(
			"member-modal-name");

	const saveButton =
		document.getElementById(
			"member-save-button");


	const firstNameInput =
		document.getElementById(
			"member-first-name");

	const middleNameInput =
		document.getElementById(
			"member-middle-name");

	const lastNameInput =
		document.getElementById(
			"member-last-name");

	const mobileNumberInput =
		document.getElementById(
			"member-mobile-number");

	const birthDateInput =
		document.getElementById(
			"member-birth-date");

	const genderInput =
		document.getElementById(
			"member-gender");

	const addressInput =
		document.getElementById(
			"member-address");


	// =========================================================
	// INITIALIZE
	// =========================================================

	initialize();


	function initialize() {

		camsForm.makeNumeric(
			mobileNumberInput,
			11);


		registerEvents();


		loadMembers();
	}


	// =========================================================
	// EVENTS
	// =========================================================

	function registerEvents() {

		searchButton.addEventListener(
			"click",
			applyFilters);


		resetButton.addEventListener(
			"click",
			resetFilters);


		searchInput.addEventListener(
			"keydown",
			event => {

				if (
					event.key !==
					"Enter"
				) {
					return;
				}


				event.preventDefault();

				applyFilters();
			});


		statusFilter.addEventListener(
			"change",
			applyFilters);


		genderFilter.addEventListener(
			"change",
			applyFilters);


		pageSizeSelect.addEventListener(
			"change",
			() => {

				state.pageSize =
					Number(
						pageSizeSelect.value);

				state.page =
					1;


				loadMembers();
			});


		tableBody.addEventListener(
			"click",
			handleTableClick);


		form.addEventListener(
			"submit",
			handleSave);


		modalElement.addEventListener(
			"hidden.bs.modal",
			resetForm);
	}


	// =========================================================
	// FILTERS
	// =========================================================

	function applyFilters() {

		state.search =
			searchInput.value.trim();

		state.isActive =
			statusFilter.value;

		state.gender =
			genderFilter.value;

		state.page =
			1;


		loadMembers();
	}


	function resetFilters() {

		searchInput.value =
			"";

		statusFilter.value =
			"";

		genderFilter.value =
			"";

		pageSizeSelect.value =
			"20";


		state.search =
			"";

		state.isActive =
			"";

		state.gender =
			"";

		state.page =
			1;

		state.pageSize =
			20;


		loadMembers();
	}


	// =========================================================
	// LOAD MEMBERS
	// =========================================================

	async function loadMembers() {

		try {

			camsTable.showLoading(
				tableBody,
				{
					columnCount:
						7,

					message:
						"Loading members..."
				});


			const params =
				buildQueryParameters();


			const response =
				await camsApi.get(
					"/api/v1/members?" +
					params.toString());


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to load members."));
			}


			const result =
				payload.data ??
				payload;


			renderMembers(
				result);
		}
		catch (error) {

			console.error(
				"Unable to load members.",
				error);


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to load members.",
				{
					columnCount:
						7
				});


			tableSummary.textContent =
				"Unable to load members.";

			totalCount.textContent =
				"0";

			pagination.innerHTML =
				"";

			paginationInfo.textContent =
				"";
		}
	}


	function buildQueryParameters() {

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


		if (
			state.isActive !==
			""
		) {

			params.set(
				"filter.isActive",
				state.isActive);
		}


		if (state.gender) {

			params.set(
				"filter.gender",
				state.gender);
		}


		/*
		 * Multi-column sorting using your new
		 * Sort[] PagedRequest property.
		 */

		params.set(
			"sortBy[0].name",
			"LastName");

		params.set(
			"sortBy[0].sortDescending",
			"false");


		params.set(
			"sortBy[1].name",
			"FirstName");

		params.set(
			"sortBy[1].sortDescending",
			"false");


		return params;
	}


	// =========================================================
	// RENDER TABLE
	// =========================================================

	function renderMembers(
		result) {

		const items =
			result.items ??
			[];


		camsTable.renderRows(
			tableBody,
			items,
			renderMemberRow,
			{
				columnCount:
					7,

				title:
					"No members found",

				message:
					"No members match the selected filters.",

				icon:
					"ri-group-line"
			});


		const info =
			camsTable.getPaginationInfo(
				result,
				{
					page:
						state.page,

					pageSize:
						state.pageSize
				});


		totalCount.textContent =
			info.totalCount.toString();


		camsTable.renderSummary(
			tableSummary,
			paginationInfo,
			{
				totalCount:
					info.totalCount,

				currentPage:
					info.currentPage,

				pageSize:
					info.pageSize,

				singularLabel:
					"member",

				pluralLabel:
					"members"
			});


		camsTable.renderPagination(
			pagination,
			{
				currentPage:
					info.currentPage,

				totalPages:
					info.totalPages,

				onPageChange:
					page => {

						state.page =
							page;


						loadMembers();
					}
			});
	}


	function renderMemberRow(
		member) {

		const status =
			member.isActive
				? `
				<span class="badge
							bg-success-subtle
							text-success">

					<i class="ri-checkbox-circle-line
							  me-1">
					</i>

					Active

				</span>
			`
				: `
				<span class="badge
							bg-secondary-subtle
							text-secondary">

					<i class="ri-close-circle-line
							  me-1">
					</i>

					Inactive

				</span>
			`;


		const statusAction =
			member.isActive
				? `
				<button type="button"
						class="btn
							   btn-sm
							   btn-soft-danger
							   member-status-button"
						data-member-id="${member.id}"
						data-member-name="${camsUtils.escapeHtml(
					member.fullName)}"
						data-member-active="true"
						title="Deactivate member">

					<i class="ri-user-unfollow-line">
					</i>

				</button>
			`
				: `
				<button type="button"
						class="btn
							   btn-sm
							   btn-soft-success
							   member-status-button"
						data-member-id="${member.id}"
						data-member-name="${camsUtils.escapeHtml(
					member.fullName)}"
						data-member-active="false"
						title="Activate member">

					<i class="ri-user-follow-line">
					</i>

				</button>
			`;


		const birthDate =
			member.birthDate
				? camsUtils.formatDate(
					member.birthDate)
				: "—";


		const createdAt =
			member.createdAt
				? camsUtils.formatDate(
					member.createdAt)
				: "—";


		const gender =
			member.gender
				? camsUtils.escapeHtml(
					member.gender)
				: "—";


		return `
		<tr>

			<td class="ps-3">

				<div class="d-flex
							align-items-center
							gap-2">

					<div class="avatar-xs">

						<div class="avatar-title
									bg-primary-subtle
									text-primary
									rounded-circle">

							${getInitials(
			member)}

						</div>

					</div>


					<div>

						<div class="fw-medium">

							${camsUtils.escapeHtml(
				member.fullName)}

						</div>

						<div class="text-muted fs-sm">

							${camsUtils.escapeHtml(
					member.address ??
					"No address")}

						</div>

					</div>

				</div>

			</td>


			<td>

				${camsUtils.escapeHtml(
						member.mobileNumber)}

			</td>


			<td>
				${birthDate}
			</td>


			<td>
				${gender}
			</td>


			<td>
				${status}
			</td>


			<td>
				${createdAt}
			</td>


			<td class="text-end pe-3">

				<div class="d-inline-flex gap-1">

					<button type="button"
							class="btn
								   btn-sm
								   btn-soft-primary
								   member-edit-button"
							data-member-id="${member.id}"
							title="Edit member">

						<i class="ri-pencil-line">
						</i>

					</button>


					${statusAction}

				</div>

			</td>

		</tr>
	`;
	}


	function getInitials(
		member) {

		const first =
			member.firstName
				?.charAt(0) ??
			"";

		const last =
			member.lastName
				?.charAt(0) ??
			"";


		return camsUtils.escapeHtml(
			(first + last)
				.toUpperCase());
	}


	// =========================================================
	// TABLE ACTIONS
	// =========================================================

	function handleTableClick(
		event) {

		const editButton =
			event.target.closest(
				".member-edit-button");


		if (editButton) {

			const memberId =
				editButton.dataset.memberId;


			if (!memberId) {
				return;
			}


			openEditModal(
				memberId);

			return;
		}


		const statusButton =
			event.target.closest(
				".member-status-button");


		if (!statusButton) {
			return;
		}


		const memberId =
			statusButton.dataset.memberId;

		const memberName =
			statusButton.dataset.memberName;

		const isActive =
			statusButton.dataset.memberActive ===
			"true";


		if (!memberId) {
			return;
		}


		changeMemberStatus(
			memberId,
			memberName,
			isActive);
	}

	async function changeMemberStatus(
		memberId,
		memberName,
		isActive) {

		const action =
			isActive
				? "deactivate"
				: "activate";


		const actionLabel =
			isActive
				? "Deactivate"
				: "Activate";


		/*
		 * We'll connect this to camsUi confirmation
		 * using the exact API exposed by your cams-ui.js.
		 */

		const confirmed =
			await camsUi.confirm(
				`${actionLabel} ${memberName}?`);


		if (!confirmed) {
			return;
		}


		try {

			const response =
				await camsApi.post(
					`/api/v1/members/${memberId}/${action}`);


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						`Unable to ${action} member.`));
			}


			await loadMembers();
		}
		catch (error) {

			console.error(
				`Unable to ${action} member.`,
				error);


			await camsUi.showErrorBox(
				error.message ??
				`Unable to ${action} member.`,
				{
					title:
						"Member Status"
				});
		}
	}


	// =========================================================
	// EDIT
	// =========================================================

	async function openEditModal(
		memberId) {

		resetForm();


		state.selectedMemberId =
			memberId;


		try {

			camsUi.setButtonLoading(
				saveButton,
				true);


			const response =
				await camsApi.get(
					`/api/v1/members/${memberId}`);


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to load member."));
			}


			const member =
				payload.data ??
				payload;


			populateForm(
				member);


			modal.show();
		}
		catch (error) {

			console.error(
				"Unable to load member.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to load member.",
				{
					title:
						"Unable to Edit Member"
				});
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	function populateForm(
		member) {

		modalName.textContent =
			member.fullName ??
			"";


		firstNameInput.value =
			member.firstName ??
			"";


		middleNameInput.value =
			member.middleName ??
			"";


		lastNameInput.value =
			member.lastName ??
			"";


		mobileNumberInput.value =
			member.mobileNumber ??
			"";


		birthDateInput.value =
			camsUtils.toDateInput(
				member.birthDate);


		genderInput.value =
			member.gender ??
			"";


		addressInput.value =
			member.address ??
			"";
	}


	// =========================================================
	// SAVE
	// =========================================================

	async function handleSave(
		event) {

		event.preventDefault();


		camsForm.hideError(
			formError);


		if (
			!camsForm.validate(
				form)
		) {
			return;
		}


		if (
			!state.selectedMemberId
		) {
			return;
		}


		const request =
			buildUpdateRequest();


		try {

			camsUi.setButtonLoading(
				saveButton,
				true);


			const response =
				await camsApi.put(
					`/api/v1/members/${state.selectedMemberId}`,
					request);


			const payload =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				throw new Error(
					camsApi.getErrorMessage(
						payload,
						response.status,
						"Unable to update member."));
			}


			modal.hide();


			await camsUi.showSuccessToast(
				payload.message ??
				"Member updated successfully.");


			await loadMembers();
		}
		catch (error) {

			console.error(
				"Unable to update member.",
				error);


			camsForm.showError(
				formError,
				error.message ??
				"Unable to update member.");
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	function buildUpdateRequest() {

		return {

			firstName:
				firstNameInput
					.value
					.trim(),

			middleName:
				camsUtils.toNullableString(
					middleNameInput.value),

			lastName:
				lastNameInput
					.value
					.trim(),

			mobileNumber:
				mobileNumberInput
					.value
					.trim(),

			birthDate:
				camsUtils.toNullableString(
					birthDateInput.value),

			gender:
				camsUtils.toNullableString(
					genderInput.value),

			address:
				camsUtils.toNullableString(
					addressInput.value)
		};
	}


	// =========================================================
	// RESET FORM
	// =========================================================

	function resetForm() {

		camsForm.reset(
			form,
			{
				errorContainer:
					formError
			});


		state.selectedMemberId =
			null;


		modalName.textContent =
			"";
	}

})();