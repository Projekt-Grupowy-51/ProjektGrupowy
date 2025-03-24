import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import httpClient from '../httpClient';
import './css/ScientistProjects.css';

const SubjectVideoGroupAssignmentAdd = () => {
    const [formData, setFormData] = useState({
        subjectId: '',
        videoGroupId: ''
    });
    const [subjects, setSubjects] = useState([]);
    const [videoGroups, setVideoGroups] = useState([]);
    const [projectId, setProjectId] = useState(null);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const queryParams = new URLSearchParams(location.search);
        const projectIdParam = queryParams.get('projectId');
        if (projectIdParam) {
            setProjectId(parseInt(projectIdParam));
            fetchSubjectsAndVideoGroups(parseInt(projectIdParam));
        }
    }, [location.search]);

    const fetchSubjectsAndVideoGroups = async (projectId) => {
        setLoading(true);
        try {
            const [subjectsRes, videoGroupsRes] = await Promise.all([
                httpClient.get(`/project/${projectId}/subjects`),
                httpClient.get(`/project/${projectId}/videogroups`)
            ]);
            
            setSubjects(subjectsRes.data);
            setVideoGroups(videoGroupsRes.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to load data');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        if (!formData.subjectId || !formData.videoGroupId) {
            setError('Please select both a subject and a video group.');
            setLoading(false);
            return;
        }

        try {
            await httpClient.post('/SubjectVideoGroupAssignment', {
                subjectId: parseInt(formData.subjectId),
                videoGroupId: parseInt(formData.videoGroupId)
            });
            navigate(`/projects/${projectId}`);
        } catch (err) {
            setError(err.response?.data?.message || 'An error occurred. Please try again.');
            setLoading(false);
        }
    };

    if (loading && (!subjects.length || !videoGroups.length)) {
        return <div className="container">Loading...</div>;
    }

    return (
        <div className="container">
            <div className="content">
                <h1>Add New Assignment</h1>
                {error && <div className="error">{error}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="subjectId">Subject</label>
                        <select
                            id="subjectId"
                            name="subjectId"
                            value={formData.subjectId}
                            onChange={handleChange}
                            className="form-control"
                            required
                        >
                            <option value="">Select a subject</option>
                            {subjects.map(subject => (
                                <option key={subject.id} value={subject.id}>
                                    {subject.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="form-group">
                        <label htmlFor="videoGroupId">Video Group</label>
                        <select
                            id="videoGroupId"
                            name="videoGroupId"
                            value={formData.videoGroupId}
                            onChange={handleChange}
                            className="form-control"
                            required
                        >
                            <option value="">Select a video group</option>
                            {videoGroups.map(videoGroup => (
                                <option key={videoGroup.id} value={videoGroup.id}>
                                    {videoGroup.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="button-group">
                        <button 
                            type="submit" 
                            className="btn btn-primary"
                            disabled={loading}
                        >
                            {loading ? 'Adding...' : 'Create Assignment'}
                        </button>
                        <button 
                            type="button" 
                            className="btn btn-secondary"
                            onClick={() => navigate(`/projects/${projectId}`)}
                        >
                            Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default SubjectVideoGroupAssignmentAdd;
