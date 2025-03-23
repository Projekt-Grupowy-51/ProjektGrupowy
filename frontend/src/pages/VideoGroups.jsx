import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom';
import './css/ScientistProjects.css';

const VideoGroupsDetails = () => {
    const [videoGroups, setVideoGroups] = useState([]);
    const navigate = useNavigate();

    // Pobiera listę grup video z API
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
        <div className="container mt-4">
            <h1 className="text-primary">Video Groups</h1>
            <button className="btn btn-success mb-3" onClick={addVideoGroup}>
                Add Video Group
            </button>
            <table className="normal-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {videoGroups.map((group) => (
                        <tr key={group.id}>
                            <td>{group.id}</td>
                            <td>{group.name}</td>
                            <td>{group.description}</td>
                            <td>
                                <button
                                    className="btn btn-info me-2"
                                    onClick={() => navigate(`/VideoGroup/${group.id}`)}
                                >
                                    Details
                                </button>
                                <button
                                    className="btn btn-danger"
                                    onClick={() => deleteVideoGroup(group.id)}
                                >
                                    Delete
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default VideoGroupsDetails;