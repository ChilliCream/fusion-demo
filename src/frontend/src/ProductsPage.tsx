import { Suspense, Component, type ReactNode } from "react";
import { graphql, useLazyLoadQuery } from "react-relay";
import type { ProductsPageQuery } from "./__generated__/ProductsPageQuery.graphql";
import ProductsList from "./ProductsList";
import { Box, CircularProgress, Alert } from "@mui/material";

const ProductsPageQueryNode = graphql`
  query ProductsPageQuery($count: Int!, $cursor: String) {
    ...ProductsList_products @arguments(count: $count, cursor: $cursor)
  }
`;

const errorAlertStyles = {
  mt: 4,
  fontSize: "1.25rem",
  py: 3,
  px: 3,
  "& .MuiAlert-icon": {
    fontSize: "2rem",
  },
} as const;

class ProductsErrorBoundary extends Component<
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
          Products can not be loaded right now
        </Alert>
      );
    }

    return this.props.children;
  }
}

function ProductsPageContent() {
  const data = useLazyLoadQuery<ProductsPageQuery>(ProductsPageQueryNode, {
    count: 6,
  });

  return <ProductsList queryRef={data} />;
}

const containerStyles = {
  width: "100%",
  minHeight: "100vh",
  display: "flex",
  flexDirection: "column",
  alignItems: "center",
  pb: { xs: 2, sm: 4 },
} as const;

const loadingContainerStyles = {
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  py: 8,
} as const;

export default function ProductsPage() {
  return (
    <Box sx={containerStyles}>
      <ProductsErrorBoundary>
        <Suspense
          fallback={
            <Box sx={loadingContainerStyles}>
              <CircularProgress size={60} />
            </Box>
          }
        >
          <ProductsPageContent />
        </Suspense>
      </ProductsErrorBoundary>
    </Box>
  );
}
