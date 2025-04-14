import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";

const VideoDetails = () => {
  const { id: videoId } = useParams();
  const [videoData, setVideoData] = useState(null);
  const [videoStream, setVideoStream] = useState(null);
  const [labels, setLabels] = useState([]);
  const videoRef = useRef(null);
  const [isEditingTitle, setIsEditingTitle] = useState(false);
  const [editedTitle, setEditedTitle] = useState("");

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
        console.warn(`Unexpected response status: ${response.status}`);
      }
    } catch (error) {
      console.error("Failed to load video details:", error.message || error);
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
      console.error("Error fetching video stream:", error);
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
      console.error("Failed to load assigned labels:", error);
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
      } else {
        console.warn(`Failed to update title: ${response.status}`);
      }
    } catch (error) {
      console.error("Error updating title:", error);
    }
  };

  const handleCancelEdit = () => {
    setIsEditingTitle(false);
    setEditedTitle("");
  };

  if (!videoData) {
    return <div className="container text-center">Loading...</div>;
  }

  return (
    <div className="container">
      {isEditingTitle ? (
        <div className="edit-title-container text-center mb-4">
          <input
            type="text"
            value={editedTitle}
            onChange={(e) => setEditedTitle(e.target.value)}
            className="form-control mb-2"
          />
          <button onClick={handleSaveTitle} className="btn btn-success me-2">
            Save
          </button>
          <button onClick={handleCancelEdit} className="btn btn-secondary">
            Cancel
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
        <h3 className="">Assigned Labels</h3>
        <div className="assigned-labels-table">
          <table className="normal-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Label Id</th>
                <th>Labeler Id</th>
                <th>Start</th>
                <th>End</th>
                <th>Ins Date</th>
              </tr>
            </thead>
            <tbody>
              {labels
                .slice()
                .sort((a, b) => {
                  if (!a.insDate && !b.insDate) {
                    return a.videoId - b.videoId;
                  }

                  if (!a.insDate) return -1;
                  if (!b.insDate) return 1;

                  const dateA = new Date(a.insDate);
                  const dateB = new Date(b.insDate);

                  if (
                    dateA.getFullYear() === dateB.getFullYear() &&
                    dateA.getMonth() === dateB.getMonth() &&
                    dateA.getDate() === dateB.getDate() &&
                    dateA.getHours() === dateB.getHours() &&
                    dateA.getMinutes() === dateB.getMinutes() &&
                    dateA.getSeconds() === dateB.getSeconds()
                  ) {
                    return a.videoId - b.videoId;
                  }

                  return dateB - dateA;
                })
                .map((label, index) => (
                  <tr key={label.id}>
                    <td>{index + 1}</td>
                    <td>{label.labelId}</td>
                    <td>{label.labelerId}</td>
                    <td>{label.start}</td>
                    <td>{label.end}</td>
                    <td>{new Date(label.insDate).toLocaleString()}</td>
                  </tr>
                ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default VideoDetails;
