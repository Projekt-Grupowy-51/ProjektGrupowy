import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import httpClient from "../httpclient";
import NavigateButton from "../components/NavigateButton";
import { useNotification } from "../context/NotificationContext";

const LabelEdit = () => {
  const { id } = useParams();
  const [labelData, setLabelData] = useState({
    name: "",
    colorHex: "#ffffff",
    type: "range",
    shortcut: "",
    subjectId: null,
  });
  const [subjectName, setSubjectName] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { addNotification } = useNotification();

  useEffect(() => {
    const fetchLabelData = async () => {
      try {
        const response = await httpClient.get(`/Label/${id}`);
        setLabelData(response.data);
        fetchSubjectName(response.data.subjectId);
      } catch (error) {
        // addNotification(
        //   error.response?.data?.message || "Failed to load label data",
        //   "error"
        // );
      }
    };

    const fetchSubjectName = async (subjectId) => {
      try {
        const response = await httpClient.get(`/subject/${subjectId}`);
        setSubjectName(response.data.name);
      } catch (error) {
        //addNotification("Failed to load subject information.", "error");
      }
    };

    if (id) fetchLabelData();
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setLabelData((prev) => ({ ...prev, [name]: value }));
  };

  const validateForm = () => {
    if (labelData.shortcut.length !== 1) {
      addNotification("Shortcut must be exactly one character.", "error");
      return false;
    }
    if (!/^#[0-9A-Fa-f]{6}$/.test(labelData.colorHex)) {
      addNotification("Color must be in the format #RRGGBB.", "error");
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    setLoading(true);
    try {
      await httpClient.put(`/Label/${id}`, labelData);
      //addNotification("Label updated successfully!", "success");
      navigate(`/subjects/${labelData.subjectId}`);
    } catch (error) {
      // addNotification(
      //   error.response?.data?.message || "Failed to update label",
      //   "error"
      // );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-lg-8">
          <div className="card shadow-sm">
            <div className="card-header bg-primary text-white">
              <h1 className="heading mb-0">Edit Label</h1>
            </div>
            <div className="card-body">
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name" className="form-label">
                    Label Name
                  </label>
                  <input
                    type="text"
                    id="name"
                    name="name"
                    value={labelData.name}
                    onChange={handleChange}
                    className="form-control"
                    required
                    disabled={loading}
                  />
                </div>

                <div className="mb-3">
                  <label htmlFor="colorHex" className="form-label">
                    Label Color
                  </label>
                  <div className="input-group">
                    <input
                      type="color"
                      id="colorHex"
                      name="colorHex"
                      value={labelData.colorHex}
                      onChange={handleChange}
                      className="form-control form-control-color"
                      title="Choose label color"
                      style={{ maxWidth: "60px" }}
                      disabled={loading}
                    />
                    <input
                      type="text"
                      className="form-control form-control-color"
                      value={labelData.colorHex}
                      onChange={(e) =>
                        setLabelData({ ...labelData, colorHex: e.target.value })
                      }
                      pattern="#[0-9A-Fa-f]{6}"
                      placeholder="#RRGGBB"
                      disabled={loading}
                    />
                    <span
                      className="input-group-text"
                      style={{
                        backgroundColor: labelData.colorHex,
                        width: "40px",
                      }}
                    ></span>
                  </div>
                  <div className="form-text">
                    Color in hexadecimal format (#RRGGBB)
                  </div>
                </div>

                <div className="mb-3">
                  <label htmlFor="type" className="form-label">
                    Label Type
                  </label>
                  <select
                    id="type"
                    name="type"
                    value={labelData.type}
                    onChange={handleChange}
                    className="form-select"
                    required
                    disabled={loading}
                  >
                    <option value="range">Range</option>
                    <option value="point">Point</option>
                  </select>
                </div>

                <div className="mb-3">
                  <label htmlFor="shortcut" className="form-label">
                    Shortcut Key
                  </label>
                  <input
                    type="text"
                    id="shortcut"
                    name="shortcut"
                    value={labelData.shortcut}
                    onChange={handleChange}
                    className="form-control"
                    maxLength="1"
                    placeholder="Single character (a-z, 0-9)"
                    disabled={loading}
                  />
                  <div className="form-text">
                    Single character used as keyboard shortcut for this label
                  </div>
                </div>

                <div className="mb-4">
                  <label htmlFor="subjectId" className="form-label">
                    Subject
                  </label>
                  <div className="input-group">
                    <span className="input-group-text">
                      <i className="fas fa-folder"></i>
                    </span>
                    <input
                      type="text"
                      className="form-control"
                      value={
                        subjectName || `Subject ID: ${labelData.subjectId}`
                      }
                      disabled
                    />
                  </div>
                </div>

                <div className="d-flex">
                  <button
                    type="submit"
                    className="btn btn-primary me-2"
                    disabled={loading}
                  >
                    {loading ? (
                      <>
                        <span
                          className="spinner-border spinner-border-sm me-2"
                          role="status"
                          aria-hidden="true"
                        ></span>
                        Updating...
                      </>
                    ) : (
                      <>
                        <i className="fas fa-save me-2"></i>Save Changes
                      </>
                    )}
                  </button>
                  <NavigateButton actionType="Back" value="Cancel" />
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LabelEdit;
