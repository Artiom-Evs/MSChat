import React from 'react';
import {
  Box,
  Avatar,
  Typography,
  Button,
  IconButton,
  Menu,
  MenuItem,
} from '@mui/material';
import { MoreVert as MoreVertIcon } from '@mui/icons-material';
import { useAuth } from 'react-oidc-context';

const UserProfile: React.FC = () => {
  const auth = useAuth();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    auth.signoutRedirect();
    handleMenuClose();
  };

  const handleLogin = async () => {
    try {
      await auth.signinRedirect();
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  if (!auth.isAuthenticated || !auth.user) {
    return (
      <Box sx={{ textAlign: 'center', py: 2 }}>
        <Typography variant="h6" gutterBottom color="text.secondary">
          Welcome to MSChat
        </Typography>
        <Button
          variant="contained"
          fullWidth
          onClick={handleLogin}
          sx={{ textTransform: 'none' }}
        >
          Sign In
        </Button>
      </Box>
    );
  }

  const user = auth.user;
  const userName = user.profile?.name || user.profile?.preferred_username || 'User';
  const userEmail = user.profile?.email || '';

  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
      <Avatar
        sx={{
          width: 48,
          height: 48,
          bgcolor: 'primary.main',
          fontSize: '1.2rem',
        }}
      >
        {userName.split(' ').map(n => n[0]).join('').toUpperCase()}
      </Avatar>
      
      <Box sx={{ flexGrow: 1, minWidth: 0 }}>
        <Typography
          variant="subtitle1"
          sx={{
            fontWeight: 600,
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          }}
        >
          {userName}
        </Typography>
        <Typography
          variant="body2"
          color="text.secondary"
          sx={{
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          }}
        >
          {userEmail}
        </Typography>
      </Box>
      
      <IconButton
        size="small"
        onClick={handleMenuOpen}
        sx={{ color: 'text.secondary' }}
      >
        <MoreVertIcon />
      </IconButton>
      
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
      >
        <MenuItem onClick={handleLogout}>Sign Out</MenuItem>
      </Menu>
    </Box>
  );
};

export default UserProfile;