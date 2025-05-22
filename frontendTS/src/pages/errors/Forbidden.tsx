import React from "react";

const NotFound = () => {
  return (
    <div className="container d-flex align-items-center justify-content-center mb-0">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-danger">403</h1>
        <h2 className="mb-3">Forbidden</h2>
        <p className="text-muted">
          Looks like you don't have permission to access this page. Please
          contact your administrator if you believe this is an error.
        </p>

      </div>
    </div>
  );
};

export default NotFound;
