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
            <h1>Edit Label</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="name">Name</label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        value={labelData.name}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="colorHex">Color</label>
                    <input
                        type="color"
                        id="colorHex"
                        name="colorHex"
                        value={labelData.colorHex}
                        onChange={handleChange}
                    />
                </div>
                <div>
                    <label htmlFor="type">Type</label>
                    <input
                        type="text"
                        id="type"
                        name="type"
                        value={labelData.type}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="shortcut">Shortcut</label>
                    <input
                        type="text"
                        id="shortcut"
                        name="shortcut"
                        value={labelData.shortcut}
                        onChange={handleChange}
                        maxLength="1"
                        required
                    />
                </div>
                {error && <p className="error">{error}</p>}
                <button type="submit" className="edit-btn">
                    Update Label
                </button>
            </form>
            <button
                className="btn-back back-btn"
                onClick={() => navigate(`/subjects/details/${labelData.subjectId}`)}
            >
                Back to Subject Details
            </button>
        </div>
    );
};

export default LabelEdit;