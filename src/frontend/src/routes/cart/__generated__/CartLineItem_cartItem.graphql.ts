/**
 * @generated SignedSource<<eeab6eac74bd9658a9254904e92ba1f8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartLineItem_cartItem$data = {
  readonly id: string;
  readonly product: {
    readonly id: string;
    readonly name: string;
    readonly pictureUrl: any | null | undefined;
    readonly price: number;
    readonly promotion: {
      readonly id: string;
    } | null | undefined;
  };
  readonly quantity: number;
  readonly unitPrice: number;
  readonly " $fragmentType": "CartLineItem_cartItem";
};
export type CartLineItem_cartItem$key = {
  readonly " $data"?: CartLineItem_cartItem$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartLineItem_cartItem">;
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
  "name": "CartLineItem_cartItem",
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
    }
  ],
  "type": "CartItem",
  "abstractKey": null
};
})();

(node as any).hash = "c0d0fc49c3e289d8146e7668c850af29";

export default node;
