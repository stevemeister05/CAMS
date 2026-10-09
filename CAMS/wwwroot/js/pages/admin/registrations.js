(() => {
	"use strict";


	// =========================================================
	// CONSTANTS
	// =========================================================

	const pageContainer =
		document.getElementById(
			"registration-page");


	if (!pageContainer) {
		return;
	}


	const RegistrationStatus = {

		Pending:
			Number(
				pageContainer.dataset.statusPending),

		Approved:
			Number(
				pageContainer.dataset.statusApproved),

		Rejected:
			Number(
				pageContainer.dataset.statusRejected)
	};


	// =========================================================
	// STATE
	// =========================================================

	const state = {

		page:
			1,

		pageSize:
			20,

		status:
			RegistrationStatus.Pending,

		search:
			"",

		selectedRegistrationId:
			null,

		selectedRegistration:
			null
	};


	let searchTimer =
		null;


	// =========================================================
	// ELEMENTS
	// =========================================================

	const searchInput =
		document.getElementById(
			"registration-search");

	const statusSelect =
		document.getElementById(
			"registration-status");

	const pageSizeSelect =
		document.getElementById(
			"registration-page-size");

	const refreshButton =
		document.getElementById(
			"registration-refresh");

	const tableBody =
		document.getElementById(
			"registration-table-body");

	const resultSummary =
		document.getElementById(
			"registration-result-summary");

	const pagination =
		document.getElementById(
			"registration-pagination");

	const paginationInfo =
		document.getElementById(
			"registration-pagination-info");


	// Modal

	const reviewModalElement =
		document.getElementById(
			"registration-review-modal");

	const reviewModal =
		bootstrap.Modal.getOrCreateInstance(
			reviewModalElement);

	const reviewLoading =
		document.getElementById(
			"registration-review-loading");

	const reviewContent =
		document.getElementById(
			"registration-review-content");

	const reviewSubtitle =
		document.getElementById(
			"registration-review-subtitle");

	const approveButton =
		document.getElementById(
			"registration-approve-button");

	const rejectButton =
		document.getElementById(
			"registration-reject-button");

	const rejectSection =
		document.getElementById(
			"registration-reject-section");

	const rejectReason =
		document.getElementById(
			"registration-reject-reason");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	function initialize() {

		initializeEvents();

		loadRegistrations();
	}


	function initializeEvents() {

		searchInput.addEventListener(
			"input",
			handleSearch);


		statusSelect.addEventListener(
			"change",
			handleStatusChange);


		pageSizeSelect.addEventListener(
			"change",
			handlePageSizeChange);


		refreshButton.addEventListener(
			"click",
			() => {

				loadRegistrations();
			});


		tableBody.addEventListener(
			"click",
			handleTableClick);


		approveButton.addEventListener(
			"click",
			approveRegistration);


		rejectButton.addEventListener(
			"click",
			handleReject);


		reviewModalElement.addEventListener(
			"hidden.bs.modal",
			resetReviewModal);
	}


	// =========================================================
	// SEARCH / FILTER
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


					loadRegistrations();

				},
				350);
	}


	function handleStatusChange() {

		const value =
			statusSelect.value;


		state.status =
			value
				? Number(value)
				: null;


		state.page =
			1;


		loadRegistrations();
	}


	function handlePageSizeChange() {

		state.pageSize =
			Number(
				pageSizeSelect.value);


		state.page =
			1;


		loadRegistrations();
	}


	// =========================================================
	// LOAD REGISTRATIONS
	// =========================================================

	async function loadRegistrations() {

		showTableLoading();


		try {

			const url =
				buildSearchUrl();


			const response =
				await camsApi.get(
					url);


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to load registration requests.");


				throw new Error(
					message);
			}


			const data =
				result?.data;


			renderRegistrations(
				data);
		}
		catch (error) {

			console.error(
				"Unable to load registrations.",
				error);


			showTableError(
				error.message ??
				"Unable to load registration requests.");
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


		/*
		 * These names map directly to:
		 *
		 * PagedRequest<RegistrationFilter>.Filter.Status
		 * PagedRequest<RegistrationFilter>.Filter.Search
		 */
		if (
			state.status !==
			null
		) {

			params.set(
				"filter.status",
				state.status.toString());
		}


		if (state.search) {

			params.set(
				"filter.search",
				state.search);
		}


		/*
		 * Adjust SortBy if your repository expects
		 * a different property name.
		 */
		params.set(
			"sortBy",
			"CreatedAt");


		params.set(
			"sortDescending",
			"true");


		return (
			"/api/v1/registrations?" +
			params.toString()
		);
	}


	// =========================================================
	// TABLE
	// =========================================================

	function renderRegistrations(
		data) {

		const items =
			data?.items ??
			[];


		if (!items.length) {

			tableBody.innerHTML = `
				<tr>
					<td colspan="5"
						class="text-center py-5">

						<div class="avatar-md mx-auto mb-3">

							<div class="avatar-title
										bg-light text-muted
										rounded-circle fs-3">

								<i class="ri-user-search-line"></i>

							</div>

						</div>

						<h6 class="mb-1">
							No registration requests found
						</h6>

						<p class="text-muted mb-0">
							Try changing your search or status filter.
						</p>

					</td>
				</tr>
			`;

			renderPagination(
				data);

			renderSummary(
				data);

			return;
		}


		tableBody.innerHTML =
			items
				.map(
					createRegistrationRow)
				.join("");


		renderPagination(
			data);

		renderSummary(
			data);
	}


	function createRegistrationRow(
		registration) {

		const fullName =
			getFullName(
				registration);


		return `
		<tr>

			<td>

				<div class="fw-medium">
					${camsUtils.escapeHtml(fullName)}
				</div>

			</td>


			<td>

				${camsUtils.escapeHtml(
			registration.mobileNumber ?? "-")}

			</td>


			<td>

				${formatRegistrationDate(
					registration.registeredAt)}

			</td>


			<td>

				${getStatusBadge(
					registration.status)}

			</td>


			<td class="text-end">

				<button type="button"
						class="btn btn-soft-primary btn-sm"
						data-action="review"
						data-registration-id="${registration.registrationId}">

					<i class="ri-eye-line me-1"></i>

					Review

				</button>

			</td>

		</tr>
	`;
	}


	function showTableLoading() {

		tableBody.innerHTML = `
			<tr>
				<td colspan="5"
					class="text-center py-5">

					<div class="spinner-border
								spinner-border-sm me-2"
						 role="status">
					</div>

					Loading registration requests...

				</td>
			</tr>
		`;
	}


	function showTableError(
		message) {

		tableBody.innerHTML = `
			<tr>
				<td colspan="5"
					class="text-center py-5">

					<div class="text-danger mb-2">

						<i class="ri-error-warning-line fs-2"></i>

					</div>

					<div>
						${camsUtils.escapeHtml(message)}
					</div>

				</td>
			</tr>
		`;


		resultSummary.textContent =
			"Unable to load registrations.";


		pagination.innerHTML =
			"";


		paginationInfo.textContent =
			"";
	}


	// =========================================================
	// REVIEW
	// =========================================================

	function handleTableClick(
		event) {

		const button =
			event.target.closest(
				"[data-action='review']");


		if (!button) {
			return;
		}


		const id =
			button.dataset.registrationId;


		if (!id) {
			return;
		}


		openReview(
			id);
	}


	async function openReview(
		id) {

		state.selectedRegistrationId =
			id;


		setReviewLoading(
			true);


		reviewModal.show();


		try {

			const response =
				await camsApi.get(
					`/api/v1/registrations/${id}`);


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to load the registration request.");


				throw new Error(
					message);
			}


			state.selectedRegistration =
				result?.data;


			renderReview(
				state.selectedRegistration);


			setReviewLoading(
				false);
		}
		catch (error) {

			reviewModal.hide();


			await camsUi.showErrorBox(
				error.message ??
				"Unable to load the registration request.",
				{
					title:
						"Unable to Load Registration"
				});
		}
	}


	function renderReview(
		registration) {

		setText(
			"review-first-name",
			registration.firstName);

		setText(
			"review-middle-name",
			registration.middleName);

		setText(
			"review-last-name",
			registration.lastName);

		setText(
			"review-mobile-number",
			registration.mobileNumber);

		setText(
			"review-registered-at",
			formatRegistrationDate(
				registration.registeredAt));


		document.getElementById(
			"review-status"
		).innerHTML =
			getStatusBadge(
				registration.status);


		reviewSubtitle.textContent =
			getFullName(
				registration);


		const pending =
			Number(
				registration.status) ===
			RegistrationStatus.Pending;


		approveButton.classList.toggle(
			"d-none",
			!pending);


		rejectButton.classList.toggle(
			"d-none",
			!pending);
	}


	function setReviewLoading(
		loading) {

		reviewLoading.classList.toggle(
			"d-none",
			!loading);


		reviewContent.classList.toggle(
			"d-none",
			loading);
	}


	function resetReviewModal() {

		state.selectedRegistrationId =
			null;

		state.selectedRegistration =
			null;


		rejectSection.classList.add(
			"d-none");


		rejectReason.value =
			"";


		setReviewLoading(
			true);
	}


	// =========================================================
	// APPROVE
	// =========================================================

	async function approveRegistration() {

		if (
			!state.selectedRegistrationId
		) {
			return;
		}


		const confirmed =
			await camsUi.confirm(
				"Approve this registration and create the member account?",
				{
					title:
						"Approve Registration",

					type:
						"success",

					confirmText:
						"Yes, Approve",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		try {

			camsUi.setButtonLoading(
				approveButton,
				true,
				"Approving...");


			const response =
				await camsApi.fetch(
					`/api/v1/registrations/${state.selectedRegistrationId}/approve`,
					{
						method:
							"POST"
					});


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to approve the registration.");


				throw new Error(
					message);
			}


			reviewModal.hide();


			await camsUi.showSuccessBox(
				result?.message ??
				"Registration approved successfully.",
				{
					title:
						"Registration Approved"
				});


			await loadRegistrations();
		}
		catch (error) {

			await camsUi.showErrorBox(
				error.message ??
				"Unable to approve the registration.",
				{
					title:
						"Approval Failed"
				});
		}
		finally {

			camsUi.setButtonLoading(
				approveButton,
				false);
		}
	}


	// =========================================================
	// REJECT
	// =========================================================

	async function handleReject() {

		if (
			rejectSection.classList.contains(
				"d-none")
		) {

			rejectSection.classList.remove(
				"d-none");


			rejectReason.focus();


			rejectButton.innerHTML = `
				<i class="ri-close-circle-line me-1"></i>
				Confirm Rejection
			`;


			return;
		}


		await rejectRegistration();
	}


	async function rejectRegistration() {

		if (
			!state.selectedRegistrationId
		) {
			return;
		}


		const reason =
			rejectReason.value.trim();


		if (!reason) {

			await camsUi.showWarningBox(
				"Please provide a reason for rejecting this registration.",
				{
					title:
						"Reason Required"
				});


			rejectReason.focus();

			return;
		}


		const confirmed =
			await camsUi.confirm(
				"Are you sure you want to reject this registration?",
				{
					title:
						"Reject Registration",

					type:
						"danger",

					confirmText:
						"Yes, Reject",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		try {

			camsUi.setButtonLoading(
				rejectButton,
				true,
				"Rejecting...");


			const response =
				await camsApi.post(
					`/api/v1/registrations/${state.selectedRegistrationId}/reject`,
					{
						reason:
							reason
					});


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to reject the registration.");


				throw new Error(
					message);
			}


			reviewModal.hide();


			await camsUi.showSuccessBox(
				result?.message ??
				"Registration rejected successfully.",
				{
					title:
						"Registration Rejected"
				});


			await loadRegistrations();
		}
		catch (error) {

			await camsUi.showErrorBox(
				error.message ??
				"Unable to reject the registration.",
				{
					title:
						"Rejection Failed"
				});
		}
		finally {

			camsUi.setButtonLoading(
				rejectButton,
				false);
		}
	}


	// =========================================================
	// PAGINATION
	// =========================================================

	function renderPagination(
		data) {

		const currentPage =
			data?.page ??
			state.page;

		const totalPages =
			data?.totalPages ??
			1;


		pagination.innerHTML =
			"";


		if (
			totalPages <= 1
		) {
			return;
		}


		pagination.appendChild(
			createPageItem(
				"Previous",
				currentPage - 1,
				currentPage <= 1));


		const startPage =
			Math.max(
				1,
				currentPage - 2);

		const endPage =
			Math.min(
				totalPages,
				currentPage + 2);


		for (
			let page = startPage;
			page <= endPage;
			page++
		) {

			pagination.appendChild(
				createPageItem(
					page.toString(),
					page,
					false,
					page === currentPage));
		}


		pagination.appendChild(
			createPageItem(
				"Next",
				currentPage + 1,
				currentPage >= totalPages));
	}


	function createPageItem(
		label,
		page,
		disabled = false,
		active = false) {

		const item =
			document.createElement(
				"li");


		item.className =
			"page-item";


		if (disabled) {

			item.classList.add(
				"disabled");
		}


		if (active) {

			item.classList.add(
				"active");
		}


		const button =
			document.createElement(
				"button");


		button.type =
			"button";

		button.className =
			"page-link";

		button.textContent =
			label;

		button.disabled =
			disabled;


		button.addEventListener(
			"click",
			() => {

				if (
					disabled ||
					active
				) {
					return;
				}


				state.page =
					page;


				loadRegistrations();
			});


		item.appendChild(
			button);


		return item;
	}


	function renderSummary(
		data) {

		const totalCount =
			data?.totalCount ??
			0;

		const currentPage =
			data?.page ??
			state.page;

		const pageSize =
			data?.pageSize ??
			state.pageSize;


		resultSummary.textContent =
			`${totalCount} registration request${totalCount === 1 ? "" : "s"}`;


		if (!totalCount) {

			paginationInfo.textContent =
				"";

			return;
		}


		const start =
			((currentPage - 1) *
				pageSize) + 1;

		const end =
			Math.min(
				currentPage *
				pageSize,
				totalCount);


		paginationInfo.textContent =
			`Showing ${start}-${end} of ${totalCount}`;
	}


	// =========================================================
	// HELPERS
	// =========================================================

	function getFullName(
		registration) {

		return [
			registration.firstName,
			registration.middleName,
			registration.lastName
		]
			.filter(
				value =>
					value &&
					value.trim())
			.join(" ");
	}


	function getStatusBadge(
		status) {

		switch (
		Number(status)
		) {

			case RegistrationStatus.Pending:

				return `
					<span class="badge
								 bg-warning-subtle
								 text-warning">
						Pending
					</span>
				`;


			case RegistrationStatus.Approved:

				return `
					<span class="badge
								 bg-success-subtle
								 text-success">
						Approved
					</span>
				`;


			case RegistrationStatus.Rejected:

				return `
					<span class="badge
								 bg-danger-subtle
								 text-danger">
						Rejected
					</span>
				`;


			default:

				return `
					<span class="badge
								 bg-secondary-subtle
								 text-secondary">
						Unknown
					</span>
				`;
		}
	}


	function formatRegistrationDate(
	value) {

	if (!value) {
		return "-";
	}


	/*
	 * .NET DateTime may serialize with up to
	 * 7 fractional-second digits.
	 *
	 * JavaScript Date only needs milliseconds,
	 * so reduce it to 3 digits.
	 *
	 * Example:
	 *
	 * 2026-08-30T10:18:10.1583153
	 *
	 * becomes:
	 *
	 * 2026-08-30T10:18:10.158
	 */
	const normalizedValue =
		value.replace(
			/(\.\d{3})\d+/,
			"$1");


	const date =
		new Date(
			normalizedValue);


	if (
		Number.isNaN(
			date.getTime())
	) {
		return value;
	}


	return new Intl.DateTimeFormat(
		"en-PH",
		{
			year:
				"numeric",

			month:
				"short",

			day:
				"2-digit",

			hour:
				"numeric",

			minute:
				"2-digit",

			hour12:
				true
		})
		.format(
			date);
}


	function formatDate(
		value) {

		if (!value) {
			return "-";
		}


		return camsUtils.formatDate
			? camsUtils.formatDate(
				value)
			: value;
	}

	function formatDateTime(
		value) {

		if (!value) {
			return "-";
		}


		const date =
			new Date(
				value);


		if (
			Number.isNaN(
				date.getTime())
		) {
			return "-";
		}


		return date.toLocaleString(
			"en-PH",
			{
				year:
					"numeric",

				month:
					"short",

				day:
					"2-digit",

				hour:
					"numeric",

				minute:
					"2-digit",

				hour12:
					true
			});
	}


	function setText(
		id,
		value) {

		const element =
			document.getElementById(
				id);


		if (!element) {
			return;
		}


		element.textContent =
			value ||
			"-";
	}

})();