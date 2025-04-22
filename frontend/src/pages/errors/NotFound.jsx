import React from "react";
import { useNavigate } from "react-router-dom";

const NotFound = () => {
  const navigate = useNavigate();

  const goBack = () => {
    navigate(-1); // Go to previous page
  };

  return (
    <div className="container d-flex align-items-center justify-content-center mb-0">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-danger">404</h1>
        <h2 className="mb-3">Page Not Found</h2>
        <p className="text-muted mb-4">
          The page you’re looking for doesn’t exist or was moved.
        </p>
        <button onClick={goBack} className="btn btn-outline-primary">
          ← Go Back
        </button>
      </div>
    </div>
  );
};

export default NotFound;
