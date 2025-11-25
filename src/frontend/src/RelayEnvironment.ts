import {
  Environment,
  Network,
  RecordSource,
  Store,
  type FetchFunction,
} from "relay-runtime";

const HTTP_ENDPOINT = import.meta.env.VITE_GRAPHQL_ENDPOINT || "http://localhost:5116/graphql/";
const TOKEN_KEY = 'auth_token';

const fetchFn: FetchFunction = async (request, variables) => {
  // Get token from localStorage
  const token = localStorage.getItem(TOKEN_KEY);

  const headers: HeadersInit = {
    Accept:
      "application/graphql-response+json; charset=utf-8, application/json; charset=utf-8",
    "Content-Type": "application/json",
  };

  // Add Authorization header if token exists
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const resp = await fetch(HTTP_ENDPOINT, {
    method: "POST",
    headers,
    body: JSON.stringify({
      query: request.text,
      variables,
    }),
  });

  return await resp.json();
};

function createRelayEnvironment() {
  return new Environment({
    network: Network.create(fetchFn),
    store: new Store(new RecordSource()),
  });
}

export const RelayEnvironment = createRelayEnvironment();
