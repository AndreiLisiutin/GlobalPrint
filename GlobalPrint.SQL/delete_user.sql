-- Function: public.delete_user_by_email(text)

-- DROP FUNCTION public.delete_user_by_email(text);

CREATE OR REPLACE FUNCTION public.delete_user_by_email(_email text)
  RETURNS void AS
$BODY$BEGIN

  DELETE FROM public."print_order" WHERE user_id IN (
    SELECT user_id 
    FROM public."user"
    WHERE email=_email
  );
 
  DELETE FROM public."printer_schedule" WHERE printer_id IN (
    SELECT printer_id
    FROM public."printer"
    WHERE user_id_owner IN (
      SELECT user_id 
      FROM public."user"
      WHERE email=_email
    )
  );  

  DELETE FROM public."printer_service" WHERE printer_id IN (
    SELECT printer_id
    FROM public."printer"
    WHERE user_id_owner IN (
      SELECT user_id 
      FROM public."user"
      WHERE email=_email
    )
  );  
  
  DELETE FROM public."print_order" WHERE printer_id IN (
    SELECT printer_id 
    FROM public."printer"
    WHERE user_id_owner IN (
	  SELECT user_id FROM 
	  FROM public."user"
      WHERE email=_email
	)
  );
 
  DELETE FROM public."printer"
  WHERE user_id_owner IN (
    SELECT user_id 
    FROM public."user"
    WHERE email=_email
  ); 
 
  DELETE FROM public."user_offer" WHERE user_id IN (
    SELECT user_id 
    FROM public."user"
    WHERE email=_email
  ); 
 
  DELETE FROM public."user" WHERE email=_email;

END$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.delete_user_by_email(text)
  OWNER TO postgres;
COMMENT ON FUNCTION public.delete_user_by_email(text) IS 'Delete user by email.';
