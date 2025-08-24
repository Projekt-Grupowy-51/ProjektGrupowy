import React from "react";
import {
    BrowserRouter as Router,
    Routes,
    Route,
} from "react-router-dom";
import ProjectAddPage from "./pages/ProjectAdd.jsx";
import ProjectsPage from "./pages/Projects.jsx";
import ProjectEditPage from "./pages/ProjectEdit.jsx";
import ProjectDetailsPage from "./pages/ProjectDetails.jsx";
import SubjectDetailsPage from "./pages/SubjectDetails.jsx";
import SubjectAddPage from "./pages/SubjectAdd.jsx";
import SubjectEditPage from "./pages/SubjectEdit.jsx";
import VideoDetailsPage from "./pages/VideoDetails.jsx";
import VideoGroupAddPage from "./pages/VideoGroupAdd.jsx";
import VideoGroupDetailsPage from "./pages/VideoGroupDetails.jsx";
import LabelAddPage from "./pages/LabelAdd.jsx";
import LabelEditPage from "./pages/LabelEdit.jsx";
import SubjectVideoGroupAssignmentAddPage from './pages/SubjectVideoGroupAssignmentAdd.jsx';
import './App.css';
import './i18n.js';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
    return (
        <Router>
            <div className="App">
                <Routes>
                    <Route path="/projects" element={<ProjectsPage />} />
                    <Route path="/projects/add" element={<ProjectAddPage />} />
                    <Route path="/projects/:id" element={<ProjectDetailsPage />} />
                    <Route path="/projects/:id/edit" element={<ProjectEditPage />} />
                    <Route path="/subjects/:id" element={<SubjectDetailsPage />} />
                    <Route path="/subjects/:id/edit" element={<SubjectEditPage />} />
                    <Route path="/subjects/add" element={<SubjectAddPage />} />
                    <Route path="/videogroups/add" element={<VideoGroupAddPage />} />
                    <Route path="/videogroups/:id" element={<VideoGroupDetailsPage />} />
                    <Route path="/labels/add" element={<LabelAddPage />} />
                    <Route path="/labels/:id/edit" element={<LabelEditPage />} />
                    <Route path="/projects/:projectId/assignments/add" element={<SubjectVideoGroupAssignmentAddPage />} />
                    <Route path="/videos/:id" element={<VideoDetailsPage />} />
                    <Route path="/" element={
                        <div className="container mt-5">
                            <h1>Welcome to Refactored App</h1>
                            <p>This is a clean, refactored version of your frontend.</p>
                            <div className="mt-4">
                                <a href="/projects" className="btn btn-primary">
                                    <i className="fas fa-folder-open me-2"></i>
                                    View Projects
                                </a>
                            </div>
                        </div>
                    } />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
