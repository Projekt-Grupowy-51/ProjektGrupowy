import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const SubjectVideoGroupAssignmentDetails = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [assignmentDetails, setAssignmentDetails] = useState(null);
    const [subject, setSubject] = useState(null);
    const [videoGroup, setVideoGroup] = useState(null);
    const [labelers, setLabelers] = useState([]);
    const [assignedLabels, setAssignedLabels] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState({
        main: '',
        labelers: '',
        labels: ''
    });

    const fetchData = async () => {
        try {
            setLoading(true);
            setError({ main: '', labelers: '', labels: '' });

            // Pobierz podstawowe dane przypisania
            const assignmentResponse = await httpClient.get(`/SubjectVideoGroupAssignment/${id}`);
            setAssignmentDetails(assignmentResponse.data);

            // Pobierz dane subject i videoGroup r�wnolegle
            const [subjectResponse, videoGroupResponse] = await Promise.all([
                httpClient.get(`/subject/${assignmentResponse.data.subjectId}`),
                httpClient.get(`/videogroup/${assignmentResponse.data.videoGroupId}`)
            ]);

            setSubject(subjectResponse.data);
            setVideoGroup(videoGroupResponse.data);

            // Pobierz dane labelers i assignedLabels
            try {
                const [labelersResponse, labelsResponse] = await Promise.all([
                    httpClient.get(`/SubjectVideoGroupAssignment/${id}/labelers`),
                    httpClient.get(`/SubjectVideoGroupAssignment/${id}/assignedlabels`)
                ]);
                setLabelers(labelersResponse.data);
                setAssignedLabels(labelsResponse.data);
                console.log(labelsResponse.data);
            } catch (err) {
                setError(prev => ({
                    ...prev,
                    labelers: 'Error loading labelers',
                    labels: 'Error loading labels'
                }));
            }

        } catch (error) {
            setError(prev => ({ ...prev, main: 'Failed to load assignment details' }));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) fetchData();
    }, [id]);

    const handleDelete = async () => {
        if (!window.confirm('Are you sure you want to delete this assignment?')) return;

        try {
            await httpClient.delete(`/SubjectVideoGroupAssignment/${id}`);
            navigate('/assignments');
        } catch (error) {
            setError(prev => ({ ...prev, main: 'Deletion failed' }));
        }
    };

    if (loading) return <div className="container"><div className="loading">Loading...</div></div>;
    if (error.main) return <div className="container"><div className="error">{error.main}</div></div>;

    return (
        <div className="container">
            <div className="content">
                <div className="auth-header">
                    <h1 className="heading">Assignment Details</h1>
                    <div className="action-buttons">
                        <button className="edit-btn" onClick={() => navigate(`/assignments/edit/${id}`)}>
                            Edit
                        </button>
                        <button className="btn btn-danger" onClick={handleDelete}>
                            Delete
                        </button>
                        <button className="refresh-btn add-btn" onClick={fetchData}>
                            Refresh
                        </button>
                    </div>
                </div>

                {error.labelers && <div className="error">{error.labelers}</div>}
                {error.labels && <div className="error">{error.labels}</div>}

                <div className="tables-container">
                    {/* Basic Assignment Info */}
                    <div className="table-wrapper">
                        <h2>Basic Information</h2>
                        <table className="details-table">
                            <tbody>
                                <tr>
                                    <th>Assignment ID</th>
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

                    {/* Subject Details */}
                    <div className="table-wrapper">
                        <h2>Subject Details</h2>
                        {subject ? (
                            <table className="details-table">
                                <tbody>
                                    <tr>
                                        <th>Name</th>
                                        <td>{subject.name}</td>
                                    </tr>
                                    <tr>
                                        <th>Description</th>
                                        <td>{subject.description}</td>
                                    </tr>
                                    <tr>
                                        <th>Project ID</th>
                                        <td>{subject.projectId}</td>
                                    </tr>
                                </tbody>
                            </table>
                        ) : (
                            <p className="error">Error loading subject</p>
                        )}
                    </div>

                    {/* Video Group Details */}
                    <div className="table-wrapper">
                        <h2>Video Group Details</h2>
                        {videoGroup ? (
                            <table className="details-table">
                                <tbody>
                                    <tr>
                                        <th>Name</th>
                                        <td>{videoGroup.name}</td>
                                    </tr>
                                    <tr>
                                        <th>Description</th>
                                        <td>{videoGroup.description}</td>
                                    </tr>
                                    <tr>
                                        <th>Project ID</th>
                                        <td>{videoGroup.projectId}</td>
                                    </tr>
                                </tbody>
                            </table>
                        ) : (
                            <p className="error">Error loading video group</p>
                        )}
                    </div>
                </div>

                {/* Labelers Table */}
                <div className="tables-container">
                    <div className="table-wrapper">
                        <h2>Assigned Labelers</h2>
                        {labelers.length > 0 ? (
                            <table className="normal-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {labelers.map(labeler => (
                                        <tr key={labeler.id}>
                                            <td>{labeler.id}</td>
                                            <td>{labeler.name} {labeler.surname}</td>
                                            <td>{labeler.email}</td>
                                            <td>{labeler.role}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <div className="no-data-info">
                                {error.labelers ? 'Error loading labelers' : 'No labelers assigned'}
                            </div>
                        )}
                    </div>
                </div>

                {/* Assigned Labels Table */}
                <div className="tables-container">
                    <div className="table-wrapper">
                        <h2>Assigned Labels</h2>
                        {assignedLabels.length > 0 ? (
                            <table className="normal-table">
                                <thead>
                                    <tr>
                                        <th>Label ID</th>
                                        <th>Name</th>
                                        <th>Start Time</th>
                                        <th>End Time</th>
                                        <th>Labeler</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {assignedLabels.map(label => (
                                        <tr key={label.id}>
                                            <td>{label.id}</td>
                                            <td>{label.labelName}</td>
                                            <td>{label.start}</td>
                                            <td>{label.end}</td>
                                            <td>{label.labelerName}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <div className="no-data-info">
                                {error.labels ? 'Error loading labels' : 'No labels assigned'}
                            </div>
                        )}
                    </div>
                </div>

                <div className="action-buttons">
                    <button className="back-btn">
                        <Link to={videoGroup ? `/projects/${videoGroup.projectId}` : "/projects"}>
                            Back to Project
                        </Link>
                    </button>
                </div>
            </div>
        </div>
    );
};

export default SubjectVideoGroupAssignmentDetails;