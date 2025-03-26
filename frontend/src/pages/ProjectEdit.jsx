import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import httpClient from '../httpClient';
import "./css/ScientistProjects.css";

function ProjectEdit() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: "",
        description: "",
        scientistId: "",
        finished: false
    });
    const [error, setError] = useState("");

    useEffect(() => {
        const fetchProject = async () => {
            try {
                const response = await httpClient.get(`/Project/${id}`);
                const data = response.data;
                setFormData({
                    name: data.name,
                    description: data.description,
                    scientistId: data.scientistId.toString(),
                    finished: data.finished
                });
            } catch (error) {
                console.error("Error fetching project:", error);
                setError(error.response?.data?.message || "Failed to load project data");
            }
        };
        fetchProject();
    }, [id]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        if (!Number.isInteger(Number(formData.scientistId))) {
            setError("Scientist ID must be a valid number");
            return;
        }

        try {
            await httpClient.put(`/Project/${id}`, {
                ...formData,
                scientistId: parseInt(formData.scientistId)
            });
            alert("Project updated successfully!");
            navigate(`/projects/${id}`);
        } catch (error) {
            console.error("Error updating project:", error);
            setError(error.response?.data?.message || "Failed to update project");
        }
    };

    const handleChange = (e) => {
        const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
        setFormData({
            ...formData,
            [e.target.name]: value
        });
    };

    return (
        <div className="container">
            <h1 className="heading">Edit Project</h1>
            {error && <div className="error">{error}</div>}

            <form onSubmit={handleSubmit} className="auth-form">
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
                    <label>Status:</label>
                    <select
                        name="finished"
                        className="form-select"
                        value={formData.finished}
                        onChange={handleChange}
                    >
                        <option value={false}>Active</option>
                        <option value={true}>Completed</option>
                    </select>
                </div>

                <div className="button-group">
                    <button type="submit" className="auth-btn edit-btn">
                        Save Changes
                    </button>
                    <button
                        type="button"
                        className="auth-btn back-btn"
                        onClick={() => navigate(`/projects/${id}`)}
                    >
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
}

export default ProjectEdit;