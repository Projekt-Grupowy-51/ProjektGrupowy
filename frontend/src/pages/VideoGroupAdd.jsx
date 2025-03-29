import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const VideoGroupAdd = () => {
    const [formData, setFormData] = useState({
        name: '',
        description: '',
        projectId: null
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        // Extract projectId from query params if available
        const queryParams = new URLSearchParams(location.search);
        const projectId = queryParams.get('projectId');
        if (projectId) {
            setFormData(prev => ({ ...prev, projectId: parseInt(projectId) }));
        }
    }, [location.search]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        // Validate form
        if (!formData.name || !formData.description || !formData.projectId) {
            setError('Please fill in all required fields.');
            setLoading(false);
            return;
        }

        try {
            await httpClient.post('/videogroup', formData);
            navigate(`/projects/${formData.projectId}`);
        } catch (err) {
            setError(err.response?.data?.message || 'An error occurred. Please try again.');
            setLoading(false);
        }
    };

    return (
        <div className="container">
            <div className="content">
                <h1>Add New Video Group</h1>
                {error && <div className="error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="name">Name</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            className="form-control"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="description">Description</label>
                        <textarea
                            id="description"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            className="form-control"
                            rows="5"
                            required
                        ></textarea>
                    </div>

                    <div className="button-group">
                        <button 
                            type="submit" 
                            className="btn btn-primary"
                            disabled={loading}
                        >
                            {loading ? 'Adding...' : 'Add Video Group'}
                        </button>
                        <button 
                            type="button" 
                            className="btn btn-secondary"
                            onClick={() => navigate(`/projects/${formData.projectId}`)}
                        >
                            Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default VideoGroupAdd;
