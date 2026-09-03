-- Up Migration
CREATE TABLE promo_codes (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code text NOT NULL UNIQUE,
    title text NOT NULL,
    discount_percent integer NOT NULL
        CONSTRAINT promo_codes_discount_percent_range
        CHECK (discount_percent BETWEEN 1 AND 100),
    expires_at timestamptz,
    created_at timestamptz NOT NULL DEFAULT now()
);

-- Down Migration
DROP TABLE promo_codes;
