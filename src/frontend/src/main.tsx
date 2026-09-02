import "./styles/globals.css";
import { RelayEnvironmentProvider } from "react-relay";
import { RelayEnvironment } from "./RelayEnvironment";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import { AuthProvider } from "./auth/AuthProvider";

// AuthProvider now lives inside RelayEnvironmentProvider (rather than
// wrapping it) so it can use the standard `useMutation` hook - consistent
// with the rest of the app - to replay a pending add-to-cart intent right
// after login. RelayEnvironment itself is unaffected: it reads the auth
// token straight from localStorage on every request, independent of
// component tree position.
createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <RelayEnvironmentProvider environment={RelayEnvironment}>
      <AuthProvider>
        <App />
      </AuthProvider>
    </RelayEnvironmentProvider>
  </StrictMode>
);
