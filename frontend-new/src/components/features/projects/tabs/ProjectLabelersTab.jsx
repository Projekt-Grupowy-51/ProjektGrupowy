import React from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { Card, Button, Table, Select } from "../../../ui";
import { EmptyState } from "../../../common";
import { useProjectLabelers } from "../../../../hooks/useProjectLabelers";

const ProjectLabelersTab = ({ projectId }) => {
  const { t } = useTranslation(["common", "projects"]);
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
    assignAllSelectedLoading,
  } = useProjectLabelers(projectId);

  const formatAssignmentOption = (assignment) =>
    `Assignment #${assignment.id} - ${t("projects:assignment.subject")}: ${
      assignment.subjectName || t("common:states.unknown")
    } (ID: ${assignment.subjectId}), ` +
    `${t("projects:assignment.video_group")}: ${
      assignment.videoGroupName || t("common:states.unknown")
    } (ID: ${assignment.videoGroupId})`;

  const handleAssign = () => {
    assignLabeler(selectedLabeler, selectedAssignment);
  };

  const handleUnassign = (assignmentId, labelerId) => {
    unassignLabeler(assignmentId, labelerId);
  };

  const handleDistributeLabelers = async () => {
    await distributeLabelers();
  };

  const handleUnassignAllLabelers = async () => {
    await unassignAllLabelers();
  };

  const handleAssignAllSelected = async () => {
    if (Object.keys(selectedCustomAssignments).length === 0) {
      alert(t("projects:labeler_tab.no_assignments_selected"));
      return;
    }
    await assignAllSelected();
  };

  if (loading) return <div>{t("common:loading")}</div>;

  return (
    <div>
      {/* Simple Assignment Form */}
      <Card className="mb-4">
        <Card.Header>
          <Card.Title level={5}>
            {t("projects:labeler_tab.assign_labeler_to_assignment")}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <div className="row g-3">
            <div className="col-md-4">
              <Select
                value={selectedLabeler}
                onChange={(e) => setSelectedLabeler(e.target.value)}
                options={[
                  {
                    value: "",
                    label: t("projects:labeler_tab.select_labeler"),
                  },
                  ...(labelers || []).map((labeler) => ({
                    value: labeler.id,
                    label: labeler.name,
                  })),
                ]}
              />
            </div>
            <div className="col-md-4">
              <Select
                value={selectedAssignment}
                onChange={(e) => setSelectedAssignment(e.target.value)}
                options={[
                  {
                    value: "",
                    label: t("projects:labeler_tab.select_assignment"),
                  },
                  ...(assignments || []).map((assignment) => ({
                    value: assignment.id,
                    label: formatAssignmentOption(assignment),
                  })),
                ]}
              />
            </div>
            <div className="col-md-4">
              <Button
                variant="dark"
                size="sm"
                className="btn-standard"
                disabled={!selectedLabeler || !selectedAssignment}
                onClick={() => handleAssign()}
              >
                {t("projects:labeler_tab.assign_labeler")}
              </Button>
            </div>
            </div>
        </Card.Body>
      </Card>

      {/* Unassigned Labelers Section */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4 className="mb-0">
          {t("projects:labeler_tab.unassigned_labelers")}
        </h4>
        <div className="d-flex gap-2">
          {unassignedLabelers?.length > 0 && (
            <Button
              variant="primary"
              disabled={distributeLoading}
              loading={distributeLoading}
              confirmAction={true}
              confirmTitle={t("common:acceptConfirmation.title")}
              confirmMessage={t(
                "projects:labeler_tab.distribute_confirm_message"
              )}
              confirmText={t("common:acceptConfirmation.confirm")}
              cancelText={t("common:acceptConfirmation.cancel")}
              onConfirm={handleDistributeLabelers}
            >
              <i className="fa-solid fa-wand-magic-sparkles me-2"></i>
              {t("projects:labeler_tab.distribute_labelers")}
            </Button>
          )}
          {Object.keys(selectedCustomAssignments).length > 0 && (
            <Button
              variant="success"
              onClick={handleAssignAllSelected}
              disabled={assignAllSelectedLoading}
            >
              <i className="fas fa-user-plus me-2"></i>
              {t("projects:labeler_tab.assign_all_selected")}
            </Button>
          )}
        </div>
      </div>

      {!unassignedLabelers || unassignedLabelers.length === 0 ? (
        <EmptyState
          icon="fas fa-info-circle"
          message={t("projects:not_found.unassigned_labelers")}
        />
      ) : (
        <Card className="mb-4">
          <Card.Body>
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>{t("common:form.username")}</Table.Cell>
                  <Table.Cell header>
                    {t("projects:labeler_tab.assign_labeler")}
                  </Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {unassignedLabelers.map((labeler) => (
                  <Table.Row key={labeler.id}>
                    <Table.Cell>{labeler.name}</Table.Cell>
                    <Table.Cell>
                      <div className="d-flex justify-content-between align-items-center">
                        <Select
                          value={selectedCustomAssignments[labeler.id] || ""}
                          onChange={(e) =>
                            handleCustomLabelerAssignmentChange(
                              labeler.id,
                              e.target.value
                            )
                          }
                          options={[
                            {
                              value: "",
                              label: t(
                                "projects:labeler_tab.select_assignment"
                              ),
                            },
                            ...(assignments || []).map((assignment) => ({
                              value: assignment.id,
                              label: formatAssignmentOption(assignment),
                            })),
                          ]}
                          className="me-2"
                          style={{ minWidth: "300px" }}
                        />
                        <Button
                          variant="success"
                          size="sm"
                          onClick={() =>
                            assignLabeler(
                              labeler.id,
                              selectedCustomAssignments[labeler.id]
                            )
                          }
                          disabled={!selectedCustomAssignments[labeler.id]}
                        >
                          <i className="fas fa-user-plus me-2"></i>
                          {t("projects:labeler_tab.assign_labeler")}
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
        <h4 className="mb-0">
          {t("projects:labeler_tab.assigned_labelers")} (
          {assignedLabelerRows?.length || 0})
        </h4>
        {assignedLabelerRows?.length > 0 && (
          <Button
            variant="danger"
            disabled={unassignAllLoading}
            loading={unassignAllLoading}
            confirmAction={true}
            confirmTitle={t("common:deleteConfirmation.title")}
            confirmMessage={t(
              "projects:labeler_tab.unassign_all_confirm_message"
            )}
            confirmText={t("common:deleteConfirmation.confirm")}
            cancelText={t("common:deleteConfirmation.cancel")}
            onConfirm={handleUnassignAllLabelers}
          >
            <i className="fa-solid fa-user-xmark me-1"></i>
            {t("projects:labeler_tab.unassign_all")}
          </Button>
        )}
      </div>

      <Card>
        <Card.Header>
          <Card.Title level={5}>
            {t("projects:labeler_tab.assigned_labelers")}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          {!assignedLabelerRows || assignedLabelerRows.length === 0 ? (
            <EmptyState
              icon="fas fa-user-check"
              message={t("projects:not_found.assigned_labelers")}
            />
          ) : (
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>
                    {t("projects:labeler_tab.labeler")}
                  </Table.Cell>
                  <Table.Cell header>
                    {t("projects:assignment.subject")}
                  </Table.Cell>
                  <Table.Cell header>
                    {t("projects:assignment.video_group")}
                  </Table.Cell>
                  <Table.Cell header>{t("common:table.actions")}</Table.Cell>
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
                          onClick={() =>
                            navigate(`/assignments/${item.assignmentId}`)
                          }
                        >
                          {t("common:buttons.details")}
                        </Button>
                        <Button
                          size="sm"
                          variant="outline-danger"
                          confirmAction={true}
                          confirmTitle={t("common:deleteConfirmation.title")}
                          confirmMessage={t(
                            "projects:labeler_tab.unassign_confirm_message",
                            { labelerName: item.labelerName }
                          )}
                          confirmText={t("common:deleteConfirmation.confirm")}
                          cancelText={t("common:deleteConfirmation.cancel")}
                          onConfirm={() =>
                            handleUnassign(item.assignmentId, item.labelerId)
                          }
                        >
                          {t("projects:labeler_tab.unassign_selected")}
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
        <h4 className="mb-0">{t("projects:labeler_tab.all_labelers")}</h4>
        <div className="d-flex gap-2">
          {allLabelers?.length > 0 &&
            Object.keys(selectedCustomAssignments).length > 0 && (
              <Button
                variant="success"
                onClick={handleAssignAllSelected}
                disabled={assignAllSelectedLoading}
              >
                <i className="fas fa-user-plus me-2"></i>
                {t("projects:labeler_tab.assign_all_selected")}
              </Button>
            )}
        </div>
      </div>

      {!allLabelers || allLabelers.length === 0 ? (
        <EmptyState
          icon="fas fa-info-circle"
          message={t("projects:not_found.all_labelers")}
        />
      ) : (
        <Card>
          <Card.Body>
            <Table striped>
              <Table.Head>
                <Table.Row>
                  <Table.Cell header>{t("common:form.username")}</Table.Cell>
                  <Table.Cell header>
                    {t("projects:labeler_tab.assign_labeler")}
                  </Table.Cell>
                </Table.Row>
              </Table.Head>
              <Table.Body>
                {allLabelers.map((labeler) => (
                  <Table.Row key={labeler.id}>
                    <Table.Cell>{labeler.name}</Table.Cell>
                    <Table.Cell>
                      <div className="d-flex justify-content-between align-items-center">
                        <Select
                          value={selectedCustomAssignments[labeler.id] || ""}
                          onChange={(e) =>
                            handleCustomLabelerAssignmentChange(
                              labeler.id,
                              e.target.value
                            )
                          }
                          options={[
                            {
                              value: "",
                              label: t(
                                "projects:labeler_tab.select_assignment"
                              ),
                            },
                            ...(assignments || []).map((assignment) => ({
                              value: assignment.id,
                              label: formatAssignmentOption(assignment),
                            })),
                          ]}
                          className="me-2"
                          style={{ minWidth: "300px" }}
                        />
                        <Button
                          variant="success"
                          size="sm"
                          onClick={() =>
                            assignLabeler(
                              labeler.id,
                              selectedCustomAssignments[labeler.id]
                            )
                          }
                          disabled={!selectedCustomAssignments[labeler.id]}
                        >
                          <i className="fas fa-user-plus me-2"></i>
                          {t("projects:labeler_tab.assign_labeler")}
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
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
    .isRequired,
};

export default ProjectLabelersTab;
