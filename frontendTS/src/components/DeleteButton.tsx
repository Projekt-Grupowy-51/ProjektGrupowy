import React, { useState } from 'react';
import DeleteConfirmationModal from './DeleteConfirmationModal';
import { useTranslation } from 'react-i18next';

const DeleteButton = ({ onClick, itemType = 'item', className = '', style = {} }) => {
    const [showModal, setShowModal] = useState(false);
    const { t } = useTranslation(['common']);

    const handleOpenModal = (e) => {
        e.preventDefault();
        e.stopPropagation();
        setShowModal(true);
    };

    const handleCloseModal = () => {
        setShowModal(false);
    };

    const handleConfirm = async () => {
        await onClick();
        setShowModal(false);
    };

    return (
        <>
            <button
                type="button"
                className={`btn btn-danger ${className}`}
                onClick={handleOpenModal}
                style={style}
            >
                <i className="fas fa-trash-alt"></i> {t('buttons.delete')}
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
