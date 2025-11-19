import { graphql, usePaginationFragment } from "react-relay";
import type { ProductsPageQuery } from "./__generated__/ProductsPageQuery.graphql";
import type { ProductsList_products$key } from "./__generated__/ProductsList_products.graphql";
import ProductCard from "./ProductCard";
import { Box, Button, CircularProgress } from "@mui/material";

const ProductsListFragment = graphql`
  fragment ProductsList_products on Query
  @refetchable(queryName: "ProductsListPaginationQuery")
  @argumentDefinitions(
    count: { type: "Int", defaultValue: 12 }
    cursor: { type: "String" }
  ) {
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

export default function ProductsList({ queryRef }: ProductsListProps) {
  const { data, loadNext, hasNext, isLoadingNext } =
    usePaginationFragment<ProductsPageQuery, ProductsList_products$key>(
      ProductsListFragment,
      queryRef
    );

  const products = data.products?.edges || [];

  return (
    <>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 400px)",
          gap: 4,
          "@media (max-width: 1300px)": {
            gridTemplateColumns: "repeat(2, 400px)",
          },
          "@media (max-width: 900px)": {
            gridTemplateColumns: "repeat(2, minmax(300px, 400px))",
          },
          "@media (max-width: 768px)": {
            gridTemplateColumns: "1fr",
            maxWidth: "400px",
          },
        }}
      >
        {products.map((edge) => (
          <ProductCard key={edge.node.id} product={edge.node} />
        ))}
      </Box>
      {hasNext && (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 6 }}>
          <Button
            variant="contained"
            size="large"
            onClick={() => loadNext(12)}
            disabled={isLoadingNext}
            sx={{ minWidth: 200 }}
          >
            {isLoadingNext ? (
              <>
                <CircularProgress size={24} sx={{ mr: 1 }} color="inherit" />
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
