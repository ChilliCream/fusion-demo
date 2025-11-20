/**
 * @generated SignedSource<<968de84a07778e8b9cf37d08ff7ac8b8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CartPopover_cart$data = {
  readonly id: string;
  readonly items: {
    readonly nodes: ReadonlyArray<{
      readonly id: string;
      readonly " $fragmentSpreads": FragmentRefs<"CartItem_item">;
    }> | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "CartPopover_cart";
};
export type CartPopover_cart$key = {
  readonly " $data"?: CartPopover_cart$data;
  readonly " $fragmentSpreads": FragmentRefs<"CartPopover_cart">;
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
  "name": "CartPopover_cart",
  "selections": [
    (v0/*: any*/),
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
            (v0/*: any*/),
            {
              "args": null,
              "kind": "FragmentSpread",
              "name": "CartItem_item"
            }
          ],
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

(node as any).hash = "668710a284524cf06a0453df600afa6a";

export default node;
