import React from "react";

const Error = () => {

  return (
    <div className="container d-flex align-items-center justify-content-center mb-0">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-danger">Error</h1>
        <h2 className="mb-3">Connection Issue</h2>
        <p className="text-muted">
          We're having trouble connecting to the server. Please try again later.
        </p>


      </div>
    </div>
  );
};

export default Error;
