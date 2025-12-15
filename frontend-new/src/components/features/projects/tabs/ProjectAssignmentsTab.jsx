import React from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { Card, Button } from "../../../ui";
import { EmptyState, LoadingSpinner } from "../../../common";
import { useProjectAssignments } from "../../../../hooks/useProjectAssignments";

const ProjectAssignmentsTab = ({ projectId }) => {
  const { t } = useTranslation(["common", "projects", "assignments"]);
  const navigate = useNavigate();
  const { assignments, loading, error, deleteAssignment } =
    useProjectAssignments(projectId);

  return (
    <Card>
      <Card.Header>
        <div className="d-flex justify-content-between align-items-center">
          <Card.Title level={5}>
            <i className="fas fa-tasks me-2"></i>
            {t("projects:tabs.assignments")}
          </Card.Title>
          <div className="d-flex gap-1">
            <Button
              variant="primary"
              size="sm"
              icon="fas fa-plus"
              onClick={() => navigate(`/projects/${projectId}/assignments/add`)}
            >
              {t("projects:add.assignment")}
            </Button>
          </div>
        </div>
      </Card.Header>
      <Card.Body>
        {loading ? (
          <LoadingSpinner message={t("projects:buttons.loading")} />
        ) : error ? (
          <div className="alert alert-danger">
            <i className="fas fa-exclamation-triangle me-2"></i>
            {t("projects:notifications.load_error")}: {error.message}
          </div>
        ) : assignments.length === 0 ? (
          <EmptyState
            icon="fas fa-tasks"
            message={t("projects:not_found.assignment")}
            actionText={t("projects:add.assignment")}
            onAction={() => navigate(`/projects/${projectId}/assignments/add`)}
          />
        ) : (
          <div className="list-group">
            {assignments.map((assignment) => (
              <div
                key={assignment.id}
                className="list-group-item d-flex justify-content-between align-items-center"
              >
                <div>
                  <div className="fw-bold">
                    {t("assignments:details.title", { id: assignment.id })}
                  </div>
                  <small className="text-muted">
                    {t("assignments:details.subject")}: {assignment.subjectId} |{" "}
                    {t("assignments:details.video_group")}:{" "}
                    {assignment.videoGroupId}
                  </small>
                </div>
                <div className="d-flex gap-1">
                  {assignment.isCompleted ? (
                    <span className="badge bg-success">
                      <i className="fas fa-check me-1"></i>
                      {t("assignments:status.completed")}
                    </span>
                  ) : (
                    <span className="badge bg-warning p-2">
                      <i className="fas fa-clock me-1"></i>
                      {t("assignments:status.pending")}
                    </span>
                  )}
                  <Button
                    size="sm"
                    variant="outline"
                    icon="fas fa-eye"
                    onClick={() => navigate(`/assignments/${assignment.id}`)}
                  >
                    {t("common:buttons.details")}
                  </Button>
                  <Button
                    size="sm"
                    variant="outline-danger"
                    icon="fas fa-trash"
                    confirmAction={true}
                    confirmTitle={t("common:deleteConfirmation.title")}
                    confirmMessage={t("projects:confirm_delete_assignment", {
                      id: assignment.id,
                    })}
                    confirmText={t("common:deleteConfirmation.confirm")}
                    cancelText={t("common:deleteConfirmation.cancel")}
                    onConfirm={() => deleteAssignment(assignment.id)}
                  >
                    {t("common:buttons.delete")}
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

ProjectAssignmentsTab.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
    .isRequired,
};

export default ProjectAssignmentsTab;
