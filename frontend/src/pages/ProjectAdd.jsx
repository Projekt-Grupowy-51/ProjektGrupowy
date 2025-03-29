import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import httpClient from '../httpClient';
import "./css/ScientistProjects.css";

function ProjectAdd() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: "",
        description: "",
        scientistId: "",
        finished: false
    });
    const [error, setError] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        // Walidacja scientistId
        if (!Number.isInteger(Number(formData.scientistId))) {
            setError("Scientist ID must be a valid number");
            return;
        }

        try {
            const response = await httpClient.post("/Project", {
                ...formData,
                scientistId: parseInt(formData.scientistId)
            });

            if (response.status === 201) {
                alert("Project added successfully!");
                navigate("/projects");
            }
        } catch (error) {
            console.error("Error adding project:", error);
            setError(error.response?.data?.message || "Failed to add project");
        }
    };

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    return (
        <div className="container auth-container">
            <h1 className="heading">Add New Project</h1>

            <form onSubmit={handleSubmit} className="auth-form">
                {error && <div className="error">{error}</div>}

                <div className="form-group">
                    <label>Project Name:</label>
                    <input
                        type="text"
                        name="name"
                        className="form-input"
                        value={formData.name}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="form-group">
                    <label>Description:</label>
                    <textarea
                        name="description"
                        className="form-input"
                        value={formData.description}
                        onChange={handleChange}
                        required
                        rows="4"
                    />
                </div>

                <div className="form-group">
                    <label>Scientist ID:</label>
                    <input
                        type="number"
                        name="scientistId"
                        className="form-input"
                        value={formData.scientistId}
                        onChange={handleChange}
                        required
                        min="1"
                    />
                </div>

                <div className="form-group">
                    <button type="submit" className="auth-btn add-btn">
                        Create Project
                    </button>

                    <button
                        type="button"
                        className="auth-btn back-btn"
                        onClick={() => navigate("/projects")}
                    >
                        Back to Projects
                    </button>
                </div>
            </form>
        </div>
    );
}

export default ProjectAdd;