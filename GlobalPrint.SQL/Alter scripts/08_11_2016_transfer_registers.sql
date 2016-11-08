-- Table: public.cash_request_status

-- DROP TABLE public.cash_request_status;

CREATE TABLE public.cash_request_status
(
  cash_request_status_id integer NOT NULL DEFAULT nextval('cash_request_status_cash_request_status_id_seq'::regclass),
  name text NOT NULL,
  CONSTRAINT cash_request_status_pkey PRIMARY KEY (cash_request_status_id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.cash_request_status
  OWNER TO postgres;
  
  
-- Table: public.transfers_register

-- DROP TABLE public.transfers_register;

CREATE TABLE public.transfers_register
(
  transfers_register_id integer NOT NULL DEFAULT nextval('transfers_register_transfers_register_id_seq'::regclass),
  created_on timestamp with time zone NOT NULL,
  user_id integer NOT NULL,
  CONSTRAINT transfers_register_pkey PRIMARY KEY (transfers_register_id),
  CONSTRAINT transfers_register_user_id_fkey FOREIGN KEY (user_id)
      REFERENCES public."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.transfers_register
  OWNER TO postgres;
 

-- Table: public.cash_request

-- DROP TABLE public.cash_request;

CREATE TABLE public.cash_request
(
  cash_request_id integer NOT NULL DEFAULT nextval('cash_request_cash_request_id_seq'::regclass),
  user_id integer NOT NULL,
  amount_of_money numeric(10,2) NOT NULL,
  transfers_register_id integer,
  created_on timestamp with time zone NOT NULL,
  cash_request_status_id integer NOT NULL,
  CONSTRAINT cash_request_pkey PRIMARY KEY (cash_request_id),
  CONSTRAINT cash_request_cash_request_status_id_fkey FOREIGN KEY (cash_request_status_id)
      REFERENCES public.cash_request_status (cash_request_status_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT cash_request_transfers_register_id_fkey FOREIGN KEY (transfers_register_id)
      REFERENCES public.transfers_register (transfers_register_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT cash_request_user_id_fkey FOREIGN KEY (user_id)
      REFERENCES public."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.cash_request
  OWNER TO postgres;
  
INSERT INTO public.cash_request_status(cash_request_status_id, name) VALUES (1, 'В процессе');
INSERT INTO public.cash_request_status(cash_request_status_id, name) VALUES (2, 'Завершено успешно');
INSERT INTO public.cash_request_status(cash_request_status_id, name) VALUES (3, 'Завершено неуспешно');
