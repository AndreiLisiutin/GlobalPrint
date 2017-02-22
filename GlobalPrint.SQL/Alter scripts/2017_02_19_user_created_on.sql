ALTER TABLE public."user"
   ADD COLUMN created_on DATE;

UPDATE public."user"
   SET created_on = now()
   WHERE created_on IS NULL;
   
ALTER TABLE public."user"
   ALTER COLUMN created_on SET NOT NULL;
