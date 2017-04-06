<div class="container verticalWrap">
	<div class="loginForm">
		<div class="loginTitle">
			Войдите с помощью
			
			<a href="#"><img src="./img/login_vk.png" alt="VK"></a>
			<a href="#"><img src="./img/login_google.png" alt="Google+"></a>
		</div>
		
		<div class="or">
			Или зарегистрируйтесь в форме ниже
		</div>
		
		<div class="loginFormLabel">
			<p>E-Mail</p>
			
			<input type="text" class="loginFormInput">
		</div>
		
		<div class="loginFormLabel">
			<p>Имя пользователя</p>
			
			<input type="text" class="loginFormInput">
		</div>
		
		<div class="loginFormLabel">
			<p>Пароль</p>
			
			<input type="text" class="loginFormInput">
		</div>
		
		<div class="loginFormLabel">
			<p>Подтвердите пароль</p>
			
			<input type="text" class="loginFormInput">
		</div>
		
		<div class="row" style="margin-top:15px;">
			<div class="col-md-1 col-xs-2" style="text-align:center">
				<input type="checkbox" class="setClick">
				
				<div class="loginCheckBox" onclick="$(this).toggleClass('checked'); $('.setClick').click();">
					<div class="checked"></div>
				</div>
			</div>
			<div class="col-md-6 col-xs-10" style="font-size: 11px;">
				<div class="row">
					Настоящим подтверждаю, что я ознакомлен <br>
					и согласен с условиями Оферты и Политикой <br>
					Конфиденциальности.
				</div>
			</div>
			<div class="col-md-5" align="right">
				<button class="goReg">РЕГИСТРАЦИЯ</button>
			</div>
		</div>
	</div>
</div>