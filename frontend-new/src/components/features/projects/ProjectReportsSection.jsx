import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { Button, Card } from "../../ui";
import { LoadingSpinner, ErrorAlert, EmptyState } from "../../common";
import { useProjectReports } from "../../../hooks/useProjectReports.js";
import { useSignalRConnection } from "../../../hooks/useSignalRConnection.js";

const ProjectReportsSection = ({ projectId }) => {
  const { t } = useTranslation(["common", "projects"]);

  const {
    reports,
    loading: reportsLoading,
    error: reportsError,
    generateReport,
    deleteReport,
    downloadReport,
    refetch,
  } = useProjectReports(projectId);

  const signalR = useSignalRConnection();

  useEffect(() => {
    if (!signalR) return;

    // Listen for ReportGenerated event
    const handleReportGenerated = (data) => {
      // Check if the report belongs to this project
      if (data.projectId === projectId) {
        refetch();
      }
    };

    // Register the listener
    signalR.on("ReportGenerated", handleReportGenerated);

    // Cleanup on unmount
    return () => {
      signalR.off("ReportGenerated", handleReportGenerated);
    };
  }, [signalR, projectId, refetch]);

  const formatDate = (dateString) => {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <Card>
      <Card.Header>
        <Card.Title level={5}>
          <i className="fas fa-chart-bar me-2"></i>
          {t("projects:reports.title")}
        </Card.Title>
      </Card.Header>
      <Card.Body>
        {reportsLoading ? (
          <div className="text-center py-3">
            <LoadingSpinner size="small" />
          </div>
        ) : reportsError ? (
          <ErrorAlert error={reportsError} />
        ) : reports.length === 0 ? (
          <EmptyState
            icon="fas fa-chart-bar"
            message={t("projects:reports.no_reports")}
          />
        ) : (
          <div
            className="list-group"
            style={{ maxHeight: "300px", overflowY: "auto" }}
          >
            {reports.map((report) => (
              <div
                key={report.id}
                className="list-group-item d-flex justify-content-between align-items-center"
              >
                <div>
                  <div className="fw-bold">
                    {report.name ||
                      t("projects:reports.fallback_name", { id: report.id })}
                  </div>
                  <small className="text-muted">
                    {formatDate(report.createdAt)}
                  </small>
                </div>
                <div className="d-flex gap-1">
                  <Button
                    size="sm"
                    variant="outline"
                    icon="fas fa-download"
                    onClick={() => downloadReport(report.id)}
                  >
                    {t("common:buttons.download")}
                  </Button>
                  <Button
                    size="sm"
                    variant="outline-danger"
                    icon="fas fa-trash"
                    confirmAction={true}
                    confirmTitle={t("common:deleteConfirmation.title")}
                    confirmMessage={t("projects:reports.confirm_delete", {
                      name:
                        report.name ||
                        t("projects:reports.fallback_name", { id: report.id }),
                    })}
                    confirmText={t("common:deleteConfirmation.confirm")}
                    cancelText={t("common:deleteConfirmation.cancel")}
                    onConfirm={() => deleteReport(report.id)}
                  >
                    {t("common:buttons.delete")}
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </Card.Body>
      <Card.Footer>
        <Button
          variant="primary"
          icon="fas fa-plus"
          onClick={() => generateReport()}
          className="w-100"
          disabled={reportsLoading}
        >
          {reportsLoading
            ? t("common:states.loading")
            : t("projects:reports.generate")}
        </Button>
      </Card.Footer>
    </Card>
  );
};

ProjectReportsSection.propTypes = {
  projectId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
    .isRequired,
};

export default ProjectReportsSection;
