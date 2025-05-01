import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpclient";
import "./css/ScientistProjects.css";
import DeleteButton from "../components/DeleteButton";
import DataTable from "../components/DataTable";
import { useNotification } from "../context/NotificationContext";
import { formatISODate } from "../utils/dateFormatter";
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
      // addNotification("Failed to load video group details", "error");
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

  return {
    videos,
    streams,
    videoPositions,
    fetchVideos,
    onBatchChangedAsync,
  };
}

function useVideoControls(videoRefs) {
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [timeLeft, setTimeLeft] = useState(0);
  const [playbackSpeed, setPlaybackSpeed] = useState(1.0);

  // Enhanced method to synchronize all video elements
  const syncAllVideos = (action, value = null) => {
    videoRefs.current.forEach((video) => {
      if (!video) return;

      switch (action) {
        case "play":
          video.play().catch((err) => console.error("Play failed:", err));
          break;
        case "pause":
          video.pause();
          break;
        case "reset":
          video.pause();
          video.currentTime = 0;
          break;
        case "setTime":
          video.currentTime = value;
          break;
        case "setSpeed":
          video.playbackRate = value;
          break;
        default:
          break;
      }
    });
  };

  // Play/pause toggle with proper state management
  const handlePlayStop = () => {
    const video = videoRefs.current[0];
    if (!video) return;

    if (isPlaying) {
      syncAllVideos("pause");
    } else {
      syncAllVideos("play");
    }

    setIsPlaying(!isPlaying);
  };

  useEffect(() => {
    // Ensure playback state is synchronized with the video element
    const video = videoRefs.current[0];
    if (!video) return;

    const handlePlay = () => setIsPlaying(true);
    const handlePause = () => setIsPlaying(false);

    video.addEventListener("play", handlePlay);
    video.addEventListener("pause", handlePause);

    return () => {
      video.removeEventListener("play", handlePlay);
      video.removeEventListener("pause", handlePause);
    };
  }, [videoRefs]);

  useEffect(() => {
    // Ensure progress bar updates correctly after playback speed change
    const video = videoRefs.current[0];
    if (!video) return;

    const handleTimeUpdate = () => {
      setCurrentTime(video.currentTime);
      setDuration(video.duration);
    };

    video.addEventListener("timeupdate", handleTimeUpdate);

    return () => {
      video.removeEventListener("timeupdate", handleTimeUpdate);
    };
  }, [playbackSpeed, videoRefs]);

  // Apply playback speed to all videos
  const handlePlaybackSpeedChange = (speed) => {
    setPlaybackSpeed(speed);
    syncAllVideos("setSpeed", speed);

    // Trigger a re-render to ensure the progress bar updates
    setCurrentTime((prevTime) => prevTime);

    // Explicitly trigger the timeupdate event on all videos
    videoRefs.current.forEach((video) => {
      if (video) {
        video.dispatchEvent(new Event("timeupdate"));
      }
    });
  };

  // Update UI based on video time (called continuously during playback)
  const handleTimeUpdate = () => {
    const video = videoRefs.current[0];
    if (!video) return;

    setTimeLeft(Math.round(video.duration - video.currentTime));
    setCurrentTime(video.currentTime);
    setDuration(video.duration);
  };

  // Seek to specific position in all videos
  const handleSeek = (newTime) => {
    syncAllVideos("setTime", newTime);
    setCurrentTime(newTime);
  };

  const handleRewind = (time) => {
    const newTime = Math.max(currentTime - time, 0);
    handleSeek(newTime);
  };

  const handleFastForward = (time) => {
    const newTime = Math.min(currentTime + time, duration);
    handleSeek(newTime);
  };

  // Complete playback reset - important for batch changes
  const resetPlayback = () => {
    setIsPlaying(false);
    setCurrentTime(0);
    setTimeLeft(duration);
    syncAllVideos("reset");
  };

  // Properly apply speed to newly loaded videos
  useEffect(() => {
    // This ensures new videos get the current playback speed
    videoRefs.current.forEach((video) => {
      if (video) video.playbackRate = playbackSpeed;
    });
  }, [videoRefs.current.length, playbackSpeed]);

  useEffect(() => {
    // Apply playback speed to all videos when it changes
    videoRefs.current.forEach((video) => {
      if (video) {
        video.playbackRate = playbackSpeed;
        // Force a small seek to trigger timeupdate
        const currentTime = video.currentTime;
        video.currentTime = currentTime + 0.001;
        video.currentTime = currentTime;
      }
    });
  }, [playbackSpeed]);

  useEffect(() => {
    // Add event listeners for ratechange to ensure progress bar updates
    videoRefs.current.forEach((video) => {
      if (video) {
        const handleRateChange = () => {
          // Force a state update to refresh the progress bar
          setCurrentTime(video.currentTime);
        };

        video.addEventListener("ratechange", handleRateChange);

        // Cleanup listener on unmount
        return () => {
          video.removeEventListener("ratechange", handleRateChange);
        };
      }
    });
  }, [videoRefs]);

  return {
    isPlaying,
    setIsPlaying,
    currentTime,
    setCurrentTime,
    duration,
    timeLeft,
    playbackSpeed,
    handlePlayStop,
    handlePlaybackSpeedChange,
    handleTimeUpdate,
    handleRewind,
    handleFastForward,
    resetPlayback,
    handleSeek,
  };
}

