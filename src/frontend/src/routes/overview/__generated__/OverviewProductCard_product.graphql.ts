/**
 * @generated SignedSource<<59ab8e31c8ec38c2b08659f9f820f6ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OverviewProductCard_product$data = {
  readonly discountedPrice: number;
  readonly id: string;
  readonly name: string;
  readonly pictureUrl: any | null | undefined;
  readonly price: number;
  readonly promotion: {
    readonly discountPercent: number;
  } | null | undefined;
  readonly reviews: {
    readonly nodes: ReadonlyArray<{
      readonly stars: number;
    }> | null | undefined;
    readonly pageInfo: {
      readonly hasNextPage: boolean;
    };
  } | null | undefined;
  readonly " $fragmentType": "OverviewProductCard_product";
};
export type OverviewProductCard_product$key = {
  readonly " $data"?: OverviewProductCard_product$data;
  readonly " $fragmentSpreads": FragmentRefs<"OverviewProductCard_product">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "OverviewProductCard_product",
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
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Literal",
          "name": "first",
          "value": 50
        }
      ],
      "concreteType": "ProductReviewsConnection",
      "kind": "LinkedField",
      "name": "reviews",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "Review",
          "kind": "LinkedField",
          "name": "nodes",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "stars",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PageInfo",
          "kind": "LinkedField",
          "name": "pageInfo",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasNextPage",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": "reviews(first:50)"
    }
  ],
  "type": "Product",
  "abstractKey": null
};

(node as any).hash = "8f8ab75443ebe856244b9976738c4252";

export default node;
