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
            const filteredLabels = data.filter((label) => label.subjectId === parseInt(id)); // Filter labels by subject ID
            const sortedLabels = filteredLabels.sort((a, b) => a.id - b.id); // Sort labels by ID in ascending order
            setLabels(sortedLabels);
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
                    <p><strong>Description:</strong> {subjectDetails.description}</p>
                </div>
                <button className="add-btn" onClick={addLabel}>Add new label</button>
                <button className="back-btn">
                    <Link to={`/projects/${subjectDetails.projectId}`} onClick={() => setActiveTab('subjects')}>Back to Project</Link>
                </button>

                <h2>Labels</h2>
                {labels.length > 0 ? (
                    <table className="project-table" id="label-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Type</th>
                                <th>Color</th>  {/* New column for Color */}
                                <th>Shortcut</th>  {/* New column for Shortcut */}
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {labels.map((label) => (
                                <tr key={label.id}>
                                    <td>{label.id}</td>
                                    <td>{label.name}</td>
                                    <td>{label.type}</td>  {/* Correct display of description */}
                                    <td>
                                        <div
                                            style={{
                                                backgroundColor: label.colorHex,
                                                width: '30px',
                                                height: '30px',
                                                borderRadius: '50%',
                                            }}
                                        ></div>
                                    </td>
                                    <td>{label.shortcut}</td>
                                    <td>
                                        <button
                                            className="details-btn"
                                            onClick={() => navigate(`/labels/edit/${label.id}`)}
                                        >
                                            Edit
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
