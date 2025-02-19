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
    const navigate = useNavigate();

    const fetchData = async () => {
        try {
            const [projectRes, subjectsRes, videoGroupsRes, assignmentsRes] = await Promise.all([
                httpClient.get(`/project/${id}`),
                httpClient.get(`/project/${id}/subjects`),
                httpClient.get(`/project/${id}/videogroups`),
                httpClient.get(`/project/${id}/SubjectVideoGroupAssignments`)
            ]);

            setProject(projectRes.data);
            setSubjects(subjectsRes.data);
            setVideoGroups(videoGroupsRes.data);
            setAssignments(assignmentsRes.data);
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

    useEffect(() => {
        fetchData();
    }, [id]);

    if (!project) return <div className="container">Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1>{project.name}</h1>
                {error && <div className="error">{error}</div>}

                {/* Zak³adki w starym stylu */}
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
                                <table className="project-table">
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
                                                        <button className="details-btn">View</button>
                                                    </Link>
                                                    <button
                                                        className="delete-btn"
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
                                <table className="project-table">
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
                                                        <button className="details-btn">View</button>
                                                    </Link>
                                                    <button
                                                        className="delete-btn"
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
                                <table className="project-table">
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
                                                    <Link to={`/subject-video-group-assignments/${assignment.id}`}>
                                                        <button className="details-btn">View</button>
                                                    </Link>
                                                    <button
                                                        className="delete-btn"
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
                </div>
            </div>
        </div>
    );
};

export default ProjectDetails;