import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { CssBaseline } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import '@fontsource/roboto/400.css';

const theme = createTheme();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <CssBaseline />
        <ThemeProvider theme={theme}>
      <CssBaseline />
      <App />
    </ThemeProvider>
  </StrictMode>,
)
