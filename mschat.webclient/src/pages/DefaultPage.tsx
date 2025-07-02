import React, { useEffect, useState } from 'react';
import { 
  Box, 
  Typography, 
  Table, 
  TableBody, 
  TableCell, 
  TableContainer, 
  TableHead, 
  TableRow, 
  Paper,
  CircularProgress,
  Alert
} from '@mui/material';

interface Forecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

const DefaultPage: React.FC = () => {
  const [forecasts, setForecasts] = useState<Forecast[]>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    populateWeatherData();
  }, []);

  const populateWeatherData = async () => {
    try {
      setLoading(true);
      const response = await fetch('weatherforecast');
      if (response.ok) {
        const data = await response.json();
        setForecasts(data);
      } else {
        setError('Failed to load weather data');
      }
    } catch {
      setError('Network error occurred');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '50vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ mt: 2 }}>
        {error}
      </Alert>
    );
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 600 }}>
        Weather Forecast
      </Typography>
      
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        This component demonstrates fetching data from the server.
      </Typography>

      {forecasts && forecasts.length > 0 && (
        <TableContainer component={Paper} elevation={1}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600 }}>Date</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>Temp. (C)</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>Temp. (F)</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>Summary</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {forecasts.map((forecast) => (
                <TableRow key={forecast.date} hover>
                  <TableCell>{forecast.date}</TableCell>
                  <TableCell>{forecast.temperatureC}</TableCell>
                  <TableCell>{forecast.temperatureF}</TableCell>
                  <TableCell>{forecast.summary}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default DefaultPage;