import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom';
import './css/ScientistProjects.css';

const VideoGroupsDetails = () => {
    const [videoGroups, setVideoGroups] = useState([]);
    const navigate = useNavigate();

    // Pobiera listê grup video z API
    async function fetchVideoGroups() {
        try {
            const response = await fetch('http://localhost:5000/api/VideoGroup');
            const data = await response.json();
            setVideoGroups(data);
        } catch (error) {
            console.error('Error fetching video groups:', error);
        }
    }

    useEffect(() => {
        fetchVideoGroups();
    }, []);

    // Dodawanie nowej grupy video
    function addVideoGroup() {
        console.log('Add new video group');
        // Implementacja logiki dodawania projektu
    }

    // Usuwanie grupy video
    async function deleteVideoGroup(id) {
        try {
            const response = await fetch(`http://localhost:5000/api/VideoGroup/${id}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                setVideoGroups(videoGroups.filter((videoGroups) => videoGroup.id !== id));
                console.log('Deleted video group:', id);
            } else {
                console.error('Error while deleting video group:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting video group:', error);
        }
    }

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Video Groups List</h1>
                <button className="add-btn" onClick={addVideoGroup}>Add new video group</button>

                <table className="project-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Project ID</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {videoGroups.map((videoGroup) => (
                            <tr key={videoGroup.id}>
                                <td>{videoGroup.id}</td>
                                <td>{videoGroup.name}</td>
                                <td>{videoGroup.description}</td>
                                <td>{videoGroup.projectId}</td>
                                <td>
                                    <button className="details-btn" onClick={() => navigate(`/VideoGroup/${videoGroup.id}`)}>Details</button>
                                    <button className="delete-btn" onClick={() => deleteVideoGroup(videoGroup.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default VideoGroupsDetails;