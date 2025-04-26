import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import DataTable from "../components/DataTable";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";
import { formatISODate } from "../utils/dateFormatter.jsx";
import { useTranslation } from "react-i18next";

const VideoDetails = () => {
  const { id: videoId } = useParams();
  const [videoData, setVideoData] = useState(null);
  const [videoStream, setVideoStream] = useState(null);
  const [labels, setLabels] = useState([]);
  const videoRef = useRef(null);
  const [isEditingTitle, setIsEditingTitle] = useState(false);
  const [editedTitle, setEditedTitle] = useState("");
  const { addNotification } = useNotification();
  const { t } = useTranslation(['videos', 'common']);

  useEffect(() => {
    fetchVideoDetails(videoId);
    fetchVideoStream(videoId);
    fetchAssignedLabels(videoId);
  }, [videoId]);

  const fetchVideoDetails = async (videoId) => {
    try {
      const response = await httpClient.get(`/Video/${videoId}`, {
        withCredentials: true,
      });

      if (response.status === 200) {
        setVideoData(response.data);
      } else {
        addNotification(
          `Unexpected response status: ${response.status}`,
          "error"
        );
      }
    } catch (error) {
      addNotification(t('errors.load_video_details'), "error");
    }
  };

  async function fetchVideoStream(videoId) {
    try {
      const response = await httpClient.get(`/Video/${videoId}/stream`, {
        withCredentials: true,
        responseType: "blob",
      });
      const streamUrl = URL.createObjectURL(response.data);
      setVideoStream(streamUrl);
    } catch (error) {
      addNotification(t('errors.load_video_stream'), "error");
      setVideoStream(null);
    }
  }

  const fetchAssignedLabels = async (videoId) => {
    try {
      const response = await httpClient.get(
        `/Video/${videoId}/assignedlabels`,
        {
          withCredentials: true,
        }
      );
      setLabels(response.data);
    } catch (error) {
      addNotification(t('errors.load_labels'), "error");
    }
  };

  const handleEditTitle = () => {
    setIsEditingTitle(true);
    setEditedTitle(videoData.title);
  };

  const handleSaveTitle = async () => {
    try {
      const response = await httpClient.put(
        `/Video/${videoId}`,
        { title: editedTitle },
        { withCredentials: true }
      );

      if (response.status === 200) {
        setVideoData((prevData) => ({ ...prevData, title: editedTitle }));
        setIsEditingTitle(false);
        addNotification(t('success.title_updated'), "success");
      } else {
        addNotification(t('errors.title_update_failed', { status: response.status }), "error");
      }
    } catch (error) {
      addNotification(t('errors.update_title'), "error");
    }
  };

  const handleCancelEdit = () => {
    setIsEditingTitle(false);
    setEditedTitle("");
  };

  // Define columns for the assigned labels table
  const labelColumns = [
    { field: "labelName", header: t('table.label') },
    { field: "labelerName", header: t('table.labeler') },
    { field: "start", header: t('table.start') },
    { field: "end", header: t('table.end') },
    {
      field: "insDate",
      header: t('table.ins_date'),
      render: (label) => new Date(label.insDate).toLocaleString(),
    },
  ];

  if (!videoData) {
    return <div className="container text-center">{t('details.loading')}</div>;
  }

  return (
    <div className="container">
      <div className="d-flex justify-content-end mb-3">
        <NavigateButton actionType="Back" />
      </div>

      {isEditingTitle ? (
        <div className="edit-title-container text-center mb-4">
          <input
            type="text"
            value={editedTitle}
            onChange={(e) => setEditedTitle(e.target.value)}
            className="form-control mb-2"
          />
          <button onClick={handleSaveTitle} className="btn btn-success me-2">
            {t('buttons.save')}
          </button>
          <button onClick={handleCancelEdit} className="btn btn-secondary">
            {t('buttons.cancel')}
          </button>
        </div>
      ) : (
        <h1 className="text-center mb-4">
          {videoData.title}
          <button
            onClick={handleEditTitle}
            className="btn btn-link ms-2"
            style={{ textDecoration: "none" }}
          >
            ✏️
          </button>
        </h1>
      )}
      <div
        className="video-container mb-4"
        style={{ position: "relative", paddingTop: "56.25%" }}
      >
        <video
          ref={videoRef}
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            objectFit: "contain",
          }}
          controls
          src={videoStream}
          type="video/mp4"
        />
      </div>
      <div className="assigned-labels">
        <h3 className="">{t('details.assigned_labels')}</h3>
        <div className="assigned-labels-table">
          <DataTable
            showRowNumbers={true}
            columns={labelColumns}
            data={labels}
          />
        </div>
      </div>
    </div>
  );
};

export default VideoDetails;
