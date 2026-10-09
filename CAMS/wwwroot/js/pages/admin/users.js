(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"users-page");


	if (!page) {
		return;
	}


	const Roles =
		Object.freeze({

			Administrator:
				page.dataset.roleAdministrator,

			AttendanceStaff:
				page.dataset.roleAttendanceStaff,

			Member:
				page.dataset.roleMember
		});


	// =========================================================
	// STATE
	// =========================================================

	let users =
		[];


	// =========================================================
	// ELEMENTS
	// =========================================================

	const tableBody =
		document.getElementById(
			"user-table-body");


	const searchInput =
		document.getElementById(
			"user-search");


	const roleFilter =
		document.getElementById(
			"user-role-filter");


	const statusFilter =
		document.getElementById(
			"user-status-filter");


	const createButton =
		document.getElementById(
			"create-user-button");


	const modalElement =
		document.getElementById(
			"user-modal");


	const userModal =
		bootstrap.Modal.getOrCreateInstance(
			modalElement);


	const modalTitle =
		document.getElementById(
			"user-modal-title");


	const form =
		document.getElementById(
			"user-form");


	const formAlert =
		document.getElementById(
			"user-form-alert");


	const idInput =
		document.getElementById(
			"user-id");


	const roleInput =
		document.getElementById(
			"user-role");


	const userNameInput =
		document.getElementById(
			"user-name");


	const passwordGroup =
		document.getElementById(
			"user-password-group");


	const passwordInput =
		document.getElementById(
			"user-password");


	const saveButton =
		document.getElementById(
			"save-user-button");

	const refreshButton =
		document.getElementById(
			"refresh-users-button");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		initializeEvents();

		await loadUsers();
	}


	function initializeEvents() {

		createButton?.addEventListener(
			"click",
			openCreateModal);


		form?.addEventListener(
			"submit",
			saveUser);


		searchInput?.addEventListener(
			"input",
			renderUsers);


		roleFilter?.addEventListener(
			"change",
			renderUsers);


		statusFilter?.addEventListener(
			"change",
			renderUsers);


		tableBody?.addEventListener(
			"click",
			handleTableAction);

		refreshButton?.addEventListener(
			"click",
			refreshUsers);
	}


	// =========================================================
	// LOAD USERS
	// =========================================================

	async function loadUsers() {

		camsTable.showLoading(
			tableBody,
			{
				columnCount:
					6,

				message:
					"Loading users..."
			});


		try {

			const response =
				await camsApi.get(
					"/api/v1/users");


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
						"Unable to load users."));
			}


			const data =
				payload?.data ??
				payload ??
				[];


			users =
				Array.isArray(
					data)
					? data
					: [];


			renderUsers();
		}
		catch (error) {

			console.error(
				"Unable to load users.",
				error);


			camsTable.showError(
				tableBody,
				error.message ??
				"Unable to load users.",
				{
					columnCount:
						6
				});
		}
	}

	async function refreshUsers() {

		camsUi.setButtonLoading(
			refreshButton,
			true,
			"Refreshing...");


		try {

			await loadUsers();
		}
		catch (error) {

			console.error(
				"Unable to refresh users.",
				error);
		}
		finally {

			camsUi.setButtonLoading(
				refreshButton,
				false);
		}
	}


	// =========================================================
	// FILTERING
	// =========================================================

	function renderUsers() {

		const search =
			searchInput.value
				.trim()
				.toLowerCase();


		const selectedRole =
			roleFilter.value;


		const selectedStatus =
			statusFilter.value;


		const filteredUsers =
			users.filter(
				user => {

					if (
						search &&
						!matchesSearch(
							user,
							search)
					) {
						return false;
					}


					if (
						selectedRole &&
						user.role !==
						selectedRole
					) {
						return false;
					}


					if (
						selectedStatus
					) {

						const isActive =
							selectedStatus ===
							"true";


						if (
							user.isActive !==
							isActive
						) {
							return false;
						}
					}


					return true;
				});


		camsTable.renderRows(
			tableBody,
			filteredUsers,
			createUserRow,
			{
				columnCount:
					6,

				title:
					"No users found",

				message:
					"No user accounts match the selected filters.",

				icon:
					"ri-user-search-line"
			});
	}


	function matchesSearch(
		user,
		search) {

		return [
			user.userName,
			user.memberName,
			user.email,
			user.role
		]
			.filter(Boolean)
			.some(
				value =>
					String(value)
						.toLowerCase()
						.includes(
							search));
	}


	// =========================================================
	// ROW
	// =========================================================

	function createUserRow(
		user) {

		const isMember =
			Boolean(
				user.memberId);


		return `
			<tr data-user-id="${camsUtils.escapeHtml(
			user.id)}">

				<td class="ps-3">

					<div class="fw-semibold">
						${camsUtils.escapeHtml(
				user.userName ??
				"-")}
					</div>

					${user.email
				? `
							<div class="text-muted fs-13">
								${camsUtils.escapeHtml(
					user.email)}
							</div>
						`
				: ""
			}

				</td>


				<td>
					${renderRoleBadge(
				user.role)}
				</td>


				<td>

					${isMember
				? `
							<div class="fw-medium">
								${camsUtils.escapeHtml(
					user.memberName ??
					"-")}
							</div>
						`
				: `
							<span class="text-muted">
								—
							</span>
						`
			}

				</td>


				<td>

					${user.mustChangePassword
				? `
							<span class="badge bg-warning-subtle text-warning">
								Must Change
							</span>
						`
				: `
							<span class="badge bg-success-subtle text-success">
								Set
							</span>
						`
			}

				</td>


				<td>

					${user.isActive
				? `
							<span class="badge bg-success-subtle text-success">
								Active
							</span>
						`
				: `
							<span class="badge bg-danger-subtle text-danger">
								Disabled
							</span>
						`
			}

				</td>


				<td class="text-end pe-3">

					<div class="btn-group btn-group-sm">

						${!isMember
				? `
								<button type="button"
										class="btn btn-soft-primary
											   user-edit-button"
										data-id="${camsUtils.escapeHtml(
					user.id)}"
										title="Edit">

									<i class="ri-pencil-line">
									</i>

								</button>
							`
				: ""
			}


						<button type="button"
								class="btn btn-soft-warning
									   user-reset-password-button"
								data-id="${camsUtils.escapeHtml(
				user.id)}"
								title="Reset Password">

							<i class="ri-lock-password-line">
							</i>

						</button>


						<button type="button"
								class="btn ${user.isActive
				? "btn-soft-danger"
				: "btn-soft-success"}
									   user-status-button"
								data-id="${camsUtils.escapeHtml(
					user.id)}"
								title="${user.isActive
				? "Disable"
				: "Enable"}">

							<i class="${user.isActive
				? "ri-user-forbid-line"
				: "ri-user-follow-line"}">
							</i>

						</button>

					</div>

				</td>

			</tr>
		`;
	}


	function renderRoleBadge(
		role) {

		switch (role) {

			case Roles.Administrator:

				return `
					<span class="badge bg-primary-subtle text-primary">
						Administrator
					</span>
				`;


			case Roles.AttendanceStaff:

				return `
					<span class="badge bg-info-subtle text-info">
						Attendance Staff
					</span>
				`;


			case Roles.Member:

				return `
					<span class="badge bg-secondary-subtle text-secondary">
						Member
					</span>
				`;


			default:

				return `
					<span class="badge bg-light text-muted">
						${camsUtils.escapeHtml(
					role ??
					"Unknown")}
					</span>
				`;
		}
	}


	// =========================================================
	// ACTIONS
	// =========================================================

	async function handleTableAction(
		event) {

		const editButton =
			event.target.closest(
				".user-edit-button");


		if (editButton) {

			await openEditModal(
				editButton.dataset.id);

			return;
		}


		const resetButton =
			event.target.closest(
				".user-reset-password-button");


		if (resetButton) {

			const user =
				findUser(
					resetButton.dataset.id);


			if (user) {

				await resetPassword(
					user,
					resetButton);
			}

			return;
		}


		const statusButton =
			event.target.closest(
				".user-status-button");


		if (statusButton) {

			const user =
				findUser(
					statusButton.dataset.id);


			if (user) {

				await changeUserStatus(
					user,
					statusButton);
			}
		}
	}


	function findUser(
		id) {

		return users.find(
			user =>
				user.id === id);
	}


	// =========================================================
	// CREATE
	// =========================================================

	function openCreateModal() {

		resetForm();


		modalTitle.textContent =
			"Add Staff User";


		roleInput.disabled =
			false;


		passwordGroup.classList.remove(
			"d-none");


		passwordInput.required =
			true;


		saveButton.textContent =
			"Create User";


		userModal.show();
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
					`/api/v1/users/${id}`);


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
						"Unable to load user."));
			}


			const user =
				payload?.data ??
				payload;


			if (user.memberId) {

				await camsUi.showWarningBox(
					"Member user accounts cannot be edited.",
					{
						title:
							"Member User"
					});

				return;
			}


			idInput.value =
				user.id;


			userNameInput.value =
				user.userName ??
				"";


			roleInput.value =
				user.role ??
				"";


			roleInput.disabled =
				true;


			passwordGroup.classList.add(
				"d-none");


			passwordInput.required =
				false;


			passwordInput.value =
				"";


			modalTitle.textContent =
				"Edit User";


			saveButton.textContent =
				"Save Changes";


			userModal.show();
		}
		catch (error) {

			console.error(
				"Unable to load user.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to load user.",
				{
					title:
						"User"
				});
		}
	}


	// =========================================================
	// SAVE
	// =========================================================

	async function saveUser(
		event) {

		event.preventDefault();


		camsUi.clearAlert(
			"user-form-alert");


		if (
			!camsForm.validate(
				form)
		) {
			return;
		}


		const id =
			idInput.value.trim();


		const isEdit =
			Boolean(
				id);


		camsUi.setButtonLoading(
			saveButton,
			true,
			isEdit
				? "Saving..."
				: "Creating...");


		try {

			let response;


			if (isEdit) {

				response =
					await camsApi.put(
						`/api/v1/users/${id}`,
						{
							userName:
								userNameInput.value.trim()
						});
			}
			else {

				const request =
				{
					userName:
						userNameInput.value.trim(),

					password:
						passwordInput.value
				};


				switch (roleInput.value) {

					case Roles.Administrator:

						response =
							await camsApi.post(
								"/api/v1/users/administrators",
								request);

						break;


					case Roles.AttendanceStaff:

						response =
							await camsApi.post(
								"/api/v1/users/attendance-staff",
								request);

						break;


					default:

						throw new Error(
							"Please select a valid user type.");
				}
			}


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
							? "Unable to update user."
							: "Unable to create user."));
			}


			userModal.hide();


			await loadUsers();


			camsUi.showSuccessToast(
				isEdit
					? "User updated successfully."
					: "User created successfully.",
				{
					title:
						"Users"
				});
		}
		catch (error) {

			console.error(
				"Unable to save user.",
				error);


			camsUi.showError(
				formAlert,
				error.message ??
				"Unable to save user.",
				"user-form-alert");
		}
		finally {

			camsUi.setButtonLoading(
				saveButton,
				false);
		}
	}


	// =========================================================
	// RESET PASSWORD
	// =========================================================

	async function resetPassword(
		user,
		button) {

		const confirmed =
			await camsUi.confirm(
				`Reset the password for "${user.userName}"? ` +
				`The temporary password will be "${user.userName}", ` +
				"and the user will be required to change it on their next login.",
				{
					title:
						"Reset Password",

					type:
						"warning",

					confirmText:
						"Reset Password",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		camsUi.setButtonLoading(
			button,
			true,
			"Resetting...");


		try {

			const response =
				await camsApi.fetch(
					`/api/v1/users/${user.id}/reset-password`,
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
						"Unable to reset the password."));
			}


			await loadUsers();


			camsUi.showSuccessToast(
				`Password reset successfully. Temporary password: ${user.userName}`,
				{
					title:
						"Password Reset"
				});
		}
		catch (error) {

			console.error(
				"Unable to reset password.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to reset the password.",
				{
					title:
						"Reset Password"
				});
		}
		finally {

			camsUi.setButtonLoading(
				button,
				false);
		}
	}


	// =========================================================
	// ENABLE / DISABLE
	// =========================================================

	async function changeUserStatus(
		user,
		button) {

		const enable =
			!user.isActive;


		const confirmed =
			await camsUi.confirm(
				enable
					? `Enable the account "${user.userName}"?`
					: `Disable the account "${user.userName}"? ` +
					"The user will no longer be able to sign in.",
				{
					title:
						enable
							? "Enable User"
							: "Disable User",

					type:
						enable
							? "info"
							: "danger",

					confirmText:
						enable
							? "Enable"
							: "Disable",

					cancelText:
						"Cancel"
				});


		if (!confirmed) {
			return;
		}


		camsUi.setButtonLoading(
			button,
			true,
			enable
				? "Enabling..."
				: "Disabling...");


		try {

			const response =
				await camsApi.put(
					`/api/v1/users/${user.id}/status`,
					{
						isActive:
							enable
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
						enable
							? "Unable to enable the user."
							: "Unable to disable the user."));
			}


			await loadUsers();


			camsUi.showSuccessToast(
				enable
					? "User enabled successfully."
					: "User disabled successfully.",
				{
					title:
						"Users"
				});
		}
		catch (error) {

			console.error(
				"Unable to update user status.",
				error);


			await camsUi.showErrorBox(
				error.message ??
				"Unable to update user status.",
				{
					title:
						"User Status"
				});
		}
		finally {

			camsUi.setButtonLoading(
				button,
				false);
		}
	}


	// =========================================================
	// FORM
	// =========================================================

	function resetForm() {

		camsForm.reset(
			form);


		camsForm.resetValidation(
			form);


		camsUi.clearAlert(
			"user-form-alert");


		idInput.value =
			"";


		roleInput.disabled =
			false;


		passwordGroup.classList.remove(
			"d-none");


		passwordInput.required =
			true;
	}

})();