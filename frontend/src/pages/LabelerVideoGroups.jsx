import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const LabelerVideoGroups = () => {
    const { labelerId } = useParams();
    const [assignments, setAssignments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [accessCode, setAccessCode] = useState('');
    const [joinError, setJoinError] = useState('');
    const navigate = useNavigate();

    const handleJoinProject = async () => {
        setJoinError('');
        if (!accessCode.trim()) {
            setJoinError('Please enter an access code');
            return;
        }

        try {
            await httpClient.post('/project/join', {
                AccessCode: accessCode.trim()
            });
            alert('Successfully joined the project!');
            setAccessCode('');
            fetchAssignments();
        } catch (error) {
            setJoinError(error.response?.data?.message || 'Invalid or expired access code');
        }
    };

    const fetchAssignments = async () => {
        try {
            const response = await httpClient.get(`/SubjectVideoGroupAssignment`);
            setAssignments(response.data);
            setError('');
        } catch (error) {
            setError(error.response?.data?.message || 'Failed to load assignments');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const fetchAssignments = async () => {
            try {
                const response = await httpClient.get(`/SubjectVideoGroupAssignment`);
                setAssignments(response.data);
                setError('');
            } catch (error) {
                setError(error.response?.data?.message || 'Failed to load assignments');
            } finally {
                setLoading(false);
            }
        };

        fetchAssignments();
    }, [labelerId]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p className="error">{error}</p>;

    return (
        <div className="container">
            <div className="content">
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                    <h1 className="heading">SubjectVideoGroupAssignments</h1>
                    <div className="join-project-section">
                        <input
                            type="text"
                            placeholder="Paste access code"
                            value={accessCode}
                            onChange={(e) => setAccessCode(e.target.value)}
                            className="access-code-input"
                        />
                        <button
                            onClick={handleJoinProject}
                            className="join-btn"
                        >
                            Join Project
                        </button>
                        {joinError && <div className="error" style={{ marginTop: '10px' }}>{joinError}</div>}
                    </div>
                </div>

                {error && <div className="error">{error}</div>}

                {loading ? (
                    <div style={{ padding: '20px', textAlign: 'center' }}>
                        Loading SubjectVideoGroupAssignments...
                    </div>
                ) : assignments.length > 0 ? (
                    <table className="assignments-table">
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>Subject</th>
                            <th>Video Group</th>
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
                                    <button
                                        className="details-btn"
                                        onClick={() => navigate(`/video/${assignment.id}`)}
                                    >
                                        Details
                                    </button>
                                </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                ) : (
                    <p className="error">No assignments found</p>
                )}
            </div>
        </div>
    );
};

export default LabelerVideoGroups;
