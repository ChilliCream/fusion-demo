/**
 * @generated SignedSource<<298348d7fbb89952a72756f70a702a7a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartView_cart$data = {
  readonly items: {
    readonly nodes: ReadonlyArray<{
      readonly id: string;
      readonly product: {
        readonly discountedPrice: number;
        readonly price: number;
        readonly promotion: {
          readonly discountPercent: number;
        } | null | undefined;
      };
      readonly quantity: number;
      readonly " $fragmentSpreads": FragmentRefs<"CartLineItem_cartItem">;
    }> | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "CartView_cart";
};
export type CartView_cart$key = {
  readonly " $data"?: CartView_cart$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartView_cart">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "CartView_cart",
  "selections": [
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
              "name": "id",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "quantity",
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
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "discountPercent",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "args": null,
              "kind": "FragmentSpread",
              "name": "CartLineItem_cartItem"
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": "items(first:50)"
    }
  ],
  "type": "Cart",
  "abstractKey": null
};

(node as any).hash = "7b2a4377f6a58f68ef13feab35db0a80";

export default node;
