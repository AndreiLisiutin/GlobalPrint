<div class="container lk">
	
	<div class="topNavigate">
		<div class="row">
			<div class="col-md-6">
				Моя информация
			</div>
			<div class="col-md-6">
				<div class="nowIn">
					Главная <i class="fa fa-angle-right"></i> <span class="rzd">Личный кабинет</span>
				</div>
			</div>
		</div>
	</div>

	<div class="col-md-4">
		<div class="lkBox lkMenuBox">
			<div class="menuLogo">
				<img src="./img/lkLogo.png" alt="">
			</div>
			
			<ul class="lkMenu">
				<a href="?page=cabinet">
					<li class="<? if(!$_GET['md'] || $_GET['md'] == "home") echo "active";?>">Моя информация</li>
				</a>
				<a href="?page=cabinet&md=orders">
					<li class="<? if($_GET['md'] == "orders") echo "active";?>">Мои заказы</li>
				</a>
				<a href="?page=cabinet&md=income">
					<li class="<? if($_GET['md'] == "income") echo "active";?>">Входящие заказы</li>
				</a>
				<a href="?page=cabinet&md=myprinters">
					<li class="<? if($_GET['md'] == "myprinters") echo "active";?>">Мои принтеры</li>
				</a>
				<a href="?page=cabinet&md=control">
					<li class="<? if($_GET['md'] == "control") echo "active";?>">Управление счётом</li>
				</a>
				<a href="?page=cabinet&md=history">
					<li class="<? if($_GET['md'] == "history") echo "active";?>">История платежей</li>
				</a>
				<a href="?page=cabinet&md=registry">
					<li class="<? if($_GET['md'] == "registry") echo "active";?>">Реестр заказов</li>
				</a>
			</ul>
		</div>
	</div>
	
	<?php
		if(!$_GET['md']) {
			include("./pages/cabinet/home.php");
		}else{
			include("./pages/cabinet/".$_GET['md'].".php");
		}
	?>
	
</div>