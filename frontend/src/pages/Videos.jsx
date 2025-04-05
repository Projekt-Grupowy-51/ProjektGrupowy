import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";

const Videos = () => {
  const { id } = useParams();
  const [videos, setVideos] = useState([]);
  const [streams, setStreams] = useState([]);
  const [isPlaying, setIsPlaying] = useState(false);
  const [labels, setLabels] = useState([]);
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [subjectId, setSubjectId] = useState(null);
  const [labelTimestamps, setLabelTimestamps] = useState({});
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [currentBatch, setCurrentBatch] = useState(1);
  const [videoPositions, setVideoPositions] = useState({});
  const videoRefs = useRef([]);

  useEffect(() => {
    if (id) {
      fetchVideoGroupDetails(id);
      fetchSubjectId();
    }
  }, [id]);

  useEffect(() => {
    if (subjectId !== null) {
      fetchLabels();
    }
  }, [subjectId]);

  useEffect(() => {
    fetchAssignedLabels();
  }, [videos, subjectId]);

  const fetchVideoGroupDetails = async (videoId) => {
    try {
      const response = await httpClient.get(`/VideoGroup/${videoId}`, {
        withCredentials: true,
      });
      console.log(response.data);
      setVideoPositions(response.data.videosAtPositions);

      fetchVideos(currentBatch);
    } catch (error) {
      console.error("Failed to load video group details");
    }
  };

  const onBatchChangedAsync = async (newBatch) => {
    setCurrentBatch(newBatch);
    console.log("Current batch set to: ", newBatch);
    await fetchVideos(newBatch);
  };

  const fetchVideos = async (batch) => {
    try {
      const batchToFetch = batch || currentBatch;
      console.log("Fetching Current batch: ", batchToFetch);
      const response = await httpClient.get(
        `/Video/batch/${id}/${batchToFetch}`,
        {
          withCredentials: true,
        }
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
        const response = await httpClient.get(`/Video/${videoId}/stream`, {
          withCredentials: true,
          responseType: "blob",
        });

        if (response.status === 200 && response.data) {
          return URL.createObjectURL(response.data);
        } else {
          console.error(
            `Failed to fetch stream for video ID ${videoId}:`,
            response
          );
          return null;
        }
      } catch (error) {
        if (error.response) {
          console.error(
            `Server responded with an error for video ID ${videoId}:`,
            error.response
          );
        } else if (error.request) {
          console.error(
            `No response received for video ID ${videoId}. Possible network error:`,
            error.request
          );
        } else {
          console.error(
            `Error setting up request for video ID ${videoId}:`,
            error.message
          );
        }
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

  const fetchSubjectId = async () => {
    try {
      const response = await httpClient.get(
        `/SubjectVideoGroupAssignment/${id}`,
        { withCredentials: true }
      );
      setSubjectId(response.data.subjectId || null);
    } catch (error) {
      console.error("Error fetching subject video group assignment:", error);
    }
  };

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
          .get(`/video/${video.id}/assignedlabels`, {
            withCredentials: true,
          })
          .then((response) => ({
            videoId: video.id,
            labels: response.data,
          }))
      );

      const results = await Promise.all(fetchPromises);

      const allLabels = results.flatMap((result) => result.labels);
      setAssignedLabels(allLabels);
    } catch (error) {
      console.error("Error fetching assigned labels:", error);
      setError("Failed to load assigned labels");
    }
  };

  const handlePlayStop = () => {
    if (isPlaying) {
      videoRefs.current.forEach((video) => {
        if (video) {
          video.pause();
        }
      });
    } else {
      videoRefs.current.forEach((video) => {
        if (video) {
          video.play();
        }
      });
    }
    setIsPlaying(!isPlaying);
  };

  const labelStateRef = useRef({});

  const handleLabelClick = (labelId) => {
    const video = videoRefs.current[0];
    if (!video) return;

    const time = video.currentTime;

    setLabelTimestamps((prevTimestamps) => {
      const newTimestamps = {};
      Object.keys(prevTimestamps).forEach((key) => {
        if (parseInt(key) !== labelId) {
          delete labelStateRef.current[key];
        }
      });

      const currentLabel = prevTimestamps[labelId] || {};
      if (!currentLabel.start) {
        newTimestamps[labelId] = { start: time, sent: false };
      } else if (!currentLabel.sent) {
        newTimestamps[labelId] = { ...currentLabel, sent: true };
      }
      return newTimestamps;
    });

    if (
      labelStateRef.current[labelId]?.start &&
      !labelStateRef.current[labelId]?.sent
    ) {
      sendLabelData(labelId, labelStateRef.current[labelId].start, time);
    } else {
      labelStateRef.current[labelId] = { start: time, sent: false };
    }
  };

  const isMeasuring = (labelId) => {
    return labelTimestamps[labelId] && !labelTimestamps[labelId].sent;
  };

  const formatTime = (seconds) => {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const remainingSeconds = Math.floor(seconds % 60);
    const milliseconds = Math.round(((seconds % 1) * 1000) / 10) * 10; // Round to nearest 10ms

    return `${padZero(hours)}:${padZero(minutes)}:${padZero(
      remainingSeconds
    )}.${milliseconds.toString().padStart(3, "0")}`;
  };

  const padZero = (num) => num.toString().padStart(2, "0");

  const sendLabelData = async (labelId, start, end) => {
    const labelerId = -1;
    const startFormatted = formatTime(start);
    const endFormatted = formatTime(end);

    if (videos.length === 0) {
      console.error("No videos available to assign labels to");
      return;
    }

    try {
      const assignPromises = videos.map((video) => {
        const data = {
          labelId,
          videoId: video.id,
          labelerId,
          start: startFormatted,
          end: endFormatted,
        };
        return httpClient.post("/AssignedLabel", data);
      });

      await Promise.all(assignPromises);

      const newAssignedLabel = {
        labelId,
        start: startFormatted,
        end: endFormatted,
        colorHex:
          labels.find((label) => label.id === labelId)?.colorHex || "#000000",
      };

      setAssignedLabels((prevLabels) => [newAssignedLabel, ...prevLabels]);

      setLabelTimestamps((prev) => {
        const newTimestamps = { ...prev };
        delete newTimestamps[labelId];
        return newTimestamps;
      });

      labelStateRef.current[labelId] = { start: null, sent: false };

      fetchAssignedLabels();
    } catch (error) {
      console.error("Error assigning label:", error);
    }
  };

  const handleTimeUpdate = () => {
    const video = videoRefs.current[0];
    if (video) {
      setCurrentTime(video.currentTime);
      setDuration(video.duration);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this assigned label?"))
      return;

    try {
      await httpClient.delete(`/AssignedLabel/${id}`);
      setAssignedLabels((prev) =>
        prev.filter((assignedLabel) => assignedLabel.id !== id)
      );

      fetchAssignedLabels();
    } catch (error) {
      setError("Failed to delete label");
    }
  };

  useEffect(() => {
    const handleKeyPress = (event) => {
      if (event.key === " ") {
        event.preventDefault();
        handlePlayStop();
      }

      const label = labels.find(
        (l) => l.shortcut.toLowerCase() === event.key.toLowerCase()
      );
      if (label) {
        handleLabelClick(label.id);
      }
    };

    window.addEventListener("keydown", handleKeyPress);
    return () => {
      window.removeEventListener("keydown", handleKeyPress);
    };
  }, [labels, isPlaying]);

  const handleRewind = (time) => {
    videoRefs.current.forEach((video) => {
      if (video) {
        video.currentTime = Math.max(video.currentTime - time, 0);
      }
    });
    setCurrentTime((prevTime) => Math.max(prevTime - time, 0));
  };

  const handleFastForward = (time) => {
    videoRefs.current.forEach((video) => {
      if (video) {
        video.currentTime = Math.min(video.currentTime + time, video.duration);
      }
    });
    setCurrentTime((prevTime) => Math.min(prevTime + time, duration));
  };

  const getTextColor = (backgroundColor) => {
    const rgb = parseInt(backgroundColor.slice(1), 16);
    const r = (rgb >> 16) & 0xff;
    const g = (rgb >> 8) & 0xff;
    const b = rgb & 0xff;
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness > 128 ? "#000000" : "#ffffff";
  };

  return (
    <div className="container">
      <div className="content">
        <div className="container" id="video-container">
          <div className="row" id="video-row">
            {streams.length > 0 ? (
              streams.map((streamUrl, index) => (
                <div
                  className={`col-12 ${
                    streams.length === 1
                      ? ""
                      : streams.length <= 4
                      ? "col-md-5"
                      : "col-md-4"
                  }`}
                  key={index}
                >
                  <div className="video-cell">
                    <video
                      ref={(el) => (videoRefs.current[index] = el)}
                      width="100%"
                      height="auto"
                      src={streamUrl}
                      type="video/mp4"
                      controls
                      onTimeUpdate={handleTimeUpdate}
                    />
                  </div>
                </div>
              ))
            ) : (
              <p>Loading video streams...</p>
            )}
          </div>
        </div>
        <div className="progress-bar text-center">
          <input
            type="range"
            min="0"
            max={duration || 100}
            value={currentTime}
            step="0.01"
            onChange={(e) => {
              const newTime = parseFloat(e.target.value);
              videoRefs.current.forEach((video) => {
                if (video) {
                  video.currentTime = newTime;
                }
              });
              setCurrentTime(newTime);
            }}
          />
        </div>
        <div className="row">
          <div className="col-12">
            <div className="pagination d-flex justify-content-between">
              <button
                className="btn btn-primary pagination-button"
                onClick={() => onBatchChangedAsync(currentBatch - 1)}
                disabled={currentBatch <= 1}
              >
                Previous
              </button>
              <div className="controls">
                <div className="controls-row time-display">
                  {formatTime(currentTime)} /{" "}
                  {formatTime(isNaN(duration) ? 0 : duration)}
                </div>
                <div className="controls-row">
                  <button
                    className="btn btn-primary seek-btn"
                    onClick={() => handleRewind(5)}
                  >
                    <i className="fas fa-backward"></i>
                    <span>-5s</span>
                  </button>
                  <button
                    className="btn btn-primary seek-btn"
                    onClick={() => handleRewind(1)}
                  >
                    <i className="fas fa-backward"></i>
                    <span>-1s</span>
                  </button>
                  <button
                    className="btn btn-primary play-stop-btn"
                    onClick={handlePlayStop}
                  >
                    <i
                      className={`fas ${isPlaying ? "fa-stop" : "fa-play"}`}
                    ></i>
                  </button>
                  <button
                    className="btn btn-primary seek-btn"
                    onClick={() => handleFastForward(1)}
                  >
                    <i className="fas fa-forward"></i>
                    <span>+1s</span>
                  </button>
                  <button
                    className="btn btn-primary seek-btn"
                    onClick={() => handleFastForward(5)}
                  >
                    <i className="fas fa-forward"></i>
                    <span>+5s</span>
                  </button>
                </div>
              </div>
              <button
                className="btn btn-primary pagination-button"
                onClick={() => onBatchChangedAsync(currentBatch + 1)}
                disabled={currentBatch === Object.keys(videoPositions).length}
              >
                Next
              </button>
            </div>
          </div>
        </div>
        <div className="text-center mt-3">
          {/* <label htmlFor="playbackSpeedSelect" className="form-label me-2">
            Playback Speed:
          </label> */}
          <select
            className="form-select w-25 d-inline-block" // Bootstrap classes
            id="playbackSpeedSelect"
            onChange={(e) => {
              const selectedSpeed = parseFloat(e.target.value);
              videoRefs.current.forEach((video) => {
                if (video) {
                  video.playbackRate = selectedSpeed;
                }
              });
            }}
            defaultValue="1.0"
          >
            <option value="0.25">0.25x</option>
            <option value="0.5">0.5x</option>
            <option value="0.75">0.75x</option>
            <option value="1.0">1.0x (normal)</option>
            <option value="1.25">1.25x</option>
            <option value="1.5">1.5x</option>
            <option value="1.75">1.75x</option>
            <option value="2.0">2.0x</option>
          </select>
        </div>
        <div className="labels-container">
          {labels.length > 0 ? (
            labels.map((label, index) => {
              const measuring = isMeasuring(label.id);
              const textColor = getTextColor(label.colorHex);
              return (
                <button
                  key={index}
                  className="btn label-btn"
                  style={{
                    backgroundColor: label.colorHex,
                    color: textColor,
                  }}
                  onClick={() => handleLabelClick(label.id)}
                >
                  {label.name + " [" + label.shortcut + "]"}{" "}
                  {measuring ? "STOP" : "START"}
                </button>
              );
            })
          ) : (
            <p>No labels available</p>
          )}
        </div>

        <div className="assigned-labels">
          <h3>Assigned Labels:</h3>
          <div className="assigned-labels-table">
            {assignedLabels.length > 0 ? (
              <table className="normal-table">
                <thead className="table-dark">
                  <tr>
                    <th>Label ID</th>
                    <th>Video ID</th>
                    <th>Start Time</th>
                    <th>End Time</th>
                    <th>Ins Date</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {assignedLabels
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
                    .map((label, index) => {
                      const matchingLabel = labels.find(
                        (l) => l.id === label.labelId
                      );
                      return (
                        <tr key={index}>
                          <td>
                            {matchingLabel ? matchingLabel.name : "Unknown"}
                          </td>
                          <td>{label.videoId}</td>
                          <td>{label.start}</td>
                          <td>{label.end}</td>
                          <td>{new Date(label.insDate).toLocaleString()}</td>
                          <td>
                            <button
                              className="btn btn-danger"
                              onClick={() => handleDelete(label.id)}
                            >
                              Delete
                            </button>
                          </td>
                        </tr>
                      );
                    })}
                </tbody>
              </table>
            ) : (
              <div className="text-center py-4">
                <i className="fas fa-tags fs-1 text-muted"></i>
                <p className="text-muted mt-2">No labels assigned yet</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Videos;
