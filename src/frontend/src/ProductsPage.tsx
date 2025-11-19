import { Suspense } from "react";
import { graphql, useLazyLoadQuery } from "react-relay";
import type { ProductsPageQuery } from "./__generated__/ProductsPageQuery.graphql";
import ProductsList from "./ProductsList";
import { Typography, Box, CircularProgress } from "@mui/material";

const ProductsPageQueryNode = graphql`
  query ProductsPageQuery($count: Int!, $cursor: String) {
    ...ProductsList_products @arguments(count: $count, cursor: $cursor)
  }
`;

function ProductsPageContent() {
  const data = useLazyLoadQuery<ProductsPageQuery>(ProductsPageQueryNode, {
    count: 6,
  });

  return (
    <Box
      sx={{
        width: "100vw",
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        py: 4,
        px: 2,
      }}
    >
      <Typography
        variant="h2"
        component="h1"
        align="center"
        sx={{
          mb: 6,
          fontWeight: 900,
          fontSize: { xs: "2.5rem", sm: "3rem", md: "3.5rem" },
          color: "primary.main",
          textTransform: "uppercase",
          letterSpacing: "0.1em",
          background: "linear-gradient(45deg, #1976d2 30%, #42a5f5 90%)",
          WebkitBackgroundClip: "text",
          WebkitTextFillColor: "transparent",
          backgroundClip: "text",
          textShadow: "0 2px 4px rgba(25, 118, 210, 0.1)",
          position: "relative",
        }}
      >
        Products
      </Typography>
      <ProductsList queryRef={data} />
    </Box>
  );
}

export default function ProductsPage() {
  return (
    <Suspense
      fallback={
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            minHeight: "100vh",
          }}
        >
          <CircularProgress size={60} />
        </Box>
      }
    >
      <ProductsPageContent />
    </Suspense>
  );
}
