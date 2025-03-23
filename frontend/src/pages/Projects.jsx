import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const Projects = () => {
    const [projects, setProjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const effectRan = useRef(false);

    const fetchProjects = async () => {
        try {
            const response = await httpClient.get('/Project');
            setProjects(response.data);
            setError('');
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to load projects');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this project?')) return;

        try {
            await httpClient.delete(`/Project/${id}`);
            setProjects(prev => prev.filter(project => project.id !== id));
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to delete project');
        }
    };

    useEffect(() => {
        fetchProjects();
    }, []);

    return (
        <div className="container">
            <div className="content">
                <div className="d-flex justify-content-between align-items-center mb-4">
                    <h1 className="heading">Projects</h1>
                    <button 
                        className="btn btn-primary" 
                        onClick={() => navigate('/projects/add')}
                    >
                        <i className="fas fa-plus-circle me-2"></i>Add New Project
                    </button>
                </div>

                {error && <div className="error mb-4">{error}</div>}

                {loading ? (
                    <div className="text-center py-5">
                        <div className="spinner-border text-primary" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                        <p className="mt-3">Loading projects...</p>
                    </div>
                ) : projects.length > 0 ? (
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
                            {projects.map((project) => (
                                <tr key={project.id}>
                                    <td>{project.id}</td>
                                    <td>{project.name}</td>
                                    <td>{project.description}</td>
                                    <td>
                                        <div className="btn-group">
                                            <button 
                                                className="btn btn-info" 
                                                onClick={() => navigate(`/projects/${project.id}`)}
                                            >
                                                <i className="fas fa-eye me-1"></i>Details
                                            </button>
                                            <button 
                                                className="btn btn-danger" 
                                                onClick={() => handleDelete(project.id)}
                                            >
                                                <i className="fas fa-trash me-1"></i>Delete
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <div className="alert alert-info text-center">
                        <i className="fas fa-info-circle me-2"></i>No projects found
                    </div>
                )}
            </div>
        </div>
    );
};

export default Projects;