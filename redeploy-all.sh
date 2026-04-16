#!/usr/bin/env bash
set -euo pipefail

workflows=(
  deploy-accounts
  deploy-cart
  deploy-inventory
  deploy-order
  deploy-payments
  deploy-products
  deploy-reviews
  deploy-shipping
  deploy-gateway
  deploy-load-generator
  deploy-frontend
)

for wf in "${workflows[@]}"; do
  echo "Triggering $wf..."
  gh workflow run "$wf.yml"
done

echo "All workflows triggered."
