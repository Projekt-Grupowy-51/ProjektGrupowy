import React, { createContext, useContext, useState } from 'react';
import DeleteConfirmationModal from '../components/DeleteConfirmationModal';

const ModalContext = createContext();

export const useModal = () => useContext(ModalContext);

export const ModalProvider = ({ children }) => {
  const [modalState, setModalState] = useState({
    show: false,
    itemType: '',
    onConfirm: null,
  });

  const showDeleteModal = (itemType, onConfirm) => {
    setModalState({
      show: true,
      itemType,
      onConfirm,
    });
  };

  const hideModal = () => {
    setModalState({
      show: false,
      itemType: '',
      onConfirm: null,
    });
  };

  const handleConfirm = () => {
    if (modalState.onConfirm) {
      modalState.onConfirm();
    }
    hideModal();
  };

  return (
    <ModalContext.Provider value={{ showDeleteModal }}>
      {children}
      <DeleteConfirmationModal
        show={modalState.show}
        itemType={modalState.itemType}
        onConfirm={handleConfirm}
        onCancel={hideModal}
      />
    </ModalContext.Provider>
  );
};
