import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

// Komponenty dla podstron

import Projects from './pages/Projects';
import Subjects from './pages/Subjects';
import SubjectDetails from './pages/SubjectDetails';
import VideoGroups from './pages/VideoGroups';
import ProjectDetails from './pages/ProjectDetails';
import VideoGroupsDetails from './pages/VideoGroupsDetails';
import Video from './pages/Video';
import AddSubject from './pages/SubjectAdd';
import AddVideoGroup from './pages/VideoGroupAdd';
import AddLabel from './pages/LabelAdd';
import SubjectVideoGroupAssignmentDetails from './pages/SubjectVideoGroupAssignmentDetails';


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
                {/* Project Routes */}
                <Route path="/projects" element={<Projects />} />
                <Route path="/projects/:id" element={<ProjectDetails />} />

                {/* Subject Routes */}
                <Route path="/subjects" element={<Subjects />} />
                <Route path="/subjects/:id" element={<SubjectDetails />} />
                <Route path="/subjects/add" element={<AddSubject />} />

                {/* Video Group Routes */}
                <Route path="/video-groups" element={<VideoGroups />} />
                <Route path="/video-groups/:id" element={<VideoGroupsDetails />} />
                <Route path="/video-groups/add" element={<AddVideoGroup />} />

                {/* Video Routes */}
                <Route path="/videos/:id" element={<Video />} />

                <Route path="/video/:id" element={<Video />} />

                { /* Subject Video Group Assignments Routes */ }

                <Route path="/subject-video-group-assignments/:id" element={<SubjectVideoGroupAssignmentDetails/> } />

                {/* Label Routes */}
                <Route path="/labels/add" element={<AddLabel />} />
            </Routes>
        </Router>
    );
}

export default App;
