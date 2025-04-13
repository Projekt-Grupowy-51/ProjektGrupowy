import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import httpClient from '../httpClient';
import "./css/ScientistProjects.css";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";

function ProjectAdd() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: "",
        description: "",
        finished: false
    });
    const [loading, setLoading] = useState(false);
    const { addNotification } = useNotification();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await httpClient.post("/Project", formData);

            if (response.status === 201) {
                addNotification("Project added successfully!", "success");
                navigate("/projects");
            }
        } catch (error) {
            addNotification(error.response?.data?.message || "Failed to add project", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    return (
        <div className="container py-4">
            <div className="row justify-content-center">
                <div className="col-lg-8">
                    <div className="card shadow-sm">
                        <div className="card-header bg-primary text-white">
                            <h1 className="heading mb-0">Add New Project</h1>
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

                                <div className="d-flex">
                                    <button
                                        type="submit"
                                        className="btn btn-primary me-2"
                                        disabled={loading}
                                    >
                                        <i className="fas fa-plus-circle me-2"></i>
                                        {loading ? "Creating..." : "Create Project"}
                                    </button>
                                    <NavigateButton actionType="Back" value="Cancel"/>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProjectAdd;
