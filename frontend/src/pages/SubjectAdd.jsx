import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const AddSubject = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [subjectData, setSubjectData] = useState({
        name: '',
        description: '',
        projectId: new URLSearchParams(location.search).get('projectId'),
    });
    const [error, setError] = useState('');

    const handleChange = (e) => {
        const { name, value } = e.target;
        setSubjectData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        if (!subjectData.name.trim() || !subjectData.description.trim()) {
            setError('All fields are required');
            setLoading(false);
            return;
        }

        try {
            await httpClient.post('/Subject', subjectData);
            navigate(`/projects/${subjectData.projectId}`, {
                state: { successMessage: "Subject added successfully!" }
            });
        } catch (error) {
            console.error('Error adding subject:', error);
            setError(error.response?.data?.message || 'Failed to add subject');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container py-4">
            <div className="row justify-content-center">
                <div className="col-lg-8">
                    <div className="card shadow-sm">
                        <div className="card-header bg-primary text-white">
                            <h1 className="heading mb-0">Add New Subject</h1>
                        </div>
                        <div className="card-body">
                            {error && <div className="alert alert-danger mb-4">{error}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label htmlFor="name" className="form-label">Subject Name</label>
                                    <input
                                        type="text"
                                        id="name"
                                        name="name"
                                        className="form-control"
                                        value={subjectData.name}
                                        onChange={handleChange}
                                        required
                                    />
                                </div>

                                <div className="mb-4">
                                    <label htmlFor="description" className="form-label">Description</label>
                                    <textarea
                                        id="description"
                                        name="description"
                                        className="form-control"
                                        value={subjectData.description}
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
                                        {loading ? "Adding..." : "Add Subject"}
                                    </button>
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        onClick={() => navigate(`/projects/${subjectData.projectId}`)}
                                        disabled={loading}
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
};

export default AddSubject;