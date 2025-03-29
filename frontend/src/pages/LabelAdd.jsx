import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const LabelAdd = () => {
    const [formData, setFormData] = useState({
        name: '',
        colorHex: '#3498db', // More pleasing default blue color
        type: 'range',
        shortcut: '',
        subjectId: null
    });
    const [subjectName, setSubjectName] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        // Extract subjectId from query params
        const queryParams = new URLSearchParams(location.search);
        const subjectId = queryParams.get('subjectId');
        
        if (!subjectId) {
            setError('Subject ID is required. Please go back and try again.');
            return;
        }
        
        setFormData(prev => ({ ...prev, subjectId: parseInt(subjectId) }));
        fetchSubjectName(parseInt(subjectId));
    }, [location.search]);

    const fetchSubjectName = async (id) => {
        try {
            const response = await httpClient.get(`/subject/${id}`);
            setSubjectName(response.data.name);
        } catch (err) {
            setError('Failed to load subject information.');
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        if (!formData.name || !formData.colorHex || !formData.type || !formData.subjectId) {
            setError('Please fill in all required fields.');
            setLoading(false);
            return;
        }

        try {
            await httpClient.post('/label', formData);
            navigate(`/subjects/${formData.subjectId}`);
        } catch (err) {
            setError(err.response?.data?.message || 'An error occurred. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    if (!formData.subjectId) {
        return (
            <div className="container">
                <div className="content">
                    <div className="error">{error}</div>
                    <button 
                        className="btn btn-secondary"
                        onClick={() => navigate('/subjects')}
                    >
                        Back to Subjects
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="container">
            <div className="content">
                <h1>Add New Label</h1>
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
                        <label htmlFor="colorHex">Color</label>
                        <div className="color-selector">
                            <input
                                type="color"
                                id="colorHex"
                                name="colorHex"
                                value={formData.colorHex}
                                onChange={handleChange}
                                className="color-input"
                            />
                            <div className="color-preview" style={{
                                backgroundColor: formData.colorHex,
                                width: '30px',
                                height: '30px',
                                border: '1px solid #ccc',
                                borderRadius: '4px',
                                marginLeft: '10px',
                                marginRight: '10px'
                            }}></div>
                            <span className="color-hex-value">{formData.colorHex}</span>
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="type">Type</label>
                        <select
                            id="type"
                            name="type"
                            value={formData.type}
                            onChange={handleChange}
                            className="form-control"
                            required
                        >
                            <option value="range">Range</option>
                            <option value="point">Point</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <label htmlFor="shortcut">Shortcut Key (Optional)</label>
                        <input
                            type="text"
                            id="shortcut"
                            name="shortcut"
                            value={formData.shortcut}
                            onChange={handleChange}
                            className="form-control"
                            maxLength="1"
                            placeholder="Single character (a-z, 0-9)"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="subjectId">Subject</label>
                        <div className="subject-display">
                            <input
                                type="text"
                                value={subjectName || `Subject ID: ${formData.subjectId}`}
                                className="form-control"
                                disabled
                            />
                            <input
                                type="hidden"
                                name="subjectId"
                                value={formData.subjectId}
                            />
                        </div>
                    </div>

                    <div className="button-group">
                        <button 
                            type="submit" 
                            className="btn btn-primary"
                            disabled={loading}
                        >
                            {loading ? 'Adding...' : 'Add Label'}
                        </button>
                        <button 
                            type="button" 
                            className="btn btn-secondary"
                            onClick={() => navigate(`/subjects/${formData.subjectId}`)}
                        >
                            Cancel
                        </button>
                    </div>
                </form>
            </div>

            <style jsx>{`
                .color-selector {
                    display: flex;
                    align-items: center;
                }
                
                .color-input {
                    width: 50px;
                    height: 40px;
                    padding: 0;
                    border: none;
                    background: none;
                    cursor: pointer;
                }
                
                .color-hex-value {
                    font-family: monospace;
                    font-size: 1rem;
                }
                
                .subject-display {
                    display: flex;
                    align-items: center;
                }
            `}</style>
        </div>
    );
};

export default LabelAdd;