import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './css/ScientistProjects.css';

const LabelEdit = () => {
    const { id } = useParams();  // Pobranie ID etykiety z URL
    const [labelData, setLabelData] = useState({
        name: '',
        colorHex: '#ffffff',
        type: '',
        shortcut: '',
        subjectId: 1,
    });
    const navigate = useNavigate();

    // Fetch the label data by ID
    async function fetchLabelData() {
        try {
            const response = await fetch(`http://localhost:5000/api/label/${id}`);
            if (!response.ok) {
                throw new Error('Failed to fetch label data');
            }
            const data = await response.json();
            setLabelData(data);
        } catch (error) {
            console.error('Error fetching label data:', error);
        }
    }

    // Fetch label data when the component mounts
    useEffect(() => {
        if (id) fetchLabelData();
    }, [id]);

    // Handle form input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        setLabelData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Handle form submission (update the label)
    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`http://localhost:5000/api/label/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(labelData),
            });

            if (response.ok) {
                alert('Label updated successfully');
                navigate(`/subjects/${labelData.subjectId}`); // Redirect to subject details page
            } else {
                alert('Failed to update label');
            }
        } catch (error) {
            console.error('Error updating label:', error);
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
                        required
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
                        maxLength="1" // Ensure the shortcut is only 1 character
                        required
                    />
                </div>
                <button type="submit">Update Label</button>
            </form>
            <button
                className="btn-back"
                onClick={() => navigate(`/subjects/details/${labelData.subjectId}`)}
            >
                Back to Subject Details
            </button>
        </div>
    );
};

export default LabelEdit;
