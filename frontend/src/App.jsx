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
import VideoGroup from "./pages/Videos";
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
import { ModalProvider } from './context/ModalContext';
import { NotificationProvider } from './context/NotificationContext';
import NotificationSystem from './components/NotificationSystem';

const AuthContext = createContext();

const AuthProvider = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(null);
  const [roles, setRoles] = useState([]); // Store user roles
  const [user, setUser] = useState(null); // Store user details
  const [authError, setAuthError] = useState(null); // Store authentication errors

  // Check authentication and fetch user details
  const checkAuth = useCallback(async () => {
    try {
      const response = await authService.verifyToken();
      setIsAuthenticated(response.isAuthenticated);
      setRoles(response.roles || []);
      setUser(response.username || null);
      setAuthError(null);
    } catch (error) {
      setIsAuthenticated(false);
      setRoles([]);
      setUser(null);
      setAuthError("Failed to verify authentication.");
    }
  }, []);

  // Handle login and update authentication state
  const handleLogin = useCallback(
    async (username, password) => {
      try {
        await authService.login(username, password);
        await checkAuth(); // Re-check authentication after login
        setAuthError(null);
      } catch (error) {
        setAuthError("Login failed. Please check your credentials.");
        throw error;
      }
    },
    [checkAuth]
  );

  // Handle logout and clear authentication state
  const handleLogout = useCallback(async () => {
    try {
      await authService.logout();
      setIsAuthenticated(false);
      setRoles([]);
      setUser(null);
      setAuthError(null);
    } catch (error) {
      console.error("Logout error:", error);
      setAuthError("Failed to log out.");
    }
  }, []);

  // Check if the user has a specific role
  const hasRole = useCallback((role) => roles.includes(role), [roles]);

  // Automatically check authentication on component mount
  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        roles,
        user,
        authError,
        checkAuth,
        handleLogin,
        handleLogout,
        hasRole,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

const useAuth = () => {
  return useContext(AuthContext);
};

const ProtectedRoute = () => {
  const { isAuthenticated, checkAuth } = useAuth();
  const location = useLocation();

  if (isAuthenticated === null) {
    return <div className="loading">Loading...</div>;
  }

  return isAuthenticated ? (
    <Outlet />
  ) : (
    <Navigate to="/login" state={{ from: location }} replace />
  );
};

const Navbar = () => {
  const { isAuthenticated, roles, user, hasRole, handleLogout } = useAuth();

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
                      Scientist Dashboard
                    </Link>
                  </li>
                )}
                {hasRole("Labeler") && (
                  <li className="nav-item">
                    <Link
                      to="/labeler-video-groups/1"
                      className="nav-link text-white text-nowrap"
                    >
                      Labeler Dashboard
                    </Link>
                  </li>
                )}
                <li className="nav-item" style={{ width: "100%" }}>
                  <button
                    onClick={handleLogout}
                    className="btn btn-danger nav-link text-white w-100"
                  >
                    Logout
                  </button>
                </li>
              </>
            ) : (
              <li className="nav-item">
                <Link to="/login" className="nav-link text-white">
                  Login
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
        <Router>
          <ModalProvider>
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

                <Route path="/videos/:id" element={<VideoDetails />} />
                <Route path="/videos/add" element={<AddVideo />} />

                <Route path="/video-group/:id" element={<VideoGroup />} />

                <Route
                  path="/assignments/:id"
                  element={<SubjectVideoGroupAssignmentDetails />}
                />
                <Route
                  path="/assignments/add"
                  element={<SubjectVideoGroupAssignmentAdd />}
                />

                <Route
                  path="/subject-video-group-assignments/:id"
                  element={<SubjectVideoGroupAssignmentDetails />}
                />
                <Route
                  path="/labeler-video-groups/:id"
                  element={<LabelerVideoGroups />}
                />

                <Route path="/labels/add" element={<LabelAdd />} />
                <Route path="/labels/edit/:id" element={<EditLabel />} />
              </Route>

              <Route path="*" element={<Navigate to="/projects" replace />} />
            </Routes>
            <NotificationSystem />
          </ModalProvider>
        </Router>
      </AuthProvider>
    </NotificationProvider>
  );
}

export default App;
export { useAuth };
