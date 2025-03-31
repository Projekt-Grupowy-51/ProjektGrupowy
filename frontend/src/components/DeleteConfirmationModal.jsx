import React from 'react';
import './css/Modal.css'

const DeleteConfirmationModal = ({
                                     show,
                                     itemType,
                                     onConfirm,
                                     onCancel
                                 }) => {
    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h5>Confirm Delete</h5>
                <p>Are you sure you want to delete this {itemType}?</p>
                <div className="modal-actions">
                    <button className="btn btn-secondary" onClick={onCancel}>
                        Cancel
                    </button>
                    <button className="btn btn-danger" onClick={onConfirm}>
                        Delete
                    </button>
                </div>
            </div>
        </div>
    );
};

export default DeleteConfirmationModal;