function useLabels(videos, subjectId) {
  const [labels, setLabels] = useState([]);
  const [assignedLabels, setAssignedLabels] = useState([]);
  const [labelTimestamps, setLabelTimestamps] = useState({});
  const labelStateRef = useRef({});

  useEffect(() => {
    if (subjectId !== null) {
      fetchLabels();
    }
  }, [subjectId]);

  useEffect(() => {
    fetchAssignedLabels();
  }, [videos, subjectId]);

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
    }
  };

  const handleLabelClick = (labelId, videoRefs) => {
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

  const sendLabelData = async (labelId, start, end) => {
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

  const handleDelete = async (id) => {
    try {
      await httpClient.delete(`/AssignedLabel/${id}`);
      setAssignedLabels((prev) =>
        prev.filter((assignedLabel) => assignedLabel.id !== id)
      );

      fetchAssignedLabels();
    } catch (error) {
      console.error("Failed to delete label");
    }
  };

  const isMeasuring = (labelId) => {
    return labelTimestamps[labelId] && !labelTimestamps[labelId].sent;
  };

  return {
    labels,
    assignedLabels,
    labelTimestamps,
    handleLabelClick,
    handleDelete,
    isMeasuring,
    fetchAssignedLabels,
    sendLabelData,
  };
}

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

const getTextColor = (backgroundColor) => {
  const rgb = parseInt(backgroundColor.slice(1), 16);
  const r = (rgb >> 16) & 0xff;
  const g = (rgb >> 8) & 0xff;
  const b = rgb & 0xff;
  const brightness = (r * 299 + g * 587 + b * 114) / 1000;
  return brightness > 128 ? "#000000" : "#ffffff";
};

const Videos = () => {
  const { id } = useParams();
  const [subjectId, setSubjectId] = useState(null);
  const [videoGroupId, setVideoGroupId] = useState(null);
  const [currentBatch, setCurrentBatch] = useState(1);
  const videoRefs = useRef([]);
    const { t } = useTranslation(['videos', 'common']);

  // Initialize refs for batch transitions
  const endedVideosCount = useRef(0);
  const endedIndexes = useRef(new Set());
  const shouldAutoplayRef = useRef(false);

  // Custom hooks for specific functionality
  const videoControls = useVideoControls(videoRefs);
  const {
    isPlaying,
    setIsPlaying,
    currentTime,
    setCurrentTime,
    duration,
    timeLeft,
    playbackSpeed,
    handlePlayStop,
    handlePlaybackSpeedChange,
    handleTimeUpdate,
    handleRewind,
    handleFastForward,
    resetPlayback,
    handleSeek,
  } = videoControls;

  const videoGroup = useVideoGroup(videoGroupId, currentBatch, setCurrentBatch);
  const { videos, streams, videoPositions, onBatchChangedAsync } = videoGroup;

  const labelsManager = useLabels(videos, subjectId);
  const {
    labels,
    assignedLabels,
    labelTimestamps,
    isMeasuring,
    handleDelete,
    sendLabelData,
  } = labelsManager;

  // Define columns for the assigned labels table
    const labelColumns = [
        { field: "labelName", header: t('table.label') },
        { field: "labelerName", header: t('table.labeler') },
        { field: "subjectName", header: t('table.subject') },
        { field: "start", header: t('table.start') },
        { field: "end", header: t('table.end') },
        {
            field: "insDate",
            header: t('table.ins_date'),
            render: (label) => new Date(label.insDate).toLocaleString(),
        },
        { field: "videoId", header: t('table.videoId') }
    ];

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

  useEffect(() => {
    videoRefs.current = [];
    videoRefs.current.forEach((video) => {
      if (video) {
        video.playbackRate = playbackSpeed;
      }
    });
  }, [streams, playbackSpeed]);

  useEffect(() => {
    if (shouldAutoplayRef.current && streams.length > 0) {
      setTimeout(() => {
        videoRefs.current.forEach((video) => {
          if (video) {
            video.currentTime = 0;
            video.play();
          }
        });
        setIsPlaying(true);
        shouldAutoplayRef.current = false;
      }, 300);
    }
  }, [streams]);

  // Improved batch change handling
  const handleBatchChange = async (newBatch) => {
    // Store current speed before reset
    const currentSpeed = playbackSpeed;

    // Reset playback before changing batch
    resetPlayback();

    // Change batch
    await onBatchChangedAsync(newBatch);

    // After batch change, these will be applied when videos load
    shouldAutoplayRef.current = false; // Don't autoplay on manual change
  };

  // Automatic batch change when videos end
  const handleVideoEnd = async (index) => {
    if (endedIndexes.current.has(index)) return;

    endedIndexes.current.add(index);
    endedVideosCount.current += 1;

    // When all videos in batch have ended
    if (endedVideosCount.current === streams.length) {
      endedVideosCount.current = 0;
      endedIndexes.current.clear();

      const totalBatches = Object.keys(videoPositions).length;

      // If there's a next batch available
      if (currentBatch < totalBatches) {
        const currentSpeed = playbackSpeed; // Remember current speed

        // Signal that we want to autoplay on the next batch
        shouldAutoplayRef.current = true;

        // Reset and navigate
        resetPlayback();
        await onBatchChangedAsync(currentBatch + 1);
      } else {
        // If it's the last batch, just stop
        setIsPlaying(false);
      }
    }
  };

  // Effect to handle autoplay when new videos load
  useEffect(() => {
    if (streams.length === 0) return;

    // Apply current playback speed to all videos
    videoRefs.current.forEach((video) => {
      if (video) video.playbackRate = playbackSpeed;
    });

    // If autoplay is flagged (from automatic batch change)
    if (shouldAutoplayRef.current) {
      // Use timeout to ensure videos are properly loaded
      const timer = setTimeout(() => {
        videoRefs.current.forEach((video) => {
          if (video) {
            video.currentTime = 0;
            video
              .play()
              .catch((err) => console.error("Failed to autoplay:", err));
          }
        });
        setIsPlaying(true);
        shouldAutoplayRef.current = false;
      }, 300);

      return () => clearTimeout(timer);
    }
  }, [streams, playbackSpeed]);

  useEffect(() => {
    const handleKeyPress = (event) => {
      if (event.key === " ") {
        event.preventDefault();
        handlePlayStop();
      }

      if (event.key === "ArrowLeft") {
        event.preventDefault();
        handleRewind(1);
      }

      if (event.key === "ArrowRight") {
        event.preventDefault();
        handleFastForward(1);
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

  const handleLabelClick = (labelId) => {
    const label = labels.find((l) => l.id === labelId);
    if (!label) return;

    const video = videoRefs.current[0];
    if (!video) return;

    const time = video.currentTime;

    if (label.type === "point") {
      // Handle "point" type labels
      sendLabelData(labelId, time, time);
    } else {
      // Handle "range" type labels
      labelsManager.handleLabelClick(labelId, videoRefs);
    }
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
                      onTimeUpdate={handleTimeUpdate}
                      onEnded={() => handleVideoEnd(index)}
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
            onChange={(e) => handleSeek(parseFloat(e.target.value))}
          />
        </div>

        <div className="row">
          <div className="col-12">
            <div className="pagination d-flex align-items-center justify-content-between">
              <button
                className="btn btn-primary pagination-button"
                onClick={() => handleBatchChange(currentBatch - 1)}
                disabled={currentBatch <= 1}
              >
                Previous
              </button>
              <div className="time-display text-center">
                {formatTime(currentTime)} /{" "}
                {formatTime(isNaN(duration) ? 0 : duration)}
              </div>
              <div className="controls d-inline">
                <button
                  className="btn btn-primary seek-btn mx-1"
                  onClick={() => handleRewind(5)}
                >
                  <span className="prev-span">-5s</span>
                  <i className="fas fa-backward"></i>
                </button>
                <button
                  className="btn btn-primary seek-btn mx-1"
                  onClick={() => handleRewind(1)}
                >
                  <span className="prev-span">-1s</span>
                  <i className="fas fa-backward"></i>
                </button>
                <button
                  className="btn btn-primary play-stop-btn mx-1"
                  onClick={handlePlayStop}
                >
                  <i className={`fas ${isPlaying ? "fa-stop" : "fa-play"}`}></i>
                </button>
                <button
                  className="btn btn-primary seek-btn mx-1"
                  onClick={() => handleFastForward(1)}
                >
                  <i className="fas fa-forward"></i>
                  <span className="next-span">+1s</span>
                </button>
                <button
                  className="btn btn-primary seek-btn mx-1"
                  onClick={() => handleFastForward(5)}
                >
                  <i className="fas fa-forward"></i>
                  <span className="next-span">+5s</span>
                </button>
              </div>
              <select
                className="form-select w-auto mx-3"
                id="playbackSpeedSelect"
                onChange={(e) =>
                  handlePlaybackSpeedChange(parseFloat(e.target.value))
                }
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
              <button
                className="btn btn-primary pagination-button"
                onClick={() => handleBatchChange(currentBatch + 1)}
                disabled={currentBatch === Object.keys(videoPositions).length}
              >
                Next
                {timeLeft > 0 &&
                  timeLeft < 6 &&
                  currentBatch !== Object.keys(videoPositions).length &&
                  ` (${timeLeft}s)`}
              </button>
            </div>
          </div>
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
                  {label.type === "point"
                    ? "ADD POINT"
                    : measuring
                    ? "STOP"
                    : "START"}
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
              <DataTable
                columns={labelColumns}
                data={assignedLabels}
                navigateButton={(label) => {
                  const matchingLabel = labels.find(
                    (l) => l.id === label.labelId
                  );
                  return (
                    <DeleteButton
                      onClick={() => handleDelete(label.id)}
                      itemType={`assigned label for "${
                        matchingLabel ? matchingLabel.name : "Unknown"
                      }"`}
                    />
                  );
                }}
              />
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
