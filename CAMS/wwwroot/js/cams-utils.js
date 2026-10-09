(() => {
	"use strict";

	// =========================================================
	// PRIVATE DATE HELPERS
	// =========================================================

	function parseDateTime(
		value,
		options = {}) {

		if (!value) {
			return null;
		}


		if (
			value instanceof Date
		) {
			return value;
		}


		const {
			assumeUtc = false
		} = options;


		let normalizedValue =
			value.toString();


		/*
		 * .NET can serialize DateTime values with
		 * more than 3 fractional second digits.
		 *
		 * JavaScript only needs milliseconds.
		 */
		normalizedValue =
			normalizedValue.replace(
				/(\.\d{3})\d+/,
				"$1");


		/*
		 * Attendance/audit timestamps are stored as UTC.
		 *
		 * If the API returns a DateTime without timezone
		 * information, explicitly mark it as UTC.
		 *
		 * Example:
		 *
		 * 2026-09-03T06:00:00
		 *
		 * becomes:
		 *
		 * 2026-09-03T06:00:00Z
		 */
		if (
			assumeUtc &&
			!/[zZ]$/.test(
				normalizedValue) &&
			!/[+-]\d{2}:\d{2}$/.test(
				normalizedValue)
		) {

			normalizedValue +=
				"Z";
		}


		const date =
			new Date(
				normalizedValue);


		return Number.isNaN(
			date.getTime())
			? null
			: date;
	}


	const camsUtils = {

		// =====================================================
		// STRING
		// =====================================================

		toNullableString(value) {

			const result =
				value?.trim();

			return result
				? result
				: null;
		},


		escapeHtml(value) {

			if (
				value === null ||
				value === undefined
			) {
				return "";
			}

			return String(value)
				.replaceAll("&", "&amp;")
				.replaceAll("<", "&lt;")
				.replaceAll(">", "&gt;")
				.replaceAll('"', "&quot;")
				.replaceAll("'", "&#039;");
		},


		normalizeGuid(value) {

			return String(
				value ?? "")
				.trim()
				.toLowerCase();
		},


		// =====================================================
		// MOBILE NUMBER
		// =====================================================

		isPhilippineMobileNumber(value) {

			return /^09\d{9}$/
				.test(
					String(value ?? ""));
		},


		normalizeMobileNumber(value) {

			return String(
				value ?? "")
				.replace(/\D/g, "")
				.slice(0, 11);
		},


		// =====================================================
		// DATE
		// =====================================================

		formatDate(
			value,
			options = {}) {

			if (!value) {
				return "-";
			}


			const {
				locale = "en-PH",
				month = "short"
			} = options;


			const match =
				/^(\d{4})-(\d{2})-(\d{2})$/
					.exec(
						value);


			let date;


			if (match) {

				date =
					new Date(
						Number(
							match[1]),

						Number(
							match[2]) - 1,

						Number(
							match[3]));
			}
			else {

				date =
					parseDateTime(
						value);
			}


			if (
				!date ||
				Number.isNaN(
					date.getTime())
			) {
				return value;
			}


			return date.toLocaleDateString(
				locale,
				{
					year:
						"numeric",

					month:
						month,

					day:
						"2-digit"
				});
		},


		formatDateTime(
			value,
			options = {}) {

			if (!value) {
				return "-";
			}


			const {
				locale = "en-PH",
				includeSeconds = false,
				hour12 = true,
				assumeUtc = false
			} = options;


			const date =
				parseDateTime(
					value,
					{
						assumeUtc:
							assumeUtc
					});


			if (!date) {
				return value;
			}


			const formatOptions = {

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
					hour12
			};


			if (
				includeSeconds
			) {

				formatOptions.second =
					"2-digit";
			}


			return date.toLocaleString(
				locale,
				formatOptions);
		},

		formatTime(
			value,
			options = {}) {

			if (!value) {
				return "-";
			}


			const {
				locale = "en-PH",
				includeSeconds = false,
				hour12 = true,
				assumeUtc = false
			} = options;


			const date =
				parseDateTime(
					value,
					{
						assumeUtc:
							assumeUtc
					});


			if (!date) {
				return value;
			}


			const formatOptions = {

				hour:
					"numeric",

				minute:
					"2-digit",

				hour12:
					hour12
			};


			if (
				includeSeconds
			) {

				formatOptions.second =
					"2-digit";
			}


			return date.toLocaleTimeString(
				locale,
				formatOptions);
		},


		formatTimeOnly(
			value,
			options = {}) {

			if (!value) {
				return "-";
			}


			const {
				hour12 = true
			} = options;


			const parts =
				value
					.toString()
					.split(":");


			if (
				parts.length < 2
			) {
				return value;
			}


			const hour =
				Number(
					parts[0]);

			const minute =
				Number(
					parts[1]);


			if (
				Number.isNaN(
					hour) ||
				Number.isNaN(
					minute)
			) {
				return value;
			}


			if (!hour12) {

				return (
					String(hour)
						.padStart(
							2,
							"0") +
					":" +
					String(minute)
						.padStart(
							2,
							"0")
				);
			}


			const suffix =
				hour >= 12
					? "PM"
					: "AM";


			const displayHour =
				hour % 12 ||
				12;


			return (
				displayHour +
				":" +
				String(minute)
					.padStart(
						2,
						"0") +
				" " +
				suffix
			);
		},


		toDateInput(
			value) {

			if (!value) {
				return "";
			}


			const match =
				/^(\d{4}-\d{2}-\d{2})/
					.exec(
						value);


			return match
				? match[1]
				: "";
		},


		toTimeInput(
			value) {

			if (!value) {
				return "";
			}


			const match =
				/^(\d{2}):(\d{2})/
					.exec(
						value);


			if (!match) {
				return "";
			}


			return (
				match[1] +
				":" +
				match[2]
			);
		},


		normalizeTimeRequest(
			value) {

			if (!value) {
				return null;
			}


			if (
				/^\d{2}:\d{2}$/
					.test(
						value)
			) {

				return `${value}:00`;
			}


			return value;
		},


		parseDateTime(
			value) {

			return parseDateTime(
				value);
		}
	};


	window.camsUtils =
		Object.freeze(
			camsUtils);

})();