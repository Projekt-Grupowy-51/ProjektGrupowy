import React from "react";
import { useNavigate } from "react-router-dom";

const ViewDetailsButton = ({ path }) => {
  const navigate = useNavigate();

  return (
    <button
      className="btn btn-info me-2"
      onClick={() => navigate(path)}
    >
      <i className="fas fa-eye me-1"></i>Details
    </button>
  );
};

export default ViewDetailsButton;
