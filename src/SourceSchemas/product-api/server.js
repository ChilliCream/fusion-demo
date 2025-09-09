const express = require('express');
const cors = require('cors');
const { graphqlHTTP } = require('express-graphql');
const { execute, parse } = require('graphql');
const { makeExecutableSchema } = require('graphql-tools');
const typeDefs = require('./schema');
const resolvers = require('./resolvers');

// Create the executable schema
const schema = makeExecutableSchema({
  typeDefs,
  resolvers
});

const app = express();
const PORT = process.env.PORT || 4000;

// Middleware
app.use(cors());
app.use(express.json({ limit: '10mb' }));

// Custom GraphQL handler with variable batching support
app.post('/graphql', async (req, res) => {
  try {
    const { query, variables, operationName } = req.body;

    if (!query) {
      return res.status(400).json({ error: 'Query is required' });
    }

    // Check if variables is an array (batch execution)
    if (Array.isArray(variables)) {
      console.log('Batch execution detected with', variables.length, 'requests');
      
      // Execute each request in the batch
      const results = await Promise.all(
        variables.map(async (variableSet, index) => {
          try {
            const result = await execute({
              schema,
              document: parse(query),
              variableValues: variableSet,
              operationName
            });
            return {
              variableIndex: index,
              ...result
            };
          } catch (error) {
            console.error('Error in batch execution:', error);
            return {
              variableIndex: index,
              errors: [{
                message: error.message,
                locations: error.locations,
                path: error.path
              }]
            };
          }
        })
      );

      // Convert results to JSONL format
      const jsonlResponse = results.map(result => JSON.stringify(result)).join('\n');
      
      // Set JSONL content type
      res.setHeader('Content-Type', 'application/jsonl');
      return res.send(jsonlResponse);
    } else {
      // Standard GraphQL execution (variables is object, null, or undefined)
      console.log('Standard GraphQL execution');
      
      const result = await execute({
        schema,
        document: parse(query),
        variableValues: variables || {},
        operationName
      });

      return res.json(result);
    }
  } catch (error) {
    console.error('GraphQL execution error:', error);
    return res.status(500).json({
      errors: [{
        message: error.message,
        locations: error.locations,
        path: error.path
      }]
    });
  }
});

// Health check endpoint
app.get('/health', (req, res) => {
  res.json({ status: 'ok', timestamp: new Date().toISOString() });
});

// GraphQL playground endpoint (optional)
app.use('/graphql-playground', graphqlHTTP({
  schema,
  graphiql: true
}));

// Start server
app.listen(PORT, () => {
  console.log(`🚀 Product API GraphQL server running on port ${PORT}`);
  console.log(`📊 GraphQL endpoint: http://localhost:${PORT}/graphql`);
  console.log(`🎮 GraphQL Playground: http://localhost:${PORT}/graphql-playground`);
  console.log(`❤️  Health check: http://localhost:${PORT}/health`);
});

module.exports = app;
