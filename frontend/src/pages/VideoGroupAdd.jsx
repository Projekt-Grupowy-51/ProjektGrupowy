import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const AddVideoGroup = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [videoGroupData, setVideoGroupData] = useState({
        name: '',
        description: '',
        projectId: new URLSearchParams(location.search).get('projectId'), // Pobranie projectId z URL
    });
    const [error, setError] = useState("");

    // Obs³uga zmiany wartoœci w formularzu
    const handleChange = (e) => {
        const { name, value } = e.target;
        setVideoGroupData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Obs³uga przesy³ania formularza
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        try {
            const response = await httpClient.post("/videogroup", videoGroupData);

            if (response.status === 201) {
                alert("Video Group added successfully");
                navigate(`/projects/${videoGroupData.projectId}`); // Przekierowanie na stronê projektu
            }
        } catch (error) {
            console.error("Error adding video group:", error);
            setError(error.response?.data?.message || "Failed to add video group");
        }
    };

    return (
        <div className="container">
            <h1>Add Video Group</h1>
            {error && <div className="error">{error}</div>}
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="name">Title</label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        value={videoGroupData.name}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        name="description"
                        value={videoGroupData.description}
                        onChange={handleChange}
                        required
                    ></textarea>
                </div>
                <button type="submit">Add Video Group</button>
            </form>
            <button className="back-btn" onClick={() => navigate(`/projects/${videoGroupData.projectId}`)}>
                Back to Project
            </button>
        </div>
    );
};

export default AddVideoGroup;
