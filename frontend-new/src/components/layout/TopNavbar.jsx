import React from 'react';
import { useTranslation } from 'react-i18next';
import { useKeycloak } from '../../hooks/useKeycloak.js';

const TopNavbar = () => {
  const { isAuthenticated, user, logout } = useKeycloak();
  const { t, i18n } = useTranslation('common');

  const changeLanguage = (language) => {
    i18n.changeLanguage(language);
  };

  const getCurrentLanguage = () => {
    return i18n.language || 'en';
  };

  const getLanguageFlag = (lang) => {
    const flags = {
      en: '🇺🇸',
      pl: '🇵🇱'
    };
    return flags[lang] || '🌐';
  };

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
          {/* Language Selector */}
          <div className="nav-item dropdown me-3">
            <button 
              className="btn btn-link nav-link dropdown-toggle text-white text-decoration-none d-flex align-items-center"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <span className="me-2">{getLanguageFlag(getCurrentLanguage())}</span>
              {getCurrentLanguage().toUpperCase()}
            </button>
            <ul className="dropdown-menu dropdown-menu-end">
              <li>
                <button 
                  className={`dropdown-item ${getCurrentLanguage() === 'en' ? 'active' : ''}`}
                  onClick={() => changeLanguage('en')}
                >
                  <span className="me-2">🇺🇸</span>
                  {t('language.en')}
                </button>
              </li>
              <li>
                <button 
                  className={`dropdown-item ${getCurrentLanguage() === 'pl' ? 'active' : ''}`}
                  onClick={() => changeLanguage('pl')}
                >
                  <span className="me-2">🇵🇱</span>
                  {t('language.pl')}
                </button>
              </li>
            </ul>
          </div>

          {/* User Dropdown */}
          <div className="nav-item dropdown">
            <button 
              className="btn btn-link nav-link dropdown-toggle text-white text-decoration-none"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <i className="fas fa-user-circle me-2"></i>
              {user?.preferred_username || user?.name || t('user.fallback')}
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
                  {t('buttons.logout')}
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