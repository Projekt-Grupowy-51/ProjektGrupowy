import React from 'react';
import PropTypes from 'prop-types';
import { Card, Button } from '../ui';

const TabHeader = ({ icon, title, badge, actionText, onAction }) => {
  return (
    <Card.Header>
      <div className="d-flex justify-content-between align-items-center">
        <Card.Title level={5}>
          <i className={`${icon} me-2`}></i>
          {title}
          {badge !== undefined && (
            <span className="badge bg-primary rounded-pill ms-2">
              {badge}
            </span>
          )}
        </Card.Title>
        {actionText && onAction && (
          <Button
            variant="primary"
            size="sm"
            icon="fas fa-plus"
            onClick={onAction}
          >
            {actionText}
          </Button>
        )}
      </div>
    </Card.Header>
  );
};

TabHeader.propTypes = {
  icon: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  badge: PropTypes.number,
  actionText: PropTypes.string,
  onAction: PropTypes.func
};

export default TabHeader;