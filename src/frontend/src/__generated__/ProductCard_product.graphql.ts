/**
 * @generated SignedSource<<a72e21d7e084808db2d21f5658fec123>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ProductCard_product$data = {
  readonly id: string;
  readonly name: string;
  readonly pictureUrl: any | null | undefined;
  readonly price: number;
  readonly " $fragmentType": "ProductCard_product";
};
export type ProductCard_product$key = {
  readonly " $data"?: ProductCard_product$data;
  readonly " $fragmentSpreads": FragmentRefs<"ProductCard_product">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "ProductCard_product",
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
  "type": "Product",
  "abstractKey": null
};

(node as any).hash = "ad1318fb4a200d11ea3437f51920fec1";

export default node;
