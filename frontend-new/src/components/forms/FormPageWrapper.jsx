import React from 'react';

const FormPageWrapper = ({ title, children, maxWidth = 600 }) => (
    <div className="d-flex justify-content-center py-5">
        <div
            className="water-tile p-4"
            style={{
                width: '100%',
                maxWidth: `${maxWidth}px`,
            }}
        >
            <h1 className="mb-4 text-primary">{title}</h1>
            {children}
        </div>
    </div>
);

export default FormPageWrapper;
