<?php
	# index php
?>

<!doctype html>
<html>
<head>
	<meta charset="UTF-8">
	<meta name="viewport" content="initial-scale=1.0, maximum-scale=1.0">
	
	<title>GlobalPrint</title>
	
	<link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,600i,700" rel="stylesheet">
	<link rel="stylesheet" href="./css/bootstrap.min.css">
	<link rel="stylesheet" href="./css/bootstrap-theme.css">
	<link rel="stylesheet" href="./css/main.css">
	<link rel="stylesheet" href="./css/media.css">
	<link rel="stylesheet" href="./css/owl.carousel.css">
	<link rel="stylesheet" href="./css/owl.theme.default.css">
	<link rel="stylesheet" href="./css/animate.css">
	
	<link rel="shortcut icon" href="./img/favicon.ico" type="image/ico">
	
	<script src="https://use.fontawesome.com/36b364e3c1.js"></script>
	<script src="./js/jquery.js"></script>
	<script src="./js/bootstrap.min.js"></script>
	<script src="./js/owl.carousel.min.js"></script>
	<script src="./js/wow.js"></script>
	<script src="./js/main.js"></script>
	<script src="./js/info.js"></script>
	
	<script>
		$(function() {
			new WOW().init();
		})
	</script>
</head>
<body>
	<!--[if lt IE 8]>
	    <p class="browsehappy">You are using an <strong>outdated</strong> browser. Please <a href="http://browsehappy.com/">upgrade your browser</a> to improve your experience.</p>
	<![endif]-->
	
	<div class="load"></div>
	<div class="topMargin"></div>
	
	<div class="top">
		<div class="topLine">
			<div class="container">
				<div class="col-md-6 hidden-xs hidden-sm">
					<a href="#">Условия использования</a>
					<a href="#">Политика конфедициальности</a>
				</div>
				<div class="col-md-6">
					<div class="languageSelect">
						<div class="langContainer">
							<img src="./img/languages/ru-ru.png" alt=""> <span>Русский</span> <img src="./img/langDrop.png" alt="">
							
							<div class="selectBox">
								<div class="sItem selected">
									<img src="./img/languages/ru-ru.png" alt=""> <span>Русский</span>
								</div>
								<div class="sItem">
									<img src="./img/languages/en-us.png" alt=""> <span>Английский</span>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		
		<?php
			if(!$_GET['page']) {
				@include("timed/default-menu.php");
			}else{
				@include("timed/profile-menu.php");
			}
		?>
		
	</div>
	
	<?php
		if(!$_GET['page']) {
			@include("pages/home.php");
		}else{
			$get = @include("pages/".$_GET['page'].".php");
			
			if(!$get) {
				echo "<div class='container mgTop'>";
				echo "<div class='alert alert-danger'>";
				echo "<b>Ошибка #404:</b> Страница перемещена или отсутствует";
				echo "</div>";
				echo "</div>";
			}
		}
	?>
	
	<div class="btLine">
			<div class="visible-xs visible-sm hidden-md hidden-lg" align="center">
				<a href="#">Условия использования</a>
				<a href="#">Политика конфедициальности</a>
			</div>
	</div>
</body>
</html>