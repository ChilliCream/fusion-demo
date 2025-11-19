import { graphql, usePaginationFragment } from "react-relay";
import type { ProductsPageQuery } from "./__generated__/ProductsPageQuery.graphql";
import type { ProductsList_products$key } from "./__generated__/ProductsList_products.graphql";
import ProductCard from "./ProductCard";
import { Box, Button, CircularProgress } from "@mui/material";
import { useCallback } from "react";

const ProductsListFragment = graphql`
  fragment ProductsList_products on Query
  @refetchable(queryName: "ProductsListPaginationQuery")
  @argumentDefinitions(count: { type: "Int" }, cursor: { type: "String" }) {
    products(first: $count, after: $cursor)
      @connection(key: "ProductsList_products") {
      edges {
        node {
          id
          ...ProductCard_product
        }
      }
    }
  }
`;

interface ProductsListProps {
  queryRef: ProductsList_products$key;
}

const gridStyles = {
  display: "grid",
  gridTemplateColumns: {
    xs: "1fr",
    sm: "repeat(2, 1fr)",
    md: "repeat(2, 1fr)",
    lg: "repeat(3, 1fr)",
  },
  gap: { xs: 2, sm: 3, md: 4 },
  width: "100%",
  maxWidth: { xs: "100%", sm: "700px", md: "900px", lg: "1300px" },
  px: { xs: 2, sm: 3, md: 4 },
} as const;

const loadMoreContainerStyles = {
  display: "flex",
  justifyContent: "center",
  mt: 6,
} as const;

const buttonStyles = { minWidth: 200 } as const;
const progressStyles = { mr: 1 } as const;

export default function ProductsList({ queryRef }: ProductsListProps) {
  const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment<
    ProductsPageQuery,
    ProductsList_products$key
  >(ProductsListFragment, queryRef);

  const products = data.products?.edges || [];

  const handleLoadMore = useCallback(() => {
    loadNext(12);
  }, [loadNext]);

  return (
    <>
      <Box sx={gridStyles}>
        {products.map((edge) => (
          <ProductCard key={edge.node.id} product={edge.node} />
        ))}
      </Box>
      {hasNext && (
        <Box sx={loadMoreContainerStyles}>
          <Button
            variant="contained"
            size="large"
            onClick={handleLoadMore}
            disabled={isLoadingNext}
            sx={buttonStyles}
          >
            {isLoadingNext ? (
              <>
                <CircularProgress size={24} sx={progressStyles} color="inherit" />
                Loading...
              </>
            ) : (
              "Load More"
            )}
          </Button>
        </Box>
      )}
    </>
  );
}
