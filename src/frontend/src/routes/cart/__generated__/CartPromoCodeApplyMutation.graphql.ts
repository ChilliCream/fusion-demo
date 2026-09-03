/**
 * @generated SignedSource<<2a5f7c5ec76b5cea0a41355332f11f2d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ApplyPromoCodeInput = {
  cartId: string;
  code: string;
};
export type CartPromoCodeApplyMutation$variables = {
  input: ApplyPromoCodeInput;
};
export type CartPromoCodeApplyMutation$data = {
  readonly applyPromoCode: {
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
export type CartPromoCodeApplyMutation = {
  response: CartPromoCodeApplyMutation$data;
  variables: CartPromoCodeApplyMutation$variables;
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
    "name": "CartPromoCodeApplyMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ApplyPromoCodePayload",
        "kind": "LinkedField",
        "name": "applyPromoCode",
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
    "name": "CartPromoCodeApplyMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ApplyPromoCodePayload",
        "kind": "LinkedField",
        "name": "applyPromoCode",
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
                        "kind": "ScalarField",
                        "name": "unitPrice",
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
                  (v2/*: any*/)
                ],
                "storageKey": null
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
    "cacheID": "cb99a265ffa924b5aa3545349ade48b7",
    "id": null,
    "metadata": {},
    "name": "CartPromoCodeApplyMutation",
    "operationKind": "mutation",
    "text": "mutation CartPromoCodeApplyMutation(\n  $input: ApplyPromoCodeInput!\n) {\n  applyPromoCode(input: $input) {\n    cart {\n      id\n      ...CartBadge_cart\n      ...CartView_cart\n    }\n    errors {\n      __typename\n      ... on Error {\n        __isError: __typename\n        message\n      }\n    }\n  }\n}\n\nfragment CartBadge_cart on Cart {\n  items(first: 50) {\n    nodes {\n      quantity\n      id\n    }\n  }\n}\n\nfragment CartLineItem_cartItem on CartItem {\n  id\n  quantity\n  unitPrice\n  product {\n    id\n    name\n    pictureUrl\n    price\n    promotion {\n      id\n    }\n  }\n}\n\nfragment CartPromoCode_cart on Cart {\n  id\n  promoCode {\n    code\n    title\n    discountPercent\n    isExpired\n    id\n  }\n}\n\nfragment CartView_cart on Cart {\n  items(first: 50) {\n    nodes {\n      id\n      quantity\n      unitPrice\n      product {\n        price\n        discountedPrice\n        promotion {\n          id\n        }\n        id\n      }\n      ...CartLineItem_cartItem\n    }\n  }\n  promoCode {\n    code\n    title\n    discountPercent\n    isExpired\n    id\n  }\n  ...CartPromoCode_cart\n}\n"
  }
};
})();

(node as any).hash = "f063ab6115970bcf835857f9b4af1a80";

export default node;
