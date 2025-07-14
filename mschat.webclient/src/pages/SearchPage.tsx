import React, { useState } from 'react';
import {
  Box,
  TextField,
  Typography,
  Tabs,
  Tab,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Chip,
  Paper,
  InputAdornment,
  CircularProgress,
  Alert,
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import GroupIcon from '@mui/icons-material/Group';
import PersonIcon from '@mui/icons-material/Person';
import { useChats } from '../hooks/useChats';
import { useMembers } from '../hooks/useMembers';
import { useNavigate } from 'react-router-dom';
import { ChatType } from '../types';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`search-tabpanel-${index}`}
      aria-labelledby={`search-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
    </div>
  );
}

const SearchPage: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeTab, setActiveTab] = useState(0);
  const navigate = useNavigate();

  const { data: chats, isLoading: chatsLoading, error: chatsError } = useChats();
  const { data: members, isLoading: membersLoading, error: membersError } = useMembers(searchQuery);

  const filteredChats = chats?.filter(chat =>
    chat.name.toLowerCase().includes(searchQuery.toLowerCase())
  ) || [];

  const filteredMembers = members?.filter(member =>
    member.name.toLowerCase().includes(searchQuery.toLowerCase())
  ) || [];

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  const handleChatClick = (chatId: number) => {
    navigate(`/chats/${chatId}`);
  };

  const handleMemberClick = (memberId: string) => {
    navigate(`/members/${memberId}`);
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  };

  const getChatTypeLabel = (type: ChatType) => {
    return type === ChatType.Public ? 'Public' : 'Personal';
  };

  const getChatTypeColor = (type: ChatType) => {
    return type === ChatType.Public ? 'primary' : 'secondary';
  };

  return (
    <Box sx={{ maxWidth: 800, mx: 'auto' }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Search
      </Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <TextField
          fullWidth
          placeholder="Search for groups and members..."
          value={searchQuery}
          onChange={handleSearchChange}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ mb: 2 }}
        />

        <Tabs value={activeTab} onChange={handleTabChange} aria-label="search tabs">
          <Tab
            icon={<GroupIcon />}
            label={`Groups (${filteredChats.length})`}
            iconPosition="start"
          />
          <Tab
            icon={<PersonIcon />}
            label={`Members (${filteredMembers.length})`}
            iconPosition="start"
          />
        </Tabs>
      </Paper>

      <TabPanel value={activeTab} index={0}>
        {chatsLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : chatsError ? (
          <Alert severity="error" sx={{ mb: 2 }}>
            Error loading chats. Please try again.
          </Alert>
        ) : filteredChats.length === 0 ? (
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              {searchQuery ? 'No groups found matching your search.' : 'Enter a search term to find groups.'}
            </Typography>
          </Paper>
        ) : (
          <Paper>
            <List>
              {filteredChats.map((chat, index) => (
                <ListItem
                  key={chat.id}
                  sx={{
                    cursor: 'pointer',
                    '&:hover': { backgroundColor: 'action.hover' },
                    borderBottom: index < filteredChats.length - 1 ? '1px solid' : 'none',
                    borderColor: 'divider',
                  }}
                  onClick={() => handleChatClick(chat.id)}
                >
                  <ListItemAvatar>
                    <Avatar>
                      <GroupIcon />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="body1">{chat.name}</Typography>
                        <Chip
                          label={getChatTypeLabel(chat.type)}
                          size="small"
                          color={getChatTypeColor(chat.type)}
                        />
                      </Box>
                    }
                    secondary={`Created: ${new Date(chat.createdAt).toLocaleDateString()}`}
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        )}
      </TabPanel>

      <TabPanel value={activeTab} index={1}>
        {membersLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : membersError ? (
          <Alert severity="error" sx={{ mb: 2 }}>
            Error loading members. Please try again.
          </Alert>
        ) : filteredMembers.length === 0 ? (
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              {searchQuery ? 'No members found matching your search.' : 'Enter a search term to find members.'}
            </Typography>
          </Paper>
        ) : (
          <Paper>
            <List>
              {filteredMembers.map((member, index) => (
                <ListItem
                  key={member.id}
                  sx={{
                    cursor: 'pointer',
                    '&:hover': { backgroundColor: 'action.hover' },
                    borderBottom: index < filteredMembers.length - 1 ? '1px solid' : 'none',
                    borderColor: 'divider',
                  }}
                  onClick={() => handleMemberClick(member.id)}
                >
                  <ListItemAvatar>
                    <Avatar src={member.photoUrl || undefined}>
                      {!member.photoUrl && <PersonIcon />}
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                    primary={member.name}
                    secondary={`Joined: ${new Date(member.createdAt).toLocaleDateString()}`}
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        )}
      </TabPanel>
    </Box>
  );
};

export default SearchPage;