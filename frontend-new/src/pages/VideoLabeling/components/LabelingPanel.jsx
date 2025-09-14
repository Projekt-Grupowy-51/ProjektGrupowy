import React, { useMemo } from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { Card, Button, Table, Alert } from '../../../components/ui';
import { LoadingSpinner } from '../../../common';
import { sortAssignedLabels } from '../utils/labelUtils.js';
import { formatTime } from '../utils/timeUtils.js';

const LabelingPanel = ({ 
  labels, 
  assignedLabels, 
  onDeleteLabel,
  loading = false,
  operationLoading = false
}) => {
  const { t } = useTranslation(['videos', 'common']);
  const sortedAssignedLabels = useMemo(() => 
    sortAssignedLabels(assignedLabels), 
    [assignedLabels]
  );

  return (
    <div className="labeling-panel">
      {/* Assigned Labels */}
      <Card>
        <Card.Header>
          <Card.Title level={6}>
            <i className="fas fa-list me-2"></i>
            {t('videos:labeling.assigned_labels')} ({sortedAssignedLabels.length})
          </Card.Title>
        </Card.Header>
        <Card.Body className="p-0" style={{ maxHeight: '400px', overflowY: 'auto' }}>
          {sortedAssignedLabels.length > 0 ? (
            <Table size="sm" hover responsive maxHeight="250px">
              <Table.Head variant="light">
                <Table.Row>
                  <Table.Cell header>{t('videos:table.label')}</Table.Cell>
                  <Table.Cell header>{t('videos:table.start')}</Table.Cell>
                  <Table.Cell header>{t('videos:table.end')}</Table.Cell>
                  <Table.Cell header>{t('common:actions')}</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {loading ? (
                  <Table.Row>
                    <Table.Cell colSpan={4} className="text-center py-4">
                      <LoadingSpinner size="small" message={t('videos:labeling.loading_labels')} />
                    </Table.Cell>
                  </Table.Row>
                ) : (
                  sortedAssignedLabels.map((label) => {
                  const matchingLabel = labels.find(l => l.id === label.labelId);
                  
                  return (
                    <Table.Row key={label.id}>
                      <Table.Cell>
                        <div className="d-flex align-items-center">
                          <div
                            style={{
                              backgroundColor: matchingLabel?.colorHex || '#ccc',
                              width: '12px',
                              height: '12px',
                              borderRadius: '50%',
                              marginRight: '6px'
                            }}
                          />
                          {label.labelName}
                        </div>
                      </Table.Cell>
                      <Table.Cell className="font-monospace">
                        {label.start || label.startTime || t('common:states.empty')}
                      </Table.Cell>
                      <Table.Cell className="font-monospace">
                        {label.end || label.endTime || t('common:states.empty')}
                      </Table.Cell>
                      <Table.Cell>
                        <Button
                          size="sm"
                          variant="outline-danger"
                          icon="fas fa-trash"
                          disabled={operationLoading}
                          loading={operationLoading}
                          confirmAction={true}
                          confirmTitle={t('common:deleteConfirmation.title')}
                          confirmMessage={t('videos:labeling.confirm_delete_label', { labelName: label.labelName })}
                          confirmText={t('common:deleteConfirmation.confirm')}
                          cancelText={t('common:deleteConfirmation.cancel')}
                          onConfirm={() => onDeleteLabel(label.id)}
                        />
                      </Table.Cell>
                    </Table.Row>
                  );
                })
                )}
              </Table.Body>
            </Table>
          ) : (
            <div className="text-center py-4">
              <i className="fas fa-tags fs-4 text-muted opacity-50"></i>
              <p className="small text-muted mt-2 mb-0">
                {t('videos:labeling.no_labels_assigned')}
              </p>
              <small className="text-muted">
                {t('videos:labeling.use_shortcuts_hint')}
              </small>
            </div>
          )}
        </Card.Body>
      </Card>

    </div>
  );
};

LabelingPanel.propTypes = {
  labels: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    name: PropTypes.string.isRequired,
    colorHex: PropTypes.string.isRequired
  })).isRequired,
  assignedLabels: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.number.isRequired,
    labelId: PropTypes.number.isRequired,
    labelName: PropTypes.string.isRequired,
    videoId: PropTypes.number.isRequired,
    start: PropTypes.string,
    end: PropTypes.string,
    startTime: PropTypes.string,
    endTime: PropTypes.string,
    insDate: PropTypes.string
  })).isRequired,
  onDeleteLabel: PropTypes.func.isRequired,
  loading: PropTypes.bool,
  operationLoading: PropTypes.bool
};

export default LabelingPanel;