import { Outlet } from "react-router";
import { LoginModal } from "../../auth/LoginModal";
import { Footer } from "./Footer";
import { Header } from "./Header";

/**
 * The page shell shared by every route: sticky header, flexible main
 * content area, and slim footer. The starfield background comes from the
 * design base's `body` styles in `src/styles/globals.css`. The login modal
 * is mounted once here (rather than inside `Header`) so it isn't nested
 * under the header's landmark, and shows/hides itself via `AuthContext`.
 */
export function AppShell() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
      <LoginModal />
    </div>
  );
}
