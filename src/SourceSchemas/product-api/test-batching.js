const fetch = require('node-fetch');

const SERVER_URL = 'http://localhost:4000/graphql';

async function testStandardQuery() {
  console.log('🧪 Testing Standard GraphQL Query...');
  
  const response = await fetch(SERVER_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      query: `
        query ($id: ID!) {
          productById(id: $id) {
            id
            name
            price
            weight
          }
        }
      `,
      variables: {
        id: "1"
      }
    })
  });

  const result = await response.json();
  console.log('✅ Standard Query Result:', JSON.stringify(result, null, 2));
  console.log('');
}

async function testBatchQuery() {
  console.log('🧪 Testing Batch GraphQL Query...');
  
  const response = await fetch(SERVER_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      query: `
        query ($id: ID!) {
          productById(id: $id) {
            id
            name
            price
            weight
          }
        }
      `,
      variables: [
        { id: "1" },
        { id: "2" },
        { id: "3" }
      ]
    })
  });

  const result = await response.json();
  console.log('✅ Batch Query Result:', JSON.stringify(result, null, 2));
  console.log('');
}

async function testProductsQuery() {
  console.log('🧪 Testing Products Query...');
  
  const response = await fetch(SERVER_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      query: `
        query {
          products {
            nodes {
              id
              name
              price
              weight
              dimension {
                length
                width
                height
              }
            }
          }
        }
      `
    })
  });

  const result = await response.json();
  console.log('✅ Products Query Result:', JSON.stringify(result, null, 2));
  console.log('');
}

async function runTests() {
  try {
    console.log('🚀 Starting GraphQL API Tests...\n');
    
    await testStandardQuery();
    await testBatchQuery();
    await testProductsQuery();
    
    console.log('🎉 All tests completed successfully!');
  } catch (error) {
    console.error('❌ Test failed:', error.message);
    console.log('Make sure the server is running on port 4000');
  }
}

// Run tests if this file is executed directly
if (require.main === module) {
  runTests();
}

module.exports = { testStandardQuery, testBatchQuery, testProductsQuery };
