import { Outlet } from "react-router";
import { Header } from "./Header";
import { Footer } from "./Footer";

/**
 * The page shell shared by every route: sticky header, flexible main
 * content area, and slim footer. The starfield background comes from the
 * design base's `body` styles in `src/styles/globals.css`.
 */
export function AppShell() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
