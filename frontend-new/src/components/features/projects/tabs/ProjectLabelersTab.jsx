import React from 'react';
import PropTypes from 'prop-types';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { Card, Button, Table, Select } from '../../../ui';
import { EmptyState } from '../../../common';
import { useProjectLabelers } from '../../../../hooks/useProjectLabelers';

const ProjectLabelersTab = ({ projectId }) => {
  const { t } = useTranslation(['common', 'projects']);
  const navigate = useNavigate();
  
  const {
    labelers,
    assignments,
    unassignedLabelers,
    assignedLabelerRows,
    selectedLabeler,
    setSelectedLabeler,
    selectedAssignment,
    setSelectedAssignment,
    assignLabeler,
    unassignLabeler,
    loading
  } = useProjectLabelers(projectId);

  const handleAssign = () => {
    assignLabeler(selectedLabeler, selectedAssignment);
  };

  const handleUnassign = (assignmentId, labelerId) => {
    if (confirm('Are you sure you want to unassign this labeler?')) {
      unassignLabeler(assignmentId, labelerId);
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      {/* Simple Assignment Form */}
      <Card className="mb-4">
        <Card.Header>
          <Card.Title level={5}>Assign Labeler</Card.Title>
        </Card.Header>
        <Card.Body>
          <div className="row g-3">
            <div className="col-md-5">
              <Select
                value={selectedLabeler}
                onChange={(e) => setSelectedLabeler(e.target.value)}
                options={[
                  { value: '', label: 'Select labeler...' },
                  ...(labelers || []).map(labeler => ({
                    value: labeler.id,
                    label: labeler.name
                  }))
                ]}
              />
            </div>
            <div className="col-md-5">
              <Select
                value={selectedAssignment}
                onChange={(e) => setSelectedAssignment(e.target.value)}
                options={[
                  { value: '', label: 'Select assignment...' },
                  ...(assignments || []).map(assignment => ({
                    value: assignment.id,
                    label: `${assignment.subjectName} - ${assignment.videoGroupName}`
                  }))
                ]}
              />
            </div>
            <div className="col-md-2">
              <Button
                variant="success"
                disabled={!selectedLabeler || !selectedAssignment}
                onClick={handleAssign}
                className="w-100"
              >
                Assign
              </Button>
            </div>
          </div>
        </Card.Body>
      </Card>

      {/* Assigned Labelers */}
      <Card>
        <Card.Header>
          <Card.Title level={5}>
            Assigned Labelers ({assignedLabelerRows?.length || 0})
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {!assignedLabelerRows || assignedLabelerRows.length === 0 ? (
            <EmptyState
              icon="fas fa-user-check"
              message="No assigned labelers"
            />
          ) : (
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>Labeler</Table.Cell>
                  <Table.Cell header>Subject</Table.Cell>
                  <Table.Cell header>Video Group</Table.Cell>
                  <Table.Cell header>Actions</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {assignedLabelerRows?.map((item) => (
                  <Table.Row key={`${item.assignmentId}-${item.labelerId}`}>
                    <Table.Cell>{item.labelerName}</Table.Cell>
                    <Table.Cell>{item.subjectName}</Table.Cell>
                    <Table.Cell>{item.videoGroupName}</Table.Cell>
                    <Table.Cell>
                      <div className="d-flex gap-2">
                        <Button
                          size="sm"
                          variant="primary"
                          onClick={() => navigate(`/assignments/${item.assignmentId}`)}
                        >
                          Details
                        </Button>
                        <Button
                          size="sm"
                          variant="outline-danger"
                          onClick={() => handleUnassign(item.assignmentId, item.labelerId)}
                        >
                          Unassign
                        </Button>
                      </div>
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

ProjectLabelersTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired
};

export default ProjectLabelersTab;
