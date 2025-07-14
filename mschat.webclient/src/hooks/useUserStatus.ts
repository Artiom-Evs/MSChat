import { useEffect, useState } from "react";
import { usePresenceHub } from "../context";

interface UseUserStatusProps {
  userId: string;
}

export function useUserStatus(props: UseUserStatusProps) {
  const hub = usePresenceHub();
  const [status, setStatus] = useState<string | null>(null);

  useEffect(() => {
    if (!hub.connection || !props.userId) return;

    const conn = hub.connection;
    console.log(
      "PresenceHub: Setting up event listeners for user:",
      props.userId
    );

    // Subscribe to SignalR events for real-time updates
    conn.on("PresenceStatusChanged", handleStatusChange);

    // Subscribe to user status updates
    conn
      .invoke("SubscribeToUserStatus", props.userId)
      .then(() => {
        console.log("PresenceHub: Successfully subscribed.");
      })
      .catch((err) => {
        console.error("PresenceHub: Subscription failed.", err);
      });

    updateUserStatus();

    return () => {
      console.log(
        "PresenceHub: Cleaning up event listeners for user:",
        props.userId
      );
      // Clean up event listeners
      conn.off("PresenceStatusChanged", handleStatusChange);

      // Unsubscribe from chat updates
      conn.invoke("UnsubscribeFromUserStatus", props.userId);
    };
  }, [hub.connection, props.userId]);

  const handleStatusChange = (userId: string, newStatus: string) => {
    if (props.userId == userId) {
      setStatus(newStatus);
    }
  };

  const updateUserStatus = async () => {
    if (!hub.connection) {
      console.warn(
        "PresenceHub: Not not connected to PresenceAPI SignalR hub."
      );
      return;
    }

    hub.connection
      .invoke<string>("GetUserStatus", props.userId)
      .then(setStatus)
      .catch((err) => {
        console.error("PresenceHub: User status update failed.", err);
      });
  };

  return { status };
}
