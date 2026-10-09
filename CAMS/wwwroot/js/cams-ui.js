(() => {
	"use strict";

	const camsUi = {

		// ALERTS
		showAlert(
			container,
			message,
			options = {}) {

			if (!container) {
				return null;
			}

			const {
				type = "danger",
				icon = null,
				id = "cams-alert",
				dismissible = true
			} = options;

			this.clearAlert(id);

			const alert = document.createElement("div");
			alert.id = id;
			alert.className = `alert alert-${type} fade show`;
			alert.setAttribute("role", "alert");

			if (dismissible) {
				alert.classList.add("alert-dismissible");
			}

			const content =
				document.createElement("div");

			if (icon) {
				const iconElement = document.createElement("i");

				iconElement.className = `${icon} me-1`;

				content.appendChild(iconElement);
			}

			content.appendChild(document.createTextNode(message ?? ""));

			alert.appendChild(content);

			if (dismissible) {
				const closeButton = document.createElement("button");
				closeButton.type = "button";
				closeButton.className = "btn-close";
				closeButton.setAttribute("data-bs-dismiss", "alert");
				closeButton.setAttribute("aria-label", "Close");

				alert.appendChild(closeButton);
			}

			container.prepend(alert);

			return alert;
		},


		showSuccess(container, message, id = "cams-alert") {

			return this.showAlert(
				container,
				message,
				{
					id: id,
					type: "success",
					icon: "ri-checkbox-circle-line"
				});
		},

		showError(
			container,
			message,
			id = "cams-alert") {

			return this.showAlert(
				container,
				message,
				{
					id: id,
					type: "danger",
					icon: "ri-error-warning-line"
				});
		},

		showWarning(
			container,
			message,
			id = "cams-alert") {

			return this.showAlert(
				container,
				message,
				{
					id: id,
					type: "warning",
					icon: "ri-alert-line"
				});
		},

		showInfo(
			container,
			message,
			id = "cams-alert") {

			return this.showAlert(
				container,
				message,
				{
					id: id,
					type: "info",
					icon: "ri-information-line"
				});
		},

		clearAlert(id = "cams-alert") {
			document.getElementById(id)?.remove();
		},

		// TOASTS
		showToast(message, options = {}) {

			const {
				type = "success",
				title = null,
				delay = 4000,
				autohide = true,
				onHidden = null
			} = options;


			const config = this.getToastConfig(type);
			const container = this.getToastContainer();
			const toast = document.createElement("div");

			toast.className = "toast";
			toast.setAttribute("role", "alert");
			toast.setAttribute("aria-live", "assertive");
			toast.setAttribute("aria-atomic", "true");

			// HEADER
			const header = document.createElement("div");
			header.className = "toast-header";


			const icon = document.createElement("i");
			icon.className = `${config.icon} ${config.textClass} fs-5 me-2`;

			const titleElement = document.createElement("strong");
			titleElement.className = "me-auto";
			titleElement.textContent = title ?? config.title;

			const closeButton = document.createElement("button");
			closeButton.type = "button";
			closeButton.className = "btn-close";
			closeButton.setAttribute("data-bs-dismiss", "toast");
			closeButton.setAttribute("aria-label", "Close");


			header.appendChild(icon);
			header.appendChild(titleElement);
			header.appendChild(closeButton);

			// BODY
			const body = document.createElement("div");
			body.className = "toast-body";
			body.textContent = message ?? "";

			toast.appendChild(header);
			toast.appendChild(body);

			container.appendChild(toast);

			// BOOTSTRAP TOAST
			const instance =
				bootstrap.Toast.getOrCreateInstance(
					toast,
					{
						autohide:
							autohide,

						delay:
							delay
					});

			toast.addEventListener(
				"hidden.bs.toast",
				() => {
					instance.dispose();
					toast.remove();
					if (typeof onHidden === "function") {
						onHidden();
					}
				});

			instance.show();

			return instance;
		},


		showSuccessToast(
			message,
			options = {}) {

			return this.showToast(
				message,
				{
					...options,
					type: "success"
				});
		},


		showErrorToast(message, options = {}) {
			return this.showToast(
				message,
				{
					...options,
					type: "danger"
				});
		},

		showWarningToast(message, options = {}) {
			return this.showToast(
				message,
				{
					...options,
					type: "warning"
				});
		},


		showInfoToast(
			message,
			options = {}) {
			return this.showToast(
				message,
				{
					...options,
					type: "info"
				});
		},

		getToastContainer() {

			let container =
				document.getElementById(
					"cams-toast-container");


			if (container) {
				return container;
			}


			container =
				document.createElement("div");

			container.id = "cams-toast-container";

			container.className = "toast-container position-fixed top-0 end-0 p-3";

			/*
			 * Higher than Bootstrap modals, navbars,
			 * and most Steex elements.
			 */
			container.style.zIndex = "11000";

			document.body.appendChild(container);

			return container;
		},


		getToastConfig(
			type) {

			switch (type) {
				case "danger":
					return {
						title: "Error",
						icon: "ri-error-warning-line",
						textClass: "text-danger"
					};


				case "warning":
					return {
						title: "Warning",
						icon: "ri-alert-line",
						textClass: "text-warning"
					};

				case "info":
					return {
						title: "Information",
						icon: "ri-information-line",
						textClass: "text-info"
					};

				case "success":
				default:
					return {
						title: "Success",
						icon: "ri-checkbox-circle-line",
						textClass: "text-success"
					};
			}
		},
		setButtonLoading(
			button,
			isLoading,
			text = "Processing...") {

			if (!button) {
				return;
			}

			if (isLoading) {

				if (!button.dataset.originalHtml) {
					button.dataset.originalHtml = button.innerHTML;
				}

				button.disabled = true;

				button.innerHTML = `
					<span
						class="spinner-border spinner-border-sm me-1"
						role="status"
						aria-hidden="true">
					</span>

					${camsUtils.escapeHtml(text)}
				`;

				return;
			}

			button.disabled = false;
			if (button.dataset.originalHtml) {
				button.innerHTML = button.dataset.originalHtml;
			}
		},

		// MESSAGE BOX
		showMessageBox(
			message,
			options = {}) {

			const {
				type = "info",
				title = null,
				buttonText = "OK",
				backdrop = "static",
				keyboard = true
			} = options;


			const config = this.getMessageBoxConfig(type);

			/*
			 * Fallback if Bootstrap is unavailable.
			 */
			if (typeof bootstrap === "undefined" || !bootstrap.Modal) {
				window.alert(message ?? "");
				return Promise.resolve();
			}

			return new Promise(
				resolve => {
					const modal = document.createElement("div");
					modal.className = "modal fade";
					modal.tabIndex = -1;
					modal.setAttribute("aria-hidden", "true");

					// DIALOG
					const dialog = document.createElement("div");
					dialog.className = "modal-dialog modal-dialog-centered";

					const content = document.createElement("div");
					content.className = "modal-content";

					// HEADER
					const header = document.createElement("div");
					header.className = "modal-header border-0 pb-0";


					const titleElement = document.createElement("h5");

					titleElement.className = "modal-title";
					titleElement.textContent =
						title ??
						config.title;


					const closeButton =
						document.createElement(
							"button");

					closeButton.type =
						"button";

					closeButton.className =
						"btn-close";

					closeButton.setAttribute(
						"data-bs-dismiss",
						"modal");

					closeButton.setAttribute(
						"aria-label",
						"Close");


					header.appendChild(
						titleElement);

					header.appendChild(
						closeButton);


					// BODY
					const body =
						document.createElement(
							"div");

					body.className =
						"modal-body text-center px-4 py-4";


					const iconWrapper =
						document.createElement(
							"div");

					iconWrapper.className =
						`mb-3 ${config.textClass}`;


					const icon =
						document.createElement(
							"i");

					icon.className =
						`${config.icon} display-5`;

					iconWrapper.appendChild(
						icon);


					const messageElement =
						document.createElement(
							"div");

					messageElement.className =
						"fs-6";

					messageElement.textContent =
						message ?? "";


					body.appendChild(
						iconWrapper);

					body.appendChild(
						messageElement);


					// FOOTER
					const footer =
						document.createElement(
							"div");

					footer.className =
						"modal-footer border-0 justify-content-center pt-0";


					const okButton =
						document.createElement(
							"button");

					okButton.type =
						"button";

					okButton.className =
						`btn ${config.buttonClass} px-4`;

					okButton.textContent =
						buttonText;

					okButton.setAttribute(
						"data-bs-dismiss",
						"modal");


					footer.appendChild(
						okButton);


					// BUILD
					content.appendChild(
						header);

					content.appendChild(
						body);

					content.appendChild(
						footer);

					dialog.appendChild(
						content);

					modal.appendChild(
						dialog);

					document.body.appendChild(
						modal);


					const instance =
						new bootstrap.Modal(
							modal,
							{
								backdrop:
									backdrop,

								keyboard:
									keyboard
							});


					modal.addEventListener(
						"hidden.bs.modal",
						() => {

							instance.dispose();

							modal.remove();

							resolve();
						},
						{
							once:
								true
						});


					instance.show();
				});
		},


		// MESSAGE BOX TYPES
		showSuccessBox(
			message,
			options = {}) {

			return this.showMessageBox(
				message,
				{
					...options,
					type:
						"success"
				});
		},


		showErrorBox(
			message,
			options = {}) {

			return this.showMessageBox(
				message,
				{
					...options,
					type:
						"danger"
				});
		},


		showWarningBox(
			message,
			options = {}) {

			return this.showMessageBox(
				message,
				{
					...options,
					type:
						"warning"
				});
		},


		showInfoBox(
			message,
			options = {}) {

			return this.showMessageBox(
				message,
				{
					...options,
					type:
						"info"
				});
		},


		// CONFIRMATION BOX
		confirm(
			message,
			options = {}) {

			const {
				title = "Confirmation",
				type = "warning",
				confirmText = "Yes",
				cancelText = "No"
			} = options;


			const config =
				this.getMessageBoxConfig(
					type);


			/*
			 * Browser fallback.
			 */
			if (
				typeof bootstrap ===
				"undefined" ||
				!bootstrap.Modal
			) {
				return Promise.resolve(
					window.confirm(
						message ?? ""));
			}


			return new Promise(
				resolve => {

					let result =
						false;


					const modal =
						document.createElement(
							"div");

					modal.className =
						"modal fade";

					modal.tabIndex =
						-1;

					modal.setAttribute(
						"aria-hidden",
						"true");


					// DIALOG
					const dialog =
						document.createElement(
							"div");

					dialog.className =
						"modal-dialog modal-dialog-centered";


					const content =
						document.createElement(
							"div");

					content.className =
						"modal-content";


					// HEADER
					const header =
						document.createElement(
							"div");

					header.className =
						"modal-header border-0 pb-0";


					const titleElement =
						document.createElement(
							"h5");

					titleElement.className =
						"modal-title";

					titleElement.textContent =
						title;


					const closeButton =
						document.createElement(
							"button");

					closeButton.type =
						"button";

					closeButton.className =
						"btn-close";

					closeButton.setAttribute(
						"data-bs-dismiss",
						"modal");

					closeButton.setAttribute(
						"aria-label",
						"Close");


					header.appendChild(
						titleElement);

					header.appendChild(
						closeButton);


					// BODY
					const body =
						document.createElement(
							"div");

					body.className =
						"modal-body text-center px-4 py-4";


					const iconWrapper =
						document.createElement(
							"div");

					iconWrapper.className =
						`mb-3 ${config.textClass}`;


					const icon =
						document.createElement(
							"i");

					icon.className =
						`${config.icon} display-5`;

					iconWrapper.appendChild(
						icon);


					const messageElement =
						document.createElement(
							"div");

					messageElement.className =
						"fs-6";

					messageElement.textContent =
						message ?? "";


					body.appendChild(
						iconWrapper);

					body.appendChild(
						messageElement);


					// FOOTER
					const footer =
						document.createElement(
							"div");

					footer.className =
						"modal-footer border-0 justify-content-center pt-0";


					const noButton =
						document.createElement(
							"button");

					noButton.type =
						"button";

					noButton.className =
						"btn btn-light px-4";

					noButton.textContent =
						cancelText;

					noButton.setAttribute(
						"data-bs-dismiss",
						"modal");


					const yesButton =
						document.createElement(
							"button");

					yesButton.type =
						"button";

					yesButton.className =
						`btn ${config.buttonClass} px-4`;

					yesButton.textContent =
						confirmText;


					yesButton.addEventListener(
						"click",
						() => {

							result =
								true;

							instance.hide();
						});


					footer.appendChild(
						noButton);

					footer.appendChild(
						yesButton);


					// BUILD
					content.appendChild(
						header);

					content.appendChild(
						body);

					content.appendChild(
						footer);

					dialog.appendChild(
						content);

					modal.appendChild(
						dialog);

					document.body.appendChild(
						modal);


					const instance =
						new bootstrap.Modal(
							modal,
							{
								backdrop:
									"static",

								keyboard:
									true
							});


					modal.addEventListener(
						"hidden.bs.modal",
						() => {

							instance.dispose();

							modal.remove();

							resolve(
								result);
						},
						{
							once:
								true
						});


					instance.show();
				});
		},


		// MESSAGE CONFIG
		getMessageBoxConfig(type) {
			switch (type) {

				case "success":
					return {
						title: "Success",
						icon: "ri-checkbox-circle-line",
						textClass: "text-success",
						buttonClass: "btn-success"
					};
				case "danger":
					return {
						title: "Error",
						icon: "ri-error-warning-line",
						textClass: "text-danger",
						buttonClass: "btn-danger"
					};
				case "warning":
					return {
						title: "Warning",
						icon: "ri-alert-line",
						textClass: "text-warning",
						buttonClass: "btn-warning"
					};
				case "info":
				default:
					return {
						title: "Information",
						icon: "ri-information-line",
						textClass: "text-primary",
						buttonClass: "btn-primary"
					};
			}
		},
	};


	window.camsUi =
		Object.freeze(
			camsUi);

})();