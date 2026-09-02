-- Up Migration

-- The campaign copy targets the furniture catalog seeded by the Products
-- service, but descriptions stay category-agnostic: promotionForProduct assigns
-- promotions by hashing the product id, so a description must fit whatever
-- product it lands on.
INSERT INTO promotions (id, title, description, discount_percent)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'Deal of the Week', NULL, 20),
    (2, 'New Home Bundle', 'Stock up on essentials for your new place.', 10),
    (3, 'Showroom Clearance', 'Last season''s floor models have to go.', 30),
    (4, 'Spring Refresh Sale', 'Freshen up your home with a discount on selected pieces.', 15);

SELECT setval(pg_get_serial_sequence('promotions', 'id'), (SELECT max(id) FROM promotions));

-- Down Migration
DELETE FROM promotions WHERE id BETWEEN 1 AND 4;
