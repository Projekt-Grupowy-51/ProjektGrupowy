import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import httpClient from '../httpClient'; 
import './css/ScientistProjects.css';

const SubjectDetails = () => {
    const { id } = useParams();
    const [subjectDetails, setSubjectDetails] = useState(null);
    const [labels, setLabels] = useState([]);
    const navigate = useNavigate();

    const fetchSubjectDetails = async () => {
        try {
            const response = await httpClient.get(`/subject/${id}`);
            setSubjectDetails(response.data);
            await fetchLabels();
        } catch (error) {
            console.error('Błąd pobierania szczegółów tematu:', error);
        }
    };

    const fetchLabels = async () => {
        try {
            const response = await httpClient.get(`subject/${id}/label`);
            const filteredLabels = response.data
                .filter(label => label.subjectId === parseInt(id))
                .sort((a, b) => a.id - b.id);
            setLabels(filteredLabels);
        } catch (error) {
            console.error('Błąd pobierania etykiet:', error);
        }
    };

    useEffect(() => {
        if (id) fetchSubjectDetails();
    }, [id]);

    const addLabel = () => {
        navigate(`/labels/add?subjectId=${id}`);
    };

    const deleteLabel = async (labelId) => {
        try {
            await httpClient.delete(`/label/${labelId}`);
            setLabels(labels.filter(label => label.id !== labelId));
        } catch (error) {
            console.error('Błąd usuwania etykiety:', error);
        }
    };

    if (!subjectDetails) return (
        <div className="container d-flex justify-content-center align-items-center py-5">
            <div className="spinner-border text-primary" role="status">
                <span className="visually-hidden">Loading...</span>
            </div>
        </div>
    );

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading mb-4">{subjectDetails.name}</h1>

                <div className="card shadow-sm mb-4">
                    <div className="card-header bg-info text-white">
                        <h5 className="card-title mb-0">Subject Details</h5>
                    </div>
                    <div className="card-body">
                        <p className="card-text">
                            <strong>Description:</strong> {subjectDetails.description}
                        </p>
                    </div>
                </div>

                <div className="d-flex justify-content-between mb-4">
                    <button className="btn btn-primary" onClick={addLabel}>
                        <i className="fas fa-plus-circle me-2"></i>Add New Label
                    </button>
                    <Link
                        className="btn btn-secondary"
                        to={`/projects/${subjectDetails.projectId}`}
                    >
                        <i className="fas fa-arrow-left me-2"></i>Back to Project
                    </Link>
                </div>

                <h2 className="section-title">Labels</h2>

                {labels.length > 0 ? (
                    <table className="normal-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Type</th>
                                <th>Color</th>
                                <th>Shortcut</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {labels.map(label => (
                                <tr key={label.id}>
                                    <td>{label.id}</td>
                                    <td>{label.name}</td>
                                    <td>{label.type}</td>
                                    <td>
                                        <div className="d-flex align-items-center">
                                            <div
                                                className="color-preview me-2"
                                                style={{ backgroundColor: label.colorHex }}
                                            />
                                            <span>{label.colorHex}</span>
                                        </div>
                                    </td>
                                    <td>{label.shortcut}</td>
                                    <td>
                                        <div className="d-flex">
                                            <button
                                                className="btn btn-primary btn-sm me-2"
                                                onClick={() => navigate(`/labels/edit/${label.id}`)}
                                            >
                                                <i className="fas fa-edit"></i>
                                            </button>
                                            <button
                                                className="btn btn-danger btn-sm"
                                                onClick={() => deleteLabel(label.id)}
                                            >
                                                <i className="fas fa-trash"></i>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <div className="alert alert-info">
                        <i className="fas fa-info-circle me-2"></i>No labels found for this subject
                    </div>
                )}
            </div>
        </div>
    );
};

export default SubjectDetails;