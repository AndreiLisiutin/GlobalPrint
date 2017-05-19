GlobalPrint.namespace('GlobalPrint.UserProfile.UserProfileEdit');
(function (UserProfileEdit) {

	UserProfileEdit.defineValidation = function () {
		$("#userProfileEditForm").validate({
			rules: {
				"UserName": {
					required: true
				},
				"Email": {
					required: true,
					email: true
				}
			}
		});
	};

	var getDataByBic = function (bic, callback) {
		$.ajax({
			type: 'GET',
			contentType: 'application/json',
			dataType: 'json',
			data: { bic: bic },
			url: '/UserProfile/GetBankInfo'
		}).done(function (json) {
			if (!json) {
				console.log('Error: ajax response is empty.');
				return;
			}
			printerServices = json;

			if (callback) {
				callback(json);
			}
		}).fail(function () {
			console.log('Error: ajax call failed.');
		});
	};

	UserProfileEdit.fillDataByBic = function () {
		var bankBic = $('#bankBic');
		var bankName = $('#bankName');
		var bankCorrespondentAccount = $('#bankCorrespondentAccount');

		if (!bankBic.val()) {
			return;
		}

		var bankInfoCallback = function (json) {
			if (json) {
				bankName.val(json.ShortName);
				bankCorrespondentAccount.val(json.CorrespondentAccount);
			} else {
				bankName.val(null);
				bankCorrespondentAccount.val(null);
			}
		};

		getDataByBic(bankBic.val(), bankInfoCallback)
	};

	UserProfileEdit.geolocateLatLong = function (cmp) {
		// Call Google geolocation service to find out Latitude, Longtitude 
		// of address string and format the address
		var location = $(cmp).val();

		GlobalPrint.Utils.Services.googleGeolocation(location, function (result) {
			$(cmp).val(result.address);
		});
	};

})(GlobalPrint.UserProfile.UserProfileEdit);

$(document).ready(function () {
	GlobalPrint.UserProfile.UserProfileEdit.defineValidation();

	$("#autoFillBankData").click(function (e) {
		e.preventDefault();
		GlobalPrint.UserProfile.UserProfileEdit.fillDataByBic();
	});
	$('#legalGeolocation').click(function () {
		var legalAddress = $('#legalAddress');
		GlobalPrint.UserProfile.UserProfileEdit.geolocateLatLong(legalAddress);
	});
	$('#postGeolocation').click(function () {
		var postAddress = $('#postAddress');
		GlobalPrint.UserProfile.UserProfileEdit.geolocateLatLong(postAddress);
	});

	var handleFileUploadError = function (error) {
		console.error(error);
	};

	$('#userPhotoFileupload').fileupload({
		acceptFileTypes: /(\.|\/)(jpe?g|png|gif)$/i,
		maxFileSize: 1024 * 1024 * 10, // = 10Mb
		autoUpload: false,
		maxNumberOfFiles: 1,
		dataType: 'json',
		messages: {
			maxNumberOfFiles: 'Превышено максимально допустимое количество файлов для загрузки',
			acceptFileTypes: 'Тип файла недопустим',
			maxFileSize: 'Размер файла превышает максимально допустимый'
		}
	}).on('fileuploadadd', function (e, data) {
		var $this = $(this);
		var file = data.files[0];
	}).on('fileuploadprocessdone', function (e, data) {
		var index = data.index,
			file = data.files[index];

		// Показать файл
		var reader = new FileReader();
		reader.readAsDataURL(file);
		reader.onload = function (e) {
			GlobalPrint.Utils.CommonUtils.showCropWindow(
				e.target.result,
				function (croppedPhoto) {
					$('#userPhotoPreview').attr('src', croppedPhoto);
				}
			);
		};
	}).on('fileuploadprocessfail', function (e, data) {
		var index = data.index,
			file = data.files[index];
		// File validation error
		if (data.files[0].error) {
			handleFileUploadError(data.files[0].error);
		}
	});

});

