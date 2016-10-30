DELETE FROM public."print_order"
 WHERE user_id IN (
 SELECT user_id 
 FROM public."user"
 WHERE email='93Grey10@mail.ru'
 );
 
 DELETE FROM public."printer_schedule"
 WHERE printer_id IN (
	 SELECT printer_id
	 FROM public."printer"
	 WHERE user_id_owner IN (
		 SELECT user_id 
		 FROM public."user"
		 WHERE email='93Grey10@mail.ru'
	 )
 );  

 DELETE FROM public."printer_service"
 WHERE printer_id IN (
	 SELECT printer_id
	 FROM public."printer"
	 WHERE user_id_owner IN (
		 SELECT user_id 
		 FROM public."user"
		 WHERE email='93Grey10@mail.ru'
	 )
 );  
 
DELETE FROM public."printer"
 WHERE user_id_owner IN (
 SELECT user_id 
 FROM public."user"
 WHERE email='93Grey10@mail.ru'
 ); 
 
 DELETE FROM public."user_offer"
 WHERE user_id IN (
 SELECT user_id 
 FROM public."user"
 WHERE email='93Grey10@mail.ru'
 ); 
 
DELETE FROM public."user"
 WHERE email='93Grey10@mail.ru';