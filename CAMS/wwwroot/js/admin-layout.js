(() => {
	"use strict";


	// =========================================================
	// INITIALIZATION
	// =========================================================

	document.addEventListener(
		"DOMContentLoaded",
		initialize);


	function initialize() {

		initializeLogout();
		initializePageRestore();
	}


	// =========================================================
	// PAGE RESTORE
	// =========================================================

	/*
	 * Browsers may restore Admin pages from the
	 * Back-Forward Cache (BFCache).
	 *
	 * If the user dropdown was previously open,
	 * explicitly reset its visual state.
	 *
	 * We intentionally do NOT create or control a
	 * Bootstrap Dropdown instance here.
	 */
	function initializePageRestore() {

		window.addEventListener(
			"pageshow",
			() => {

				resetUserDropdownState();
			});
	}


	function resetUserDropdownState() {

		const container =
			document.querySelector(
				".topbar-user");

		const button =
			document.getElementById(
				"page-header-user-dropdown");

		const menu =
			container?.querySelector(
				".dropdown-menu");


		if (menu) {

			menu.classList.remove(
				"show");
		}


		if (button) {

			button.classList.remove(
				"show");

			button.setAttribute(
				"aria-expanded",
				"false");
		}


		if (container) {

			container.classList.remove(
				"show");
		}
	}


	// =========================================================
	// LOGOUT
	// =========================================================

	function initializeLogout() {

		const logoutButtons =
			document.querySelectorAll(
				"#sidebar-logout, #topbar-logout");


		if (!logoutButtons.length) {
			return;
		}


		logoutButtons.forEach(
			button => {

				button.addEventListener(
					"click",
					handleLogout);
			});
	}


	async function handleLogout(
		event) {

		event.preventDefault();


		try {

			const confirmed =
				await confirmLogout();


			if (!confirmed) {
				return;
			}


			setLogoutButtonsDisabled(
				true);


			const response =
				await camsApi.fetch(
					"/api/v1/auth/logout",
					{
						method:
							"POST",

						/*
						 * Logout is itself an authentication
						 * operation.
						 *
						 * We do not want camsApi to automatically
						 * redirect when authentication errors occur.
						 */
						handleAuthErrors:
							false
					});


			/*
			 * If the session has already expired,
			 * our desired destination is still Login.
			 */
			if (
				response.status === 401
			) {

				redirectToLogin();

				return;
			}


			if (!response.ok) {

				const message =
					await getLogoutErrorMessage(
						response);


				await camsUi.showErrorBox(
					message,
					{
						title:
							"Sign Out Failed"
					});


				return;
			}


			redirectToLogin();
		}
		catch (error) {

			console.error(
				"An error occurred while signing out.",
				error);


			await camsUi.showErrorBox(
				"Unable to sign out. Please try again.",
				{
					title:
						"Sign Out Failed"
				});
		}
		finally {

			setLogoutButtonsDisabled(
				false);
		}
	}


	// =========================================================
	// LOGOUT CONFIRMATION
	// =========================================================

	async function confirmLogout() {

		return await camsUi.confirm(
			"Are you sure you want to sign out?",
			{
				title:
					"Sign Out",

				type:
					"warning",

				confirmText:
					"Yes, Sign Out",

				cancelText:
					"No"
			});
	}


	// =========================================================
	// ERROR HANDLING
	// =========================================================

	async function getLogoutErrorMessage(
		response) {

		const fallbackMessage =
			"Unable to sign out. Please try again.";


		try {

			const result =
				await camsApi.readJson(
					response);


			return camsApi.getErrorMessage(
				result,
				response.status,
				fallbackMessage);
		}
		catch {

			return fallbackMessage;
		}
	}


	// =========================================================
	// UI STATE
	// =========================================================

	function setLogoutButtonsDisabled(
		disabled) {

		const logoutButtons =
			document.querySelectorAll(
				"#sidebar-logout, #topbar-logout");


		logoutButtons.forEach(
			button => {

				button.disabled =
					disabled;


				if (disabled) {

					button.setAttribute(
						"aria-disabled",
						"true");
				}
				else {

					button.removeAttribute(
						"aria-disabled");
				}
			});
	}


	// =========================================================
	// NAVIGATION
	// =========================================================

	function redirectToLogin() {

		window.location.href =
			"/auth/login";
	}

})();