import React from "react";

const NotFound = () => {

  return (
    <div className="container d-flex align-items-center justify-content-center mb-0">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-warning">404</h1>
        <h2 className="mb-3">Page Not Found</h2>
        <p className="text-muted">
          The page you’re looking for doesn’t exist or has been moved.
        </p>

      </div>
    </div>
  );
};

export default NotFound;
