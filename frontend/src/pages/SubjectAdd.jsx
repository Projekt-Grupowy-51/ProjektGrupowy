import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './css/ScientistProjects.css';


const AddSubject = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [subjectData, setSubjectData] = useState({
        name: '',
        description: '',
        projectId: new URLSearchParams(location.search).get('projectId'), // Get projectId from URL
    });

    // Handle form input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        setSubjectData((prevState) => ({
            ...prevState,
            [name]: value,
        }));
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`http://localhost:5000/api/Subject`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(subjectData),
            });

            if (response.ok) {
                alert('Subject added successfully');
                navigate(`/projects/${subjectData.projectId}`); // Redirect to the project details page
            } else {
                alert('Failed to add subject');
            }
        } catch (error) {
            console.error('Error adding subject:', error);
        }
    };

    return (
        <div className="container">
            <h1>Add Subject</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="name">Name</label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        value={subjectData.name}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        name="description"
                        value={subjectData.description}
                        onChange={handleChange}
                        required
                    ></textarea>
                </div>
                <button type="submit">Add Subject</button>
            </form>
        </div>
    );
};

export default AddSubject;
