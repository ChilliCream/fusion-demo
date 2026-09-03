/**
 * @generated SignedSource<<f8efb7b7219b23f1d67e8e774ee4b938>>
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
                        "kind": "ScalarField",
                        "name": "lineTotal",
                        "storageKey": null
                      },
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
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "unitPrice",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": "items(first:50)"
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PromoCode",
                "kind": "LinkedField",
                "name": "promoCode",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "code",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "title",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "discountPercent",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "isExpired",
                    "storageKey": null
                  },
                  (v0/*: any*/)
                ],
                "storageKey": null
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
    "cacheID": "8b61e6fb2365566fa91f41808b1c239f",
    "id": null,
    "metadata": {},
    "name": "CartSummaryCheckoutMutation",
    "operationKind": "mutation",
    "text": "mutation CartSummaryCheckoutMutation {\n  checkout {\n    cart {\n      id\n      ...CartBadge_cart\n      ...CartView_cart\n    }\n  }\n}\n\nfragment CartBadge_cart on Cart {\n  items(first: 50) {\n    nodes {\n      quantity\n      id\n    }\n  }\n}\n\nfragment CartLineItem_cartItem on CartItem {\n  id\n  quantity\n  unitPrice\n  lineTotal\n  product {\n    id\n    name\n    pictureUrl\n    price\n    promotion {\n      id\n    }\n  }\n}\n\nfragment CartView_cart on Cart {\n  items(first: 50) {\n    nodes {\n      id\n      quantity\n      lineTotal\n      product {\n        price\n        discountedPrice\n        promotion {\n          id\n        }\n        id\n      }\n      ...CartLineItem_cartItem\n    }\n  }\n  promoCode {\n    code\n    title\n    discountPercent\n    isExpired\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "00564eae557394390003e47e2221c4db";

export default node;
