import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Card, Button, Alert } from '../../../ui';

const ProjectAccessCodesTab = ({ projectId, accessCodes = [] }) => {
  const { t } = useTranslation(['common', 'projects']);

  const copyToClipboard = (code) => {
    navigator.clipboard.writeText(code).then(() => {
      alert(t('projects:access_codes.success.copied'));
    }).catch(() => {
      alert(t('projects:access_codes.errors.copy_failed'));
    });
  };

  return (
    <Card>
      <Card.Header>
        <div className="d-flex justify-content-between align-items-center">
          <Card.Title level={5}>
            <i className="fas fa-key me-2"></i>
            {t('projects:tabs.access_codes')}
          </Card.Title>
          <Button
            variant="primary"
            size="sm"
            icon="fas fa-plus"
            onClick={() => alert('Generate access code - Not implemented in mock')}
          >
            {t('projects:access_codes.buttons.generate')}
          </Button>
        </div>
      </Card.Header>
      <Card.Body>
        {accessCodes.length === 0 ? (
          <Alert variant="info">
            <i className="fas fa-info-circle me-2"></i>
            No access codes generated yet
          </Alert>
        ) : (
          <div className="list-group">
            {accessCodes.map((accessCode) => (
              <div key={accessCode.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div className="fw-bold font-monospace">{accessCode.code}</div>
                  <small className="text-muted">
                    Project ID: {accessCode.projectId}
                  </small>
                </div>
                <div className="d-flex gap-1">
                  {accessCode.active ? (
                    <span className="badge bg-success">
                      <i className="fas fa-check me-1"></i>
                      Active
                    </span>
                  ) : (
                    <span className="badge bg-secondary">
                      <i className="fas fa-times me-1"></i>
                      Inactive
                    </span>
                  )}
                  <Button
                    size="sm"
                    variant="outline-primary"
                    icon="fas fa-copy"
                    onClick={() => copyToClipboard(accessCode.code)}
                  >
                    Copy
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </Card.Body>
    </Card>
  );
};

ProjectAccessCodesTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  accessCodes: PropTypes.array
};

export default ProjectAccessCodesTab;
