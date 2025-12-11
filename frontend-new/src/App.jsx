import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { ConfirmationProvider } from "./contexts/ConfirmationContext.jsx";
import { NotificationProvider } from "./contexts/NotificationContext.jsx";
import ConfirmationModal from "./components/ui/ConfirmationModal.jsx";
import NotificationToast from "./components/ui/NotificationToast.jsx";
import { useSignalR } from "./hooks/useSignalR.js";
import ProtectedRoute from "./components/auth/ProtectedRoute.jsx";
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
import VideoGroupEditPage from "./pages/VideoGroupEdit.jsx";
import VideoGroupDetailsPage from "./pages/VideoGroupDetails.jsx";
import VideoAddPage from "./pages/VideoAdd.jsx";
import LabelAddPage from "./pages/LabelAdd.jsx";
import LabelEditPage from "./pages/LabelEdit.jsx";
import SubjectVideoGroupAssignmentAddPage from "./pages/SubjectVideoGroupAssignmentAdd.jsx";
import ErrorPage from "./pages/errors/Error.jsx";
import ForbiddenPage from "./pages/errors/Forbidden.jsx";
import NotFoundPage from "./pages/errors/NotFound.jsx";
import LabelerVideoGroupsPage from "./pages/LabelerVideoGroups.jsx";
import SubjectVideoGroupAssignmentDetailsPage from "./pages/SubjectVideoGroupAssignmentDetails.jsx";
import VideoLabelingInterfacePage from "./pages/VideoLabeling/VideoLabelingInterface.jsx";
import "./App.css";
import "./i18n.js";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import "./styles/theme.css";
import "./styles/water-tiles.css";
import "./styles/loading.css";
import "./styles/tabs.css";
import "./styles/buttons-minimal.css";

function AppContent() {
  const { t } = useTranslation("common");

  // Initialize SignalR connection when authenticated
  useSignalR();

  return (
    <ProtectedRoute>
      <div className="App">
        <TopNavbar />
        <Routes>
          <Route
            path="/projects"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <ProjectsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/projects/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <ProjectAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/projects/:id"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <ProjectDetailsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/projects/:id/edit"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <ProjectEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/subjects/:id"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <SubjectDetailsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/subjects/:id/edit"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <SubjectEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/subjects/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <SubjectAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/videogroups/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <VideoGroupAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/videogroups/:id"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <VideoGroupDetailsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/videogroups/:id/edit"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <VideoGroupEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/videos/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <VideoAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/labels/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <LabelAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/labels/:id/edit"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <LabelEditPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/projects/:projectId/assignments/add"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <SubjectVideoGroupAssignmentAddPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/assignments/:id"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <SubjectVideoGroupAssignmentDetailsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/videos/:id"
            element={
              <ProtectedRoute
                allowedRoles={["Scientist"]}
                autoRedirectFromRoot={false}
              >
                <VideoDetailsPage />
              </ProtectedRoute>
            }
          />

          <Route
            path="/labeler-video-groups"
            element={
              <ProtectedRoute
                allowedRoles={["Labeler"]}
                autoRedirectFromRoot={false}
              >
                <LabelerVideoGroupsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/video-labeling/:assignmentId"
            element={
              <ProtectedRoute
                allowedRoles={["Labeler"]}
                autoRedirectFromRoot={false}
              >
                <VideoLabelingInterfacePage />
              </ProtectedRoute>
            }
          />

          {/* Public routes (accessible to all authenticated users) */}
          <Route path="/error" element={<ErrorPage />} />
          <Route path="/forbidden" element={<ForbiddenPage />} />
          <Route path="/not-found" element={<NotFoundPage />} />
          <Route
            path="/"
            element={
              <div className="container py-5">
                <div className="row justify-content-center">
                  <div className="col-lg-8 text-center">
                    <div className="water-tile p-5 mb-4">
                      <h1 className="display-4 fw-bold text-primary mb-3">
                        {t("home.welcome_title")}
                      </h1>
                      <p className="lead text-muted mb-4">
                        {t("home.welcome_subtitle")}
                      </p>
                      <div className="d-flex flex-wrap justify-content-center gap-3">
                        <a href="/projects" className="btn btn-primary btn-lg">
                          <i className="fas fa-folder-open me-2"></i>
                          {t("home.view_projects")}
                        </a>
                        <a
                          href="/labeler-video-groups"
                          className="btn btn-outline-primary btn-lg"
                        >
                          <i className="fas fa-video me-2"></i>
                          Video Labeling
                        </a>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            }
          />
        </Routes>
      </div>
    </ProtectedRoute>
  );
}

function App() {
  return (
    <NotificationProvider>
      <ConfirmationProvider>
        <Router>
          <AppContent />
          <NotificationToast />
          <ConfirmationModal />
        </Router>
      </ConfirmationProvider>
    </NotificationProvider>
  );
}

export default App;
