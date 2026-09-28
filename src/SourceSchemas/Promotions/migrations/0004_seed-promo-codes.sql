-- Up Migration
INSERT INTO promo_codes (id, code, title, discount_percent, expires_at)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'SAVE10', '10% off your order', 10, NULL),
    (2, 'HALF', 'Half price', 50, NULL),
    (3, 'EXPIRED20', 'Expired winter deal', 20, '2026-01-01T00:00:00Z');

SELECT setval(pg_get_serial_sequence('promo_codes', 'id'), (SELECT max(id) FROM promo_codes));

-- Down Migration
DELETE FROM promo_codes WHERE id BETWEEN 1 AND 3;
