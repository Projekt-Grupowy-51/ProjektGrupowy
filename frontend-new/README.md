# Frontend New - Refactored Application

## Overview
This is a refactored version of the frontend application with clean architecture, separation of concerns, and improved maintainability.

## Architecture

### 📁 Project Structure
```
src/
├── components/           # Reusable components
│   ├── ui/              # Basic UI components (Button, Card, Input, etc.)
│   ├── forms/           # Form components (ProjectForm, etc.)
│   └── features/        # Feature-specific components
│       └── projects/    # Project-related components
├── hooks/               # Custom React hooks
├── pages/               # Page components (routes)
├── services/            # API services
├── utils/               # Utility functions
├── languages/           # i18n translation files
├── App.jsx              # Main application component
├── main.jsx             # Application entry point
└── index.css            # Global styles
```

### 🏗️ Architecture Principles

1. **Separation of Concerns**
   - UI components handle only presentation
   - Custom hooks manage state and business logic
   - Services handle API communication

2. **Component Hierarchy**
   - `ui/` - Generic, reusable Bootstrap wrapper components
   - `forms/` - Form components that can be reused for create/edit
   - `features/` - Domain-specific components
   - `pages/` - Route components that compose other components

3. **State Management**
   - Local state with `useState`
   - Custom hooks for complex state logic
   - No external state management (keeping it simple)

## Components

### UI Components
- **Button** - Bootstrap button wrapper with variants
- **Card** - Bootstrap card with header, body, footer sub-components
- **Input** - Form input with validation and error handling
- **Container** - Bootstrap container with Row/Col sub-components
- **Table** - Bootstrap table wrapper
- **Alert** - Bootstrap alert component
- **Modal** - Simple modal component

### Custom Hooks
- **useProjects** - Manages projects state (CRUD operations)
- **useProject** - Fetches single project
- **useNotification** - Simple notification system

### Services
- **ApiClient** - Centralized HTTP client
- **ProjectService** - Project-specific API calls

## Features

### ✅ Implemented
- Project CRUD operations
- Clean form handling
- Responsive design with Bootstrap
- Internationalization (i18n)
- Loading states
- Error handling
- Validation

### 🎯 Benefits of Refactor
1. **Maintainability** - Clear separation of concerns
2. **Reusability** - Generic UI components
3. **Testability** - Isolated business logic in hooks
4. **Readability** - Simple, clean code structure
5. **Scalability** - Easy to add new features

## Getting Started

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

## Development Guidelines

1. **New UI Component**: Add to `components/ui/` and export from `index.js`
2. **New Feature**: Create folder in `components/features/`
3. **Business Logic**: Use custom hooks in `hooks/`
4. **API Calls**: Add to appropriate service in `services/`
5. **New Page**: Add to `pages/` and register route in `App.jsx`

## Technology Stack
- React 18
- React Router 7
- Bootstrap 5
- i18next
- Axios
- Vite
