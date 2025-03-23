import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const ProjectDetails = () => {
    const { id } = useParams();
    const [project, setProject] = useState(null);
    const [subjects, setSubjects] = useState([]);
    const [videoGroups, setVideoGroups] = useState([]);
    const [assignments, setAssignments] = useState([]);
    const [activeTab, setActiveTab] = useState('details');
    const [error, setError] = useState('');
    const [labelers, setLabelers] = useState([]);
    const [accessCodes, setAccessCodes] = useState([]);
    const [creationError, setCreationError] = useState('');
    const [selectedDuration, setSelectedDuration] = useState('');
    const navigate = useNavigate();

    const fetchData = async () => {
        try {
            const [projectRes, subjectsRes, videoGroupsRes, assignmentsRes, labelersRes, accessCodesRes] = await Promise.all([
                httpClient.get(`/project/${id}`),
                httpClient.get(`/project/${id}/subjects`),
                httpClient.get(`/project/${id}/videogroups`),
                httpClient.get(`/project/${id}/SubjectVideoGroupAssignments`),
                httpClient.get(`/project/${id}/Labelers`),
                httpClient.get(`/AccessCode/project/${id}`)
            ]);

            setProject(projectRes.data);
            setSubjects(subjectsRes.data);
            setVideoGroups(videoGroupsRes.data);
            setAssignments(assignmentsRes.data);
            setLabelers(labelersRes.data);
            setAccessCodes(accessCodesRes.data);
        } catch (error) {
            console.error('Error fetching data:', error);
            setError(error.response?.data?.message || 'Failed to load project data');
        }
    };

    const handleDeleteItem = async (endpoint, itemId) => {
        if (!window.confirm('Are you sure you want to delete this item?')) return;

        try {
            await httpClient.delete(`/${endpoint}/${itemId}`);
            await fetchData();
            alert('Item deleted successfully');
        } catch (error) {
            console.error('Error deleting item:', error);
            setError(error.response?.data?.message || 'Deletion failed');
        }
    };

    const handleCreateAccessCode = async () => {
        setCreationError('');

        let expiresAt = null;

        if (selectedDuration === 'unlimited') {
            expiresAt = null;
        } else {
            const days = selectedDuration === '14days' ? 14 : 30;
            
            const now = new Date();
            expiresAt = new Date(Date.UTC(
                now.getUTCFullYear(),
                now.getUTCMonth(),
                now.getUTCDate() + days,
                now.getUTCHours(),
                now.getUTCMinutes(),
                now.getUTCSeconds()
            ));
            
            expiresAt = expiresAt.toISOString().split('.')[0] + 'Z';
        }

        try {
            await httpClient.post('/AccessCode/project', {
                projectId: parseInt(id),
                expiresAtUtc: expiresAt
            });

            setSelectedDuration('');
            await fetchData();
            alert('Access code created successfully!');
        } catch (error) {
            setCreationError(error.response?.data?.message || 'Failed to create code');
        }
    };

    const handleCopyCode = (code) => {
        navigator.clipboard.writeText(code)
            .then(() => {
                alert('Code copied to clipboard!');
            })
            .catch(() => {
                alert('Failed to copy code. Please try again.');
            });
    };

    const handleDistributeLabelers = async () => {
        try {
            await httpClient.post(`/project/${id}/distribute`);
            await fetchData();
            alert('Labelers distributed successfully!');
        } catch (error) {
            setError(error.response?.data?.message || 'Distribution failed');
        }
    };

    useEffect(() => {
        fetchData();
    }, [id]);

    if (!project) return <div className="container">Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1>{project.name}</h1>
                {error && <div className="error">{error}</div>}
                <div className="tab-navigation">
                    <button
                        className={`tab-button ${activeTab === 'details' ? 'active' : ''}`}
                        onClick={() => setActiveTab('details')}
                    >
                        Details
                    </button>
                    <button
                        className={`tab-button ${activeTab === 'subjects' ? 'active' : ''}`}
                        onClick={() => setActiveTab('subjects')}
                    >
                        Subjects
                    </button>
                    <button
                        className={`tab-button ${activeTab === 'videos' ? 'active' : ''}`}
                        onClick={() => setActiveTab('videos')}
                    >
                        Video Groups
                    </button>
                    <button
                        className={`tab-button ${activeTab === 'assignments' ? 'active' : ''}`}
                        onClick={() => setActiveTab('assignments')}
                    >
                        Assignments
                    </button>
                    <button
                        className={`tab-button ${activeTab === 'labelers' ? 'active' : ''}`}
                        onClick={() => setActiveTab('labelers')}
                    >
                        Labelers
                    </button>
                    <button
                        className={`tab-button ${activeTab === 'accessCodes' ? 'active' : ''}`}
                        onClick={() => setActiveTab('accessCodes')}
                    >
                        Access Codes
                    </button>
                </div>

                <div className="tab-content">
                    {activeTab === 'details' && (
                        <div className="details">
                            <p><strong>Description:</strong> {project.description}</p>
                            <div className="button-group">
                                <button
                                    className="edit-btn"
                                    onClick={() => navigate(`/projects/edit/${project.id}`)}
                                >
                                    Edit
                                </button>
                                <button
                                    className="back-btn"
                                    onClick={() => navigate('/projects')}
                                >
                                    Back to Projects
                                </button>
                            </div>
                        </div>
                    )}

                    {activeTab === 'subjects' && (
                        <div className="subjects">
                            <div className="add-buttons">
                                <Link to={`/subjects/add?projectId=${id}`}>
                                    <button className="add-btn">Add Subject</button>
                                </Link>
                            </div>
                            {subjects.length > 0 ? (
                                <table className="normal-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Description</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {subjects.map((subject) => (
                                            <tr key={subject.id}>
                                                <td>{subject.id}</td>
                                                <td>{subject.name}</td>
                                                <td>{subject.description}</td>
                                                <td>
                                                    <Link to={`/subjects/${subject.id}`}>
                                                        <button className="btn btn-info">View</button>
                                                    </Link>
                                                    <button
                                                        className="btn btn-danger"
                                                        onClick={() => handleDeleteItem('subject', subject.id)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No subjects found</p>
                            )}
                        </div>
                    )}

                    {activeTab === 'videos' && (
                        <div className="videos">
                            <div className="add-buttons">
                                <Link to={`/video-groups/add?projectId=${id}`}>
                                    <button className="add-btn">Add Video Group</button>
                                </Link>
                            </div>
                            {videoGroups.length > 0 ? (
                                <table className="normal-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Description</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {videoGroups.map((video) => (
                                            <tr key={video.id}>
                                                <td>{video.id}</td>
                                                <td>{video.name}</td>
                                                <td>{video.description}</td>
                                                <td>
                                                    <Link to={`/video-groups/${video.id}`}>
                                                        <button className="btn btn-info">View</button>
                                                    </Link>
                                                    <button
                                                        className="btn btn-danger"
                                                        onClick={() => handleDeleteItem('videogroup', video.id)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No video groups found</p>
                            )}
                        </div>
                    )}

                    {activeTab === 'assignments' && (
                        <div className="assignments">
                            <div className="add-buttons">
                                <Link to={`/assignments/add?projectId=${id}`}>
                                    <button className="add-btn">Add Assignment</button>
                                </Link>
                            </div>
                            {assignments.length > 0 ? (
                                <table className="normal-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Subject ID</th>
                                            <th>Video Group ID</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {assignments.map((assignment) => (
                                            <tr key={assignment.id}>
                                                <td>{assignment.id}</td>
                                                <td>{assignment.subjectId}</td>
                                                <td>{assignment.videoGroupId}</td>
                                                <td>
                                                    <Link to={`/assignments/${assignment.id}`}>
                                                        <button className="btn btn-info">View</button>
                                                    </Link>
                                                    <button
                                                        className="btn btn-danger"
                                                        onClick={() => handleDeleteItem('SubjectVideoGroupAssignment', assignment.id)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No assignments found</p>
                            )}
                            
                        </div>
                    )}
                    {activeTab === 'labelers' && (
                        <div className="labelers">
                            <h2>Labeler Assignments</h2>
                            <button
                                className="distribute-btn"
                                onClick={handleDistributeLabelers}
                            >
                                Distribute labelers
                            </button>
                            {labelers.length > 0 ? (
                                <table className="normal-table">
                                    <thead>
                                    <tr>
                                        <th>Labeler ID</th>
                                        <th>Username</th>
                                        <th>Video Group ID</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    {labelers.map((labeler) => {
                                        // Get all assignments for this labeler in current project
                                        const labelerAssignments = assignments.filter(assignment =>
                                            assignment.labelers?.some(l => l.id === labeler.id) &&
                                            videoGroups.some(vg =>
                                                vg.id === assignment.videoGroupId &&
                                                vg.projectId === parseInt(id)
                                            )
                                        );

                                        return (
                                            <React.Fragment key={labeler.id}>
                                                <tr>
                                                    <td>{labeler.id}</td>
                                                    <td>{labeler.name}</td>
                                                    {labelerAssignments.length > 0 ? (
                                                        labelerAssignments.map((assignment) => (
                                                            <td key={assignment.id}>{assignment.videoGroupId}</td>
                                                        ))
                                                    ) : (
                                                        <td>No assignments</td> // Or leave this empty if preferred
                                                    )}
                                                </tr>
                                            </React.Fragment>
                                        );
                                    })}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No labelers found in this project</p>
                            )}
                        </div>
                    )}
                    {activeTab === 'accessCodes' && (
                        <div className="access-codes">
                            <div className="add-buttons">
                                <div className="duration-options">
                                    <button
                                        className={`btn btn-primary ${selectedDuration === '14days' ? 'active' : ''}`}
                                        onClick={() => setSelectedDuration('14days')}
                                    >
                                        14 Days
                                    </button>
                                    <button
                                        className={`btn btn-primary ${selectedDuration === '30days' ? 'active' : ''}`}
                                        onClick={() => setSelectedDuration('30days')}
                                    >
                                        30 Days
                                    </button>
                                    <button
                                        className={`btn btn-primary ${selectedDuration === 'unlimited' ? 'active' : ''}`}
                                        onClick={() => setSelectedDuration('unlimited')}
                                    >
                                        Unlimited
                                    </button>

                                    <button
                                        className="btn btn-success"
                                        onClick={handleCreateAccessCode}
                                    >
                                        Generate Access Code
                                    </button>
                                </div>
                                {creationError && <div className="error">{creationError}</div>}
                            </div>

                            {accessCodes.length > 0 ? (
                                <table className="normal-table">
                                    <thead>
                                    <tr>
                                        <th>Code</th>
                                        <th>Created At</th>
                                        <th>Expires At</th>
                                        <th>Valid</th>
                                        <th>Methods</th>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    {accessCodes.map((code) => (
                                        <tr key={code.code}>
                                            <td>{code.code}</td>
                                            <td>{new Date(code.createdAtUtc).toLocaleString()}</td>
                                            <td>
                                                {code.expiresAtUtc
                                                    ? new Date(code.expiresAtUtc).toLocaleString()
                                                    : 'Never'}
                                            </td>
                                            <td>{code.isValid ? '✅' : '❌'}</td>
                                            <td>
                                                <button
                                                    className="btn btn-primary"
                                                    onClick={() => handleCopyCode(code.code)}
                                                >
                                                    Copy
                                                </button>
                                            </td>
                                        </tr>
                                    ))}

                                    </tbody>
                                </table>
                            ) : (
                                <p>No access codes found</p>
                            )}
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
};

export default ProjectDetails;