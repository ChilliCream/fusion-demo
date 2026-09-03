/**
 * @generated SignedSource<<d7e60b8778a71d0d7a8fe238dbf4211a>>
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
          readonly id: string;
        } | null | undefined;
      };
      readonly quantity: number;
      readonly unitPrice: number;
      readonly " $fragmentSpreads": FragmentRefs<"CartLineItem_cartItem">;
    }> | null | undefined;
  } | null | undefined;
  readonly promoCode: {
    readonly code: string;
    readonly discountPercent: number;
    readonly isExpired: boolean;
    readonly title: string;
  } | null | undefined;
  readonly " $fragmentType": "CartView_cart";
};
export type CartView_cart$key = {
  readonly " $data"?: CartView_cart$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartView_cart">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
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
            (v0/*: any*/),
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
                    (v0/*: any*/)
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
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Cart",
  "abstractKey": null
};
})();

(node as any).hash = "6de3f0173408dd509c25962775f9869e";

export default node;
