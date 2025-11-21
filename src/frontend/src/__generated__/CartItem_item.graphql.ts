/**
 * @generated SignedSource<<69b343e06ccbb7027248913087bc8c7a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartItem_item$data = {
  readonly addedAt: any;
  readonly id: string;
  readonly product: {
    readonly id: string;
    readonly name: string;
    readonly pictureUrl: any | null | undefined;
    readonly price: number;
  };
  readonly quantity: number;
  readonly " $fragmentType": "CartItem_item";
};
export type CartItem_item$key = {
  readonly " $data"?: CartItem_item$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartItem_item">;
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
  "name": "CartItem_item",
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
  "type": "CartItem",
  "abstractKey": null
};
})();

(node as any).hash = "5c01dfbaad79560333ac273e92e885e9";

export default node;
