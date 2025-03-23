import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import authService from '../auth';
import './css/ScientistProjects.css';

const roleMap = {
    Labeler: "Labeler",
    Scientist: "Scientist"
};

const AuthPage = () => {
    const [isLoginView, setIsLoginView] = useState(true);
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        role: 'Labeler'
    });
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleInputChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        if (!isLoginView && formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            setLoading(false);
            return;
        }

        try {
            if (isLoginView) {
                await authService.login(formData.username, formData.password);
                navigate('/projects');
            } else {
                await authService.register({
                    userName: formData.username,
                    email: formData.email,
                    password: formData.password,
                    role: roleMap[formData.role]
                });
                // On successful registration, switch to login view
                setIsLoginView(true);
                setFormData({ ...formData, password: '', confirmPassword: '' });
                alert('Registration successful! Please log in.');
            }
        } catch (err) {
            setError(err.message || (isLoginView ? 'Invalid credentials' : 'Registration failed'));
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container py-5">
            <div className="row justify-content-center">
                <div className="col-lg-6 col-md-8">
                    <div className="card shadow-sm">
                        <div className="card-header bg-primary text-white text-center py-4">
                            <h1 className="heading mb-0">
                                {isLoginView ? 'Welcome Back!' : 'Create Account'}
                            </h1>
                            <p className="mt-2 mb-0">
                                {isLoginView ? 'Sign in to continue' : 'Join our labeling community'}
                            </p>
                        </div>
                        
                        <div className="card-body p-4">
                            <ul className="nav nav-pills nav-justified mb-4">
                                <li className="nav-item">
                                    <button
                                        className={`nav-link w-100 ${isLoginView ? 'active' : ''}`}
                                        onClick={() => setIsLoginView(true)}
                                    >
                                        <i className="fas fa-sign-in-alt me-2"></i>Login
                                    </button>
                                </li>
                                <li className="nav-item">
                                    <button
                                        className={`nav-link w-100 ${!isLoginView ? 'active' : ''}`}
                                        onClick={() => setIsLoginView(false)}
                                    >
                                        <i className="fas fa-user-plus me-2"></i>Register
                                    </button>
                                </li>
                            </ul>

                            {error && (
                                <div className="alert alert-danger mb-3">
                                    <i className="fas fa-exclamation-circle me-2"></i>
                                    {error}
                                </div>
                            )}

                            <form onSubmit={handleSubmit}>
                                <div className="mb-3">
                                    <label htmlFor="username" className="form-label">Username</label>
                                    <div className="input-group">
                                        <span className="input-group-text">
                                            <i className="fas fa-user"></i>
                                        </span>
                                        <input
                                            type="text"
                                            id="username"
                                            name="username"
                                            className="form-control"
                                            placeholder="Enter your username"
                                            value={formData.username}
                                            onChange={handleInputChange}
                                            required
                                            disabled={loading}
                                        />
                                    </div>
                                </div>

                                {!isLoginView && (
                                    <>
                                        <div className="mb-3">
                                            <label htmlFor="email" className="form-label">Email address</label>
                                            <div className="input-group">
                                                <span className="input-group-text">
                                                    <i className="fas fa-envelope"></i>
                                                </span>
                                                <input
                                                    type="email"
                                                    id="email"
                                                    name="email"
                                                    className="form-control"
                                                    placeholder="Enter your email"
                                                    value={formData.email}
                                                    onChange={handleInputChange}
                                                    required
                                                    disabled={loading}
                                                />
                                            </div>
                                        </div>
                                        <div className="mb-3">
                                            <label htmlFor="role" className="form-label">Role</label>
                                            <div className="input-group">
                                                <span className="input-group-text">
                                                    <i className="fas fa-user-tag"></i>
                                                </span>
                                                <select
                                                    id="role"
                                                    name="role"
                                                    className="form-select"
                                                    value={formData.role}
                                                    onChange={handleInputChange}
                                                    required
                                                    disabled={loading}
                                                >
                                                    <option value="Labeler">Labeler</option>
                                                    <option value="Scientist">Scientist</option>
                                                </select>
                                            </div>
                                        </div>
                                    </>
                                )}

                                <div className="mb-3">
                                    <label htmlFor="password" className="form-label">Password</label>
                                    <div className="input-group">
                                        <span className="input-group-text">
                                            <i className="fas fa-lock"></i>
                                        </span>
                                        <input
                                            type="password"
                                            id="password"
                                            name="password"
                                            className="form-control"
                                            placeholder="Enter your password"
                                            value={formData.password}
                                            onChange={handleInputChange}
                                            required
                                            disabled={loading}
                                        />
                                    </div>
                                    {!isLoginView && (
                                        <div className="form-text text-muted">
                                            <small>
                                                Password must be 8-255 characters and include:
                                                <ul className="mb-0 ps-3">
                                                    <li>Lowercase letter</li>
                                                    <li>Uppercase letter</li>
                                                    <li>Number</li>
                                                </ul>
                                            </small>
                                        </div>
                                    )}
                                </div>

                                {!isLoginView && (
                                    <div className="mb-3">
                                        <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                                        <div className="input-group">
                                            <span className="input-group-text">
                                                <i className="fas fa-lock"></i>
                                            </span>
                                            <input
                                                type="password"
                                                id="confirmPassword"
                                                name="confirmPassword"
                                                className="form-control"
                                                placeholder="Confirm your password"
                                                value={formData.confirmPassword}
                                                onChange={handleInputChange}
                                                required
                                                disabled={loading}
                                            />
                                        </div>
                                    </div>
                                )}

                                <button 
                                    type="submit" 
                                    className="btn btn-primary w-100 mt-3 py-2"
                                    disabled={loading}
                                >
                                    {loading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                            {isLoginView ? 'Signing in...' : 'Creating account...'}
                                        </>
                                    ) : (
                                        <>
                                            <i className={`fas ${isLoginView ? 'fa-sign-in-alt' : 'fa-user-plus'} me-2`}></i>
                                            {isLoginView ? 'Sign In' : 'Create Account'}
                                        </>
                                    )}
                                </button>
                            </form>

                            <div className="text-center mt-4">
                                {isLoginView ? (
                                    <p className="mb-0">
                                        Don't have an account?{' '}
                                        <button
                                            className="btn btn-link p-0"
                                            onClick={() => setIsLoginView(false)}
                                            type="button"
                                        >
                                            Sign up now
                                        </button>
                                    </p>
                                ) : (
                                    <p className="mb-0">
                                        Already registered?{' '}
                                        <button
                                            className="btn btn-link p-0"
                                            onClick={() => setIsLoginView(true)}
                                            type="button"
                                        >
                                            Sign in here
                                        </button>
                                    </p>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;