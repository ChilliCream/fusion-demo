import { graphql, useFragment, useMutation } from "react-relay";
import type { CartItem_item$key } from "./__generated__/CartItem_item.graphql";
import type { CartItemRemoveMutation } from "./__generated__/CartItemRemoveMutation.graphql";
import { Box, Typography, IconButton, CircularProgress } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import { useState } from "react";

const CartItemFragment = graphql`
  fragment CartItem_item on CartItem {
    id
    addedAt
    product {
      id
      name
      price
      pictureUrl
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

export default function CartItem({ item }: CartItemProps) {
  const data = useFragment(CartItemFragment, item);
  const [isRemoving, setIsRemoving] = useState(false);
  const [commitRemove] = useMutation<CartItemRemoveMutation>(
    RemoveFromCartMutation
  );

  const handleRemove = () => {
    setIsRemoving(true);
    commitRemove({
      variables: {
        input: {
          productId: data.product.id,
        },
      },
      onCompleted: () => {
        setIsRemoving(false);
      },
      onError: () => {
        setIsRemoving(false);
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
      </Box>
      <IconButton
        onClick={handleRemove}
        disabled={isRemoving}
        sx={deleteButtonStyles}
        size="medium"
      >
        {isRemoving ? (
          <CircularProgress size={24} />
        ) : (
          <DeleteIcon fontSize="medium" />
        )}
      </IconButton>
    </Box>
  );
}
