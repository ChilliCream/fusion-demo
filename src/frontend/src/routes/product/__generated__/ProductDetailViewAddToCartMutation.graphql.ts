/**
 * @generated SignedSource<<d4ac990cd416a914451ad25c2d60b08d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type AddProductToCartInput = {
  productId: string;
  quantity: number;
};
export type ProductDetailViewAddToCartMutation$variables = {
  input: AddProductToCartInput;
};
export type ProductDetailViewAddToCartMutation$data = {
  readonly addProductToCart: {
    readonly cart: {
      readonly id: string;
      readonly " $fragmentSpreads": FragmentRefs<"CartBadge_cart">;
    } | null | undefined;
  };
};
export type ProductDetailViewAddToCartMutation = {
  response: ProductDetailViewAddToCartMutation$data;
  variables: ProductDetailViewAddToCartMutation$variables;
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
    "name": "ProductDetailViewAddToCartMutation",
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
                "args": null,
                "kind": "FragmentSpread",
                "name": "CartBadge_cart"
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
    "name": "ProductDetailViewAddToCartMutation",
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
                      (v2/*: any*/)
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
    "cacheID": "4e94671bb1e3e6bb12d943a6b9699b99",
    "id": null,
    "metadata": {},
    "name": "ProductDetailViewAddToCartMutation",
    "operationKind": "mutation",
    "text": "mutation ProductDetailViewAddToCartMutation(\n  $input: AddProductToCartInput!\n) {\n  addProductToCart(input: $input) {\n    cart {\n      id\n      ...CartBadge_cart\n    }\n  }\n}\n\nfragment CartBadge_cart on Cart {\n  items(first: 50) {\n    nodes {\n      quantity\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cb42d8a360fe86917a8b290a1df8f3d3";

export default node;
