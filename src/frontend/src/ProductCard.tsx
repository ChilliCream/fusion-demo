import { graphql, useFragment } from "react-relay";
import type { ProductCard_product$key } from "./__generated__/ProductCard_product.graphql";
import {
  Card,
  CardMedia,
  CardContent,
  CardActions,
  Typography,
  Button,
} from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import { memo } from "react";

const ProductCardFragment = graphql`
  fragment ProductCard_product on Product {
    id
    name
    price
    pictureUrl
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

  const handleAddToCart = () => {
    alert("Added to cart: " + data.name);
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
          startIcon={<ShoppingCartIcon />}
          onClick={handleAddToCart}
          size="large"
        >
          Add to Cart
        </Button>
      </CardActions>
    </Card>
  );
}

export default memo(ProductCard);
