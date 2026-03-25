import {
  Environment,
  Network,
  RecordSource,
  Store,
  type FetchFunction,
} from "relay-runtime";

const HTTP_ENDPOINT = import.meta.env.VITE_GRAPHQL_ENDPOINT || "http://localhost:5116/graphql/";
const CLIENT_ID = import.meta.env.VITE_GRAPHQL_CLIENT_ID || "";
const CLIENT_VERSION = import.meta.env.VITE_GRAPHQL_CLIENT_VERSION || "";
const ENABLE_PERSISTED_OPERATIONS =
  String(import.meta.env.VITE_ENABLE_PERSISTED_OPERATIONS).toLowerCase() === "true";
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

  if (CLIENT_ID) {
    headers["GraphQL-Client-Id"] = CLIENT_ID;
  }

  if (CLIENT_VERSION) {
    headers["GraphQL-Client-Version"] = CLIENT_VERSION;
  }

  const resp = await fetch(HTTP_ENDPOINT, {
    method: "POST",
    headers,
    body: JSON.stringify(
      ENABLE_PERSISTED_OPERATIONS
        ? {
            id: request.id,
            variables,
          }
        : {
            query: request.text,
            variables,
          },
    ),
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
