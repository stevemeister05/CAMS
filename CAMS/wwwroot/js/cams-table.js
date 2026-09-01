(() => {
	"use strict";


	const camsTable = {


		// =====================================================
		// LOADING
		// =====================================================

		showLoading(
			tableBody,
			options = {}) {

			if (!tableBody) {
				return;
			}


			const {
				columnCount = 1,
				message = "Loading..."
			} = options;


			tableBody.innerHTML = `
				<tr>
					<td colspan="${columnCount}"
						class="text-center py-5 text-muted">

						<div class="spinner-border
									spinner-border-sm me-2"
							 role="status">
						</div>

						${camsUtils.escapeHtml(
				message)}

					</td>
				</tr>
			`;
		},


		// =====================================================
		// EMPTY
		// =====================================================

		showEmpty(
			tableBody,
			options = {}) {

			if (!tableBody) {
				return;
			}


			const {
				columnCount = 1,
				title = "No records found",
				message = "",
				icon = "ri-inbox-line"
			} = options;


			tableBody.innerHTML = `
				<tr>
					<td colspan="${columnCount}"
						class="text-center py-5">

						<div class="avatar-md mx-auto mb-3">

							<div class="avatar-title
										bg-light text-muted
										rounded-circle fs-3">

								<i class="${camsUtils.escapeHtml(
				icon)}"></i>

							</div>

						</div>

						<h6 class="mb-1">
							${camsUtils.escapeHtml(
					title)}
						</h6>

						${message
					? `
								<p class="text-muted mb-0">
									${camsUtils.escapeHtml(
						message)}
								</p>
							`
					: ""
				}

					</td>
				</tr>
			`;
		},


		// =====================================================
		// ERROR
		// =====================================================

		showError(
			tableBody,
			message,
			options = {}) {

			if (!tableBody) {
				return;
			}


			const {
				columnCount = 1,
				icon = "ri-error-warning-line"
			} = options;


			tableBody.innerHTML = `
				<tr>
					<td colspan="${columnCount}"
						class="text-center py-5">

						<div class="text-danger mb-2">

							<i class="${camsUtils.escapeHtml(
				icon)} fs-2"></i>

						</div>

						<div>
							${camsUtils.escapeHtml(
					message)}
						</div>

					</td>
				</tr>
			`;
		},


		// =====================================================
		// ROWS
		// =====================================================

		renderRows(
			tableBody,
			items,
			rowRenderer,
			emptyOptions = {}) {

			if (!tableBody) {
				return;
			}


			if (
				!Array.isArray(
					items) ||
				!items.length
			) {

				this.showEmpty(
					tableBody,
					emptyOptions);

				return;
			}


			tableBody.innerHTML =
				items
					.map(
						rowRenderer)
					.join("");
		},


		// =====================================================
		// PAGINATION
		// =====================================================

		renderPagination(
			container,
			options = {}) {

			if (!container) {
				return;
			}


			const {
				currentPage = 1,
				totalPages = 1,
				siblingCount = 2,
				onPageChange
			} = options;


			container.innerHTML =
				"";


			if (
				totalPages <= 1
			) {
				return;
			}


			container.appendChild(
				createPageItem(
					{
						label:
							"Previous",

						page:
							currentPage - 1,

						disabled:
							currentPage <= 1,

						onPageChange
					}));


			const firstPage =
				Math.max(
					1,
					currentPage -
					siblingCount);


			const lastPage =
				Math.min(
					totalPages,
					currentPage +
					siblingCount);


			if (
				firstPage > 1
			) {

				container.appendChild(
					createPageItem(
						{
							label:
								"1",

							page:
								1,

							active:
								currentPage === 1,

							onPageChange
						}));


				if (
					firstPage > 2
				) {

					container.appendChild(
						createEllipsis());
				}
			}


			for (
				let page = firstPage;
				page <= lastPage;
				page++
			) {

				container.appendChild(
					createPageItem(
						{
							label:
								page.toString(),

							page,

							active:
								page === currentPage,

							onPageChange
						}));
			}


			if (
				lastPage < totalPages
			) {

				if (
					lastPage <
					totalPages - 1
				) {

					container.appendChild(
						createEllipsis());
				}


				container.appendChild(
					createPageItem(
						{
							label:
								totalPages.toString(),

							page:
								totalPages,

							active:
								currentPage ===
								totalPages,

							onPageChange
						}));
			}


			container.appendChild(
				createPageItem(
					{
						label:
							"Next",

						page:
							currentPage + 1,

						disabled:
							currentPage >=
							totalPages,

						onPageChange
					}));
		},


		// =====================================================
		// SUMMARY
		// =====================================================

		renderSummary(
			summaryElement,
			infoElement,
			options = {}) {

			const {
				totalCount = 0,
				currentPage = 1,
				pageSize = 20,
				singularLabel = "record",
				pluralLabel = "records"
			} = options;


			if (summaryElement) {

				summaryElement.textContent =
					`${totalCount} ${totalCount === 1
						? singularLabel
						: pluralLabel
					}`;
			}


			if (!infoElement) {
				return;
			}


			if (
				totalCount <= 0
			) {

				infoElement.textContent =
					"";

				return;
			}


			const start =
				((currentPage - 1) *
					pageSize) + 1;


			const end =
				Math.min(
					currentPage *
					pageSize,
					totalCount);


			infoElement.textContent =
				`Showing ${start}-${end} of ${totalCount}`;
		},


		// =====================================================
		// PAGED RESULT
		// =====================================================

		getPaginationInfo(
			result,
			fallback = {}) {

			const page =
				Number(
					result?.page ??
					fallback.page ??
					1);


			const pageSize =
				Number(
					result?.pageSize ??
					fallback.pageSize ??
					20);


			const totalCount =
				Number(
					result?.totalCount ??
					0);


			const totalPages =
				Number(
					result?.totalPages ??
					Math.max(
						1,
						Math.ceil(
							totalCount /
							pageSize)));


			return {
				page,
				pageSize,
				totalCount,
				totalPages
			};
		}
	};


	// =========================================================
	// PRIVATE HELPERS
	// =========================================================

	function createPageItem(
		options) {

		const {
			label,
			page,
			disabled = false,
			active = false,
			onPageChange
		} = options;


		const item =
			document.createElement(
				"li");


		item.className =
			"page-item";


		if (disabled) {

			item.classList.add(
				"disabled");
		}


		if (active) {

			item.classList.add(
				"active");
		}


		const button =
			document.createElement(
				"button");


		button.type =
			"button";

		button.className =
			"page-link";

		button.textContent =
			label;

		button.disabled =
			disabled;


		button.addEventListener(
			"click",
			() => {

				if (
					disabled ||
					active ||
					typeof onPageChange !==
					"function"
				) {
					return;
				}


				onPageChange(
					page);
			});


		item.appendChild(
			button);


		return item;
	}


	function createEllipsis() {

		const item =
			document.createElement(
				"li");


		item.className =
			"page-item disabled";


		const span =
			document.createElement(
				"span");


		span.className =
			"page-link";

		span.textContent =
			"…";


		item.appendChild(
			span);


		return item;
	}


	// =========================================================
	// EXPORT
	// =========================================================

	window.camsTable =
		Object.freeze(
			camsTable);

})();