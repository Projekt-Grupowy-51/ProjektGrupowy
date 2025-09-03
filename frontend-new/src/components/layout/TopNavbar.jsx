import React from 'react';
import { useKeycloak } from '../../hooks/useKeycloak.js';

const TopNavbar = () => {
  const { isAuthenticated, user, logout } = useKeycloak();

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
      <div className="container-fluid">
        <a className="navbar-brand" href="/">
          <i className="fas fa-video me-2"></i>
          VidMark
        </a>
        
        <div className="navbar-nav ms-auto">
          <div className="nav-item dropdown">
            <button 
              className="btn btn-link nav-link dropdown-toggle text-white text-decoration-none"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <i className="fas fa-user-circle me-2"></i>
              {user?.preferred_username || user?.name || 'User'}
            </button>
            <ul className="dropdown-menu dropdown-menu-end">
              <li>
                <span className="dropdown-item-text">
                  <small className="text-muted">
                    {user?.email}
                  </small>
                </span>
              </li>
              <li><hr className="dropdown-divider" /></li>
              <li>
                <button 
                  className="dropdown-item" 
                  onClick={logout}
                >
                  <i className="fas fa-sign-out-alt me-2"></i>
                  Logout
                </button>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default TopNavbar;