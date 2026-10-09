(() => {
	"use strict";


	// =========================================================
	// PAGE
	// =========================================================

	const page =
		document.getElementById(
			"portal-attendance-scan-page");


	if (!page) {
		return;
	}


	// =========================================================
	// ELEMENTS
	// =========================================================

	const alertContainer =
		document.getElementById(
			"attendance-scan-alert");

	const scannerStatus =
		document.getElementById(
			"scanner-status");

	const restartButton =
		document.getElementById(
			"restart-scanner-button");


	// =========================================================
	// STATE
	// =========================================================

	let scanner =
		null;

	let scannerStarted =
		false;

	let isProcessing =
		false;


	// =========================================================
	// INITIALIZATION
	// =========================================================

	initialize();


	async function initialize() {

		restartButton?.addEventListener(
			"click",
			startScanner);


		if (!window.isSecureContext) {

			showScannerError(
				"Camera access requires a secure HTTPS connection.");

			return;
		}


		if (
			typeof Html5Qrcode ===
			"undefined"
		) {

			showScannerError(
				"The QR scanner could not be loaded.");

			return;
		}


		scanner =
			new Html5Qrcode(
				"qr-reader");


		await startScanner();
	}


	// =========================================================
	// START SCANNER
	// =========================================================

	async function startScanner() {

		if (
			scannerStarted ||
			isProcessing
		) {
			return;
		}


		camsUi.clearAlert(
			"attendance-scan-alert-message");


		restartButton?.classList.add(
			"d-none");


		scannerStatus.textContent =
			"Starting camera...";


		try {

			await scanner.start(
				{
					facingMode:
						"environment"
				},
				{
					fps:
						10,

					qrbox:
						getQrBox
				},
				handleScanSuccess,
				handleScanFailure);


			scannerStarted =
				true;


			scannerStatus.textContent =
				"Ready to scan.";
		}
		catch (error) {

			console.error(
				"Unable to start QR scanner.",
				error);


			showScannerError(
				getCameraErrorMessage(
					error));
		}
	}


	// =========================================================
	// STOP SCANNER
	// =========================================================

	async function stopScanner() {

		if (
			!scanner ||
			!scannerStarted
		) {
			return;
		}


		try {

			await scanner.stop();
		}
		catch (error) {

			console.warn(
				"Unable to stop QR scanner.",
				error);
		}
		finally {

			scannerStarted =
				false;
		}
	}


	// =========================================================
	// QR BOX
	// =========================================================

	function getQrBox(
		viewfinderWidth,
		viewfinderHeight) {

		const minimumDimension =
			Math.min(
				viewfinderWidth,
				viewfinderHeight);


		const size =
			Math.floor(
				minimumDimension *
				0.75);


		return {
			width:
				size,

			height:
				size
		};
	}


	// =========================================================
	// SCAN SUCCESS
	// =========================================================

	async function handleScanSuccess(
		decodedText) {

		if (isProcessing) {
			return;
		}


		const token =
			decodedText?.trim();


		if (!token) {
			return;
		}


		isProcessing =
			true;


		scannerStatus.textContent =
			"QR code detected. Recording attendance...";


		await stopScanner();


		try {

			const response =
				await camsApi.post(
					"/api/v1/portal/attendance/qr",
					{
						token:
							token
					});


			if (!response) {

				isProcessing =
					false;

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
						"Unable to record attendance."));
			}


			scannerStatus.textContent =
				"Attendance recorded successfully.";


			await camsUi.showSuccessBox(
				payload.message ??
				"Attendance recorded successfully.",
				{
					title:
						"Attendance Recorded"
				});


			isProcessing =
				false;


			await startScanner();
		}
		catch (error) {

			console.error(
				"Unable to record QR attendance.",
				error);


			scannerStatus.textContent =
				"Attendance was not recorded.";


			await camsUi.showErrorBox(
				error.message ??
				"Unable to record attendance.",
				{
					title:
						"Attendance Not Recorded"
				});


			isProcessing =
				false;


			restartButton?.classList.remove(
				"d-none");
		}
	}


	// =========================================================
	// SCAN FAILURE
	// =========================================================

	function handleScanFailure() {

		/*
		 * This callback fires continuously whenever
		 * the current video frame does not contain
		 * a readable QR code.
		 *
		 * This is normal while scanning, so we do
		 * not show an error to the member.
		 */
	}


	// =========================================================
	// ERROR HANDLING
	// =========================================================

	function showScannerError(
		message) {

		scannerStatus.textContent =
			"Camera unavailable.";


		camsUi.showError(
			alertContainer,
			message,
			"attendance-scan-alert-message");


		restartButton?.classList.remove(
			"d-none");
	}


	function getCameraErrorMessage(
		error) {

		const name =
			error?.name ??
			"";


		switch (name) {

			case "NotAllowedError":

				return "Camera permission was denied. " +
					"Please allow camera access and try again.";


			case "NotFoundError":

				return "No camera was found on this device.";


			case "NotReadableError":

				return "The camera is currently unavailable " +
					"or is being used by another application.";


			default:

				return "Unable to access the camera. " +
					"Please check your camera permission " +
					"and try again.";
		}
	}


	// =========================================================
	// CLEANUP
	// =========================================================

	window.addEventListener(
		"pagehide",
		() => {

			if (
				!scanner ||
				!scannerStarted
			) {
				return;
			}


			scanner.stop()
				.catch(
					() => {
					});
		});

})();