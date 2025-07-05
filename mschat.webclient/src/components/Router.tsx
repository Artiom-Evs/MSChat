import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import DefaultPage from '../pages/DefaultPage';
import ChatPage from '../pages/ChatPage';
import ChatParticipantsPage from '../pages/ChatParticipantsPage';
import CreateChatPage from '../pages/CreateChatPage';
import UpdateChatPage from '../pages/UpdateChatPage';
import SearchPage from '../pages/SearchPage';
import MemberPage from '../pages/MemberPage';
import ProtectedRoute from './ProtectedRoute';

const Router: React.FC = () => {
  return (
    <Routes>
      {/* Protected routes - with layout */}
      <Route 
        path="/" 
        element={
          <ProtectedRoute>
            <DefaultPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/chats/create" 
        element={
          <ProtectedRoute>
            <CreateChatPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/chats/:id/edit" 
        element={
          <ProtectedRoute>
            <UpdateChatPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/chats/:id/participants" 
        element={
          <ProtectedRoute>
            <ChatParticipantsPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/chats/:id" 
        element={
          <ProtectedRoute>
            <ChatPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/search" 
        element={
          <ProtectedRoute>
            <SearchPage />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/members/:id" 
        element={
          <ProtectedRoute>
            <MemberPage />
          </ProtectedRoute>
        } 
      />
      
      {/* Catch all route - redirect to home */}
      <Route path="*" element={
        <ProtectedRoute>
          <Navigate to="/" replace />
        </ProtectedRoute>
      } />
    </Routes>
  );
};

export default Router;