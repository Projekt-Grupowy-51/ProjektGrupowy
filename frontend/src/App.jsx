import React, { useEffect, useState, createContext, useContext, useCallback } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate, Outlet, useLocation } from 'react-router-dom';
import authService from './auth';
import Projects from './pages/Projects';
import SubjectDetails from './pages/SubjectDetails';
import ProjectDetails from './pages/ProjectDetails';
import VideoGroupsDetails from './pages/VideoGroupsDetails';
import Video from './pages/Videos';
import AddVideo from './pages/VideoAdd';
import AddSubject from './pages/SubjectAdd';
import AddVideoGroup from './pages/VideoGroupAdd';
import AddProjectPage from './pages/ProjectAdd';
import EditProjectPage from './pages/ProjectEdit';
import LabelAdd from './pages/LabelAdd';
import EditLabel from './pages/LabelEdit';
import SubjectVideoGroupAssignmentDetails from './pages/SubjectVideoGroupAssignmentDetails';
import LabelerVideoGroups from './pages/LabelerVideoGroups';
import Login from './pages/Login';
import SubjectVideoGroupAssignmentAdd from './pages/SubjectVideoGroupAssignmentAdd';

// Utw�rz kontekst uwierzytelnienia
const AuthContext = createContext();

// Provider kontekstu uwierzytelnienia
const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(null);

    const checkAuth = useCallback(async () => {
        try {
            const response = await authService.verifyToken();
            setIsAuthenticated(response.isAuthenticated);
        } catch {
            setIsAuthenticated(false);
        }
    }, []);

    const handleLogout = useCallback(async () => {
        try {
            await authService.logout();
            setIsAuthenticated(false);
        } catch (error) {
            console.error('Logout error:', error);
        }
    }, []);

    useEffect(() => {
        checkAuth();
    }, [checkAuth]);

    return (
        <AuthContext.Provider value={{ isAuthenticated, checkAuth, handleLogout }}>
            {children}
        </AuthContext.Provider>
    );
};

// Hook do u�ycia kontekstu uwierzytelnienia
const useAuth = () => {
    return useContext(AuthContext);
};

const ProtectedRoute = () => {
    const { isAuthenticated, checkAuth } = useAuth();
    const location = useLocation();

    useEffect(() => {
        checkAuth();
    }, [location, checkAuth]);

    if (isAuthenticated === null) {
        return <div className="loading">Loading...</div>;
    }

    return isAuthenticated ? <Outlet /> : <Navigate to="/login" state={{ from: location }} replace />;
};

const Navbar = () => {
    const { isAuthenticated, handleLogout } = useAuth();

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container-fluid justify-content-center">
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                    aria-controls="navbarNav"
                    aria-expanded="false"
                    aria-label="Toggle navigation"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse justify-content-center" id="navbarNav">
                    <ul className="navbar-nav">
                        {isAuthenticated ? (
                            <>
                                <li className="nav-item">
                                    <Link to="/projects" className="nav-link text-white">Scientist</Link>
                                </li>
                                <li className="nav-item">
                                    <Link to="/labeler-video-groups/1" className="nav-link text-white">User</Link>
                                </li>
                                <li className="nav-item" style={{ width: '100%' }}>
                                    <button onClick={handleLogout} className="btn btn-danger nav-link text-white w-100">Logout</button>
                                </li>
                            </>
                        ) : (
                            <li className="nav-item">
                                <Link to="/login" className="nav-link text-white">Login</Link>
                            </li>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
};

function App() {
    return (
        <AuthProvider>
            <Router>
                <Navbar />
                <Routes>
                    <Route path="/login" element={<Login />} />

                    <Route element={<ProtectedRoute />}>
                        <Route path="/projects" element={<Projects />} />
                        <Route path="/projects/:id" element={<ProjectDetails />} />
                        <Route path="/projects/edit/:id" element={<EditProjectPage />} />
                        <Route path="/projects/add" element={<AddProjectPage />} />

                        <Route path="/subjects/:id" element={<SubjectDetails />} />
                        <Route path="/subjects/add" element={<AddSubject />} />

                        <Route path="/video-groups/add" element={<AddVideoGroup />} />
                        <Route path="/video-groups/:id" element={<VideoGroupsDetails />} />

                        <Route path="/videos/:id" element={<Video />} />
                        <Route path="/videos/add" element={<AddVideo />} />

                        <Route path="/video/:id" element={<Video />} />

                        <Route path="/assignments/:id" element={<SubjectVideoGroupAssignmentDetails />} />
                        <Route path="/assignments/add" element={<SubjectVideoGroupAssignmentAdd />} />

                        <Route path="/subject-video-group-assignments/:id" element={<SubjectVideoGroupAssignmentDetails />} />
                        <Route path="/labeler-video-groups/:id" element={<LabelerVideoGroups />} />

                        <Route path="/labels/add" element={<LabelAdd />} />
                        <Route path="/labels/edit/:id" element={<EditLabel />} />
                    </Route>

                    <Route path="*" element={<Navigate to="/projects" replace />} />
                </Routes>
            </Router>
        </AuthProvider>
    );
}

export default App;