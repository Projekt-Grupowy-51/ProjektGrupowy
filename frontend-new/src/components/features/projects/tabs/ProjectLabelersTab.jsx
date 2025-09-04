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
    allLabelers,
    assignments,
    unassignedLabelers,
    assignedLabelerRows,
    selectedLabeler,
    setSelectedLabeler,
    selectedAssignment,
    setSelectedAssignment,
    selectedCustomAssignments,
    handleCustomLabelerAssignmentChange,
    assignLabeler,
    unassignLabeler,
    distributeLabelers,
    unassignAllLabelers,
    assignAllSelected,
    loading,
    distributeLoading,
    unassignAllLoading,
    assignAllSelectedLoading
  } = useProjectLabelers(projectId);

  const formatAssignmentOption = (assignment) => 
    `Assignment #${assignment.id} - Subject: ${
      assignment.subjectName || 'Unknown'
    } (ID: ${assignment.subjectId}), ` +
    `Video Group: ${assignment.videoGroupName || 'Unknown'} (ID: ${
      assignment.videoGroupId
    })`;

  const handleAssign = () => {
    assignLabeler(selectedLabeler, selectedAssignment);
  };

  const handleUnassign = (assignmentId, labelerId) => {
    if (confirm('Are you sure you want to unassign this labeler?')) {
      unassignLabeler(assignmentId, labelerId);
    }
  };

  const handleDistributeLabelers = async () => {
    if (confirm('Are you sure you want to distribute labelers automatically?')) {
      await distributeLabelers();
    }
  };

  const handleUnassignAllLabelers = async () => {
    if (confirm('Are you sure you want to unassign all labelers?')) {
      await unassignAllLabelers();
    }
  };

  const handleAssignAllSelected = async () => {
    if (Object.keys(selectedCustomAssignments).length === 0) {
      alert('No labelers have been assigned to any assignment.');
      return;
    }
    await assignAllSelected();
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
                    label: formatAssignmentOption(assignment)
                  }))
                ]}
              />
            </div>
            <div className="col-md-2">
              <Button
                variant="success"
                disabled={!selectedLabeler || !selectedAssignment}
                onClick={() => handleAssign()}
                className="w-100"
              >
                Assign
              </Button>
            </div>
          </div>
        </Card.Body>
      </Card>

      {/* Unassigned Labelers Section */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4 className="mb-0">Unassigned Labelers</h4>
        <div className="d-flex gap-2">
          {unassignedLabelers?.length > 0 && (
            <Button
              variant="primary"
              onClick={handleDistributeLabelers}
              disabled={distributeLoading}
            >
              <i className="fa-solid fa-wand-magic-sparkles me-2"></i>
              Distribute Labelers
            </Button>
          )}
          {Object.keys(selectedCustomAssignments).length > 0 && (
            <Button
              variant="success"
              onClick={handleAssignAllSelected}
              disabled={assignAllSelectedLoading}
            >
              <i className="fas fa-user-plus me-2"></i>
              Assign All Selected
            </Button>
          )}
        </div>
      </div>

      {!unassignedLabelers || unassignedLabelers.length === 0 ? (
        <EmptyState
          icon="fas fa-info-circle"
          message="No unassigned labelers"
        />
      ) : (
        <Card className="mb-4">
          <Card.Body>
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>Username</Table.Cell>
                  <Table.Cell header>Assign labeler</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {unassignedLabelers.map((labeler) => (
                  <Table.Row key={labeler.id}>
                    <Table.Cell>{labeler.name}</Table.Cell>
                    <Table.Cell>
                      <div className="d-flex justify-content-between align-items-center">
                        <Select
                          value={selectedCustomAssignments[labeler.id] || ''}
                          onChange={(e) => handleCustomLabelerAssignmentChange(labeler.id, e.target.value)}
                          options={[
                            { value: '', label: '-- Select Assignment --' },
                            ...(assignments || []).map(assignment => ({
                              value: assignment.id,
                              label: formatAssignmentOption(assignment)
                            }))
                          ]}
                          className="me-2"
                          style={{ minWidth: '300px' }}
                        />
                        <Button
                          variant="success"
                          size="sm"
                          onClick={() => assignLabeler(labeler.id, selectedCustomAssignments[labeler.id])}
                          disabled={!selectedCustomAssignments[labeler.id]}
                        >
                          <i className="fas fa-user-plus me-2"></i>
                          Assign
                        </Button>
                      </div>
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      )}


      {/* Assigned Labelers */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4 className="mb-0">Assigned Labelers ({assignedLabelerRows?.length || 0})</h4>
        {assignedLabelerRows?.length > 0 && (
          <Button
            variant="danger"
            onClick={handleUnassignAllLabelers}
            disabled={unassignAllLoading}
          >
            <i className="fa-solid fa-user-xmark me-1"></i>
            Unassign All
          </Button>
        )}
      </div>
      
      <Card>
        <Card.Header>
          <Card.Title level={5}>
            Assigned Labelers List
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

      {/* All Labelers Section */}
      <div className="d-flex justify-content-between align-items-center mb-3 mt-5">
        <h4 className="mb-0">All Labelers</h4>
        <div className="d-flex gap-2">
          {allLabelers?.length > 0 && Object.keys(selectedCustomAssignments).length > 0 && (
            <Button
              variant="success"
              onClick={handleAssignAllSelected}
              disabled={assignAllSelectedLoading}
            >
              <i className="fas fa-user-plus me-2"></i>
              Assign All Selected
            </Button>
          )}
        </div>
      </div>

      {!allLabelers || allLabelers.length === 0 ? (
        <EmptyState
          icon="fas fa-info-circle"
          message="No labelers found"
        />
      ) : (
        <Card>
          <Card.Body>
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>Username</Table.Cell>
                  <Table.Cell header>Assign labeler</Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {allLabelers.map((labeler) => (
                  <Table.Row key={labeler.id}>
                    <Table.Cell>{labeler.name}</Table.Cell>
                    <Table.Cell>
                      <div className="d-flex justify-content-between align-items-center">
                        <Select
                          value={selectedCustomAssignments[labeler.id] || ''}
                          onChange={(e) => handleCustomLabelerAssignmentChange(labeler.id, e.target.value)}
                          options={[
                            { value: '', label: '-- Select Assignment --' },
                            ...(assignments || []).map(assignment => ({
                              value: assignment.id,
                              label: formatAssignmentOption(assignment)
                            }))
                          ]}
                          className="me-2"
                          style={{ minWidth: '300px' }}
                        />
                        <Button
                          variant="success"
                          size="sm"
                          onClick={() => assignLabeler(labeler.id, selectedCustomAssignments[labeler.id])}
                          disabled={!selectedCustomAssignments[labeler.id]}
                        >
                          <i className="fas fa-user-plus me-2"></i>
                          Assign
                        </Button>
                      </div>
                    </Table.Cell>
                  </Table.Row>
                ))}
              </Table.Body>
            </Table>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

ProjectLabelersTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired
};

export default ProjectLabelersTab;
