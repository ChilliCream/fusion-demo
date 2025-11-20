/**
 * @generated SignedSource<<74953cf66b6ba53d3fd34ba69f608c92>>
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
};
export type CartItemRemoveMutation$variables = {
  input: RemoveProductFromCartInput;
};
export type CartItemRemoveMutation$data = {
  readonly removeProductFromCart: {
    readonly cart: {
      readonly " $fragmentSpreads": FragmentRefs<"CartPopover_cart">;
    } | null | undefined;
  };
};
export type CartItemRemoveMutation = {
  response: CartItemRemoveMutation$data;
  variables: CartItemRemoveMutation$variables;
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "CartItemRemoveMutation",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "CartItemRemoveMutation",
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
                      (v2/*: any*/),
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
    "cacheID": "036c71b10d4e105ec38a0a39f0380db7",
    "id": null,
    "metadata": {},
    "name": "CartItemRemoveMutation",
    "operationKind": "mutation",
    "text": "mutation CartItemRemoveMutation(\n  $input: RemoveProductFromCartInput!\n) {\n  removeProductFromCart(input: $input) {\n    cart {\n      ...CartPopover_cart\n      id\n    }\n  }\n}\n\nfragment CartItem_item on CartItem {\n  id\n  addedAt\n  product {\n    id\n    name\n    price\n    pictureUrl\n  }\n}\n\nfragment CartPopover_cart on Cart {\n  id\n  items {\n    nodes {\n      id\n      ...CartItem_item\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0fb94c5d558d20de764c91eb0d6016ab";

export default node;
