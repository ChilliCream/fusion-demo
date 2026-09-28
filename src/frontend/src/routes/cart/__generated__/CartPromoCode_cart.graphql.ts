/**
 * @generated SignedSource<<d4f80aa7f1e3e563d4ec884d895f9449>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartPromoCode_cart$data = {
  readonly id: string;
  readonly promoCode: {
    readonly code: string;
    readonly discountPercent: number;
    readonly isExpired: boolean;
    readonly title: string;
  } | null | undefined;
  readonly " $fragmentType": "CartPromoCode_cart";
};
export type CartPromoCode_cart$key = {
  readonly " $data"?: CartPromoCode_cart$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartPromoCode_cart">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "CartPromoCode_cart",
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

(node as any).hash = "a3e7ff0e6b98f6b66b4ed29d6f56b0ed";

export default node;
