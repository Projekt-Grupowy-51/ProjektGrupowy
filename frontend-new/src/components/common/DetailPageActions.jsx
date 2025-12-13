import React from 'react';
import PropTypes from 'prop-types';
import { Button } from '../ui';

const DetailPageActions = ({
  onEdit,
  onDelete,
  onBack,
  editText,
  deleteText,
  backText,
  deleteConfirmTitle,
  deleteConfirmMessage,
  entityName,
  showEdit = true,
  showDelete = true,
  showBack = true,
  className = '',
  ...props
}) => {
  const actions = [];

  if (showEdit && onEdit) {
    actions.push(
      <Button
        key="edit"
        variant="warning"
        icon="fas fa-edit"
        onClick={onEdit}
      >
        {editText}
      </Button>
    );
  }

  if (showDelete && onDelete) {
    actions.push(
      <Button
        key="delete"
        variant="outline-danger"
        icon="fas fa-trash"
        confirmAction={true}
        confirmTitle={deleteConfirmTitle}
        confirmMessage={deleteConfirmMessage}
        confirmText={deleteText}
        onConfirm={onDelete}
      >
        {deleteText}
      </Button>
    );
  }

  if (showBack && onBack) {
    actions.push(
      <Button
        key="back"
        variant="outline-secondary"
        icon="fas fa-arrow-left"
        onClick={onBack}
      >
        {backText}
      </Button>
    );
  }

  if (actions.length === 0) {
    return null;
  }

  return (
    <div className={`d-flex gap-2 ${className}`} {...props}>
      {actions}
    </div>
  );
};

DetailPageActions.propTypes = {
  onEdit: PropTypes.func,
  onDelete: PropTypes.func,
  onBack: PropTypes.func,
  editText: PropTypes.string,
  deleteText: PropTypes.string,
  backText: PropTypes.string,
  deleteConfirmTitle: PropTypes.string,
  deleteConfirmMessage: PropTypes.string,
  entityName: PropTypes.string,
  showEdit: PropTypes.bool,
  showDelete: PropTypes.bool,
  showBack: PropTypes.bool,
  className: PropTypes.string
};

export default DetailPageActions;