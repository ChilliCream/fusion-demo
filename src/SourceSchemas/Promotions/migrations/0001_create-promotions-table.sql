-- Up Migration
CREATE TABLE promotions (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title text NOT NULL,
    description text,
    discount_percent integer NOT NULL
        CONSTRAINT promotions_discount_percent_range
        CHECK (discount_percent BETWEEN 1 AND 100)
);

-- Down Migration
DROP TABLE promotions;
