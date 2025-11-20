import { Suspense, Component, type ReactNode, useState } from "react";
import { graphql, useLazyLoadQuery, useMutation, useFragment } from "react-relay";
import type { CartPopoverQuery } from "./__generated__/CartPopoverQuery.graphql";
import type { CartPopoverCheckoutMutation } from "./__generated__/CartPopoverCheckoutMutation.graphql";
import type { CartPopover_cart$key } from "./__generated__/CartPopover_cart.graphql";
import CartItem from "./CartItem";
import {
  Popover,
  Box,
  Typography,
  CircularProgress,
  Alert,
  Button,
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import CheckIcon from "@mui/icons-material/Check";

const CartPopoverFragment = graphql`
  fragment CartPopover_cart on Cart {
    id
    items {
      nodes {
        id
        ...CartItem_item
      }
    }
  }
`;

const CartPopoverQueryNode = graphql`
  query CartPopoverQuery {
    viewer {
      cart {
        ...CartPopover_cart
      }
    }
  }
`;

const CheckoutMutation = graphql`
  mutation CartPopoverCheckoutMutation {
    checkout {
      cart {
        ...CartPopover_cart
      }
    }
  }
`;

interface CartPopoverProps {
  anchorEl: HTMLElement | null;
  open: boolean;
  onClose: () => void;
}

const popoverStyles = {
  "& .MuiPopover-paper": {
    width: 400,
    maxHeight: 600,
  },
} as const;

const headerStyles = {
  p: 2,
  borderBottom: "1px solid #e0e0e0",
  display: "flex",
  alignItems: "center",
  gap: 1,
} as const;

const contentStyles = {
  maxHeight: 400,
  overflowY: "auto",
} as const;

const emptyStateStyles = {
  p: 4,
  textAlign: "center",
  display: "flex",
  flexDirection: "column",
  alignItems: "center",
  gap: 2,
} as const;

const loadingContainerStyles = {
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  p: 4,
} as const;

const errorAlertStyles = {
  m: 2,
} as const;

const checkoutButtonContainerStyles = {
  p: 2,
  borderTop: "1px solid #e0e0e0",
} as const;

class CartErrorBoundary extends Component<
  { children: ReactNode },
  { hasError: boolean }
> {
  constructor(props: { children: ReactNode }) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  render() {
    if (this.state.hasError) {
      return (
        <Alert severity="error" sx={errorAlertStyles}>
          Failed to load cart
        </Alert>
      );
    }

    return this.props.children;
  }
}

function CartContent() {
  const data = useLazyLoadQuery<CartPopoverQuery>(CartPopoverQueryNode, {});
  const [isCheckingOut, setIsCheckingOut] = useState(false);
  const [checkedOut, setCheckedOut] = useState(false);
  const [commitCheckout] = useMutation<CartPopoverCheckoutMutation>(CheckoutMutation);

  const cart = useFragment<CartPopover_cart$key>(CartPopoverFragment, data.viewer.cart!);
  const items = cart.items?.nodes || [];

  const handleCheckout = () => {
    setIsCheckingOut(true);
    commitCheckout({
      variables: {},
      onCompleted: () => {
        setIsCheckingOut(false);
        setCheckedOut(true);
        setTimeout(() => setCheckedOut(false), 2000);
      },
      onError: () => {
        setIsCheckingOut(false);
      },
    });
  };

  if (items.length === 0) {
    return (
      <Box sx={emptyStateStyles}>
        <ShoppingCartIcon sx={{ fontSize: 60, color: "text.secondary" }} />
        <Typography color="text.secondary">Your cart is empty</Typography>
        <Typography variant="body2" color="text.secondary">
          Add some products to get started
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <Box sx={contentStyles}>
        {items.map((item) => (
          <CartItem key={item.id} item={item} />
        ))}
      </Box>
      <Box sx={checkoutButtonContainerStyles}>
        <Button
          variant="contained"
          fullWidth
          size="large"
          onClick={handleCheckout}
          disabled={isCheckingOut || checkedOut}
          startIcon={isCheckingOut ? <CircularProgress size={20} /> : checkedOut ? <CheckIcon /> : null}
          color={checkedOut ? "success" : "primary"}
        >
          {checkedOut ? "Checked Out!" : "Checkout"}
        </Button>
      </Box>
    </>
  );
}

export default function CartPopover({
  anchorEl,
  open,
  onClose,
}: CartPopoverProps) {
  return (
    <Popover
      anchorEl={anchorEl}
      open={open}
      onClose={onClose}
      anchorOrigin={{
        vertical: "bottom",
        horizontal: "right",
      }}
      transformOrigin={{
        vertical: "top",
        horizontal: "right",
      }}
      sx={popoverStyles}
    >
      <Box sx={headerStyles}>
        <ShoppingCartIcon color="primary" />
        <Typography variant="h6" fontWeight={600}>
          Shopping Cart
        </Typography>
      </Box>
      <CartErrorBoundary>
        <Suspense
          fallback={
            <Box sx={loadingContainerStyles}>
              <CircularProgress />
            </Box>
          }
        >
          <CartContent />
        </Suspense>
      </CartErrorBoundary>
    </Popover>
  );
}
