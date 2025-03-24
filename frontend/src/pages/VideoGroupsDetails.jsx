import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const VideoGroupDetails = () => {
    const { id } = useParams();
    const [videoGroupDetails, setVideoGroupDetails] = useState(null);
    const [videos, setVideos] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    // Fetch video group details
    async function fetchVideoGroupDetails() {
        setLoading(true);
        setError('');
        try {
            const response = await httpClient.get(`/videogroup/${id}`);
            setVideoGroupDetails(response.data);
            fetchVideos();
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to fetch video group details');
            setLoading(false);
        }
    }

    // Fetch the list of videos
    async function fetchVideos() {
        try {
            const response = await httpClient.get(`/VideoGroup/${id}/videos`);
            setVideos(response.data);
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to fetch videos');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        if (id) fetchVideoGroupDetails();
    }, [id]);

    // Redirect to "Add Video" form
    function addVideo() {
        navigate(`/videos/add?videogroupId=${id}`);
    }

    // Delete a video
    async function deleteVideo(videoId) {
        if (!window.confirm('Are you sure you want to delete this video?')) return;

        try {
            await httpClient.delete(`/video/${videoId}`);
            setVideos(videos.filter((video) => video.id !== videoId));
            alert('Video deleted successfully');
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to delete video');
        }
    }

    if (loading) return (
        <div className="container">
            <div className="loading">Loading video group details...</div>
        </div>
    );

    if (error) return (
        <div className="container">
            <div className="error">{error}</div>
            <button className="btn btn-primary" onClick={() => navigate('/projects')}>
                Back to Projects
            </button>
        </div>
    );

    if (!videoGroupDetails) return null;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">{videoGroupDetails.name}</h1>

                <div className="actions-row">
                    <button className="btn add-btn" onClick={addVideo}>
                        Add Video
                    </button>
                    <Link to={`/projects/${videoGroupDetails.projectId}`} className="btn back-btn">
                        Back to Project
                    </Link>
                </div>

                {/* Videos Table */}
                <div className="table-container">
                    <h2>Videos</h2>
                    {videos.length > 0 ? (
                        <table className="normal-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Title</th>
                                    <th>Position</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {videos.map((video) => (
                                    <tr key={video.id}>
                                        <td>{video.id}</td>
                                        <td>{video.title}</td>
                                        <td>{video.positionInQueue}</td>
                                        <td>
                                            <div className="action-buttons">
                                                <button
                                                    className="btn btn-info"
                                                    onClick={() => navigate(`/videos/${video.id}`)}
                                                >
                                                    View
                                                </button>
                                                <button
                                                    className="btn btn-danger"
                                                    onClick={() => deleteVideo(video.id)}
                                                >
                                                    Delete
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>No videos found in this group. Add some videos to get started.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default VideoGroupDetails;
