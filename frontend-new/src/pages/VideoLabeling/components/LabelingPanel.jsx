import React, { useMemo } from 'react';
import PropTypes from 'prop-types';
import { Card, Button, Table, Alert } from '../../../components/ui';
import { LoadingSpinner } from '../../../components/common';
import { sortAssignedLabels } from '../utils/labelUtils.js';
import { formatTime } from '../utils/timeUtils.js';

const LabelingPanel = ({ 
  labels, 
  assignedLabels, 
  onDeleteLabel,
  loading = false,
  operationLoading = false
}) => {
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
            Assigned Labels ({sortedAssignedLabels.length})
          </Card.Title>
        </Card.Header>
        <Card.Body className="p-0" style={{ maxHeight: '400px', overflowY: 'auto' }}>
          {sortedAssignedLabels.length > 0 ? (
            <Table size="sm" hover responsive maxHeight="250px">
              <Table.Head variant="light">
                <Table.Row>
                  <Table.Cell header>Label</Table.Cell>
                  <Table.Cell header>Start</Table.Cell>
                  <Table.Cell header>End</Table.Cell>
                  <Table.Cell header>Action</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {loading ? (
                  <Table.Row>
                    <Table.Cell colSpan={4} className="text-center py-4">
                      <LoadingSpinner size="small" message="Loading labels..." />
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
                        {label.start || label.startTime || 'N/A'}
                      </Table.Cell>
                      <Table.Cell className="font-monospace">
                        {label.end || label.endTime || 'N/A'}
                      </Table.Cell>
                      <Table.Cell>
                        <Button
                          size="sm"
                          variant="outline-danger"
                          icon="fas fa-trash"
                          disabled={operationLoading}
                          loading={operationLoading}
                          confirmAction={true}
                          confirmTitle="Potwierdź usunięcie"
                          confirmMessage={`Czy na pewno chcesz usunąć etykietę "${label.labelName}"? Ta operacja jest nieodwracalna.`}
                          confirmText="Usuń"
                          cancelText="Anuluj"
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
                No labels assigned yet
              </p>
              <small className="text-muted">
                Use shortcuts or buttons above
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