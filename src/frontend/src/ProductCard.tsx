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
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import CheckIcon from "@mui/icons-material/Check";
import { memo, useState } from "react";

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
  const [isAdding, setIsAdding] = useState(false);
  const [isAdded, setIsAdded] = useState(false);
  const [commitAddToCart] = useMutation<ProductCardAddToCartMutation>(
    AddToCartMutation
  );

  const handleAddToCart = () => {
    setIsAdding(true);
    commitAddToCart({
      variables: {
        input: {
          productId: data.id,
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

  return (
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
  );
}

export default memo(ProductCard);
