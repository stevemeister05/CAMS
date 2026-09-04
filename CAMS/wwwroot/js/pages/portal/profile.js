(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"portal-profile-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const alertContainer =
		document.getElementById(
			"portal-profile-alert");

	const profileAvatar =
		document.getElementById(
			"profile-avatar");

	const profileName =
		document.getElementById(
			"profile-name");

	const firstName =
		document.getElementById(
			"profile-first-name");

	const middleName =
		document.getElementById(
			"profile-middle-name");

	const lastName =
		document.getElementById(
			"profile-last-name");

	const mobileNumber =
		document.getElementById(
			"profile-mobile-number");

	const birthDate =
		document.getElementById(
			"profile-birth-date");

	const gender =
		document.getElementById(
			"profile-gender");

	const address =
		document.getElementById(
			"profile-address");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		await loadProfile();
	}


	// =========================================================
	// LOAD PROFILE
	// =========================================================

	async function loadProfile() {

		try {

			const response =
				await camsApi.get(
					"/api/v1/portal/profile");


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
						"Unable to load your profile."));
			}


			renderProfile(
				payload.data);
		}
		catch (error) {

			console.error(
				"Unable to load member profile.",
				error);


			profileName.textContent =
				"Unable to load profile";


			camsUi.showError(
				alertContainer,
				error.message ??
				"Unable to load your profile.",
				"portal-profile-alert-message");
		}
	}


	// =========================================================
	// RENDER PROFILE
	// =========================================================

	function renderProfile(
		profile) {

		if (!profile) {

			throw new Error(
				"Member profile was not returned.");
		}


		profileName.textContent =
			getDisplayValue(
				profile.fullName,
				"Member");


		firstName.textContent =
			getDisplayValue(
				profile.firstName);

		middleName.textContent =
			getDisplayValue(
				profile.middleName);

		lastName.textContent =
			getDisplayValue(
				profile.lastName);

		mobileNumber.textContent =
			getDisplayValue(
				profile.mobileNumber);

		gender.textContent =
			getDisplayValue(
				profile.gender);

		address.textContent =
			getDisplayValue(
				profile.address);


		birthDate.textContent =
			profile.birthDate
				? camsUtils.formatDate(
					profile.birthDate)
				: "Not provided";


		renderAvatar(
			profile);
	}


	function renderAvatar(
		profile) {

		const firstInitial =
			profile.firstName
				?.trim()
				?.charAt(0) ??
			"";

		const lastInitial =
			profile.lastName
				?.trim()
				?.charAt(0) ??
			"";


		const initials =
			`${firstInitial}${lastInitial}`
				.toUpperCase();


		if (!initials) {

			profileAvatar.innerHTML =
				'<i class="ri-user-line"></i>';

			return;
		}


		profileAvatar.textContent =
			initials;
	}


	function getDisplayValue(
		value,
		fallback = "Not provided") {

		if (
			value === null ||
			value === undefined ||
			String(value).trim() === ""
		) {
			return fallback;
		}


		return String(
			value);
	}

})();