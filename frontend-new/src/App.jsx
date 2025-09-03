import React from "react";
import {
    BrowserRouter as Router,
    Routes,
    Route,
} from "react-router-dom";
import AuthGuard from "./components/auth/AuthGuard.jsx";
import TopNavbar from "./components/layout/TopNavbar.jsx";
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
import VideoAddPage from "./pages/VideoAdd.jsx";
import LabelAddPage from "./pages/LabelAdd.jsx";
import LabelEditPage from "./pages/LabelEdit.jsx";
import SubjectVideoGroupAssignmentAddPage from './pages/SubjectVideoGroupAssignmentAdd.jsx';
import ErrorPage from './pages/errors/Error.jsx';
import ForbiddenPage from './pages/errors/Forbidden.jsx';
import NotFoundPage from './pages/errors/NotFound.jsx';
import LabelerVideoGroupsPage from './pages/LabelerVideoGroups.jsx';
import SubjectVideoGroupAssignmentDetailsPage from './pages/SubjectVideoGroupAssignmentDetails.jsx';
import VideoLabelingInterfacePage from './pages/VideoLabeling/VideoLabelingInterface.jsx';
import './App.css';
import './i18n.js';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

function App() {
    return (
        <Router>
            <AuthGuard>
                <div className="App">
                    <TopNavbar />
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
                        <Route path="/videos/add" element={<VideoAddPage />} />
                        <Route path="/labels/add" element={<LabelAddPage />} />
                        <Route path="/labels/:id/edit" element={<LabelEditPage />} />
                        <Route path="/projects/:projectId/assignments/add" element={<SubjectVideoGroupAssignmentAddPage />} />
                        <Route path="/assignments/:id" element={<SubjectVideoGroupAssignmentDetailsPage />} />
                        <Route path="/videos/:id" element={<VideoDetailsPage />} />
                        <Route path="/error" element={<ErrorPage />} />
                        <Route path="/forbidden" element={<ForbiddenPage />} />
                        <Route path="/not-found" element={<NotFoundPage />} />
                        <Route path="/labeler-video-groups" element={<LabelerVideoGroupsPage />} />
                        <Route path="/video-labeling/:assignmentId" element={<VideoLabelingInterfacePage />} />
                        <Route path="/" element={
                            <div className="container mt-5">
                                <h1>Welcome to VidMark</h1>
                                <p>Video analysis and labeling platform</p>
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
            </AuthGuard>
        </Router>
    );
}

export default App;
