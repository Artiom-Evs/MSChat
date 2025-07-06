import React, { createContext, useContext, useEffect, useRef, useState } from "react";
import { HubConnectionBuilder, HubConnection, LogLevel } from "@microsoft/signalr";
import { useAuth } from "react-oidc-context";

type ChatHubContextType = {
  connection: HubConnection | null;
  isConnected: boolean;
};

const ChatHubContext = createContext<ChatHubContextType>({
  connection: null,
  isConnected: false,
});

export const useChatHub = () => useContext(ChatHubContext);

type Props = {
  children: React.ReactNode;
  hubUrl: string;
};

export const ChatHubProvider: React.FC<Props> = ({ children, hubUrl }) => {
  const connectionRef = useRef<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const auth = useAuth();

  useEffect(() => {
    if (connectionRef.current || !auth.isAuthenticated) return;

    const accessTokenFactory = () => {
      return auth.isAuthenticated ? auth.user?.access_token ?? "" : "";
    }
    
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, { 
        accessTokenFactory
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    connectionRef.current = connection;

    const startConnection = async () => {
      try {
        await connection.start();
        setIsConnected(true);
        console.log("SignalR connected");
      } catch (err) {
        console.error("SignalR connection failed:", err);
        setIsConnected(false);
      }
    };
    
    startConnection();
  }, [auth.isAuthenticated, connectionRef.current]);

  return (
    <ChatHubContext.Provider
      value={{
        connection: connectionRef.current,
        isConnected,
      }}
    >
      {children}
    </ChatHubContext.Provider>
  );
};

