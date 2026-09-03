-- Up Migration
CREATE TABLE cart_promo_codes (
    cart_id text PRIMARY KEY,
    promo_code_id integer NOT NULL REFERENCES promo_codes(id),
    applied_at timestamptz NOT NULL DEFAULT now()
);

-- Down Migration
DROP TABLE cart_promo_codes;
