import { useState } from "react";
import ProductsPage from "./ProductsPage";
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Box,
  Button,
  Menu,
  MenuItem,
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import LoginIcon from "@mui/icons-material/Login";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import CartPopover from "./CartPopover";
import LoginDialog from "./LoginDialog";
import { useAuth } from "./AuthContext";

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
  const { isAuthenticated, user, logout } = useAuth();
  const [cartAnchorEl, setCartAnchorEl] = useState<HTMLElement | null>(null);
  const [userMenuAnchorEl, setUserMenuAnchorEl] = useState<HTMLElement | null>(null);
  const [loginDialogOpen, setLoginDialogOpen] = useState(false);
  const cartOpen = Boolean(cartAnchorEl);
  const userMenuOpen = Boolean(userMenuAnchorEl);

  const handleCartClick = (event: React.MouseEvent<HTMLElement>) => {
    setCartAnchorEl(event.currentTarget);
  };

  const handleCartClose = () => {
    setCartAnchorEl(null);
  };

  const handleLoginClick = () => {
    setLoginDialogOpen(true);
  };

  const handleLoginClose = () => {
    setLoginDialogOpen(false);
  };

  const handleUserMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setUserMenuAnchorEl(event.currentTarget);
  };

  const handleUserMenuClose = () => {
    setUserMenuAnchorEl(null);
  };

  const handleLogout = () => {
    logout();
    handleUserMenuClose();
  };

  return (
    <>
      <AppBar position="fixed">
        <Toolbar>
          {isAuthenticated ? (
            <>
              <IconButton
                onClick={handleUserMenuClick}
                color="inherit"
                size="large"
              >
                <AccountCircleIcon fontSize="large" />
              </IconButton>
              <Menu
                anchorEl={userMenuAnchorEl}
                open={userMenuOpen}
                onClose={handleUserMenuClose}
              >
                <MenuItem disabled>{user?.username}</MenuItem>
                <MenuItem onClick={handleLogout}>Logout</MenuItem>
              </Menu>
            </>
          ) : (
            <Button
              startIcon={<LoginIcon />}
              color="inherit"
              onClick={handleLoginClick}
              sx={{ mr: 2 }}
            >
              Login
            </Button>
          )}
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
      <LoginDialog open={loginDialogOpen} onClose={handleLoginClose} />
      <Box sx={{ pt: { xs: 10, sm: 12 } }}>
        <ProductsPage />
      </Box>
    </>
  );
}

export default App;
