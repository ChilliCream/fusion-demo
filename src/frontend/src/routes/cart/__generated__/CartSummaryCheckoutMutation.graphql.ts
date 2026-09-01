/**
 * @generated SignedSource<<b29729db7448e9b2ff84c44b3947a95b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartSummaryCheckoutMutation$variables = Record<PropertyKey, never>;
export type CartSummaryCheckoutMutation$data = {
  readonly checkout: {
    readonly cart: {
      readonly id: string;
      readonly " $fragmentSpreads": FragmentRefs<"CartBadge_cart" | "CartView_cart">;
    } | null | undefined;
  };
};
export type CartSummaryCheckoutMutation = {
  response: CartSummaryCheckoutMutation$data;
  variables: CartSummaryCheckoutMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "CartSummaryCheckoutMutation",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CheckoutPayload",
        "kind": "LinkedField",
        "name": "checkout",
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
              (v0/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "CartSummaryCheckoutMutation",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CheckoutPayload",
        "kind": "LinkedField",
        "name": "checkout",
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
              (v0/*: any*/),
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
                      (v0/*: any*/),
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
                              (v0/*: any*/)
                            ],
                            "storageKey": null
                          },
                          (v0/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "de1cdb66902239ca414030ebcec45915",
    "id": null,
    "metadata": {},
    "name": "CartSummaryCheckoutMutation",
    "operationKind": "mutation",
    "text": "mutation CartSummaryCheckoutMutation {\n  checkout {\n    cart {\n      id\n      ...CartBadge_cart\n      ...CartView_cart\n    }\n  }\n}\n\nfragment CartBadge_cart on Cart {\n  items(first: 50) {\n    nodes {\n      quantity\n      id\n    }\n  }\n}\n\nfragment CartLineItem_cartItem on CartItem {\n  id\n  quantity\n  product {\n    id\n    name\n    pictureUrl\n    price\n    discountedPrice\n    promotion {\n      id\n    }\n  }\n}\n\nfragment CartView_cart on Cart {\n  items(first: 50) {\n    nodes {\n      id\n      quantity\n      product {\n        price\n        discountedPrice\n        promotion {\n          id\n        }\n        id\n      }\n      ...CartLineItem_cartItem\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "00564eae557394390003e47e2221c4db";

export default node;
