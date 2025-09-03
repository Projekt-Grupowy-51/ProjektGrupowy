import React from 'react';
import PropTypes from 'prop-types';
import { Card, Button, Table, Alert } from '../../../components/ui';
import { sortAssignedLabels } from '../utils/labelUtils.js';
import { formatTime } from '../utils/timeUtils.js';

const LabelingPanel = ({ 
  labels, 
  assignedLabels, 
  onDeleteLabel 
}) => {
  const sortedAssignedLabels = sortAssignedLabels(assignedLabels);

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
                {sortedAssignedLabels.map((label) => {
                  const matchingLabel = labels.find(l => l.id === label.labelId);
                  const duration = label.endTime - label.startTime;
                  const isPoint = duration === 0;
                  
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
                        {Math.floor(label.startTime / 60)}:{(label.startTime % 60).toFixed(1).padStart(4, '0')}
                      </Table.Cell>
                      <Table.Cell className="font-monospace">
                        {isPoint ? 'Point' : `${Math.floor(label.endTime / 60)}:${(label.endTime % 60).toFixed(1).padStart(4, '0')}`}
                      </Table.Cell>
                      <Table.Cell>
                        <button
                          className="btn btn-outline-danger btn-sm"
                          onClick={() => onDeleteLabel(label.id)}
                          title="Delete label"
                        >
                          <i className="fas fa-trash"></i>
                        </button>
                      </Table.Cell>
                    </Table.Row>
                  );
                })}
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
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    labelId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    labelName: PropTypes.string.isRequired,
    startTime: PropTypes.number.isRequired,
    endTime: PropTypes.number.isRequired
  })).isRequired,
  onDeleteLabel: PropTypes.func.isRequired
};

export default LabelingPanel;