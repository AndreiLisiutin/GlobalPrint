UPDATE public."user"
   SET created_on = now()
   WHERE created_on IS NULL;
   
ALTER TABLE public."user"
   ADD COLUMN created_on DATE NOT NULL;
