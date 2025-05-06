import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import { useNotification } from "../context/NotificationContext";
import { useTranslation } from "react-i18next";

function useVideoGroup(videoGroupId, currentBatch, setCurrentBatch) {
  const { addNotification } = useNotification();
  const [videoPositions, setVideoPositions] = useState({});
  const [videos, setVideos] = useState([]);
  const [streams, setStreams] = useState([]);

  useEffect(() => {
    if (videoGroupId !== null) {
      fetchVideoGroupDetails();
    }
  }, [videoGroupId]);

  const fetchVideoGroupDetails = async () => {
    try {
      const response = await httpClient.get(`/VideoGroup/${videoGroupId}`, {
        withCredentials: true,
      });
      setVideoPositions(response.data.videosAtPositions);
      fetchVideos(currentBatch);
    } catch (error) {
      console.error("Failed to load video group details");
    }
  };

  const onBatchChangedAsync = async (newBatch) => {
    setCurrentBatch(newBatch);
    await fetchVideos(newBatch);
  };

  const fetchVideos = async (batch) => {
    try {
      const batchToFetch = batch || currentBatch;
      const response = await httpClient.get(
        `/Video/batch/${videoGroupId}/${batchToFetch}`,
        { withCredentials: true }
      );
      setVideos(response.data);
      fetchVideoStreams(response.data);
    } catch (error) {
      console.error("Failed to load video list");
    }
  };

  async function fetchVideoStreams(videos) {
    const videoIds = videos.slice(0, 4).map((video) => video.id);
    const streamsPromises = videoIds.map(async (videoId) => {
      try {
        const response = await httpClient.get(`/Video/${videoId}/${subjectId}/stream`, {
          withCredentials: true,
          responseType: "blob",
        });

        if (response.status === 200 && response.data) {
          return URL.createObjectURL(response.data);
        } else {
          console.error(`Failed to fetch stream for video ID ${videoId}`);
          return null;
        }
      } catch (error) {
        console.error(`Error fetching stream for video ID ${videoId}`);
        return null;
      }
    });

    try {
      const streamUrls = await Promise.all(streamsPromises);
      setStreams(streamUrls.filter((url) => url !== null));
    } catch (error) {
      console.error("Error processing video streams:", error);
    }
  }

  return {
    videos,
    streams,
    videoPositions,
    fetchVideos,
    onBatchChangedAsync,
  };
}

function useLabels(videos, subjectId) {
  const [labels, setLabels] = useState([]);
  const [assignedLabels, setAssignedLabels] = useState([]);

  useEffect(() => {
    if (subjectId !== null) {
      fetchLabels();
    }
  }, [subjectId]);

  useEffect(() => {
    if (videos.length > 0) {
      fetchAssignedLabels();
    }
  }, [videos]);

  const fetchLabels = async () => {
    if (subjectId === null) return;

    try {
      const response = await httpClient.get(`/subject/${subjectId}/label`, {
        withCredentials: true,
      });
      setLabels(response.data || []);
    } catch (error) {
      console.error("Error fetching labels:", error);
    }
  };

  const fetchAssignedLabels = async () => {
    try {
      const fetchPromises = videos.map((video) =>
        httpClient
          .get(`/video/${video.id}/${subjectId}/assignedlabels`, {
            withCredentials: true,
          })
          .then((response) => response.data)
      );

      const results = await Promise.all(fetchPromises);
      setAssignedLabels(results.flat());
    } catch (error) {
      console.error("Error fetching assigned labels:", error);
    }
  };

  const handleLabelClick = async (labelId, start, end) => {
    if (videos.length === 0) {
      console.error("No videos available to assign labels to");
      return;
    }

    try {
      const assignPromises = videos.map((video) =>
        httpClient.post("/AssignedLabel", {
          labelId,
          videoId: video.id,
          start,
          end,
        })
      );

      await Promise.all(assignPromises);
      fetchAssignedLabels();
    } catch (error) {
      console.error("Error assigning label:", error);
    }
  };

  return {
    labels,
    assignedLabels,
    handleLabelClick,
  };
}

const Videos = () => {
  const { id } = useParams();
  const [subjectId, setSubjectId] = useState(null);
  const [videoGroupId, setVideoGroupId] = useState(null);
  const [currentBatch, setCurrentBatch] = useState(1);
  const videoRefs = useRef([]);
  const { t } = useTranslation(["videos", "common"]);
  const [resetTrigger, setResetTrigger] = useState(0);

  const videoGroup = useVideoGroup(videoGroupId, currentBatch, setCurrentBatch);
  const { videos, streams, videoPositions, onBatchChangedAsync } = videoGroup;

  const labelsManager = useLabels(videos, subjectId);
  const { labels, assignedLabels, handleLabelClick } = labelsManager;

  useEffect(() => {
    if (id) {
      fetchAssignment();
    }
  }, [id]);

  const fetchAssignment = async () => {
    try {
      const response = await httpClient.get(
        `/SubjectVideoGroupAssignment/${id}`,
        { withCredentials: true }
      );
      setSubjectId(response.data.subjectId || null);
      setVideoGroupId(response.data.videoGroupId || null);
    } catch (error) {
      console.error("Error fetching subject video group assignment:", error);
    }
  };

  const handleBatchChange = (newBatch) => {
    onBatchChangedAsync(newBatch);
    setResetTrigger((prev) => prev + 1); 
    controls.handleBatchChange(newBatch); 
    setTimeout(() => controls.handlePlayStop(), 0); 
  };

  return (
    <div className="container">
      <div className="content">
        <div className="container" id="video-container">
          <div
            className="row"
            id="video-row"
            style={
              streams.length >= 3 && streams.length <= 4
                ? {
                    display: "grid",
                    gridTemplateColumns: "1fr 1fr",
                    gridTemplateRows: "1fr 1fr",
                    gap: "10px",
                    height: "75vh",
                    placeItems: "center",
                  }
                : {}
            }
          >
            {streams.map((streamUrl, index) => (
              <div
                key={index}
                style={
                  streams.length >= 3 && streams.length <= 4
                    ? {
                        height: "calc(75vh / 2)",
                        width: "calc((75vh / 2) * (16 / 9))",
                      }
                    : {}
                }
              >
                <div className="video-cell">
                  <video
                    ref={(el) => (videoRefs.current[index] = el)}
                    width="100%"
                    height="100%"
                    src={streamUrl}
                    type="video/mp4"
                  />
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="labels-container">
          {labels.map((label) => (
            <button
              key={label.id}
              className="btn label-btn"
              style={{ backgroundColor: label.colorHex }}
              onClick={() =>
                handleLabelClick(label.id, "00:00:00.000", "00:00:05.000")
              }
            >
              {label.name}
            </button>
          ))}
        </div>

        <div className="assigned-labels">
          <h3>Assigned Labels:</h3>
          <DataTable
            columns={[
              { field: "labelId", header: "Label ID" },
              { field: "start", header: "Start Time" },
              { field: "end", header: "End Time" },
            ]}
            data={assignedLabels}
          />
        </div>
      </div>
    </div>
  );
};

export default Videos;
