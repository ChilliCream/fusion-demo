/**
 * @generated SignedSource<<574e1d2343d6773b0b285864f699d186>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ProductDetailView_product$data = {
  readonly discountedPrice: number;
  readonly id: string;
  readonly name: string;
  readonly pictureUrl: any | null | undefined;
  readonly price: number;
  readonly promotion: {
    readonly discountPercent: number;
  } | null | undefined;
  readonly reviewSummary: {
    readonly nodes: ReadonlyArray<{
      readonly stars: number;
    }> | null | undefined;
    readonly pageInfo: {
      readonly hasNextPage: boolean;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"ProductDetailReviews_product">;
  readonly " $fragmentType": "ProductDetailView_product";
};
export type ProductDetailView_product$key = {
  readonly " $data"?: ProductDetailView_product$data;
  readonly " $fragmentSpreads": FragmentRefs<"ProductDetailView_product">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "ProductDetailView_product",
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
      "alias": "reviewSummary",
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "ProductDetailReviews_product"
    }
  ],
  "type": "Product",
  "abstractKey": null
};

(node as any).hash = "a6b5fa7a83ce00daf15be139b1a930bc";

export default node;
