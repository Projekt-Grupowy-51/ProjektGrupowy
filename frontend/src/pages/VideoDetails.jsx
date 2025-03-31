import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import httpClient from "../httpClient";
import "./css/ScientistProjects.css";

const VideoDetails = () => {
  const { id: videoId } = useParams();
  const [videoData, setVideoData] = useState(null);
  const [videoStream, setVideoStream] = useState(null);
  const [labels, setLabels] = useState([]);
  const videoRef = useRef(null);

  useEffect(() => {
    fetchVideoDetails(videoId);
    fetchVideoStream(videoId);
      //fetchAssignedLabels(videoId);

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
      const response = await httpClient.get(`/Video/${videoId}/assignedlabels`, {
        withCredentials: true,
      });
      setLabels(response.data);
    } catch (error) {
      console.error("Failed to load assigned labels:", error);
    }
  };

  if (!videoData) {
    return <div className="container text-center">Loading...</div>;
  }

  return (
    <div className="container">
      <h1 className="text-center mb-4">{videoData.title}</h1>
      <div className="video-container mb-4">
        <video
          ref={videoRef}
          className="w-100"
          controls
          src={videoStream}
          type="video/mp4"
          alt={videoData.title}
        />
      </div>
      <h3 className="mb-3">Assigned Labels</h3>
      <div className="assigned-labels-table">
        <table className="table table-striped">
          <thead>
            <tr>
              <th>#</th>
              <th>Label Name</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            {labels.map((label, index) => (
              <tr key={label.id}>
                <td>{index + 1}</td>
                <td>{label.start}</td>
                <td>{label.end}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default VideoDetails;