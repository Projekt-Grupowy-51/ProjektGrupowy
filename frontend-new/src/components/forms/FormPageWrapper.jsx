import React from 'react';

const FormPageWrapper = ({ title, children, maxWidth = 600 }) => (
    <div className="d-flex justify-content-center py-5">
        <div
            style={{
                width: '100%',
                maxWidth: `${maxWidth}px`,
                padding: '2rem',
                borderRadius: '12px',
                boxShadow: '0 4px 20px rgba(0,0,0,0.1)',
                backgroundColor: '#fff',
            }}
        >
            <h1 className="mb-4">{title}</h1>
            {children}
        </div>
    </div>
);

export default FormPageWrapper;
