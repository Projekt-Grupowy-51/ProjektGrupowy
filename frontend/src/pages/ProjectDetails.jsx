import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import './css/ScientistProjects.css';

const ProjectDetails = () => {
    const { id } = useParams();
    const [project, setProject] = useState(null);
    const [subjects, setSubjects] = useState([]);
    const [videoGroup, setVideoGroup] = useState([]);
    const [assignments, setAssignments] = useState([]); // State for SubjectVideoGroupAssignments
    const [activeTab, setActiveTab] = useState('details'); // Active tab state
    const navigate = useNavigate();

    // Fetch project details from the API
    async function fetchProjectDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/project/${id}`);
            const data = await response.json();
            setProject(data);
            fetchSubjects(id);
            fetchVideoGroup(id);
            fetchAssignments(id);
        } catch (error) {
            console.error('Error fetching project details:', error);
        }
    }

    // Fetch subjects data
    async function fetchSubjects(id) {
        if (!id) return;
        try {
            const response = await fetch(`http://localhost:5000/api/project/${id}/subjects`);
            const data = await response.json();
            setSubjects(data);
        } catch (error) {
            console.error('Error fetching subjects:', error);
        }
    }

    // Fetch video group data
    async function fetchVideoGroup(id) {
        if (!id) return;
        try {
            const response = await fetch(`http://localhost:5000/api/project/${id}/videogroups`);
            const data = await response.json();
            setVideoGroup(data);
        } catch (error) {
            console.error('Error fetching video group:', error);
        }
    }

    // Fetch SubjectVideoGroupAssignments data
    async function fetchAssignments(id) {
        if (!id) return;
        try {
            const response = await fetch(`http://localhost:5000/api/project/${id}/SubjectVideoGroupAssignments`);
            const data = await response.json();
            setAssignments(data);
        } catch (error) {
            console.error('Error fetching assignments:', error);
        }
    }

    // Handle deletion of a specific item
    async function handleDeleteItem(endpoint, itemId, fetchDataCallback) {
        try {
            const response = await fetch(`http://localhost:5000/api/${endpoint}/${itemId}`, {
                method: 'DELETE',
            });
            if (response.ok) {
                console.log(`${endpoint} with ID ${itemId} deleted successfully`);
                fetchDataCallback(id); // Refresh data
            } else {
                console.error(`Error deleting ${endpoint}:`, response.statusText);
            }
        } catch (error) {
            console.error(`Error deleting ${endpoint}:`, error);
        }
    }

    useEffect(() => {
        fetchProjectDetails();
    }, [id]);

    if (!project) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">{project.name}</h1>

                {/* Tab Navigation */}
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
                        Subject Video Group Assignments
                    </button>
                </div>

                {/* Tab Content */}
                <div className="tab-content">
                    {activeTab === 'details' && (
                        <div className="details">
                            {/*<p><strong>ID:</strong> {project.id}</p>*/}
                            {/*<p><strong>Name:</strong> {project.name}</p>*/}
                            <p><strong>Description:</strong> {project.description}</p>
                            <button className="edit-btn" onClick={() => navigate(`/projects/edit/${project.id}`)}>Edit</button>
                            <button className="back-btn">
                                <Link to="/projects" onClick={() => setActiveTab('details')}>Back to list</Link>
                            </button>
                        </div>
                    )}

                    {activeTab === 'subjects' && (
                        <div className="subjects">
                            {/*<h2>Subjects</h2>*/}
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
                                            <th>Action</th>
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
                                                        onClick={() => handleDeleteItem('subject', subject.id, fetchSubjects)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No subjects in this project</p>
                            )}
                        </div>
                    )}

                    {activeTab === 'videos' && (
                        <div className="videos">
                            {/*<h2>Video Groups</h2>*/}
                            <div className="add-buttons">
                                <Link to={`/video-groups/add?projectId=${id}`}>
                                    <button className="add-btn">Add Video Group</button>
                                </Link>
                            </div>
                            {videoGroup.length > 0 ? (
                                <table className="project-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>Description</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {videoGroup.map((video) => (
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
                                                        onClick={() => handleDeleteItem('videogroup', video.id, fetchVideoGroup)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No videos in this project</p>
                            )}
                        </div>
                    )}

                    {activeTab === 'assignments' && (
                        <div className="assignments">
                            {/*<h2>Subject Video Group Assignments</h2>*/}
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
                                            <th>Subject</th>
                                            <th>Video Group</th>
                                            <th>Action</th>
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
                                                        onClick={() => handleDeleteItem('SubjectVideoGroupAssignment', assignment.id, fetchAssignments)}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            ) : (
                                <p>No assignments in this project</p>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ProjectDetails;
