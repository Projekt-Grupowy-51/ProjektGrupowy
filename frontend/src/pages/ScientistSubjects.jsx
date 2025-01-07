import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom';
import './css/ScientistProjects.css';

const ScientistSubjects = () => {
    const [subjects, setSubjects] = useState([]);
    const navigate = useNavigate();

    // Pobiera listê tematow z API
    async function fetchSubjects() {
        try {
            const response = await fetch('http://localhost:5000/api/Subject');
            const data = await response.json();
            setSubjects(data);
        } catch (error) {
            console.error('Error fetching subjects:', error);
        }
    }

    useEffect(() => {
        fetchSubjects();
    }, []);

    // Dodawanie nowego tematu
    function addSubject() {
        console.log('Add new subject');
        // Implementacja logiki dodawania projektu
    }

    // Usuwanie tematu
    async function deleteSubject(id) {
        try {
            const response = await fetch(`http://localhost:5000/api/Subject/${id}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                setSubjects(subjects.filter((subject) => subject.id !== id));
                console.log('Deleted subject:', id);
            } else {
                console.error('Error while deleting subject:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting subject:', error);
        }
    }

    return (
        <div className="container">
            <div className="content">
                <h1 className="heading">Subjects List</h1>
                <button className="add-btn" onClick={addSubject}>Add new subject</button>

                <table className="project-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Project ID</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {subjects.map((subject) => (
                            <tr key={subject.id}>
                                <td>{subject.id}</td>
                                <td>{subject.name}</td>
                                <td>{subject.description}</td>
                                <td>{subject.projectId}</td>
                                <td>
                                    <button className="details-btn" onClick={() => navigate(`/subjects/${subject.id}`)}>Details</button>
                                    <button className="delete-btn" onClick={() => deleteSubject(subject.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ScientistSubjects;