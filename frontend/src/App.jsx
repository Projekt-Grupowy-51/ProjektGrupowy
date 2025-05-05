import React, {
    useEffect,
    useState,
    createContext,
    useContext,
    useCallback,
} from "react";
import {
    BrowserRouter as Router,
    Routes,
    Route,
    Link,
    Navigate,
    Outlet,
    useLocation,
} from "react-router-dom";
import authService from "./auth";
import Projects from "./pages/Projects";
import SubjectDetails from "./pages/SubjectDetails";
import ProjectDetails from "./pages/ProjectDetails";
import VideoGroupsDetails from "./pages/VideoGroupsDetails";
import VideoGroup from "./pages/Videos.jsx";
import VideoDetails from "./pages/VideoDetails";
import AddVideo from "./pages/VideoAdd";
import AddSubject from "./pages/SubjectAdd";
import AddVideoGroup from "./pages/VideoGroupAdd";
import AddProjectPage from "./pages/ProjectAdd";
import EditProjectPage from "./pages/ProjectEdit";
import LabelAdd from "./pages/LabelAdd";
import EditLabel from "./pages/LabelEdit";
import SubjectVideoGroupAssignmentDetails from "./pages/SubjectVideoGroupAssignmentDetails";
import LabelerVideoGroups from "./pages/LabelerVideoGroups";
import Login from "./pages/Login";
import SubjectVideoGroupAssignmentAdd from "./pages/SubjectVideoGroupAssignmentAdd";
import { ModalProvider } from "./context/ModalContext";
import { NotificationProvider } from "./context/NotificationContext";
import NotFound from "./pages/errors/NotFound";
import Forbidden from "./pages/errors/Forbidden";
import Error from "./pages/errors/Error";
import NotificationSystem from "./components/NotificationSystem";
import SignalRListener from "../src/services/SignalRListener";
import { useTranslation } from 'react-i18next';
import './i18n.js'
import { AuthProvider, useAuth } from "./context/AuthContext";

const RoleProtectedRoute = ({ allowedRoles }) => {
    const { isAuthenticated, roles } = useAuth();
    const location = useLocation();

    const hasAccess = roles.some(role => allowedRoles.includes(role));

    if (!isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }
    if (!hasAccess) {
        return <Navigate to="/forbidden" replace />;
    }

    return <Outlet />;
};


const Navbar = () => {
    const { isAuthenticated, hasRole, handleLogout } = useAuth();
    const { t, i18n } = useTranslation(['common']);
    const changeLanguage = (lang) => {
        if (i18n.language !== lang) {
            i18n.changeLanguage(lang);
        }
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container-fluid justify-content-center">
                <div className="btn-group me-3" role="group" aria-label="Language selector">
                    <button
                        className={`btn ${i18n.language === 'en' ? 'btn-light text-dark' : 'btn-outline-light'}`}
                        onClick={() => changeLanguage('en')}
                    >
                        EN
                    </button>
                    <button
                        className={`btn ${i18n.language === 'pl' ? 'btn-light text-dark' : 'btn-outline-light'}`}
                        onClick={() => changeLanguage('pl')}
                    >
                        PL
                    </button>
                </div>
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
                <div
                    className="collapse navbar-collapse justify-content-center"
                    id="navbarNav"
                >
                    <ul className="navbar-nav">
                        {isAuthenticated ? (
                            <>
                                {hasRole("Scientist") && (
                                    <li className="nav-item">
                                        <Link
                                            to="/projects"
                                            className="nav-link text-white text-nowrap"
                                        >
                                            {t('scientistDashboard')}
                                        </Link>
                                    </li>
                                )}
                                {hasRole("Labeler") && (
                                    <li className="nav-item">
                                        <Link
                                            to="/labeler-video-groups"
                                            className="nav-link text-white text-nowrap"
                                        >
                                            {t('labelerDashboard')}
                                        </Link>
                                    </li>
                                )}
                                <li className="nav-item" style={{ width: "100%" }}>
                                    <button
                                        onClick={handleLogout}
                                        className="btn btn-danger nav-link text-white w-100"
                                    >
                                        {t('logout')}
                                    </button>
                                </li>
                            </>
                        ) : (
                            <li className="nav-item">
                                <Link to="/login" className="nav-link text-white">
                                    {t('common:login')}
                                </Link>
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
        <NotificationProvider>
            <AuthProvider>
                <SignalRListener />
                <Router>
                    <ModalProvider>
                        <Navbar />
                        <Routes>
                            <Route path="/" element={<Login />} />
                            <Route path="/login" element={<Login />} />

                            <Route element={<RoleProtectedRoute allowedRoles={["Labeler"]} />}>
                                <Route path="/labeler-video-groups" element={<LabelerVideoGroups />} />
                                <Route path="/video-group/:id" element={<VideoGroup />} />
                            </Route>

                            <Route element={<RoleProtectedRoute allowedRoles={["Scientist"]} />}>
                                <Route path="/projects" element={<Projects />} />
                                <Route path="/projects/:id" element={<ProjectDetails />} />
                                <Route path="/projects/edit/:id" element={<EditProjectPage />} />
                                <Route path="/projects/add" element={<AddProjectPage />} />
                                <Route path="/subjects/:id" element={<SubjectDetails />} />
                                <Route path="/subjects/add" element={<AddSubject />} />
                                <Route path="/video-groups/add" element={<AddVideoGroup />} />
                                <Route path="/video-groups/:id" element={<VideoGroupsDetails />} />
                                <Route path="/videos/:id" element={<VideoDetails />} />
                                <Route path="/videos/add" element={<AddVideo />} />
                                <Route path="/assignments/:id" element={<SubjectVideoGroupAssignmentDetails />} />
                                <Route path="/assignments/add" element={<SubjectVideoGroupAssignmentAdd />} />
                                <Route path="/subject-video-group-assignments/:id" element={<SubjectVideoGroupAssignmentDetails />} />
                                <Route path="/labels/add" element={<LabelAdd />} />
                                <Route path="/labels/edit/:id" element={<EditLabel />} />
                            </Route>

                            <Route path="/forbidden" element={<Forbidden />} />
                            <Route path="/error" element={<Error />} />

                            <Route path="*" element={<NotFound />} />
                        </Routes>
                        <NotificationSystem />
                    </ModalProvider>
                </Router>
            </AuthProvider>
        </NotificationProvider>
    );
}

export default App;