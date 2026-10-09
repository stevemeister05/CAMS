(() => {
	"use strict";

	const form =
		document.getElementById(
			"registration-form");

	if (!form) {
		return;
	}

	const firstName =
		document.getElementById(
			"first-name");

	const middleName =
		document.getElementById(
			"middle-name");

	const lastName =
		document.getElementById(
			"last-name");

	const mobileNumber =
		document.getElementById(
			"mobile-number");

	const birthDate =
		document.getElementById(
			"birth-date");

	const gender =
		document.getElementById(
			"gender");

	const username =
		document.getElementById(
			"username");

	const password =
		document.getElementById(
			"password");

	const registerButton =
		document.getElementById(
			"register-button");


	// SETUP
	camsForm.makeNumeric(
		mobileNumber,
		11);

	camsForm.sync(
		mobileNumber,
		username);


	// SUBMIT
	form.addEventListener(
		"submit",
		async event => {

			event.preventDefault();

			camsUi.clearAlert(
				"registration-alert");

			if (
				!camsForm.validate(
					form)
			) {
				return;
			}

			if (
				!camsUtils
					.isPhilippineMobileNumber(
						mobileNumber.value)
			) {
				camsUi.showError(
					form.parentElement,
					"Please enter a valid mobile number in the format 09xxxxxxxxx.",
					"registration-alert");

				mobileNumber.focus();

				return;
			}

			let confirm = await camsUi.confirm("You are about to submit a member registration request. Are you sure you want to proceed?");

			if (!confirm) {
				return;
			}

			const request =
			{
				firstName:
					firstName.value.trim(),

				middleName:
					camsUtils.toNullableString(
						middleName.value),

				lastName:
					lastName.value.trim(),

				mobileNumber:
					mobileNumber.value,

				password:
					password.value,

				birthDate:
					camsUtils.toNullableString(
						birthDate.value),

				gender:
					camsUtils.toNullableString(
						gender.value)
			};

			await register(
				request);
		});


	// REGISTER
	async function register(
		request) {

		camsUi.setButtonLoading(
			registerButton,
			true,
			"Registering...");

		try {

			const response =
				await camsApi.post(
					"/api/v1/registration",
					request,
					{
						handleAuthErrors:
							false
					});

			if (!response) {
				return;
			}

			const result =
				await camsApi.readJson(
					response);


			// ERROR RESPONSE
			if (!response.ok) {

				const message =
					camsApi.getErrorMessage(
						result,
						response.status,
						"Unable to submit your registration.");

				console.log(
					"Registration API error:",
					{
						status:
							response.status,

						result:
							result,

						message:
							message
					});

				camsUi.showError(
					form.parentElement,
					message,
					"registration-alert");

				return;
			}


			// SUCCESS
			await camsUi.showSuccessBox(
				"Your registration request has been submitted.",
				{
					title: "Registration Complete",
					buttonText: "Continue"
				});

			window.location.href = "/auth/login";
		}
		catch (error) {

			console.error(
				"Unexpected registration error:",
				error);

			camsUi.showError(
				form.parentElement,
				"Unable to submit your registration request. Please try again.",
				"registration-alert");
		}
		finally {

			camsUi.setButtonLoading(
				registerButton,
				false);
		}
	}

})();