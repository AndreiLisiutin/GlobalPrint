-- Update user add legal requisites columns
ALTER TABLE public."user" DROP COLUMN bic;
ALTER TABLE public."user" ADD COLUMN bank_bic text;
ALTER TABLE public."user" ADD COLUMN legal_full_name text;
COMMENT ON COLUMN public."user".legal_full_name IS '������ ������������ ������������/����������� ����.';
ALTER TABLE public."user" ADD COLUMN legal_short_name text;
COMMENT ON COLUMN public."user".legal_short_name IS '������� ������������ ������������/����������� ����.';
ALTER TABLE public."user" ADD COLUMN legal_address text;
COMMENT ON COLUMN public."user".legal_address IS '����������� ����� ��/��� ����.';
ALTER TABLE public."user" ADD COLUMN inn text;
ALTER TABLE public."user" ADD COLUMN kpp text;
ALTER TABLE public."user" ADD COLUMN ogrn text;
ALTER TABLE public."user" ADD COLUMN payment_account text;
COMMENT ON COLUMN public."user".payment_account IS '��������� ����';
ALTER TABLE public."user" ADD COLUMN bank_name text;
ALTER TABLE public."user" ADD COLUMN bank_correspondent_account text;
COMMENT ON COLUMN public."user".bank_correspondent_account IS '����������������� ���� �����';
ALTER TABLE public."user" ADD COLUMN post_address text;
