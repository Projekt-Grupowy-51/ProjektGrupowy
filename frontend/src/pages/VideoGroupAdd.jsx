import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './css/ScientistProjects.css';


const AddVideoGroup = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [videoGroupData, setVideoGroupData] = useState({
        name: '',
        description: '',
        projectId: new URLSearchParams(location.search).get('projectId'), // Get projectId from URL
    });

    // Handle form input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        setVideoGroupData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`http://localhost:5000/api/videogroup`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(videoGroupData),
            });

            if (response.ok) {
                alert('Video Group added successfully');
                navigate(`/projects/${videoGroupData.projectId}`); // Redirect to the project details page
            } else {
                alert('Failed to add video group');
            }
        } catch (error) {
            console.error('Error adding video group:', error);
        }
    };

    return (
        <div className="container">
            <h1>Add Video Group</h1>
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
