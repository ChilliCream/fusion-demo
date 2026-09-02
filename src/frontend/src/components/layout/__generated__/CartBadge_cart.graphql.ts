/**
 * @generated SignedSource<<2ac4fe89d9b97ff1933e321f35b618bc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartBadge_cart$data = {
  readonly items: {
    readonly nodes: ReadonlyArray<{
      readonly quantity: number;
    }> | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "CartBadge_cart";
};
export type CartBadge_cart$key = {
  readonly " $data"?: CartBadge_cart$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartBadge_cart">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "CartBadge_cart",
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
              "name": "quantity",
              "storageKey": null
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

(node as any).hash = "1d0a6206eea640fcdac06cbf2f63f622";

export default node;
