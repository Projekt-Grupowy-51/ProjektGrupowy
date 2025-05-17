import { useState } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";
import ThemeService from "./services/theme/ThemeService";

function App() {
  const [count, setCount] = useState(0);
  const [theme, setTheme] = useState(ThemeService.getTheme());

  const toggle = () => {
    ThemeService.toggleTheme();
    setTheme(ThemeService.getTheme());
  };

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
        <div className="container py-2">
          <button className="btn btn-primary" onClick={toggle}>
            {theme === "light" ? "Dark" : "Light"} Mode
          </button>
        </div>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  );
}

export default App;
