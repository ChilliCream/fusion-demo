-- Up Migration
INSERT INTO promotions (id, title, description, discount_percent)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'Peak Season Kickoff', 'Kick off the hiking season with a discount on selected gear.', 15),
    (2, 'Base Camp Bundle', 'Stock up on camp essentials before the first ascent.', 10),
    (3, 'Summit Clearance', 'Last season''s summit gear has to go.', 30),
    (4, 'Trailhead Deal of the Week', NULL, 20);

SELECT setval(pg_get_serial_sequence('promotions', 'id'), (SELECT max(id) FROM promotions));

-- Down Migration
DELETE FROM promotions WHERE id BETWEEN 1 AND 4;
