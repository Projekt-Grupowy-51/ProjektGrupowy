import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import './css/ScientistProjects.css';

const SubjectVideoGroupAssignmentDetails = () => {
    const { id } = useParams(); // Get `id` from URL
    const [assignmentDetails, setAssignmentDetails] = useState(null);
    const [subject, setSubject] = useState(null);
    const [videoGroup, setVideoGroup] = useState(null);
    const navigate = useNavigate();

    // Fetch assignment details
    async function fetchAssignmentDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/SubjectVideoGroupAssignment/${id}`);
            if (!response.ok) {
                throw new Error('Failed to fetch assignment details');
            }
            const data = await response.json();
            setAssignmentDetails(data);
            fetchSubject(data.subjectId); // Fetch subject details associated with the assignment
            fetchVideoGroup(data.videoGroupId); // Fetch video group details associated with the assignment
        } catch (error) {
            console.error('Error fetching assignment details:', error);
        }
    }

    // Fetch subject details
    async function fetchSubject(subjectId) {
        try {
            const response = await fetch(`http://localhost:5000/api/subject/${subjectId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch subject details');
            }
            const data = await response.json();
            setSubject(data);
        } catch (error) {
            console.error('Error fetching subject details:', error);
        }
    }

    // Fetch video group details
    async function fetchVideoGroup(videoGroupId) {
        try {
            const response = await fetch(`http://localhost:5000/api/videogroup/${videoGroupId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch video group details');
            }
            const data = await response.json();
            console.log(data);

            setVideoGroup(data);
        } catch (error) {
            console.error('Error fetching video group details:', error);
        }
    }

    // Fetch assignment details when component is mounted
    useEffect(() => {
        if (id) fetchAssignmentDetails();
    }, [id]);

    // Redirect to "Edit Assignment" form
    function editAssignment() {
        navigate(`/assignments/edit/${id}`); // Navigate to the edit form
    }

    // Delete assignment
    async function deleteAssignment() {
        try {
            const response = await fetch(`http://localhost:5000/api/SubjectVideoGroupAssignment/${id}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                console.log('Assignment deleted:', id);
                navigate('/assignments'); // Redirect back to the assignments list
            } else {
                console.error('Error while deleting assignment:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting assignment:', error);
        }
    }

    // Check if assignmentDetails exists before rendering
    if (!assignmentDetails) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Subject Video Group Assignment Details</h1>

                <div className="tables-container">
                    {/* Assignment Details Table */}
                    <div className="table-wrapper">
                        <h2>Assignment Details</h2>
                        <table className="details-table">
                            <tbody>
                                <tr>
                                    <th>ID</th>
                                    <td>{assignmentDetails.id}</td>
                                </tr>
                                <tr>
                                    <th>Subject ID</th>
                                    <td>{assignmentDetails.subjectId}</td>
                                </tr>
                                <tr>
                                    <th>Video Group ID</th>
                                    <td>{assignmentDetails.videoGroupId}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    {/* Subject Details Table */}
                    <div className="table-wrapper">
                        <h2>Subject Details</h2>
                        {subject ? (
                            <table className="details-table">
                                <tbody>
                                    <tr>
                                        <th>ID</th>
                                        <td>{subject.id}</td>
                                    </tr>
                                    <tr>
                                        <th>Name</th>
                                        <td>{subject.name}</td>
                                    </tr>
                                    <tr>
                                        <th>Description</th>
                                        <td>{subject.description}</td>
                                    </tr>
                                </tbody>
                            </table>
                        ) : (
                            <p>Loading subject details...</p>
                        )}
                    </div>

                    {/* Video Group Details Table */}
                    <div className="table-wrapper">
                        <h2>Video Group Details</h2>
                        {videoGroup ? (
                            <table className="details-table">
                                <tbody>
                                    <tr>
                                        <th>ID</th>
                                        <td>{videoGroup.id}</td>
                                    </tr>
                                    <tr>
                                        <th>Name</th>
                                        <td>{videoGroup.name}</td>
                                    </tr>
                                    <tr>
                                        <th>Description</th>
                                        <td>{videoGroup.description}</td>
                                    </tr>
                                </tbody>
                            </table>
                        ) : (
                            <p>Loading video group details...</p>
                        )}
                    </div>

                    
                </div>

                <div className="tables-container pull-left">
                    <div className="table-wrapper">
                        <h2>Labelers</h2>
                        <table className="details-table">
                            <tbody>
                                <tr>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Surname</th>
                                    <th>Description</th>
                                    <th>Index</th>
                                    <th>Description</th>
                                    <th>Description</th>
                                    <th>Description</th>
                                </tr>
                                <tr>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                    <td>testowy uzytkownik</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div className="tables-container pull-left">
                    <div className="table-wrapper">
                        <h2>AsisgnedLabels</h2>
                        <table className="details-table">
                            <tbody>
                                <tr>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Description</th>
                                    <th>Color</th>
                                    <th>Start</th>
                                    <th>End</th>
                                    <th>Labeler</th>
                                </tr>
                                <tr>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                    <td>testowa etykieta</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div className="actions">
                    {videoGroup ? (
                        <button className="back-btn">
                            <Link to={`/projects/${videoGroup.projectId}`}>Back to Project</Link>
                        </button>
                    ) : (
                        <button className="back-btn">
                            <Link to="/projects">Back to Projects</Link>
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default SubjectVideoGroupAssignmentDetails;
