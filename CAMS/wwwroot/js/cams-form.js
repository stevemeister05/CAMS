(() => {
	"use strict";


	const camsForm = {


		// =====================================================
		// VALIDATION
		// =====================================================

		validate(
			form) {

			if (!form) {
				return false;
			}


			if (
				form.checkValidity()
			) {

				form.classList.remove(
					"was-validated");

				return true;
			}


			form.classList.add(
				"was-validated");

			form.reportValidity();


			return false;
		},


		resetValidation(
			form) {

			if (!form) {
				return;
			}


			form.classList.remove(
				"was-validated");


			const invalidFields =
				form.querySelectorAll(
					".is-invalid");


			invalidFields.forEach(
				field => {

					field.classList.remove(
						"is-invalid");
				});


			const validFields =
				form.querySelectorAll(
					".is-valid");


			validFields.forEach(
				field => {

					field.classList.remove(
						"is-valid");
				});
		},


		// =====================================================
		// FORM ERROR
		// =====================================================

		showError(
			container,
			message) {

			if (!container) {
				return;
			}


			container.textContent =
				message ?? "";

			container.classList.remove(
				"d-none");
		},


		hideError(
			container) {

			if (!container) {
				return;
			}


			container.textContent =
				"";

			container.classList.add(
				"d-none");
		},


		// =====================================================
		// FIELD VALIDATION STATE
		// =====================================================

		setInvalid(
			input,
			message = null,
			feedbackElement = null) {

			if (!input) {
				return;
			}


			input.classList.add(
				"is-invalid");

			input.classList.remove(
				"is-valid");


			if (
				feedbackElement &&
				message
			) {

				feedbackElement.textContent =
					message;
			}
		},


		setValid(
			input) {

			if (!input) {
				return;
			}


			input.classList.remove(
				"is-invalid");

			input.classList.add(
				"is-valid");
		},


		clearFieldState(
			input) {

			if (!input) {
				return;
			}


			input.classList.remove(
				"is-invalid");

			input.classList.remove(
				"is-valid");


			input.setCustomValidity(
				"");
		},


		// =====================================================
		// NUMERIC INPUT
		// =====================================================

		makeNumeric(
			input,
			maxLength = null) {

			if (!input) {
				return;
			}


			input.addEventListener(
				"input",
				() => {

					let value =
						input.value
							.replace(
								/\D/g,
								"");


					if (
						maxLength !== null
					) {

						value =
							value.slice(
								0,
								maxLength);
					}


					input.value =
						value;
				});
		},


		// =====================================================
		// FIELD SYNC
		// =====================================================

		sync(
			source,
			target,
			transform = null) {

			if (
				!source ||
				!target
			) {
				return;
			}


			const update =
				() => {

					const value =
						typeof transform ===
							"function"
							? transform(
								source.value)
							: source.value;


					target.value =
						value;
				};


			source.addEventListener(
				"input",
				update);


			update();
		},


		// =====================================================
		// RESET
		// =====================================================

		reset(
			form,
			options = {}) {

			if (!form) {
				return;
			}


			const {
				errorContainer = null,
				clearCustomValidity = true
			} = options;


			form.reset();


			this.resetValidation(
				form);


			if (
				clearCustomValidity
			) {

				const fields =
					form.querySelectorAll(
						"input, select, textarea");


				fields.forEach(
					field => {

						field.setCustomValidity(
							"");
					});
			}


			if (
				errorContainer
			) {

				this.hideError(
					errorContainer);
			}
		}
	};


	window.camsForm =
		Object.freeze(
			camsForm);

})();