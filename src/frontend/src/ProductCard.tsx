import { graphql, useFragment, useMutation } from "react-relay";
import type { ProductCard_product$key } from "./__generated__/ProductCard_product.graphql";
import type { ProductCardAddToCartMutation } from "./__generated__/ProductCardAddToCartMutation.graphql";
import {
  Card,
  CardMedia,
  CardContent,
  CardActions,
  Typography,
  Button,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import CheckIcon from "@mui/icons-material/Check";
import LoginIcon from "@mui/icons-material/Login";
import { memo, useState } from "react";
import { useAuth } from "./AuthContext";
import LoginDialog from "./LoginDialog";

const ProductCardFragment = graphql`
  fragment ProductCard_product on Product {
    id
    name
    price
    pictureUrl
  }
`;

const AddToCartMutation = graphql`
  mutation ProductCardAddToCartMutation($input: AddProductToCartInput!) {
    addProductToCart(input: $input) {
      cart {
        ...CartPopover_cart
      }
    }
  }
`;

interface ProductCardProps {
  product: ProductCard_product$key;
}

const cardStyles = {
  height: "100%",
  display: "flex",
  flexDirection: "column",
  willChange: "transform",
  transition: "transform 0.2s ease-out, box-shadow 0.2s ease-out",
  "&:hover": {
    transform: "translateY(-8px)",
    boxShadow: 6,
  },
} as const;

const mediaStyles = {
  aspectRatio: "1 / 1",
  width: "100%",
  backgroundColor: "#f5f5f5",
  objectFit: "cover",
} as const;

const mediaPlaceholderStyles = {
  aspectRatio: "1 / 1",
  width: "100%",
  backgroundColor: "#f5f5f5",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
} as const;

const contentStyles = { flexGrow: 1, pb: 1 } as const;

const nameStyles = {
  overflow: "hidden",
  textOverflow: "ellipsis",
  whiteSpace: "nowrap",
} as const;

const actionsStyles = { p: 2, pt: 0 } as const;

function ProductCard({ product }: ProductCardProps) {
  const data = useFragment(ProductCardFragment, product);
  const { isAuthenticated } = useAuth();
  const [isAdding, setIsAdding] = useState(false);
  const [isAdded, setIsAdded] = useState(false);
  const [signInPromptOpen, setSignInPromptOpen] = useState(false);
  const [loginDialogOpen, setLoginDialogOpen] = useState(false);
  const [commitAddToCart] = useMutation<ProductCardAddToCartMutation>(
    AddToCartMutation
  );

  const handleAddToCart = () => {
    if (!isAuthenticated) {
      setSignInPromptOpen(true);
      return;
    }

    setIsAdding(true);
    commitAddToCart({
      variables: {
        input: {
          productId: data.id,
          quantity: 1,
        },
      },
      onCompleted: () => {
        setIsAdding(false);
        setIsAdded(true);
        setTimeout(() => setIsAdded(false), 2000);
      },
      onError: () => {
        setIsAdding(false);
      },
    });
  };

  const handleSignInPromptClose = () => {
    setSignInPromptOpen(false);
  };

  const handleSignInClick = () => {
    setSignInPromptOpen(false);
    setLoginDialogOpen(true);
  };

  const handleLoginDialogClose = () => {
    setLoginDialogOpen(false);
  };

  return (
    <>
      <Card sx={cardStyles}>
        {data.pictureUrl ? (
          <CardMedia
            component="img"
            image={data.pictureUrl}
            alt={data.name}
            sx={mediaStyles}
          />
        ) : (
          <CardMedia component="div" sx={mediaPlaceholderStyles}>
            <Typography color="text.secondary">No Image</Typography>
          </CardMedia>
        )}
        <CardContent sx={contentStyles}>
          <Typography gutterBottom variant="h5" component="h2" sx={nameStyles}>
            {data.name}
          </Typography>
          <Typography variant="h6" color="primary" fontWeight="bold">
            ${data.price.toFixed(2)}
          </Typography>
        </CardContent>
        <CardActions sx={actionsStyles}>
          <Button
            variant="contained"
            fullWidth
            startIcon={
              isAdding ? (
                <CircularProgress size={20} color="inherit" />
              ) : isAdded ? (
                <CheckIcon />
              ) : (
                <ShoppingCartIcon />
              )
            }
            onClick={handleAddToCart}
            disabled={isAdding || isAdded}
            size="large"
            color={isAdded ? "success" : "primary"}
          >
            {isAdded ? "Added!" : "Add to Cart"}
          </Button>
        </CardActions>
      </Card>

      <Dialog open={signInPromptOpen} onClose={handleSignInPromptClose}>
        <DialogTitle>Sign In Required</DialogTitle>
        <DialogContent>
          <DialogContentText>
            You need to sign in to add items to your cart. Please sign in to continue shopping.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleSignInPromptClose}>Cancel</Button>
          <Button
            onClick={handleSignInClick}
            variant="contained"
            startIcon={<LoginIcon />}
            autoFocus
          >
            Sign In
          </Button>
        </DialogActions>
      </Dialog>

      <LoginDialog open={loginDialogOpen} onClose={handleLoginDialogClose} />
    </>
  );
}

export default memo(ProductCard);
