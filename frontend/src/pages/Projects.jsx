import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const Projects = () => {
    const [projects, setProjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const navigate = useNavigate();

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
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                    <h1 className="heading">Projects</h1>
                    <button className="btn add-btn text-white" onClick={() => navigate('/projects/add')}>Add New Project</button>
                </div>

                {error && <div className="error">{error}</div>}

                {loading ? (
                    <div style={{ padding: '20px', textAlign: 'center' }}>Loading projects...</div>
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
                                        <button className="btn btn-info" onClick={() => navigate(`/projects/${project.id}`)}>Details</button>
                                        <button className="btn btn-danger" onClick={() => handleDelete(project.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p className="error">No projects found</p>
                )}
            </div>
        </div>
    );
};

export default Projects;