import React, { createContext, useContext, useEffect, useRef, useState } from "react";
import { HubConnectionBuilder, HubConnection, LogLevel } from "@microsoft/signalr";
import { useAuth } from "react-oidc-context";

type PresenceHubContextType = {
  connection: HubConnection | null;
  isConnected: boolean;
};

const PresenceHubContext = createContext<PresenceHubContextType>({
  connection: null,
  isConnected: false,
});

export const usePresenceHub = () => useContext(PresenceHubContext);

type PresenceHubProps = {
  children: React.ReactNode;
  hubUrl: string;
};

export const PresenceHubProvider: React.FC<PresenceHubProps> = ({ children, hubUrl }) => {
  const connectionRef = useRef<HubConnection | null>(null);
  const broadcastRef = useRef<unknown | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const auth = useAuth();

  useEffect(() => {
    if (connectionRef.current || !auth.isAuthenticated) return;

    const accessTokenFactory = () => {
      return auth.isAuthenticated ? auth.user?.access_token ?? "" : "";
    }
    
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connectionRef.current = connection;

    const startConnection = async () => {
      try {
        await connection.start();
        setIsConnected(true);
        console.log("Connected to PresenceAPI SignalR hub.");
      } catch (err) {
        console.error("SignalR connection failed:", err);
        setIsConnected(false);
      }
    };
    
    startConnection();
  }, [auth.isAuthenticated, connectionRef.current]);

  useEffect(() => {
    if (broadcastRef.current || !connectionRef.current) return;

    broadcastRef.current = setInterval(() => {
      connectionRef.current?.invoke("BroadcastOnlineStatus")
        .catch(e => console.error("Error while broadcasting online status.", e));
    }, 5000);

    return () => {
      if (broadcastRef.current) {
        clearTimeout(broadcastRef.current as any);
      }
    }
  }, [connectionRef.current]);

  return (
    <PresenceHubContext.Provider
      value={{
        connection: connectionRef.current,
        isConnected,
      }}
    >
      {children}
    </PresenceHubContext.Provider>
  );
};

