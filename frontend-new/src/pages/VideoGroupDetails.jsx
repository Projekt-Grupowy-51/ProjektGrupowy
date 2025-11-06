import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Container, Card, Button, Table, Input } from "../components/ui";
import {
  LoadingSpinner,
  ErrorAlert,
  EmptyState,
  PageHeader,
  DetailPageActions,
  DetailSection,
} from "../components/common";
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

    const handleVideoProcessed = (data) => {
      console.log("VideoProcessed event received:", data);
      refetchVideos();
    };

    signalR.on(MessageTypes.VideoProcessed, handleVideoProcessed);

    return () => {
      signalR.off(MessageTypes.VideoProcessed, handleVideoProcessed);
    };
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

  if (loading) {
    return (
      <Container>
        <LoadingSpinner />
      </Container>
    );
  }

  if (error) {
    return (
      <Container>
        <ErrorAlert error={error} />
      </Container>
    );
  }

  if (!videoGroup) {
    return (
      <Container>
        <ErrorAlert error={t("videoGroups:not_found")} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <PageHeader
        title={videoGroup.name}
        subtitle={videoGroup.description}
        icon="fas fa-film"
        actions={
          <DetailPageActions
            onEdit={handleEditVideoGroup}
            onDelete={deleteVideoGroup}
            onBack={handleBack}
            editText={t("common:buttons.edit")}
            deleteText={t("common:buttons.delete")}
            backText={t("common:buttons.back")}
            deleteConfirmTitle={t("common:deleteConfirmation.title")}
            deleteConfirmMessage={t("videoGroups:confirm_delete_group", {
              name: videoGroup.name,
            })}
          />
        }
      />

      <Card className="mb-4">
        <Card.Header variant="info">
          <Card.Title level={5} className="mb-0">
            <i className="fas fa-info-circle me-2"></i>
            {t("videoGroups:details.title")}
          </Card.Title>
        </Card.Header>
        <Card.Body>
          <Card.Text>
            <strong>{t("videoGroups:details.description")}:</strong>{" "}
            {videoGroup.description}
          </Card.Text>
        </Card.Body>
      </Card>

      <DetailSection
        title={t("videoGroups:videos.title")}
        icon="fas fa-video"
        actions={
          <Button variant="primary" icon="fas fa-plus" onClick={handleAddVideo}>
            {t("videoGroups:videos.add")}
          </Button>
        }
      />

      {videos.length > 0 ? (
        <Card>
          <Table striped hover responsive>
            <Table.Head>
              <Table.Row>
                <Table.Cell header={true}>#</Table.Cell>
                <Table.Cell header={true}>{t("videos:table.title")}</Table.Cell>
                <Table.Cell header={true}>
                  {t("videos:table.available_qualities")}
                </Table.Cell>
                <Table.Cell header={true}>
                  {t("videos:table.position")}
                </Table.Cell>
                <Table.Cell header={true}>{t("common:actions")}</Table.Cell>
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
                          setEditForm({ ...editForm, title: e.target.value })
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
                              ? "bg-primary"
                              : "bg-info"
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
                      <span className="badge bg-secondary">
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
                            variant="success"
                            icon="fas fa-check"
                            onClick={() => saveEdit(video.id)}
                          >
                            {t("common:buttons.save")}
                          </Button>
                          <Button
                            size="sm"
                            variant="secondary"
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
                            variant="warning"
                            icon="fas fa-edit"
                            onClick={() => startEditing(video)}
                          >
                            {t("common:buttons.edit")}
                          </Button>
                          <Button
                            size="sm"
                            variant="info"
                            icon="fas fa-eye"
                            onClick={() => navigate(`/videos/${video.id}`)}
                          >
                            {t("common:buttons.details")}
                          </Button>
                          <Button
                            size="sm"
                            variant="outline-danger"
                            icon="fas fa-trash"
                            confirmAction={true}
                            confirmTitle={t("common:deleteConfirmation.title")}
                            confirmMessage={t(
                              "videoGroups:confirm_delete_video",
                              { title: video.title }
                            )}
                            confirmText={t("common:deleteConfirmation.confirm")}
                            cancelText={t("common:deleteConfirmation.cancel")}
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
        </Card>
      ) : (
        <EmptyState
          icon="fas fa-video"
          title={t("videoGroups:videos.no_videos")}
          message={t("videoGroups:videos.empty")}
          actionText={t("videoGroups:videos.add")}
          onAction={handleAddVideo}
        />
      )}
    </Container>
  );
};

export default VideoGroupDetails;
