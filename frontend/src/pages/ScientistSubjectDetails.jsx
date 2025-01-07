import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import './css/ScientistProjects.css';

const ScientistSubjectDetails = () => {
    const { id } = useParams(); // Pobierz id z URL
    const [subjectDetails, setSubjectDetails] = useState(null);
    const [labels, setLabels] = useState([]);
    const navigate = useNavigate();

    // Pobierz szczegÛ≥y subject
    async function fetchSubjectDetails() {
        try {
            const response = await fetch(`http://localhost:5000/api/Subject/${id}`);
            const data = await response.json();
            setSubjectDetails(data);
            fetchLabels(); // Pobierz etykiety powiπzane z tematem
        } catch (error) {
            console.error('Error fetching subject details:', error);
        }
    }

    // Pobierz listÍ etykiet
    async function fetchLabels() {
        try {
            const response = await fetch('http://localhost:5000/api/Label');
            const data = await response.json();
            setLabels(data);
        } catch (error) {
            console.error('Error fetching labels:', error);
        }
    }

    // Pobierz szczegÛ≥y subject po za≥adowaniu komponentu
    useEffect(() => {
        if (id) fetchSubjectDetails();
    }, [id]);

    // Dodawanie nowej etykiety
    function addLabel() {
        console.log('Add new label');
        // Logika dodawania etykiety
    }

    // Usuwanie etykiety
    async function deleteLabel(id) {
        try {
            const response = await fetch(`http://localhost:5000/api/Label/${id}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                setLabels(labels.filter((label) => label.id !== id));
                console.log('Deleted label:', id);
            } else {
                console.error('Error while deleting label:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting label:', error);
        }
    }

    // Sprawdü, czy subjectDetails istnieje przed renderowaniem
    if (!subjectDetails) return <div>Loading...</div>;

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Subject Details</h1>
                <div className="details">
                    <p><strong>ID:</strong> {subjectDetails.id}</p>
                    <p><strong>Name:</strong> {subjectDetails.name}</p>
                    <p><strong>Description:</strong> {subjectDetails.description}</p>
                    <p><strong>Scientist:</strong> {subjectDetails.scientist}</p>
                </div>
                <button className="add-btn" onClick={addLabel}>Add new label</button>
                <button className="back-btn"><Link to="/subjects">Back to list</Link></button>

                <h2>Labels</h2>
                <table className="project-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Subject ID</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {labels.map((label) => (
                            <tr key={label.id}>
                                <td>{label.id}</td>
                                <td>{label.name}</td>
                                <td>{label.description}</td>
                                <td>{label.subjectId}</td>
                                <td>
                                    <button className="details-btn" onClick={() => navigate(`/Label/${label.id}`)}>Details</button>
                                    <button className="delete-btn" onClick={() => deleteLabel(label.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ScientistSubjectDetails;

