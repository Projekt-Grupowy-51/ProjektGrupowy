import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import httpClient from '../httpClient'; 
import './css/ScientistProjects.css';

const AddLabel = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [labelData, setLabelData] = useState({
        name: '',
        colorHex: '#ffffff',
        type: 'range',
        shortcut: '',
        subjectId: new URLSearchParams(location.search).get('subjectId'),
    });
    const [error, setError] = useState('');

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
            await httpClient.post('/Label', labelData);
            alert('Label added successfully');
            navigate(`/subjects/${labelData.subjectId}`);
        } catch (error) {
            console.error('Error adding label:', error);
            alert(error.response?.data?.message || 'Failed to add label');
        }
    };

    return (
        <div className="container">
            <h1>Add Label</h1>
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
                <button type="submit" className="add-btn">Add Label</button>
            </form>
            <button className="btn-back back-btn" onClick={() => navigate(`/subjects/details/${labelData.subjectId}`)}>
                Back to Subject Details
            </button>
        </div>
    );
};

export default AddLabel;