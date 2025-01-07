import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

// Komponenty dla podstron

import ScientistProjects from './pages/ScientistProjects';
import ScientistSubjects from './pages/ScientistSubjects';
import ScientistSubjectDetails from './pages/ScientistSubjectDetails';
import ScientistVideoGroups from './pages/ScientistVideoGroups';
import ScientistProjectDetails from './pages/ScientistProjectDetails';
import Video from './pages/Video';


function App() {
    return (
        <Router>
            <nav className="navbar">
                <ul>
                    <li><Link to="/projects" className="nav-link">Projects</Link></li>
                    <li><Link to="/subjects" className="nav-link">Subjects</Link></li>
                    <li><Link to="/videoGroups" className="nav-link">Video Groups</Link></li>
                    <li><Link to="/video/1" className="nav-link">Video 1</Link></li>
                </ul>
            </nav>
            <Routes>
                <Route path="/projects/:id" element={<ScientistProjectDetails />} />
                <Route path="/projects" element={<ScientistProjects />} />
                <Route path="/subjects" element={<ScientistSubjects />} />
                <Route path="/subjects/:id" element={<ScientistSubjectDetails />} />
                <Route path="/video/:id" element={<Video />} /> 
                <Route path="/videoGroups/" element={<ScientistVideoGroups />} /> 
            </Routes>
        </Router>
    );
}

export default App;
