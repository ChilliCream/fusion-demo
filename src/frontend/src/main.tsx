import { RelayEnvironmentProvider } from "react-relay";
import { RelayEnvironment } from "./RelayEnvironment";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import { AuthProvider } from "./AuthContext";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AuthProvider>
      <RelayEnvironmentProvider environment={RelayEnvironment}>
        <App />
      </RelayEnvironmentProvider>
    </AuthProvider>
  </StrictMode>
);
