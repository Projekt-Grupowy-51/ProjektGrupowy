import React, { useState } from "react";

function ProjectAdd() {
    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [scientistId, setScientistId] = useState("");

    const handleSubmit = (e) => {
        e.preventDefault();

        const projectData = {
            name,
            description,
            scientistId: parseInt(scientistId),
            finished: false, // domyœlnie na false
        };

        // Wyœlij dane do API
        fetch("http://localhost:5000/api/Project", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(projectData),
        })
            .then((response) => {
                if (response.ok) {
                    alert("Project added successfully!");
                } else {
                    alert("Failed to add project!");
                }
            })
            .catch((error) => {
                console.error("Error adding project:", error);
            });
    };

    return (
        <div>
            <h2>Add New Project</h2>
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
                        Scientist ID:
                        <input
                            type="number"
                            value={scientistId}
                            onChange={(e) => setScientistId(e.target.value)}
                            required
                        />
                    </label>
                </div>
                <button type="submit">Add Project</button>
            </form>
        </div>
    );
}

export default ProjectAdd;
