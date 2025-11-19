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
    count: 12,
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
        gutterBottom
        sx={{ mb: 6, fontWeight: "bold" }}
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
