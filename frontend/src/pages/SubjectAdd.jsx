import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const AddSubject = () => {
    const location = useLocation();
    const navigate = useNavigate();
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

        if (!subjectData.name.trim() || !subjectData.description.trim()) {
            setError('All fields are required');
            return;
        }

        try {
            await httpClient.post('/Subject', subjectData);
            alert('Subject added successfully');
            navigate(`/projects/${subjectData.projectId}`);
        } catch (error) {
            console.error('Error adding subject:', error);
            setError(error.response?.data?.message || 'Failed to add subject');
        }
    };

    return (
        <div className="container auth-container">
            <h1 className="heading">Add New Subject</h1>
            {error && <div className="error">{error}</div>}

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="form-group">
                    <label>Subject Name:</label>
                    <input
                        type="text"
                        name="name"
                        className="form-input"
                        value={subjectData.name}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="form-group">
                    <label>Description:</label>
                    <textarea
                        name="description"
                        className="form-input"
                        value={subjectData.description}
                        onChange={handleChange}
                        required
                        rows="4"
                    />
                </div>

                <div className="button-group">
                    <button type="submit" className="auth-btn add-btn">
                        Add Subject
                    </button>
                    <button
                        type="button"
                        className="auth-btn back-btn"
                        onClick={() => navigate(-1)}
                    >
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AddSubject;