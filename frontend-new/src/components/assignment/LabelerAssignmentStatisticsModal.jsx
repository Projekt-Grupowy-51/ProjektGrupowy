import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import { useTranslation } from "react-i18next";
import { Modal, Card, Table } from "../ui";
import SubjectVideoGroupAssignmentService from "../../services/SubjectVideoGroupAssignmentService";

const LabelerAssignmentStatisticsModal = ({
  show,
  onHide,
  assignmentId,
  labelerId,
  labelerName,
}) => {
  const { t } = useTranslation(["common", "assignments"]);
  const [statistics, setStatistics] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (show && assignmentId && labelerId) {
      fetchStatistics();
    }
  }, [show, assignmentId, labelerId]);

  const fetchStatistics = async () => {
    try {
      setLoading(true);
      setError(null);
      const data =
        await SubjectVideoGroupAssignmentService.getLabelerStatistics(
          assignmentId,
          labelerId
        );
      setStatistics(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const renderStatCards = () => {
    if (!statistics) return null;

    return (
      <div className="row g-3 mb-4">
        <div className="col-md-3">
          <Card className="h-100">
            <Card.Body>
              <h6 className="text-muted mb-1">
                {t("assignments:statistics.total_videos")}
              </h6>
              <h3 className="mb-0">{statistics.totalVideos}</h3>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="h-100">
            <Card.Body>
              <h6 className="mb-1">
                {t("assignments:statistics.labeled_videos")}
              </h6>
              <h3 className="mb-0">{statistics.labeledVideos}</h3>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card className="h-100">
            <Card.Body>
              <h6 className="text-muted mb-1">
                {t("assignments:statistics.total_labels")}
              </h6>
              <h3 className="mb-0">{statistics.totalLabels}</h3>
            </Card.Body>
          </Card>
        </div>
        <div className="col-md-3">
          <Card
            className={`h-100 ${
              statistics.isCompleted ? "bg-success text-white" : ""
            }`}
          >
            <Card.Body>
              <h6
                className={statistics.isCompleted ? "mb-1" : "text-muted mb-1"}
              >
                {t("assignments:statistics.status")}
              </h6>
              <h5 className="mb-0">
                {statistics.isCompleted ? (
                  <>
                    <i className="fas fa-check me-2"></i>
                    {t("common:states.completed")}
                  </>
                ) : (
                  <>
                    <i className="fas fa-clock me-2"></i>
                    {t("common:states.in_progress")}
                  </>
                )}
              </h5>
            </Card.Body>
          </Card>
        </div>
      </div>
    );
  };

  const renderVideosTable = () => {
    if (!statistics || !statistics.videos || statistics.videos.length === 0) {
      return null;
    }

    return (
      <Card>
        <Card.Header>
          <Card.Title level={6}>
            {t("assignments:statistics.video_progress")}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <Table striped>
            <Table.Head>
              <Table.Row>
                <Table.Cell header>{t("common:form.video")}</Table.Cell>
                <Table.Cell header>
                  {t("assignments:statistics.labels_created")}
                </Table.Cell>
                <Table.Cell header>
                  {t("assignments:statistics.labeled")}
                </Table.Cell>
              </Table.Row>
            </Table.Head>
            <Table.Body>
              {statistics.videos.map((video) => (
                <Table.Row key={video.videoId}>
                  <Table.Cell>{video.videoTitle}</Table.Cell>
                  <Table.Cell>{video.labelCount}</Table.Cell>
                  <Table.Cell>
                    {video.hasLabeled ? (
                      <span className="badge bg-success">
                        <i className="fas fa-check me-1"></i>
                        {t("common:states.yes")}
                      </span>
                    ) : (
                      <span className="badge bg-secondary">
                        <i className="fas fa-times me-1"></i>
                        {t("common:states.no")}
                      </span>
                    )}
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </Card.Body>
      </Card>
    );
  };

  return (
    <Modal
      show={show}
      onHide={onHide}
      size="xl"
      title={t("assignments:statistics.labeler_title", {
        name: labelerName || labelerId,
        subject: statistics?.subjectName || "",
        videoGroup: statistics?.videoGroupName || "",
      })}
    >
      <Modal.Body>
        {loading && <div className="text-center">{t("common:loading")}</div>}
        {error && <div className="alert alert-danger">{error}</div>}
        {statistics && (
          <>
            <div className="mb-3">
              <strong>{t("common:form.email")}:</strong>{" "}
              {statistics.labelerEmail || t("common:not_available")}
            </div>
            <div className="mb-3">
              <strong>{t("common:form.subject")}:</strong>{" "}
              {statistics.subjectName}
            </div>
            <div className="mb-3">
              <strong>{t("common:form.video_group")}:</strong>{" "}
              {statistics.videoGroupName}
            </div>
            {renderStatCards()}
            {renderVideosTable()}
          </>
        )}
      </Modal.Body>
      <Modal.Footer>
        <button type="button" className="btn btn-secondary" onClick={onHide}>
          {t("common:buttons.close")}
        </button>
      </Modal.Footer>
    </Modal>
  );
};

LabelerAssignmentStatisticsModal.propTypes = {
  show: PropTypes.bool.isRequired,
  onHide: PropTypes.func.isRequired,
  assignmentId: PropTypes.oneOfType([PropTypes.string, PropTypes.number])
    .isRequired,
  labelerId: PropTypes.string,
  labelerName: PropTypes.string,
};

export default LabelerAssignmentStatisticsModal;
