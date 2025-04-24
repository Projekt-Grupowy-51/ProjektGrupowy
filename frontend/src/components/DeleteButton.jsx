import React, { useState } from "react";
import DeleteConfirmationModal from "./DeleteConfirmationModal";
import { useNotification } from "../context/NotificationContext";

const DeleteButton = ({
  onClick,
  itemType = "item",
  buttonText = "Delete",
  className = "",
  style = {},
}) => {
  const [showModal, setShowModal] = useState(false);
  const { addNotification } = useNotification();

  const handleOpenModal = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
  };

  const handleConfirm = async () => {
    try {
      await onClick();
      //addNotification(`Successfully deleted ${itemType}`, "success");
    } catch (error) {
    } finally {
      setShowModal(false);
    }
  };

  return (
    <>
      <button
        type="button"
        className={`btn btn-danger ${className}`}
        onClick={handleOpenModal}
        style={style}
      >
        <i className="fas fa-trash-alt"></i> {buttonText}
      </button>

      <DeleteConfirmationModal
        show={showModal}
        itemType={itemType}
        onConfirm={handleConfirm}
        onCancel={handleCloseModal}
      />
    </>
  );
};

export default DeleteButton;
