import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const LabelEdit = () => {
    const { id } = useParams();
    const [labelData, setLabelData] = useState({
        name: '',
        colorHex: '#ffffff',
        type: 'range',
        shortcut: '',
        subjectId: null,
    });
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const fetchLabelData = async () => {
        try {
            const response = await httpClient.get(`/Label/${id}`);
            setLabelData(response.data);
        } catch (error) {
            console.error('Error fetching label data:', error);
            alert(error.response?.data?.message || 'Failed to load label data');
        }
    };

    useEffect(() => {
        if (id) fetchLabelData();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setLabelData((prev) => ({ ...prev, [name]: value }));
    };

    const validateForm = () => {
        if (labelData.shortcut.length !== 1) {
            setError('Shortcut must be exactly one character.');
            return false;
        }
        if (!/^#[0-9A-Fa-f]{6}$/.test(labelData.colorHex)) {
            setError('Color must be in the format #ffffff.');
            return false;
        }
        setError('');
        return true;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;

        try {
            await httpClient.put(`/Label/${id}`, labelData);
            alert('Label updated successfully');
            navigate(`/subjects/${labelData.subjectId}`);
        } catch (error) {
            console.error('Error updating label:', error);
            alert(error.response?.data?.message || 'Failed to update label');
        }
    };

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Edit Label</h1>
                {error && <div className="error">{error}</div>}
                
                <form onSubmit={handleSubmit} className="auth-form">
                    <div className="form-group">
                        <label htmlFor="name">Name</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={labelData.name}
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
                                value={labelData.colorHex}
                                onChange={handleChange}
                                className="color-input"
                            />
                            <div className="color-preview" style={{
                                backgroundColor: labelData.colorHex,
                                width: '30px',
                                height: '30px',
                                border: '1px solid #ccc',
                                borderRadius: '4px',
                                marginLeft: '10px',
                                marginRight: '10px'
                            }}></div>
                            <span className="color-hex-value">{labelData.colorHex}</span>
                        </div>
                    </div>
                    
                    <div className="form-group">
                        <label htmlFor="type">Type</label>
                        <select
                            id="type"
                            name="type"
                            value={labelData.type}
                            onChange={handleChange}
                            className="form-select"
                            required
                        >
                            <option value="range">Range</option>
                            <option value="point">Point</option>
                        </select>
                    </div>
                    
                    <div className="form-group">
                        <label htmlFor="shortcut">Shortcut</label>
                        <input
                            type="text"
                            id="shortcut"
                            name="shortcut"
                            value={labelData.shortcut}
                            onChange={handleChange}
                            className="form-control"
                            maxLength="1"
                            required
                        />
                    </div>
                    
                    <div className="button-group">
                        <button type="submit" className="btn btn-primary">
                            Update Label
                        </button>
                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={() => navigate(`/subjects/${labelData.subjectId}`)}
                        >
                            Back to Subject
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
            `}</style>
        </div>
    );
};

export default LabelEdit;