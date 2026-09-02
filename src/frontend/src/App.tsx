import { BrowserRouter, Routes, Route } from "react-router";
import { AppShell } from "./components/layout/AppShell";
import OverviewPage from "./routes/OverviewPage";
import ProductDetailPage from "./routes/ProductDetailPage";
import CartPage from "./routes/CartPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppShell />}>
          <Route index element={<OverviewPage />} />
          <Route path="products/:id" element={<ProductDetailPage />} />
          <Route path="cart" element={<CartPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
