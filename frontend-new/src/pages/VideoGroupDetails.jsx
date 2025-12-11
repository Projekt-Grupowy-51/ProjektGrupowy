import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Card, Button, Table, Input } from "../components/ui";
import { LoadingSpinner, ErrorAlert, EmptyState } from "../components/common";
import { useVideoGroupDetails } from "../hooks/useVideoGroupDetails.js";
import { useSignalRConnection } from "../hooks/useSignalRConnection.js";
import { MessageTypes } from "../services/signalR/MessageTypes.js";

const VideoGroupDetails = () => {
  const { t } = useTranslation(["videoGroups", "videos", "common"]);
  const { id } = useParams();
  const navigate = useNavigate();
  const signalR = useSignalRConnection();

  const {
    videoGroup,
    videos,
    loading,
    error,
    updateVideo,
    deleteVideo,
    deleteVideoGroup,
    handleBack,
    handleAddVideo,
    handleEditVideoGroup,
    refetchVideos,
  } = useVideoGroupDetails(id);

  const [editingVideo, setEditingVideo] = useState(null);
  const [editForm, setEditForm] = useState({ title: "", positionInQueue: "" });

  useEffect(() => {
    if (!signalR) return;

    const handleVideoProcessed = () => refetchVideos();
    signalR.on(MessageTypes.VideoProcessed, handleVideoProcessed);
    return () => signalR.off(MessageTypes.VideoProcessed, handleVideoProcessed);
  }, [signalR, refetchVideos]);

  const startEditing = (video) => {
    setEditingVideo(video.id);
    setEditForm({
      title: video.title,
      positionInQueue: video.positionInQueue,
    });
  };

  const cancelEditing = () => {
    setEditingVideo(null);
    setEditForm({ title: "", positionInQueue: "" });
  };

  const saveEdit = async (videoId) => {
    try {
      await updateVideo(videoId, {
        title: editForm.title,
        positionInQueue: parseInt(editForm.positionInQueue),
        videoGroupId: parseInt(id),
      });
      cancelEditing();
    } catch (err) {
      console.error("Failed to update video:", err);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorAlert error={error} />;
  if (!videoGroup)
    return (
      <EmptyState
        icon="fas fa-film"
        message={t("videoGroups:not_found")}
        actionText={t("common:buttons.back")}
        onAction={handleBack}
      />
    );

  return (
    <div className="row justify-content-center py-3">
      <div className="col-lg-10">
        {/* VIDEO GROUP DETAILS */}
        <Card className="mb-4">
          <Card.Header>
            <Card.Title level={5}>
              <i className="fas fa-film me-2"></i>
              {videoGroup.name}
            </Card.Title>
          </Card.Header>
          <Card.Body>
            <div className="row mb-3">
              <div className="col-sm-3 fw-bold">
                {t("videoGroups:details.description")}:
              </div>
              <div className="col-sm-9">{videoGroup.description || "-"}</div>
            </div>

            <div className="row mb-3">
              <div className="col-sm-3 fw-bold">ID:</div>
              <div className="col-sm-9">{videoGroup.id}</div>
            </div>
          </Card.Body>
          <Card.Footer>
            <div className="d-flex gap-2">
              <Button
                size="sm"
                variant="outline-secondary"
                className="btn-standard"
                icon="fas fa-arrow-left"
                onClick={handleBack}
              >
                {t("common:buttons.back")}
              </Button>

              <Button
                size="sm"
                variant="outline-warning"
                className="btn-standard"
                icon="fas fa-edit"
                onClick={handleEditVideoGroup}
              >
                {t("common:buttons.edit")}
              </Button>

              <Button
                size="sm"
                variant="outline-danger"
                className="btn-standard"
                icon="fas fa-trash"
                confirmAction
                confirmTitle={t("common:deleteConfirmation.title")}
                confirmMessage={t("videoGroups:confirm_delete_group", {
                  name: videoGroup.name,
                })}
                confirmText={t("common:deleteConfirmation.confirm")}
                cancelText={t("common:deleteConfirmation.cancel")}
                onConfirm={deleteVideoGroup}
              >
                {t("common:buttons.delete")}
              </Button>
            </div>
          </Card.Footer>
        </Card>

        {/* VIDEOS UNDER DETAILS */}
        <Card>
          <Card.Header>
            <div className="d-flex justify-content-between align-items-center">
              <Card.Title level={5}>
                <i className="fas fa-video me-2"></i>
                {t("videoGroups:videos.title")}
                {videos.length > 0 && (
                  <span className="badge bg-primary ms-2">
                    {videos.length}
                  </span>
                )}
              </Card.Title>
              <Button
                variant="primary"
                size="sm"
                className="btn-primary"
                icon="fas fa-plus"
                onClick={handleAddVideo}
              >
                {t("videoGroups:videos.add")}
              </Button>
            </div>
          </Card.Header>

          <Card.Body>
            {videos.length === 0 ? (
              <EmptyState
                icon="fas fa-video"
                message={t("videoGroups:videos.empty")}
                actionText={t("videoGroups:videos.add")}
                onAction={handleAddVideo}
              />
            ) : (
              <div className="table-responsive">
                <Table striped hover>
                  <Table.Head>
                    <Table.Row>
                      <Table.Cell header>#</Table.Cell>
                      <Table.Cell header>
                        {t("videos:table.title")}
                      </Table.Cell>
                      <Table.Cell header>
                        {t("videos:table.available_qualities")}
                      </Table.Cell>
                      <Table.Cell header>
                        {t("videos:table.position")}
                      </Table.Cell>
                      <Table.Cell header>{t("common:actions")}</Table.Cell>
                    </Table.Row>
                  </Table.Head>
                  <Table.Body>
                    {videos.map((video, index) => (
                      <Table.Row key={video.id}>
                        <Table.Cell>{index + 1}</Table.Cell>
                        <Table.Cell>
                          {editingVideo === video.id ? (
                            <Input
                              type="text"
                              value={editForm.title}
                              onChange={(e) =>
                                setEditForm({
                                  ...editForm,
                                  title: e.target.value,
                                })
                              }
                              size="sm"
                            />
                          ) : (
                            video.title
                          )}
                        </Table.Cell>
                        <Table.Cell>
                          <div className="d-flex gap-1 flex-wrap">
                            {video.availableQualities.map((quality, idx) => (
                              <span
                                key={idx}
                                className={`badge rounded-pill ${
                                    quality === video.originalQuality 
                                    ? "bg-secondary text-white" 
                                    : "bg-primary"
                                }`}
                              >
                                {quality}
                              </span>
                            ))}
                          </div>
                        </Table.Cell>
                        <Table.Cell>
                          {editingVideo === video.id ? (
                            <Input
                              type="number"
                              value={editForm.positionInQueue}
                              onChange={(e) =>
                                setEditForm({
                                  ...editForm,
                                  positionInQueue: e.target.value,
                                })
                              }
                              size="sm"
                              style={{ width: "80px" }}
                            />
                          ) : (
                            <span className="badge bg-primary">
                              {video.positionInQueue}
                            </span>
                          )}
                        </Table.Cell>
                        <Table.Cell>
                          <div className="d-flex gap-2">
                            {editingVideo === video.id ? (
                              <>
                                <Button
                                  size="sm"
                                  variant="outline-success"
                                  className="btn-standard"
                                  icon="fas fa-check"
                                  onClick={() => saveEdit(video.id)}
                                >
                                  {t("common:buttons.save")}
                                </Button>
                                <Button
                                  size="sm"
                                  variant="outline-secondary"
                                  className="btn-standard"
                                  icon="fas fa-times"
                                  onClick={cancelEditing}
                                >
                                  {t("common:buttons.cancel")}
                                </Button>
                              </>
                            ) : (
                              <>
                                <Button
                                  size="sm"
                                  variant="outline-warning"
                                  className="btn-standard"
                                  icon="fas fa-edit"
                                  onClick={() => startEditing(video)}
                                >
                                  {t("common:buttons.edit")}
                                </Button>
                                <Button
                                  size="sm"
                                  variant="outline-secondary"
                                  className="btn-standard"
                                  icon="fas fa-eye"
                                  onClick={() => navigate(`/videos/${video.id}`)}
                                >
                                  {t("common:buttons.details")}
                                </Button>
                                <Button
                                  size="sm"
                                  variant="outline-danger"
                                  className="btn-standard"
                                  icon="fas fa-trash"
                                  confirmAction
                                  confirmTitle={t(
                                    "common:deleteConfirmation.title"
                                  )}
                                  confirmMessage={t(
                                    "videoGroups:confirm_delete_video",
                                    { title: video.title }
                                  )}
                                  confirmText={t(
                                    "common:deleteConfirmation.confirm"
                                  )}
                                  cancelText={t(
                                    "common:deleteConfirmation.cancel"
                                  )}
                                  onConfirm={() => deleteVideo(video.id)}
                                >
                                  {t("common:buttons.delete")}
                                </Button>
                              </>
                            )}
                          </div>
                        </Table.Cell>
                      </Table.Row>
                    ))}
                  </Table.Body>
                </Table>
              </div>
            )}
          </Card.Body>
        </Card>
      </div>
    </div>
  );
};

export default VideoGroupDetails;
