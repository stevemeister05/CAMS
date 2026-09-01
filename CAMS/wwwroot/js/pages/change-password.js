(() => {
	"use strict";


	// =========================================================
	// ELEMENTS
	// =========================================================

	const form =
		document.getElementById(
			"change-password-form");

	const currentPassword =
		document.getElementById(
			"current-password");

	const newPassword =
		document.getElementById(
			"new-password");

	const confirmPassword =
		document.getElementById(
			"confirm-password");

	const confirmPasswordError =
		document.getElementById(
			"confirm-password-error");

	const errorContainer =
		document.getElementById(
			"change-password-error");

	const submitButton =
		document.getElementById(
			"change-password-button");


	// Password requirements

	const passwordLength =
		document.getElementById(
			"password-length");

	const passwordLowercase =
		document.getElementById(
			"password-lowercase");

	const passwordUppercase =
		document.getElementById(
			"password-uppercase");

	const passwordNumber =
		document.getElementById(
			"password-number");

	const passwordSpecial =
		document.getElementById(
			"password-special");


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	function initialize() {

		if (!form) {
			return;
		}


		initializePasswordToggles();


		newPassword.addEventListener(
			"input",
			updatePasswordRequirements);


		confirmPassword.addEventListener(
			"input",
			validatePasswordConfirmation);


		form.addEventListener(
			"submit",
			handleSubmit);


		updatePasswordRequirements();
	}


	// =========================================================
	// PASSWORD VISIBILITY
	// =========================================================

	function initializePasswordToggles() {

		const buttons =
			document.querySelectorAll(
				"[data-password-toggle]");


		buttons.forEach(
			button => {

				button.addEventListener(
					"click",
					handlePasswordToggle);
			});
	}


	function handlePasswordToggle(
		event) {

		const button =
			event.currentTarget;

		const targetId =
			button.dataset.passwordToggle;

		const input =
			document.getElementById(
				targetId);


		if (!input) {
			return;
		}


		const showPassword =
			input.type ===
			"password";


		input.type =
			showPassword
				? "text"
				: "password";


		const icon =
			button.querySelector(
				"i");


		if (!icon) {
			return;
		}


		icon.className =
			showPassword
				? "ri-eye-off-fill align-middle"
				: "ri-eye-fill align-middle";
	}


	// =========================================================
	// PASSWORD RULES
	// =========================================================

	function getPasswordRules(
		password) {

		return {

			length:
				password.length >= 8,

			lowercase:
				/[a-z]/.test(
					password),

			uppercase:
				/[A-Z]/.test(
					password),

			number:
				/[0-9]/.test(
					password),

			special:
				/[^A-Za-z0-9]/.test(
					password)
		};
	}


	function updatePasswordRequirements() {

		const password =
			newPassword.value;

		const rules =
			getPasswordRules(
				password);


		updateRequirement(
			passwordLength,
			rules.length);

		updateRequirement(
			passwordLowercase,
			rules.lowercase);

		updateRequirement(
			passwordUppercase,
			rules.uppercase);

		updateRequirement(
			passwordNumber,
			rules.number);

		updateRequirement(
			passwordSpecial,
			rules.special);


		validatePasswordConfirmation();
	}


	function updateRequirement(
		element,
		valid) {

		if (!element) {
			return;
		}


		element.classList.toggle(
			"text-muted",
			!valid);

		element.classList.toggle(
			"text-success",
			valid);


		const icon =
			element.querySelector(
				"i");


		if (!icon) {
			return;
		}


		icon.className =
			valid
				? "ri-checkbox-circle-line me-1"
				: "ri-close-circle-line me-1";
	}


	function isPasswordValid() {

		const rules =
			getPasswordRules(
				newPassword.value);


		return (
			rules.length &&
			rules.lowercase &&
			rules.uppercase &&
			rules.number &&
			rules.special
		);
	}


	// =========================================================
	// CONFIRM PASSWORD
	// =========================================================

	function validatePasswordConfirmation() {

		if (
			!confirmPassword.value
		) {

			confirmPassword.setCustomValidity(
				"");


			return true;
		}


		if (
			newPassword.value !==
			confirmPassword.value
		) {

			confirmPassword.setCustomValidity(
				"Passwords do not match.");


			if (confirmPasswordError) {

				confirmPasswordError.textContent =
					"Passwords do not match.";
			}


			return false;
		}


		confirmPassword.setCustomValidity(
			"");


		if (confirmPasswordError) {

			confirmPasswordError.textContent =
				"Please confirm your new password.";
		}


		return true;
	}


	// =========================================================
	// SUBMIT
	// =========================================================

	async function handleSubmit(
		event) {

		event.preventDefault();


		hideError();


		validatePasswordConfirmation();


		if (
			!form.checkValidity()
		) {

			form.classList.add(
				"was-validated");


			form.reportValidity();


			return;
		}


		// if (
		// 	!isPasswordValid()
		// ) {

		// 	showError(
		// 		"Your new password does not meet the password requirements.");


		// 	return;
		// }


		const request = {

			currentPassword:
				currentPassword.value,

			newPassword:
				newPassword.value
		};


		try {

			camsUi.setButtonLoading(
				submitButton,
				true,
				"Changing Password...");


			const response =
				await camsApi.post(
					"/api/v1/user/change-password",
					request,
					{
						handleAuthErrors:
							false
					});


			const result =
				await camsApi.readJson(
					response);


			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to change your password.");


				showError(
					message);


				return;
			}


			await camsUi.showSuccessBox(
				result?.message ??
				"Your password has been changed successfully.",
				{
					title:
						"Password Changed",

					buttonText:
						"Continue"
				});


			window.location.href =
				"/";
		}
		catch (error) {

			console.error(
				"Unable to change password.",
				error);


			showError(
				"Unable to change your password. Please try again.");
		}
		finally {

			camsUi.setButtonLoading(
				submitButton,
				false);
		}
	}


	// =========================================================
	// ERROR UI
	// =========================================================

	function showError(
		message) {

		if (!errorContainer) {
			return;
		}


		errorContainer.textContent =
			message;

		errorContainer.classList.remove(
			"d-none");
	}


	function hideError() {

		if (!errorContainer) {
			return;
		}


		errorContainer.textContent =
			"";

		errorContainer.classList.add(
			"d-none");
	}

})();