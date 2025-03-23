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
    const [loading, setLoading] = useState(true);

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
            } finally {
                setLoading(false);
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

    if (loading) {
        return (
            <div className="container d-flex justify-content-center align-items-center py-5">
                <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    return (
        <div className="container py-4">
            <div className="row justify-content-center">
                <div className="col-lg-8">
                    <div className="card shadow-sm">
                        <div className="card-header bg-primary text-white">
                            <h1 className="heading mb-0">Edit Project</h1>
                        </div>
                        <div className="card-body">
                            {error && <div className="alert alert-danger mb-4">{error}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label htmlFor="name" className="form-label">Project Name</label>
                                    <input
                                        type="text"
                                        id="name"
                                        name="name"
                                        className="form-control"
                                        value={formData.name}
                                        onChange={handleChange}
                                        required
                                    />
                                </div>

                                <div className="mb-3">
                                    <label htmlFor="description" className="form-label">Description</label>
                                    <textarea
                                        id="description"
                                        name="description"
                                        className="form-control"
                                        value={formData.description}
                                        onChange={handleChange}
                                        required
                                        rows="4"
                                    />
                                </div>

                                <div className="mb-3">
                                    <label htmlFor="scientistId" className="form-label">Scientist ID</label>
                                    <input
                                        type="number"
                                        id="scientistId"
                                        name="scientistId"
                                        className="form-control"
                                        value={formData.scientistId}
                                        onChange={handleChange}
                                        required
                                        min="1"
                                    />
                                </div>

                                <div className="mb-4">
                                    <label htmlFor="finished" className="form-label">Status</label>
                                    <select
                                        id="finished"
                                        name="finished"
                                        className="form-select"
                                        value={formData.finished}
                                        onChange={handleChange}
                                    >
                                        <option value={false}>Active</option>
                                        <option value={true}>Completed</option>
                                    </select>
                                </div>

                                <div className="d-flex">
                                    <button type="submit" className="btn btn-primary me-2">
                                        <i className="fas fa-save me-2"></i>Save Changes
                                    </button>
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        onClick={() => navigate(`/projects/${id}`)}
                                    >
                                        <i className="fas fa-times me-2"></i>Cancel
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProjectEdit;