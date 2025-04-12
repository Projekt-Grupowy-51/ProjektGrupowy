import React from "react";

const DeleteButton = ({ onClick, label = "Delete" }) => {
  return (
    <button className="btn btn-danger me-2 text-nowrap" onClick={onClick}>
      <i className="fas fa-trash-alt me-1"></i> {label}
    </button>
  );
};

export default DeleteButton;
