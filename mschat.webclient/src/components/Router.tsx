import React, { useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import DefaultPage from '../pages/DefaultPage';
import ChatPage from '../pages/ChatPage';
import ProtectedRoute from './ProtectedRoute';
import PublicRoute from './PublicRoute';
import { useAuthStore } from '../stores/authStore';
import AuthCallback from '../auth/AuthCallback';
import SilentCallback from '../auth/SilentCallback';

const Router: React.FC = () => {
  const { initAuth } = useAuthStore();

  useEffect(() => {
    initAuth();
  }, [initAuth]);

  return (
    <Routes>
      {/* Auth callback routes - no layout */}
      <Route 
        path="/auth/callback" 
        element={
          <PublicRoute>
            <AuthCallback />
          </PublicRoute>
        } 
      />
      <Route 
        path="/auth/silent-callback" 
        element={
          <PublicRoute>
            <SilentCallback />
          </PublicRoute>
        } 
      />
      
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
        path="/chats/:id" 
        element={
          <ProtectedRoute>
            <ChatPage />
          </ProtectedRoute>
        } 
      />
      
      {/* Catch all route - redirect to home */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default Router;