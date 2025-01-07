import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import './css/ScientistProjects.css'; // Importujemy plik CSS

const Video = () => {
    const { id } = useParams(); // Pobierz ID filmu z URL
    const [videoUrl, setVideoUrl] = useState(null);
    const [videoInfo, setVideoInfo] = useState(null); // Stan do przechowywania informacji o filmie

    // Funkcja do pobierania URL strumienia wideo
    async function fetchVideoStream() {
        try {
            const url = `http://localhost:5000/api/Video/${id}/stream`;
            setVideoUrl(url); // Ustawiamy URL wideo
        } catch (error) {
            console.error('Error fetching video stream:', error);
        }
    }

    // Funkcja do pobierania informacji o filmie
    async function fetchVideoInfo() {
        try {
            const response = await fetch(`http://localhost:5000/api/Video/${id}`);
            const data = await response.json();
            setVideoInfo(data); // Ustawiamy informacje o filmie
        } catch (error) {
            console.error('Error fetching video info:', error);
        }
    }

    useEffect(() => {
        fetchVideoStream(); // Uruchomienie pobierania strumienia wideo
        fetchVideoInfo(); // Pobieranie informacji o filmie
    }, [id]); // Ponownie uruchom, gdy zmienia siê ID

    if (!videoUrl || !videoInfo) {
        return <div className="loading">Loading...</div>; // Jeœli URL lub info nie s¹ dostêpne, wyœwietl komunikat ³adowania
    }

    return (
        <div className="container">
            <h1 className="heading">Video {id}</h1>
            <video controls className="video-player">
                <source src={videoUrl} type="video/mp4" />
                Your browser does not support the video tag.
            </video>
            <div className="details">
                <p><strong>ID:</strong> {videoInfo.id}</p>
                <p><strong>Title:</strong> {videoInfo.title}</p>
                <p><strong>Description:</strong> {videoInfo.description}</p>
                <p><strong>Video Group ID:</strong> {videoInfo.videoGroupId}</p>
            </div>
        </div>
    );
};

export default Video;
