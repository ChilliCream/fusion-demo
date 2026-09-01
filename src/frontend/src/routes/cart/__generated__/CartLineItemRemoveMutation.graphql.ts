/**
 * @generated SignedSource<<4321aa4555343226cd7d3d5ae87b5848>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type RemoveProductFromCartInput = {
  productId: string;
  quantity: number;
};
export type CartLineItemRemoveMutation$variables = {
  input: RemoveProductFromCartInput;
};
export type CartLineItemRemoveMutation$data = {
  readonly removeProductFromCart: {
    readonly cart: {
      readonly id: string;
      readonly " $fragmentSpreads": FragmentRefs<"CartBadge_cart" | "CartView_cart">;
    } | null | undefined;
    readonly errors: ReadonlyArray<{
      readonly __typename: string;
      readonly message?: string;
    }> | null | undefined;
  };
};
export type CartLineItemRemoveMutation = {
  response: CartLineItemRemoveMutation$data;
  variables: CartLineItemRemoveMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": null,
  "kind": "LinkedField",
  "name": "errors",
  "plural": true,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "__typename",
      "storageKey": null
    },
    {
      "kind": "InlineFragment",
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "message",
          "storageKey": null
        }
      ],
      "type": "Error",
      "abstractKey": "__isError"
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "CartLineItemRemoveMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "RemoveProductFromCartPayload",
        "kind": "LinkedField",
        "name": "removeProductFromCart",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "Cart",
            "kind": "LinkedField",
            "name": "cart",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "args": null,
                "kind": "FragmentSpread",
                "name": "CartBadge_cart"
              },
              {
                "args": null,
                "kind": "FragmentSpread",
                "name": "CartView_cart"
              }
            ],
            "storageKey": null
          },
          (v3/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "CartLineItemRemoveMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "RemoveProductFromCartPayload",
        "kind": "LinkedField",
        "name": "removeProductFromCart",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "Cart",
            "kind": "LinkedField",
            "name": "cart",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": [
                  {
                    "kind": "Literal",
                    "name": "first",
                    "value": 50
                  }
                ],
                "concreteType": "CartItemsConnection",
                "kind": "LinkedField",
                "name": "items",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CartItem",
                    "kind": "LinkedField",
                    "name": "nodes",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "quantity",
                        "storageKey": null
                      },
                      (v2/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Product",
                        "kind": "LinkedField",
                        "name": "product",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "price",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "discountedPrice",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "Promotion",
                            "kind": "LinkedField",
                            "name": "promotion",
                            "plural": false,
                            "selections": [
                              {
                                "alias": null,
                                "args": null,
                                "kind": "ScalarField",
                                "name": "discountPercent",
                                "storageKey": null
                              },
                              (v2/*: any*/)
                            ],
                            "storageKey": null
                          },
                          (v2/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "name",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "pictureUrl",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": "items(first:50)"
              }
            ],
            "storageKey": null
          },
          (v3/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d8578d7244674d8a12be5afeb7c2e63c",
    "id": null,
    "metadata": {},
    "name": "CartLineItemRemoveMutation",
    "operationKind": "mutation",
    "text": "mutation CartLineItemRemoveMutation(\n  $input: RemoveProductFromCartInput!\n) {\n  removeProductFromCart(input: $input) {\n    cart {\n      id\n      ...CartBadge_cart\n      ...CartView_cart\n    }\n    errors {\n      __typename\n      ... on Error {\n        __isError: __typename\n        message\n      }\n    }\n  }\n}\n\nfragment CartBadge_cart on Cart {\n  items(first: 50) {\n    nodes {\n      quantity\n      id\n    }\n  }\n}\n\nfragment CartLineItem_cartItem on CartItem {\n  id\n  quantity\n  product {\n    id\n    name\n    pictureUrl\n    price\n    discountedPrice\n    promotion {\n      discountPercent\n      id\n    }\n  }\n}\n\nfragment CartView_cart on Cart {\n  items(first: 50) {\n    nodes {\n      id\n      quantity\n      product {\n        price\n        discountedPrice\n        promotion {\n          discountPercent\n          id\n        }\n        id\n      }\n      ...CartLineItem_cartItem\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0317827fdad92741880e46724dfd8f3b";

export default node;
