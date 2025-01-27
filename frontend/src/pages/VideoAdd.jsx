import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './css/ScientistProjects.css'; // Importujemy plik CSS

const VideoAdd = () => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [videoUrl, setVideoUrl] = useState('');
    const [videoGroupId, setVideoGroupId] = useState('');
    const navigate = useNavigate();

    // Funkcja do obs³ugi wysy³ania formularza
    async function handleSubmit(e) {
        e.preventDefault(); // Zatrzymaj domyœln¹ akcjê formularza

        const newVideo = {
            title,
            description,
            videoUrl,
            videoGroupId,
        };

        try {
            const response = await fetch('http://localhost:5000/api/Video', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(newVideo),
            });

            if (response.ok) {
                alert('Video successfully added!');
                navigate('/videos'); // Przekierowanie do listy wideo po udanym dodaniu
            } else {
                throw new Error('Failed to add video');
            }
        } catch (error) {
            console.error('Error adding video:', error);
            alert('Error adding video');
        }
    }

    return (
        <div className="container">
            <h1 className="heading">Add New Video</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="title">Title</label>
                    <input
                        type="text"
                        id="title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="videoUrl">Video URL</label>
                    <input
                        type="url"
                        id="videoUrl"
                        value={videoUrl}
                        onChange={(e) => setVideoUrl(e.target.value)}
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="videoGroupId">Video Group ID</label>
                    <input
                        type="text"
                        id="videoGroupId"
                        value={videoGroupId}
                        onChange={(e) => setVideoGroupId(e.target.value)}
                    />
                </div>

                <button type="submit" className="submit-btn">Add Video</button>
            </form>
        </div>
    );
};

export default VideoAdd;
