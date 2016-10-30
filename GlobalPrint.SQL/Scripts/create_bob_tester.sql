INSERT INTO public."user"(
            user_id, name, email, phone, password_hash, amount_of_money, 
            discriminator, email_confirmed, phone_confirmed, security_stamp, 
            bic, last_activity_date)
    VALUES (666, 'Bob Tester', 'bob.tester@mail.example', '+71234567890', '123123', 100, 
            '', TRUE, TRUE, '', 
            '1234567890', now());
