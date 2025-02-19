import React, { useEffect, useState, createContext, useContext, useCallback } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate, Outlet, useLocation } from 'react-router-dom';
import authService from './auth';
import Projects from './pages/Projects';
import SubjectDetails from './pages/SubjectDetails';
import VideoGroups from './pages/VideoGroups';
import ProjectDetails from './pages/ProjectDetails';
import VideoGroupsDetails from './pages/VideoGroupsDetails';
import Video from './pages/Videos';
import AddVideo from './pages/VideoAdd';
import AddSubject from './pages/SubjectAdd';
import AddVideoGroup from './pages/VideoGroupAdd';
import AddProjectPage from './pages/ProjectAdd';
import EditProjectPage from './pages/ProjectEdit';
import AddLabel from './pages/LabelAdd';
import EditLabel from './pages/LabelEdit';
import SubjectVideoGroupAssignmentDetails from './pages/SubjectVideoGroupAssignmentDetails';
import Login from './pages/Login';

// Utwórz kontekst uwierzytelnienia
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

// Hook do u¿ycia kontekstu uwierzytelnienia
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
        <nav className="navbar">
            <ul>
                {isAuthenticated ? (
                    <>
                        <li><Link to="/projects" className="nav-link">Scientist</Link></li>
                        <li><Link to="/videos/1" className="nav-link">User</Link></li>
                        <li><button onClick={handleLogout} className="nav-link logout-btn">Logout</button></li>
                    </>
                ) : (
                    <li><Link to="/login" className="nav-link">Login</Link></li>
                )}
            </ul>
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

                        <Route path="/video-groups" element={<VideoGroups />} />
                        <Route path="/video-groups/:id" element={<VideoGroupsDetails />} />

                        <Route path="/videos/:id" element={<Video />} />
                        <Route path="/videos/add" element={<AddVideo />} />

                        <Route path="/video/:id" element={<Video />} />

                        <Route path="/subject-video-group-assignments/:id" element={<SubjectVideoGroupAssignmentDetails />} />

                        <Route path="/labels/add" element={<AddLabel />} />
                        <Route path="/labels/edit/:id" element={<EditLabel />} />
                    </Route>

                    <Route path="*" element={<Navigate to="/projects" replace />} />
                </Routes>
            </Router>
        </AuthProvider>
    );
}

export default App;