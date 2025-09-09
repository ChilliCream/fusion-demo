// Helper function to encode ID in base64 with Product: prefix
function encodeProductId(id) {
  return Buffer.from(`Product:${id}`).toString('base64');
}

// Helper function to decode base64 ID back to original format
function decodeProductId(encodedId) {
  try {
    return Buffer.from(encodedId, 'base64').toString('utf-8');
  } catch (error) {
    return null;
  }
}

// Product data matching the C# ProductContext with base64 encoded IDs
const products = [
  {
    id: encodeProductId("1"),
    name: "Table",
    price: 899.99,
    weight: 100,
    dimension: {
      length: 120.0,
      width: 80.0,
      height: 75.0
    },
    pictureUrl: null,
    pictureFileName: null
  },
  {
    id: encodeProductId("2"), 
    name: "Couch",
    price: 1299.50,
    weight: 1000,
    dimension: {
      length: 200.0,
      width: 90.0,
      height: 85.0
    },
    pictureUrl: null,
    pictureFileName: null
  },
  {
    id: encodeProductId("3"),
    name: "Chair", 
    price: 54,
    weight: 50,
    dimension: {
      length: 50.0,
      width: 50.0,
      height: 100.0
    },
    pictureUrl: null,
    pictureFileName: null
  }
];

// Helper function to create pagination info
function createPageInfo(hasNextPage = false, hasPreviousPage = false, startCursor = null, endCursor = null) {
  return {
    hasNextPage,
    hasPreviousPage,
    startCursor,
    endCursor
  };
}

// Helper function to create edges
function createEdges(products) {
  return products.map((product, index) => ({
    cursor: Buffer.from(`cursor-${index}`).toString('base64'),
    node: product
  }));
}

const resolvers = {
  // Custom scalar resolvers
  URL: {
    serialize: (value) => value,
    parseValue: (value) => value,
    parseLiteral: (ast) => ast.value
  },

  Upload: {
    serialize: (value) => value,
    parseValue: (value) => value,
    parseLiteral: (ast) => ast.value
  },

  // Type resolvers
  Node: {
    __resolveType(obj) {
      if (obj.name) {
        return 'Product';
      }
      return null;
    }
  },

  Error: {
    __resolveType(obj) {
      if (obj.productId !== undefined) {
        return 'UnknownProductError';
      }
      return null;
    }
  },

  UploadProductPictureError: {
    __resolveType(obj) {
      if (obj.productId !== undefined) {
        return 'UnknownProductError';
      }
      return null;
    }
  },

  // Query resolvers
  Query: {
    node: (parent, { id }) => {
      return products.find(product => product.id === id);
    },

    nodes: (parent, { ids }) => {
      return ids.map(id => products.find(product => product.id === id)).filter(Boolean);
    },

    productById: (parent, { id }) => {
      return products.find(product => product.id === id);
    },

    products: (parent, { first, after, last, before }) => {
      // Simple pagination implementation
      let result = [...products];
      
      // For simplicity, we'll return all products with basic pagination info
      const edges = createEdges(result);
      const pageInfo = createPageInfo(false, false, edges[0]?.cursor, edges[edges.length - 1]?.cursor);
      
      return {
        pageInfo,
        edges,
        nodes: result
      };
    }
  }
};

module.exports = resolvers;
