import React from "react";
import PropTypes from "prop-types";

/**
 * Pagination component for navigating through pages of data
 */
const Pagination = ({
  currentPage,
  totalPages,
  onPageChange,
  pageSize,
  onPageSizeChange,
  totalItems,
  pageSizeOptions = [5, 10, 20, 50],
  className = "",
}) => {
  const handlePrevious = () => {
    if (currentPage > 1) {
      onPageChange(currentPage - 1);
    }
  };

  const handleNext = () => {
    if (currentPage < totalPages) {
      onPageChange(currentPage + 1);
    }
  };

  const handlePageClick = (page) => {
    if (page !== currentPage) {
      onPageChange(page);
    }
  };

  const getPageNumbers = () => {
    const pages = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      // Show all pages if total is small
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show first, last, and pages around current
      pages.push(1);

      let start = Math.max(2, currentPage - 1);
      let end = Math.min(totalPages - 1, currentPage + 1);

      if (start > 2) {
        pages.push("...");
      }

      for (let i = start; i <= end; i++) {
        pages.push(i);
      }

      if (end < totalPages - 1) {
        pages.push("...");
      }

      pages.push(totalPages);
    }

    return pages;
  };

  const startItem = (currentPage - 1) * pageSize + 1;
  const endItem = Math.min(currentPage * pageSize, totalItems);

  // Show page size selector even with single page, hide page navigation if only one page
  const showPageNavigation = totalPages > 1;

  return (
    <div
      className={`d-flex flex-column flex-md-row justify-content-between align-items-center gap-3 ${className}`}
    >
      {/* <div className="text-muted small">
        Showing {startItem} to {endItem} of {totalItems} items
      </div> */}

      {showPageNavigation && (
        <nav aria-label="Page navigation">
          <ul className="pagination mb-0">
            <li className={`page-item ${currentPage === 1 ? "disabled" : ""}`}>
              <button
                className="page-link"
                onClick={handlePrevious}
                disabled={currentPage === 1}
                aria-label="Previous"
              >
                <i className="fas fa-chevron-left"></i>
              </button>
            </li>

            {getPageNumbers().map((page, index) => (
              <li
                key={index}
                className={`page-item ${page === currentPage ? "active" : ""} ${
                  page === "..." ? "disabled" : ""
                }`}
              >
                {page === "..." ? (
                  <span className="page-link">...</span>
                ) : (
                  <button
                    className="page-link"
                    onClick={() => handlePageClick(page)}
                  >
                    {page}
                  </button>
                )}
              </li>
            ))}

            <li
              className={`page-item ${
                currentPage === totalPages ? "disabled" : ""
              }`}
            >
              <button
                className="page-link"
                onClick={handleNext}
                disabled={currentPage === totalPages}
                aria-label="Next"
              >
                <i className="fas fa-chevron-right"></i>
              </button>
            </li>
          </ul>
        </nav>
      )}

      {onPageSizeChange && (
        <div className="d-flex align-items-center gap-2">
          {/* <label htmlFor="pageSize" className="text-muted small mb-0">
            Items per page:
          </label> */}
          <select
            id="pageSize"
            className="form-select form-select-sm"
            style={{ width: "auto" }}
            value={pageSize}
            onChange={(e) => onPageSizeChange(Number(e.target.value))}
          >
            {pageSizeOptions.map((size) => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
        </div>
      )}
    </div>
  );
};

Pagination.propTypes = {
  currentPage: PropTypes.number.isRequired,
  totalPages: PropTypes.number.isRequired,
  onPageChange: PropTypes.func.isRequired,
  pageSize: PropTypes.number,
  onPageSizeChange: PropTypes.func,
  totalItems: PropTypes.number,
  pageSizeOptions: PropTypes.arrayOf(PropTypes.number),
  className: PropTypes.string,
};

export default Pagination;
