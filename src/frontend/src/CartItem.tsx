import { graphql, useFragment, useMutation } from "react-relay";
import type { CartItem_item$key } from "./__generated__/CartItem_item.graphql";
import type { CartItemRemoveMutation } from "./__generated__/CartItemRemoveMutation.graphql";
import type { CartItemAddMutation } from "./__generated__/CartItemAddMutation.graphql";
import { Box, Typography, IconButton, CircularProgress } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import RemoveIcon from "@mui/icons-material/Remove";
import { useState } from "react";

const CartItemFragment = graphql`
  fragment CartItem_item on CartItem {
    id
    amount
    addedAt
    product {
      id
      name
      price
      pictureUrl
    }
  }
`;

const AddToCartMutation = graphql`
  mutation CartItemAddMutation($input: AddProductToCartInput!) {
    addProductToCart(input: $input) {
      cart {
        ...CartPopover_cart
      }
    }
  }
`;

const RemoveFromCartMutation = graphql`
  mutation CartItemRemoveMutation($input: RemoveProductFromCartInput!) {
    removeProductFromCart(input: $input) {
      cart {
        ...CartPopover_cart
      }
    }
  }
`;

interface CartItemProps {
  item: CartItem_item$key;
}

const containerStyles = {
  display: "flex",
  gap: 2,
  p: 2,
  borderBottom: "1px solid #e0e0e0",
  "&:last-child": {
    borderBottom: "none",
  },
} as const;

const imageStyles = {
  width: 80,
  height: 80,
  borderRadius: 1,
  backgroundColor: "#f5f5f5",
  objectFit: "cover",
} as const;

const imagePlaceholderStyles = {
  width: 80,
  height: 80,
  borderRadius: 1,
  backgroundColor: "#f5f5f5",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
} as const;

const contentStyles = {
  flex: 1,
  display: "flex",
  flexDirection: "column",
  justifyContent: "center",
  minWidth: 0,
} as const;

const nameStyles = {
  fontWeight: 600,
  overflow: "hidden",
  textOverflow: "ellipsis",
  whiteSpace: "nowrap",
} as const;

const priceStyles = {
  color: "primary.main",
  fontWeight: 700,
  mt: 0.5,
} as const;

const deleteButtonStyles = {
  alignSelf: "center",
} as const;

const quantityControlsStyles = {
  display: "flex",
  alignItems: "center",
  gap: 1,
  mt: 1,
} as const;

const quantityTextStyles = {
  minWidth: "30px",
  textAlign: "center",
  fontWeight: 600,
} as const;

export default function CartItem({ item }: CartItemProps) {
  const data = useFragment(CartItemFragment, item);
  const [isUpdating, setIsUpdating] = useState(false);

  const [commitAdd] = useMutation<CartItemAddMutation>(AddToCartMutation);
  const [commitRemove] = useMutation<CartItemRemoveMutation>(
    RemoveFromCartMutation
  );

  const handleIncrease = () => {
    setIsUpdating(true);
    commitAdd({
      variables: {
        input: {
          productId: data.product.id,
          amount: 1,
        },
      },
      onCompleted: () => {
        setIsUpdating(false);
      },
      onError: () => {
        setIsUpdating(false);
      },
    });
  };

  const handleDecrease = () => {
    setIsUpdating(true);
    commitRemove({
      variables: {
        input: {
          productId: data.product.id,
          amount: 1,
        },
      },
      onCompleted: () => {
        setIsUpdating(false);
      },
      onError: () => {
        setIsUpdating(false);
      },
    });
  };

  const handleDelete = () => {
    setIsUpdating(true);
    commitRemove({
      variables: {
        input: {
          productId: data.product.id,
          amount: data.amount,
        },
      },
      onCompleted: () => {
        setIsUpdating(false);
      },
      onError: () => {
        setIsUpdating(false);
      },
    });
  };

  return (
    <Box sx={containerStyles}>
      {data.product.pictureUrl ? (
        <Box
          component="img"
          src={data.product.pictureUrl}
          alt={data.product.name}
          sx={imageStyles}
        />
      ) : (
        <Box sx={imagePlaceholderStyles}>
          <Typography variant="caption" color="text.secondary">
            No Image
          </Typography>
        </Box>
      )}
      <Box sx={contentStyles}>
        <Typography sx={nameStyles}>{data.product.name}</Typography>
        <Typography sx={priceStyles}>
          ${data.product.price.toFixed(2)}
        </Typography>
        <Box sx={quantityControlsStyles}>
          <IconButton
            onClick={handleDecrease}
            disabled={isUpdating}
            size="small"
            color="primary"
          >
            <RemoveIcon fontSize="small" />
          </IconButton>
          <Typography sx={quantityTextStyles}>{data.amount}</Typography>
          <IconButton
            onClick={handleIncrease}
            disabled={isUpdating}
            size="small"
            color="primary"
          >
            <AddIcon fontSize="small" />
          </IconButton>
        </Box>
      </Box>
      <IconButton
        onClick={handleDelete}
        disabled={isUpdating}
        sx={deleteButtonStyles}
        size="medium"
      >
        {isUpdating ? (
          <CircularProgress size={24} />
        ) : (
          <DeleteIcon fontSize="medium" />
        )}
      </IconButton>
    </Box>
  );
}
