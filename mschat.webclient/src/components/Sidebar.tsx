import React from 'react';
import { Box, Divider } from '@mui/material';
import UserProfile from './UserProfile';
import ChatList from './ChatList';

const Sidebar: React.FC = () => {
  return (
    <Box
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        backgroundColor: 'background.paper',
      }}
    >
      <Box sx={{ p: 2 }}>
        <UserProfile />
      </Box>
      
      <Divider />
      
      <Box sx={{ flexGrow: 1, overflow: 'auto' }}>
        <ChatList />
      </Box>
    </Box>
  );
};

export default Sidebar;