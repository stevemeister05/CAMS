const camsApi = {

	async fetch(
		url,
		options = {}) {

		const {
			handleAuthErrors = true,
			...fetchOptions
		} = options;


		const response =
			await fetch(
				url,
				{
					...fetchOptions,

					headers:
					{
						"Content-Type":
							"application/json",

						...(
							fetchOptions.headers ??
							{}
						)
					}
				});


		if (
			response.status === 401 &&
			handleAuthErrors
		) {

			const returnUrl =
				window.location.pathname +
				window.location.search;

			window.location.href =
				"/auth/login?returnUrl=" +
				encodeURIComponent(
					returnUrl);

			return null;
		}


		if (
			response.status === 403 &&
			handleAuthErrors
		) {

			const result =
				await this.readJson(
					response);


			if (
				result?.code ===
				"PASSWORD_CHANGE_REQUIRED"
			) {

				window.location.href =
					"/auth/change-password";

				return null;
			}
		}


		return response;
	},


	async get(
		url,
		options = {}) {

		return await this.fetch(
			url,
			{
				...options,
				method:
					"GET"
			});
	},


	async post(
		url,
		body,
		options = {}) {

		return await this.fetch(
			url,
			{
				...options,

				method:
					"POST",

				body:
					JSON.stringify(
						body)
			});
	},


	async put(
		url,
		body,
		options = {}) {

		return await this.fetch(
			url,
			{
				...options,

				method:
					"PUT",

				body:
					JSON.stringify(
						body)
			});
	},


	async delete(
		url,
		options = {}) {

		return await this.fetch(
			url,
			{
				...options,
				method:
					"DELETE"
			});
	},


	// =========================================================
	// RESPONSE
	// =========================================================

	async readJson(
		response) {

		if (!response) {
			return null;
		}


		const contentType =
			response.headers.get(
				"content-type");


		if (
			!contentType ||
			!contentType.includes(
				"application/json")
		) {
			return null;
		}


		try {

			return await response.json();

		}
		catch {

			return null;
		}
	},


	// =========================================================
	// ERRORS
	// =========================================================

	getErrorMessage(
		result,
		statusCode,
		fallback =
			"An unexpected error occurred.") {

		/*
		 * CAMS ApiResponse
		 */
		if (result?.message) {

			return result.message;
		}


		/*
		 * ASP.NET ProblemDetails
		 */
		if (result?.detail) {

			return result.detail;
		}


		/*
		 * ASP.NET ValidationProblemDetails
		 */
		if (result?.errors) {

			const validationMessage =
				this.getValidationErrorMessage(
					result.errors);

			if (validationMessage) {

				return validationMessage;
			}
		}


		if (result?.title) {

			return result.title;
		}


		switch (statusCode) {

			case 400:
				return "Please check the information you entered.";

			case 401:
				return "You must be signed in to continue.";

			case 403:
				return "You do not have permission to perform this action.";

			case 404:
				return "The requested information could not be found.";

			case 409:
				return "The request could not be completed because of a conflict.";

			case 429:
				return "Too many requests. Please try again later.";

			case 500:
				return "An unexpected server error occurred.";

			default:
				return fallback;
		}
	},


	getValidationErrorMessage(
		errors) {

		if (!errors) {
			return null;
		}


		/*
		 * ASP.NET ValidationProblemDetails:
		 *
		 * {
		 *     errors: {
		 *         FirstName: ["First name is required."]
		 *     }
		 * }
		 */
		if (
			typeof errors ===
			"object" &&
			!Array.isArray(errors)
		) {

			const messages =
				Object.values(errors)
					.flat()
					.filter(Boolean);

			if (messages.length > 0) {

				return messages[0];
			}
		}


		/*
		 * CAMS ApiError[]
		 */
		if (Array.isArray(errors)) {

			const first =
				errors[0];

			if (
				typeof first ===
				"string"
			) {

				return first;
			}


			return (
				first?.message ??
				first?.detail ??
				null
			);
		}


		return null;
	}
};


window.camsApi = camsApi;