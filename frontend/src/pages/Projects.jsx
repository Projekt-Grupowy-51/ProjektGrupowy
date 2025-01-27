import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate, useParams } from 'react-router-dom';
import './css/ScientistProjects.css'; 

const Projects = () => {
    const [projects, setProjects] = useState([]);
    const navigate = useNavigate();

    // Pobiera listê projektów z API
    async function fetchProjects() {
        try {
            const response = await fetch('http://localhost:5000/api/Project');
            const data = await response.json();
            setProjects(data);
        } catch (error) {
            console.error('Error fetching projects:', error);
        }
    }

    useEffect(() => {
        fetchProjects();
    }, []);

    // Dodawanie nowego projektu
    function addProject() {
        
        // Implementacja logiki dodawania projektu
    }

    // Usuwanie projektu
    async function deleteProject(id) {
        try {
            const response = await fetch(`http://localhost:5000/api/Project/${id}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                setProjects(projects.filter((project) => project.id !== id));
                console.log('Deleted project:', id);
            } else {
                console.error('Error while deleting project:', response.statusText);
            }
        } catch (error) {
            console.error('Error while deleting project:', error);
        }
    }

    return (
        <div className="container">
             <div className="content">
                <h1 className="heading">Projects</h1>
                <button className="add-btn" onClick={() => navigate(`/projects/add`)}>Add new project</button>

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
                        {projects.map((project) => (
                            <tr key={project.id}>
                                <td>{project.id}</td>
                                <td>{project.name}</td>
                                <td>{project.description}</td>
                                <td>
                                    <button className="details-btn" onClick={() => navigate(`/projects/${project.id}`)}>Details</button>
                                    <button className="delete-btn" onClick={() => deleteProject(project.id)}>Delete</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default Projects;