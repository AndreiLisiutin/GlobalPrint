CREATE DATABASE global_print
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'Russian_Russia.1251'
       LC_CTYPE = 'Russian_Russia.1251'
       CONNECTION LIMIT = -1;
	   
DROP TABLE print_order;
DROP TABLE print_order_status;
DROP TABLE printer_schedule;
DROP TABLE printer;
DROP TABLE "user";
	   
CREATE TABLE public."user"
(
  user_id serial NOT NULL,
  name text NOT NULL,
  email text,
  phone text,
  login text NOT NULL,
  password text,
  password_hash text,
  amount_of_money numeric(10,2) NOT NULL DEFAULT 0,
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
  printer_id serial NOT NULL,
  name text NOT NULL,
  location text NOT NULL,
  user_id integer NOT NULL,
  latitude numeric(10,7) NOT NULL,
  longtitude numeric(10,7) NOT NULL,
  black_white_print_price numeric(10,2) NOT NULL,
  phone text,
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

CREATE TABLE public.printer_schedule
(
  printer_schedule_id serial NOT NULL,
  day_of_week integer NOT NULL,
  open_time time without time zone NOT NULL,
  close_time time without time zone NOT NULL,
  printer_id integer NOT NULL,
  CONSTRAINT printer_schedule_pkey PRIMARY KEY (printer_schedule_id),
  CONSTRAINT printer_schedule_printer_id_fkey FOREIGN KEY (printer_id)
      REFERENCES public.printer (printer_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.printer_schedule
  OWNER TO postgres;
  
CREATE TABLE public.print_order_status
(
  print_order_status_id serial NOT NULL,
  status text NOT NULL,
  CONSTRAINT print_order_status_pkey PRIMARY KEY (print_order_status_id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.print_order_status
  OWNER TO postgres;  
  
CREATE TABLE public.print_order
(
  print_order_id serial NOT NULL,
  user_id integer NOT NULL,
  printer_id integer NOT NULL,
  document text NOT NULL,
  ordered_on timestamp with time zone NOT NULL,
  printed_on timestamp with time zone,
  price numeric(7,2) NOT NULL,
  pages_count integer NOT NULL,
  secret_code text NOT NULL,
  format text NOT NULL,
  is_both_sides_print boolean NOT NULL,
  print_order_status_id integer NOT NULL,
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
  
INSERT INTO public.user(user_id, name, email, phone, login, password, password_hash)
    VALUES (1, 'Андрей Лисютин', 'lisutin.andrey@gmail.com', '89503175093', 'andrei.lisiutin', 'erwinrommel7', 'APdZMiILQGrWObiEHbxXT2STTxg7yTSLuSsiLJMJP7Fr9jmWtUO3dfjIzHBxASUgbA==');
  
INSERT INTO public.printer(printer_id, name, location, user_id, latitude, longtitude, black_white_print_price, phone)
    VALUES (1, 'Принтер 1', 'ул. Мусина, 61Г, Казань, Республика Татарстан', 1, 55.8342410, 49.1215810, 5, '89503175093');
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (1, '09:00', '18:00', 1);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (2, '09:00', '18:00', 1);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (3, '09:00', '18:00', 1);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (4, '09:00', '18:00', 1);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (5, '09:00', '18:00', 1);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (6, '10:00', '17:00', 1);

INSERT INTO public.printer(printer_id, name, location, user_id, latitude, longtitude, black_white_print_price, phone)
    VALUES (2, 'Принтер 2', 'ул. Максима Горького, 19, Казань, Республика Татарстан', 1, 55.7939614, 49.1328013, 10, '89503175093');
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (1, '09:00', '18:00', 2);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (2, '09:00', '18:00', 2);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (3, '09:00', '18:00', 2);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (4, '09:00', '18:00', 2);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (5, '09:00', '18:00', 2);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (6, '10:00', '17:00', 2);

	
INSERT INTO public.printer(printer_id, name, location, user_id, latitude, longtitude, black_white_print_price, phone)
    VALUES (3, 'Принтер 3', 'просп. Калинина, 42, Пятигорск, Ставропольский край', 1, 44.044384, 43.0663835, 7, '89503175093');
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (1, '09:00', '18:00', 3);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (2, '09:00', '18:00', 3);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (3, '09:00', '18:00', 3);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (4, '09:00', '18:00', 3);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (5, '09:00', '18:00', 3);
INSERT INTO public.printer_schedule(day_of_week, open_time, close_time, printer_id)
    VALUES (6, '10:00', '17:00', 3);
	


INSERT INTO public.print_order_status(print_order_status_id, status)
    VALUES (1, 'Ожидание');
INSERT INTO public.print_order_status(print_order_status_id, status)
    VALUES (2, 'Принят');
INSERT INTO public.print_order_status(print_order_status_id, status)
    VALUES (3, 'Выполнен');
INSERT INTO public.print_order_status(print_order_status_id, status)
    VALUES (4, 'Отклонен');