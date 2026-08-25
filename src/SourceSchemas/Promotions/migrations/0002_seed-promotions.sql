-- Up Migration

-- The catalog seeded by the Products service is furniture (Table, Couch,
-- Chair, ..., ids 1-18), so the campaign copy speaks furniture-store, and the
-- descriptions stay category-agnostic because promotionForProduct assigns
-- promotions by hashing the product id — a description must fit whatever
-- product it happens to land on. With the current catalog (global ids like
-- "UHJvZHVjdDox"), ids 1-3 each cover at least one product and id 4 covers
-- none, which is why the NULL-description row sits first.
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
