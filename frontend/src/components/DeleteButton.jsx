import React, { useState } from 'react';
import DeleteConfirmationModal from './DeleteConfirmationModal';
import { useNotification } from '../context/NotificationContext';
import { useTranslation } from 'react-i18next';

const DeleteButton = ({ onClick, itemType = 'item', buttonText, className = '', style = {} }) => {
    const [showModal, setShowModal] = useState(false);
    const { addNotification } = useNotification();
    const { t } = useTranslation(['common']);

    const deleteText = t('buttons.delete');

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
            addNotification(`Successfully deleted ${itemType}`, "success");
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
                <i className="fas fa-trash-alt"></i> {deleteText}
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
