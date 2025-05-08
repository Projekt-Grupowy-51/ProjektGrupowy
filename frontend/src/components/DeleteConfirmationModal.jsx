import React from 'react';
import './css/Modal.css';
import { useTranslation } from 'react-i18next';

const DeleteConfirmationModal = ({
    show,
    itemType,
    onConfirm,
    onCancel
}) => {
    const { t } = useTranslation(['common']);
    
    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h5>{t('common:deleteConfirmation.title')}</h5>
                <p>{t('common:deleteConfirmation.message', { item: itemType })}</p>
                <div className="modal-actions">
                    <button className="btn btn-secondary" onClick={onCancel}>
                        {t('common:deleteConfirmation.cancel')}
                    </button>
                    <button className="btn btn-danger" onClick={onConfirm}>
                        {t('common:deleteConfirmation.confirm')}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default DeleteConfirmationModal;