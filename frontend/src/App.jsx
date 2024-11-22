import React, { useEffect, useState } from 'react';

function App() {
    const [forecasts, setForecasts] = useState([]); // Lista projektów
    //const [singleProject, setSingleProject] = useState(null); // Pojedynczy projekt
    const [newProject, setNewProject] = useState({ name: '', description: '' }); // Nowy projekt
    const [editProject, setEditProject] = useState({ id: null, name: '', description: '' }); // Edytowany projekt

    // Pobiera listê projektów z API
     async function populateWeatherData() {
        const response = await fetch('http://localhost:5000/api/Project');
        const data = await response.json();

        // Mapowanie danych, aby przypisaæ `$id` jako `id`
        const mappedData = data.map((project) => ({
            id: project.id, // U¿yj $id jako id
            name: project.name,
            description: project.description,
        }));

        setForecasts(mappedData);
        console.log(mappedData); // Debugowanie danych
    }

    //// Pobiera pojedynczy projekt z API
    //async function fetchSingleProject(id) {
    //    const response = await fetch(`http://localhost:5000/api/Project/${id}`);
    //    const data = await response.json();
    //    setSingleProject(data);
    //}

    // Dodaje nowy projekt
    async function addProject() {
        const response = await fetch('http://localhost:5000/api/Project', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newProject),
        });
        if (response.ok) {
            console.log('Project added!');
            setNewProject({ name: '', description: '' });
            populateWeatherData(); // Odœwie¿a listê projektów
        }
    }

    // Usuwa projekt
    // Funkcja do usuwania projektu
    async function deleteProject(id) {
        if (!id) {
            console.error('Project ID is undefined');
            return;
        }
        try {
            const response = await fetch(`http://localhost:5000/api/Project/${id}`, { method: 'DELETE' });
            if (response.ok) {
                console.log(`Project ${id} deleted successfully!`);
                populateWeatherData(); // Odœwie¿anie listy projektów
            } else {
                console.error(`Failed to delete project with ID ${id}: ${response.statusText}`);
            }
        } catch (error) {
            console.error(`Error while deleting project: ${error}`);
        }
    }


    // Edytuje projekt
    async function updateProject() {
        await fetch(`http://localhost:5000/api/Project/${editProject.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(editProject),
        });
        setEditProject({ id: null, name: '', description: '' });
        populateWeatherData(); // Odœwie¿a listê projektów
    }

    // Wywo³anie po za³adowaniu komponentu
    useEffect(() => {
        populateWeatherData();
    }, []);

    return (
        <div>
            <h1>Project Management</h1>

            {/* Lista projektów */}
            <table border="1">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map((forecast) => (
                        <tr key={forecast.id}>
                            <td>{forecast.id}</td>
                            <td>{forecast.name}</td>
                            <td>{forecast.description}</td>
                            <td>
                               {/* <button onClick={() => fetchSingleProject(forecast.id)}>View</button>*/}
                                <button onClick={() => setEditProject(forecast)}>Edit</button>
                                <button onClick={() => deleteProject(forecast.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Wyœwietlenie pojedynczego projektu */}
            {/*{singleProject && (*/}
            {/*    <div>*/}
            {/*        <h2>Project Details</h2>*/}
            {/*        <p><strong>Name:</strong> {singleProject.name}</p>*/}
            {/*        <p><strong>Description:</strong> {singleProject.description}</p>*/}
            {/*    </div>*/}
            {/*)}*/}

            {/* Dodawanie nowego projektu */}
            <h2>Add New Project</h2>
            <form onSubmit={(e) => { e.preventDefault(); addProject(); }}>
                <input
                    type="text"
                    placeholder="Name"
                    value={newProject.name}
                    onChange={(e) => setNewProject({ ...newProject, name: e.target.value })}
                />
                <textarea
                    placeholder="Description"
                    value={newProject.description}
                    onChange={(e) => setNewProject({ ...newProject, description: e.target.value })}
                />
                <button type="submit">Add Project</button>
            </form>

            {/* Edycja projektu */}
            {editProject.id && (
                <div>
                    <h2>Edit Project</h2>
                    <form onSubmit={(e) => { e.preventDefault(); updateProject(); }}>
                        <input
                            type="text"
                            placeholder="Name"
                            value={editProject.name}
                            onChange={(e) => setEditProject({ ...editProject, name: e.target.value })}
                        />
                        <textarea
                            placeholder="Description"
                            value={editProject.description}
                            onChange={(e) => setEditProject({ ...editProject, description: e.target.value })}
                        />
                        <button type="submit">Save Changes</button>
                    </form>
                </div>
            )}
        </div>
    );
}

export default App;
