import React, { useState } from 'react';
import { Box, Drawer, Toolbar, useTheme, useMediaQuery, IconButton, AppBar } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import { useNavigate } from 'react-router-dom';
import Sidebar from './Sidebar';

const DRAWER_WIDTH = 320;

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [sidebarOpen, setSidebarOpen] = useState(!isMobile);
  const navigate = useNavigate();

  const handleSidebarToggle = () => {
    setSidebarOpen(!sidebarOpen);
  };

  const handleSearchClick = () => {
    navigate('/search');
  };

  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      <AppBar
        position="fixed"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          backgroundColor: 'background.paper',
          color: 'text.primary',
          boxShadow: 1,
          borderBottom: '1px solid',
          borderColor: 'divider',
        }}
      >
        <Toolbar sx={{ minHeight: '64px !important' }}>
          <IconButton
            color="inherit"
            aria-label="toggle sidebar"
            onClick={handleSidebarToggle}
            edge="start"
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Box sx={{ flexGrow: 1 }} />
          <IconButton
            color="inherit"
            aria-label="search"
            onClick={handleSearchClick}
            edge="end"
          >
            <SearchIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Drawer
        variant={isMobile ? 'temporary' : 'persistent'}
        open={sidebarOpen}
        onClose={handleSidebarToggle}
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: DRAWER_WIDTH,
            boxSizing: 'border-box',
            borderRight: '1px solid',
            borderColor: 'divider',
            backgroundColor: 'background.paper',
          },
        }}
      >
        <Toolbar />
        <Sidebar />
      </Drawer>
      
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          display: 'flex',
          flexDirection: 'column',
          overflow: 'hidden',
          backgroundColor: 'background.default',
          marginLeft: isMobile ? 0 : sidebarOpen ? 0 : `-${DRAWER_WIDTH}px`,
          transition: 'margin 0.3s ease',
        }}
      >
        <Toolbar />
        <Box sx={{ flexGrow: 1, overflow: 'auto', p: 3 }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
};

export default Layout;