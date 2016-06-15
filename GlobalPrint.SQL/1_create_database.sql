CREATE DATABASE global_print
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'Russian_Russia.1251'
       LC_CTYPE = 'Russian_Russia.1251'
       CONNECTION LIMIT = -1;

CREATE TABLE public."user"
(
  user_id integer NOT NULL DEFAULT nextval('user_user_id_seq'::regclass),
  name text NOT NULL,
  email text,
  phone text,
  login text NOT NULL,
  password text NOT NULL,
  CONSTRAINT user_pkey PRIMARY KEY (user_id),
  CONSTRAINT user_login_key UNIQUE (login)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."user"
  OWNER TO postgres;

CREATE TABLE public.printer
(
  printer_id integer NOT NULL DEFAULT nextval('printer_printer_id_seq'::regclass),
  name text NOT NULL,
  location text NOT NULL,
  user_id integer NOT NULL,
  latitude numeric(10,7) NOT NULL,
  longtitude numeric(10,7) NOT NULL,
  CONSTRAINT printer_pkey PRIMARY KEY (printer_id),
  CONSTRAINT printer_user_id_fkey FOREIGN KEY (user_id)
      REFERENCES public."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT printer_name_key UNIQUE (name)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.printer
  OWNER TO postgres;


CREATE TABLE public.print_order
(
  print_order_id integer NOT NULL DEFAULT nextval('print_order_print_order_id_seq'::regclass),
  user_id integer NOT NULL,
  printer_id integer NOT NULL,
  document text NOT NULL,
  ordered_on timestamp with time zone NOT NULL,
  printed_on timestamp with time zone,
  price numeric(7,2) NOT NULL,
  CONSTRAINT print_order_pkey PRIMARY KEY (print_order_id),
  CONSTRAINT print_order_printer_id_fkey FOREIGN KEY (printer_id)
      REFERENCES public.printer (printer_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT print_order_user_id_fkey FOREIGN KEY (user_id)
      REFERENCES public."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.print_order
  OWNER TO postgres;
