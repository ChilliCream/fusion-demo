/**
 * @generated SignedSource<<0a48a6e0ba8357f8043ea9e92f6ba4c6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ProductsList_products$data = {
  readonly products: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly error?: number;
        readonly id: string;
        readonly " $fragmentSpreads": FragmentRefs<"ProductCard_product">;
      };
    }> | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "ProductsList_products";
};
export type ProductsList_products$key = {
  readonly " $data"?: ProductsList_products$data;
  readonly " $fragmentSpreads": FragmentRefs<"ProductsList_products">;
};

import ProductsListPaginationQuery_graphql from './ProductsListPaginationQuery.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "products"
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "count"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "cursor"
    },
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "includeErrorField"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*: any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*: any*/)
      },
      "fragmentPathInResult": [],
      "operation": ProductsListPaginationQuery_graphql
    }
  },
  "name": "ProductsList_products",
  "selections": [
    {
      "alias": "products",
      "args": null,
      "concreteType": "ProductsConnection",
      "kind": "LinkedField",
      "name": "__ProductsList_products_connection",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ProductsEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "Product",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "id",
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "ProductCard_product"
                },
                {
                  "condition": "includeErrorField",
                  "kind": "Condition",
                  "passingValue": true,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "error",
                      "storageKey": null
                    }
                  ]
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "__typename",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cursor",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PageInfo",
          "kind": "LinkedField",
          "name": "pageInfo",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "endCursor",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasNextPage",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0f19dec5118eea7a988627df78724142";

export default node;
