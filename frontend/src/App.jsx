import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [forecasts, setForecasts] = useState();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please wait.</em></p>
        : forecasts.length === 0
            ? <p><em>No projects found.</em></p>
            : <table className="table table-striped" aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map(forecast =>
                        <tr key={forecast.name}>
                            <td>{forecast.name}</td>
                            <td>{forecast.description}</td>
                        </tr>
                    )}
                </tbody>
            </table>;




    return (
        <div>
            <h1 id="tabelLabel">PROJEKTY</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );
    
    async function populateWeatherData() {
        try {
            const response = await fetch('/api/Project'); // Proxy kieruje do backendu
            const data = await response.json();
            console.log("OdpowiedŸ z API:", data);

            // Wyci¹gniêcie tablicy projektów z `$values`
            if (data?.$values) {
                setForecasts(data.$values); // Ustaw dane z klucza `$values`
            } else {
                console.error("Nie znaleziono klucza `$values` w odpowiedzi:", data);
                setForecasts([]); // Zabezpieczenie przed b³êdami
            }
        } catch (error) {
            console.error("B³¹d podczas pobierania danych z API:", error);
            setForecasts([]);
        }
    }



}

export default App;