import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

function ProjectEdit() {
    const { id } = useParams(); // ID projektu z URL-a
    const navigate = useNavigate();

    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [scientistId, setScientistId] = useState("");
    const [finished, setFinished] = useState(false);

    // Funkcja do pobrania aktualnych danych projektu
    useEffect(() => {
        fetch(`http://localhost:5000/api/Project/${id}`)
            .then((response) => response.json())
            .then((data) => {
                setName(data.name || "");
                setDescription(data.description || "");
                setScientistId(data.scientistId || "");
                setFinished(data.finished || false);
            })
            .catch((error) => {
                console.error("Error fetching project data:", error);
            });
    }, [id]);

    // Obs³uga zapisu zmian
    const handleSubmit = (e) => {
        e.preventDefault();

        const projectData = {
            name,
            description,
            scientistId: parseInt(scientistId),
            finished,
        };

        fetch(`http://localhost:5000/api/Project/${id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(projectData),
        })
            .then((response) => {
                if (response.ok) {
                    alert("Project updated successfully!");
                    navigate(`/projects/${id}`); // Przekierowanie np. do strony g³ównej
                } else {
                    alert("Failed to update project!");
                }
            })
            .catch((error) => {
                console.error("Error updating project:", error);
            });
    };

    return (
        <div>
            <h2>Edit Project</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>
                        Name:
                        <input
                            type="text"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <div>
                    <label>
                        Description:
                        <textarea
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <div>
                    <label>
                        Finished:
                        <select
                            value={finished}
                            onChange={(e) => setFinished(e.target.value === "true")}
                        >
                            <option value="true">True</option>
                            <option value="false">False</option>
                        </select>
                    </label>
                </div>
                <button type="submit">Save Changes</button>
            </form>
        </div>
    );
}

export default ProjectEdit;
