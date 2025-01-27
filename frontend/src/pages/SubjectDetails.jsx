import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import './css/ScientistProjects.css';

const SubjectDetails = () => {
    const { id } = useParams(); // Get `id` from URL
    const [subjectDetails, setSubjectDetails] = useState(null);
    const [labels, setLabels] = useState([]);
    const navigate = useNavigate();

    // Fetch subject details
    async function fetchSubjectDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/subject/${id}`);
            if (!response.ok) {
                throw new Error('Failed to fetch subject details');
            }
            const data = await response.json();
            setSubjectDetails(data);
            fetchLabels(); // Fetch labels associated with the subject
        } catch (error) {
            console.error('Error fetching subject details:', error);
        }
    }

    // Fetch the list of labels
    async function fetchLabels() {
        try {
            const response = await fetch('http://localhost:5000/api/label');
            if (!response.ok) {
                throw new Error('Failed to fetch labels');
            }
            const data = await response.json();
            setLabels(data.filter((label) => label.subjectId === parseInt(id))); // Filter labels by subject ID
        } catch (error) {
            console.error('Error fetching labels:', error);
        }
    }

    // Fetch subject details when component is mounted
    useEffect(() => {
        if (id) fetchSubjectDetails();
    }, [id]);

    // Redirect to "Add Label" form
    function addLabel() {
        navigate(`/labels/add?subjectId=${id}`); // Pass `subjectId` as a query parameter
    }

    // Delete a label
    async function deleteLabel(labelId) {
        try {
            const response = await fetch(`http://localhost:5000/api/label/${labelId}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                setLabels(labels.filter((label) => label.id !== labelId));
                console.log('Deleted label:', labelId);
            } else {
                console.error('Error while deleting label:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting label:', error);
        }
    }

    // Check if subjectDetails exists before rendering
    if (!subjectDetails) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">{subjectDetails.name}</h1>
                <div className="details">
                    {/*<p><strong>ID:</strong> {subjectDetails.id}</p>*/}
                    {/*<p><strong>Name:</strong> {subjectDetails.name}</p>*/}
                    <p><strong>Description:</strong> {subjectDetails.description}</p>
                    <p><strong>Scientist:</strong> {subjectDetails.scientist}</p>
                </div>
                <button className="add-btn" onClick={addLabel}>Add new label</button>
                <button className="back-btn">
                    <Link to={`/projects/${subjectDetails.projectId}`} onClick={() => setActiveTab('subjects')}>Back to Project</Link>
                </button>

                <h2>Labels</h2>
                {labels.length > 0 ? (
                    <table className="project-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Description</th>
                                <th>Subject ID</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {labels.map((label) => (
                                <tr key={label.id}>
                                    <td>{label.id}</td>
                                    <td>{label.name}</td>
                                    <td>{label.description}</td>
                                    <td>{label.subjectId}</td>
                                    <td>
                                        <button
                                            className="details-btn"
                                            onClick={() => navigate(`/labels/${label.id}`)}
                                        >
                                            Details
                                        </button>
                                        <button className="delete-btn" onClick={() => deleteLabel(label.id)}>
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p>No labels associated with this subject.</p>
                )}
            </div>
        </div>
    );
};

export default SubjectDetails;
