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

    if (!subjectDetails) return <div className="loading">Ładowanie...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">{subjectDetails.name}</h1>

                <div className="details-section">
                    <p className="detail-item">
                        <span className="detail-label">Opis:</span>
                        {subjectDetails.description}
                    </p>
                </div>

                <div className="action-buttons">
                    <button className="add-btn" onClick={addLabel}>
                        + Dodaj nową etykietę
                    </button>
                    <Link
                        className="back-btn"
                        to={`/projects/${subjectDetails.projectId}`}
                    >
                        ← Wróć do projektu
                    </Link>
                </div>

                <h2 className="section-title">Etykiety</h2>

                {labels.length > 0 ? (
                    <table className="normal-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Nazwa</th>
                                <th>Typ</th>
                                <th>Kolor</th>
                                <th>Skrót</th>
                                <th>Akcje</th>
                            </tr>
                        </thead>
                        <tbody>
                            {labels.map(label => (
                                <tr key={label.id}>
                                    <td>{label.id}</td>
                                    <td>{label.name}</td>
                                    <td>{label.type}</td>
                                    <td>
                                        <div
                                            className="color-preview"
                                            style={{ backgroundColor: label.colorHex }}
                                        />
                                    </td>
                                    <td>{label.shortcut}</td>
                                    <td>
                                        <div className="table-actions">
                                            <button
                                                className="btn btn-info"
                                                onClick={() => navigate(`/labels/edit/${label.id}`)}
                                            >
                                                Edytuj
                                            </button>
                                            <button
                                                className="btn btn-danger"
                                                onClick={() => deleteLabel(label.id)}
                                            >
                                                Usuń
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                ) : (
                    <p className="no-data-info">Brak etykiet dla tego tematu</p>
                )}
            </div>
        </div>
    );
};

export default SubjectDetails;