/**
 * @generated SignedSource<<ca9ece55ac834ba2521b2fea648ed914>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type AddProductToCartInput = {
  amount: number;
  productId: string;
};
export type CartItemAddMutation$variables = {
  input: AddProductToCartInput;
};
export type CartItemAddMutation$data = {
  readonly addProductToCart: {
    readonly cart: {
      readonly " $fragmentSpreads": FragmentRefs<"CartPopover_cart">;
    } | null | undefined;
  };
};
export type CartItemAddMutation = {
  response: CartItemAddMutation$data;
  variables: CartItemAddMutation$variables;
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
    "name": "CartItemAddMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "AddProductToCartPayload",
        "kind": "LinkedField",
        "name": "addProductToCart",
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
    "name": "CartItemAddMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "AddProductToCartPayload",
        "kind": "LinkedField",
        "name": "addProductToCart",
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
                        "name": "amount",
                        "storageKey": null
                      },
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
    "cacheID": "2ed353e8b1a087fea2a3b7604f5bbd07",
    "id": null,
    "metadata": {},
    "name": "CartItemAddMutation",
    "operationKind": "mutation",
    "text": "mutation CartItemAddMutation(\n  $input: AddProductToCartInput!\n) {\n  addProductToCart(input: $input) {\n    cart {\n      ...CartPopover_cart\n      id\n    }\n  }\n}\n\nfragment CartItem_item on CartItem {\n  id\n  amount\n  addedAt\n  product {\n    id\n    name\n    price\n    pictureUrl\n  }\n}\n\nfragment CartPopover_cart on Cart {\n  id\n  items {\n    nodes {\n      id\n      ...CartItem_item\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9a88931fbdfe1e5068a64b1300a0c225";

export default node;
