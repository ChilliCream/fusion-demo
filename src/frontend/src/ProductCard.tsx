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

export default function ProductCard({ product }: ProductCardProps) {
  const data = useFragment(ProductCardFragment, product);

  const handleAddToCart = () => {
    console.log("Added to cart:", data.name);
  };

  return (
    <Card
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        transition: "transform 0.2s, box-shadow 0.2s",
        "&:hover": {
          transform: "translateY(-8px)",
          boxShadow: 6,
        },
      }}
    >
      <CardMedia
        component={data.pictureUrl ? "img" : "div"}
        image={data.pictureUrl || undefined}
        alt={data.name}
        sx={{
          height: 350,
          backgroundColor: "#f5f5f5",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          objectFit: "cover",
        }}
      >
        {!data.pictureUrl && (
          <Typography color="text.secondary">No Image</Typography>
        )}
      </CardMedia>
      <CardContent sx={{ flexGrow: 1, pb: 1 }}>
        <Typography
          gutterBottom
          variant="h5"
          component="h2"
          sx={{
            overflow: "hidden",
            textOverflow: "ellipsis",
            whiteSpace: "nowrap",
          }}
        >
          {data.name}
        </Typography>
        <Typography variant="h6" color="primary" fontWeight="bold">
          ${data.price.toFixed(2)}
        </Typography>
      </CardContent>
      <CardActions sx={{ p: 2, pt: 0 }}>
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
