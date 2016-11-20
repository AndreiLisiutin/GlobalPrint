ALTER TABLE transfers_register ADD COLUMN cash_request_total_count INT NOT NULL DEFAULT 0;
ALTER TABLE transfers_register ALTER COLUMN cash_request_total_count SET DEFAULT NULL;
ALTER TABLE transfers_register ADD COLUMN cash_request_total_amount_of_money NUMERIC(10,2) NOT NULL DEFAULT 0;
ALTER TABLE transfers_register ALTER COLUMN cash_request_total_amount_of_money SET DEFAULT NULL;

UPDATE transfers_register SET cash_request_total_count = (SELECT COUNT(*) FROM cash_request WHERE cash_request.transfers_register_id = transfers_register.transfers_register_id);
UPDATE transfers_register SET cash_request_total_amount_of_money = (SELECT COALESCE(SUM(cash_request.amount_of_money), 0) FROM cash_request WHERE cash_request.transfers_register_id = transfers_register.transfers_register_id);
