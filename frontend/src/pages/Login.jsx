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
    const navigate = useNavigate();

    const handleInputChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (!isLoginView && formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
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
            }
        } catch (err) {
            setError(err.message);
            // setError(isLoginView
            //     ? 'Invalid credentials'
            //     : 'Registration failed. User may already exist.');
        }
    };

    return (
        <div className="container auth-container">
            <div className="auth-header">
                <h1 className="heading">
                    {isLoginView ? 'Welcome Back!' : 'Create Account'}
                </h1>
                <p className="auth-subtitle">
                    {isLoginView ? 'Sign in to continue' : 'Join our labeling community'}
                </p>
            </div>

            <div className="tab-navigation auth-tabs">
                <button
                    className={`tab-button ${isLoginView ? 'active' : ''}`}
                    onClick={() => setIsLoginView(true)}
                >
                    Login
                </button>
                <button
                    className={`tab-button ${!isLoginView ? 'active' : ''}`}
                    onClick={() => setIsLoginView(false)}
                >
                    Register
                </button>
            </div>

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="form-group">
                    <input
                        type="text"
                        name="username"
                        placeholder="Username"
                        className="form-input"
                        value={formData.username}
                        onChange={handleInputChange}
                        required
                    />
                </div>

                {!isLoginView && (
                    <>
                        <div className="form-group">
                            <input
                                type="email"
                                name="email"
                                placeholder="Email address"
                                className="form-input"
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <select
                                name="role"
                                className="form-select"
                                value={formData.role}
                                onChange={handleInputChange}
                                required
                            >
                                <option value="Labeler">Labeler</option>
                                <option value="Scientist">Scientist</option>
                            </select>
                        </div>
                    </>
                )}

                <div className="form-group">
                    <input
                        type="password"
                        name="password"
                        placeholder="Password"
                        className="form-input"
                        value={formData.password}
                        onChange={handleInputChange}
                        required
                    />
                    {!isLoginView && (
                        <div className="password-hint">
                            8-255 characters<br/>
                            Lowercase letter<br/>
                            Uppercase letter<br/>
                            Number
                        </div>
                    )}
                </div>

                {!isLoginView && (
                    <div className="form-group">
                        <input
                            type="password"
                            name="confirmPassword"
                            placeholder="Confirm Password"
                            className="form-input"
                            value={formData.confirmPassword}
                            onChange={handleInputChange}
                            required
                        />
                    </div>
                )}

                {error && <div className="error-message">{error}</div>}

                <button type="submit" className="btn btn-primary auth-btn">
                    {isLoginView ? 'Sign In' : 'Create Account'}
                </button>
            </form>

            <div className="auth-footer">
                {isLoginView ? (
                    <p>
                        Don't have an account?{' '}
                        <button
                            className="btn btn-link"
                            onClick={() => setIsLoginView(false)}
                        >
                            Sign up now
                        </button>
                    </p>
                ) : (
                    <p>
                        Already registered?{' '}
                        <button
                            className="btn btn-link"
                            onClick={() => setIsLoginView(true)}
                        >
                            Sign in here
                        </button>
                    </p>
                )}
            </div>
        </div>
    );
};

export default AuthPage;