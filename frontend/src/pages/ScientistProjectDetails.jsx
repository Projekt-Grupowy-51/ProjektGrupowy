import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import './css/ScientistProjects.css'; 

const ScientistProjectDetails = () => {
    const { id } = useParams();
    const [project, setProject] = useState(null);
    const [subjects, setSubjects] = useState([]);
    const [videoGroup, setVideoGroup] = useState([]);

    // Fetch project details from the API
    async function fetchProjectDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/Project/${id}`);
            const data = await response.json();
            setProject(data);
            fetchSubjects(id); // Fetch subjects if available in the project
            fetchVideoGroup(id); // Fetch video group if available in the project
        } catch (error) {
            console.error('Error fetching project details:', error);
        }
    }

    // Fetch subjects data
    async function fetchSubjects(id) {
        if (!id) return;
        try {
            const response = await fetch(`http://localhost:5000/api/Subject/project/${id}`);
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
            const response = await fetch(`http://localhost:5000/api/VideoGroup/project/${id}`);
            const data = await response.json();
            setVideoGroup(data);
        } catch (error) {
            console.error('Error fetching video group:', error);
        }
    }

    useEffect(() => {
        fetchProjectDetails();
    }, [id]);

    if (!project) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Project Details</h1>
                <div className="details">
                    <p><strong>ID:</strong> {project.id}</p>
                    <p><strong>Name:</strong> {project.name}</p>
                    <p><strong>Description:</strong> {project.description}</p>
                    <p><strong>Scientist:</strong> {project.scientist}</p>
                </div>
                <button className="edit-btn" onClick={() => console.log('Editing project')}>Edit</button>
                <button className="back-btn"><Link to="/projects">Back to list</Link></button>

                <div className="parallel-tables">
                    <h2>Subjects</h2>
                    <h2>Video Groups</h2>

                </div>
                <div className="parallel-tables">

                    {/* Subjects Table */}
                    {subjects.length > 0 ? (
                        <table className="project-table" id="details-subject-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Description</th>
                                </tr>
                            </thead>
                            <tbody>
                                {subjects.map((subject) => (
                                    <tr key={subject.id}>
                                        <td>{subject.id}</td>
                                        <td>{subject.name}</td>
                                        <td>{subject.description}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>No subjects in this project</p>
                    )}

                    {/* Video Group Table */}
                    {videoGroup.length > 0 ? (
                        <table className="project-table" id="details-video-table">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Title</th>
                                    <th>Description</th>
                                </tr>
                            </thead>
                            <tbody>
                                {videoGroup.map((video) => (
                                    <tr key={video.id}>
                                        <td>{video.id}</td>
                                        <td>{video.title}</td>
                                        <td>{video.description}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <p>No videos in this project</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ScientistProjectDetails;