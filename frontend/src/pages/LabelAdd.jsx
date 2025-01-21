import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './css/ScientistProjects.css';


const AddLabel = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [labelData, setLabelData] = useState({
        name: '',
        description: '',
        subjectId: new URLSearchParams(location.search).get('subjectId'), // Get subjectId from query params
    });

    // Handle form input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        setLabelData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`http://localhost:5000/api/Label`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(labelData),
            });

            if (response.ok) {
                alert('Label added successfully');
                navigate(`/subjects/details/${labelData.subjectId}`); // Redirect to the subject details page
            } else {
                alert('Failed to add label');
            }
        } catch (error) {
            console.error('Error adding label:', error);
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
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        name="description"
                        value={labelData.description}
                        onChange={handleChange}
                        required
                    ></textarea>
                </div>
                <button type="submit">Add Label</button>
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

export default AddLabel;
