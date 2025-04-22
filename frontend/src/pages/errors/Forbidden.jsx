import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../App";

const NotFound = () => {
  const { roles } = useAuth();
  return (
    <div className="container d-flex align-items-center justify-content-center mb-0">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-danger">403</h1>
        <h2 className="mb-3">Forbidden</h2>
        <p className="text-muted">
          Looks like you don't have permission to access this page. Please
          contact your administrator if you believe this is an error.
        </p>
        {roles.includes("Labeler") && (
          <Link
            to="/labeler-video-groups"
            className="btn btn-outline-primary me-2"
          >
            Back to labeler dashboard
          </Link>
        )}
        {roles.includes("Scientist") && (
          <Link to="/projects" className="btn btn-outline-primary">
            Back to scientist dashboard
          </Link>
        )}
      </div>
    </div>
  );
};

export default NotFound;
