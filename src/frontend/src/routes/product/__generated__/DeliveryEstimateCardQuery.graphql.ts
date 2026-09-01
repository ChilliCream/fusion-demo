/**
 * @generated SignedSource<<1a084ed7ef796ea5b6f4c34502c2198e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeliveryEstimateCardQuery$variables = {
  id: string;
  zip: string;
};
export type DeliveryEstimateCardQuery$data = {
  readonly productById: {
    readonly deliveryEstimate: number;
  } | null | undefined;
};
export type DeliveryEstimateCardQuery = {
  response: DeliveryEstimateCardQuery$data;
  variables: DeliveryEstimateCardQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "id"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zip"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "id"
  }
],
v2 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "zip",
      "variableName": "zip"
    }
  ],
  "kind": "ScalarField",
  "name": "deliveryEstimate",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "DeliveryEstimateCardQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "Product",
        "kind": "LinkedField",
        "name": "productById",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "DeliveryEstimateCardQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "Product",
        "kind": "LinkedField",
        "name": "productById",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "9179a24b6fb7c02fc3fb7ad465a53e5d",
    "id": null,
    "metadata": {},
    "name": "DeliveryEstimateCardQuery",
    "operationKind": "query",
    "text": "query DeliveryEstimateCardQuery(\n  $id: ID!\n  $zip: String!\n) {\n  productById(id: $id) {\n    deliveryEstimate(zip: $zip)\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "1fe35a4dd4378bf5a2a04b5d2fbc6551";

export default node;
