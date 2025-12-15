import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { Navigate, useLocation, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useKeycloak } from "../../hooks/useKeycloak.js";
import { Container, Card, Button } from "../ui";
import { LoadingSpinner } from "../common";
import TopNavbar from "../layout/TopNavbar";

const ProtectedRoute = ({
  children,
  allowedRoles = null,
  redirectTo = "/forbidden",
  autoRedirectFromRoot = true,
}) => {
  const { isAuthenticated, isLoading, login, user, getToken } = useKeycloak();
  const { t } = useTranslation("common");
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    if (
      autoRedirectFromRoot &&
      isAuthenticated &&
      user &&
      location.pathname === "/"
    ) {
      const token = getToken();
      if (!token) {
        return;
      }

      console.log("[ProtectedRoute] Auto-redirect check:", {
        userRole: user.userRole,
        pathname: location.pathname,
        userObject: user,
      });

      switch (user.userRole) {
        case "Scientist":
          navigate("/projects", { replace: true });
          break;
        case "Labeler":
          navigate("/labeler-video-groups", { replace: true });
          break;
        default:
          navigate("/forbidden", { replace: true });
      }
    }
  }, [
    isAuthenticated,
    user,
    navigate,
    location.pathname,
    autoRedirectFromRoot,
    getToken,
  ]);

  if (isLoading) {
    return (
      <Container className="d-flex justify-content-center align-items-center vh-100">
        <LoadingSpinner message={t("auth.initializing")} />
      </Container>
    );
  }

  if (!isAuthenticated) {
    return (
      <>
        <TopNavbar />
        <Container
          className="d-flex justify-content-center align-items-center"
          style={{ height: "86vh" }}
        >
          <Card
            className="text-center"
            style={{ maxWidth: "400px", width: "100%" }}
          >
            <Card.Header>
              <Card.Title level={3}>
                <i className="fas fa-lock me-2"></i>
                {t("auth.required_title")}
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <p className="text-muted mb-4">{t("auth.required_message")}</p>
              <Button
                variant="primary"
                onClick={login}
                icon="fas fa-sign-in-alt"
              >
                {t("auth.login_button")}
              </Button>
            </Card.Body>
          </Card>
        </Container>
      </>
    );
  }
  if (isAuthenticated && !user) {
    return (
      <Container className="d-flex justify-content-center align-items-center vh-100">
        <LoadingSpinner message={t("auth.initializing")} />
      </Container>
    );
  }

  if (allowedRoles && (!user || !allowedRoles.includes(user.userRole))) {
    console.error("[ProtectedRoute] Role check failed:", {
      allowedRoles,
      userRole: user?.userRole,
      userObject: user,
      redirectTo,
    });
    return <Navigate to={redirectTo} replace />;
  }

  return children;
};

ProtectedRoute.propTypes = {
  children: PropTypes.node.isRequired,
  allowedRoles: PropTypes.arrayOf(PropTypes.string),
  redirectTo: PropTypes.string,
  autoRedirectFromRoot: PropTypes.bool,
};

export default ProtectedRoute;
