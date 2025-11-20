/**
 * @generated SignedSource<<27920567e6e598e664703ec13670c040>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartPopoverCheckoutMutation$variables = Record<PropertyKey, never>;
export type CartPopoverCheckoutMutation$data = {
  readonly checkout: {
    readonly cart: {
      readonly " $fragmentSpreads": FragmentRefs<"CartPopover_cart">;
    } | null | undefined;
  };
};
export type CartPopoverCheckoutMutation = {
  response: CartPopoverCheckoutMutation$data;
  variables: CartPopoverCheckoutMutation$variables;
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
    "name": "CartPopoverCheckoutMutation",
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
              {
                "args": null,
                "kind": "FragmentSpread",
                "name": "CartPopover_cart"
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
    "name": "CartPopoverCheckoutMutation",
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
                "args": null,
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
                      (v0/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "addedAt",
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
                            "name": "price",
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
    "cacheID": "9bc06d67ef4844f0c4301449b3603742",
    "id": null,
    "metadata": {},
    "name": "CartPopoverCheckoutMutation",
    "operationKind": "mutation",
    "text": "mutation CartPopoverCheckoutMutation {\n  checkout {\n    cart {\n      ...CartPopover_cart\n      id\n    }\n  }\n}\n\nfragment CartItem_item on CartItem {\n  id\n  addedAt\n  product {\n    id\n    name\n    price\n    pictureUrl\n  }\n}\n\nfragment CartPopover_cart on Cart {\n  id\n  items {\n    nodes {\n      id\n      ...CartItem_item\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "efa425f4616f940b569b7598965b533b";

export default node;
