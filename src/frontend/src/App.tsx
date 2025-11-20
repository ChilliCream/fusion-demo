import { useState } from "react";
import ProductsPage from "./ProductsPage";
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Box,
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import CartPopover from "./CartPopover";

const headingStyles = {
  fontWeight: 900,
  fontSize: { xs: "1.75rem", sm: "2.25rem", md: "2.5rem" },
  textTransform: "uppercase",
  letterSpacing: "0.1em",
  color: "white",
  flexGrow: 1,
  textAlign: "center",
} as const;

function App() {
  const [cartAnchorEl, setCartAnchorEl] = useState<HTMLElement | null>(null);
  const cartOpen = Boolean(cartAnchorEl);

  const handleCartClick = (event: React.MouseEvent<HTMLElement>) => {
    setCartAnchorEl(event.currentTarget);
  };

  const handleCartClose = () => {
    setCartAnchorEl(null);
  };

  return (
    <>
      <AppBar position="fixed">
        <Toolbar>
          <Typography variant="h4" component="h1" sx={headingStyles}>
            Products
          </Typography>
          <IconButton
            onClick={handleCartClick}
            color="inherit"
            size="large"
          >
            <ShoppingCartIcon fontSize="large" />
          </IconButton>
        </Toolbar>
      </AppBar>
      <CartPopover
        anchorEl={cartAnchorEl}
        open={cartOpen}
        onClose={handleCartClose}
      />
      <Box sx={{ pt: { xs: 10, sm: 12 } }}>
        <ProductsPage />
      </Box>
    </>
  );
}

export default App;
