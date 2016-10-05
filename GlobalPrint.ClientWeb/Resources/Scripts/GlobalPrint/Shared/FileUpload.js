GlobalPrint.namespace('GlobalPrint.Shared.FileUpload');
(function (FileUpload) {
    'use strict';

    var formatBytes = function (bytes) {
        if (bytes == 0) return '0 Byte';
        var k = 1000; // or 1024 for binary
        var dm = 2;
        var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
        var i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    };

    var handleError = function (error) {
        var textfield = $('#fileupload').parents('.input-group').find(':text');
        var errorLabel = $('#fileupload').parents('.input-group').find('#errorText');
        var progressBar = $('#fileupload').parents('.input-group').find('#progress .progress-bar');

        progressBar.css('width', 0 + '%');
        progressBar.hide();
        errorLabel.show();
        errorLabel.text(error);
    };

    FileUpload.initFileUpload = function () {
        $('#fileupload').fileupload({
            url: '/Order/UploadFile',
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png|pdf|doc|docx|tif|tiff)$/i,
            paramName: 'gpUserFile',
            maxFileSize: 1024 * 1024 * 10, // = 10Mb
            autoUpload: true,
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
            var fileName = file.name + ' (' + formatBytes(file.size) + ')';
            var textfield = $('#fileupload').parents('.input-group').find(':text');
            var errorLabel = $('#fileupload').parents('.input-group').find('#errorText');
            var progressBar = $('#fileupload').parents('.input-group').find('#progress .progress-bar');

            progressBar.show();
            errorLabel.hide();
            textfield.val(fileName);

            // File validation
            if (data.autoUpload || (data.autoUpload !== false && $(this).fileupload('option', 'autoUpload'))) {
                var validation = data.process(function () {
                    return $this.fileupload('process', data);
                });
            }
        }).on('fileuploadprocessdone', function (e, data) {
            var index = data.index,
                file = data.files[index];
            // Submit file upload after validation
            data.submit();
        }).on('fileuploadprocessfail', function (e, data) {
            var index = data.index,
                file = data.files[index];
            // File validation error
            if (data.files[0].error) {
                handleError(data.files[0].error);
            }
        }).on('fileuploadprogress', function (e, data) {
            var progressBar = $('#fileupload').parents('.input-group').find('#progress .progress-bar');
            var progressPercentText = $('#fileupload').parents('.input-group').find('#progressPercentText');
            var progress = parseInt(data.loaded / data.total * 100, 10);
            progressBar.css('width', progress + '%');
            progressPercentText.text(progress + '%');
        }).on('fileuploaddone', function (e, data) {
            var fileIdHiddenField = $('#fileupload').parents('.input-group').find('#fileId');
            if (data.result.isUploaded) {
                fileIdHiddenField.val(data.result.fileId);
            } else {
                if (data.files[0].error) {
                    handleError(data.files[0].error);
                }
            }
        }).on('fileuploadfail', function (e, data) {
            if (data.files[0].error) {
                handleError(data.files[0].error);
            }
        });

        //defaults if file is already uploaded
        //TODO: needs refactor
        var guidEmpty = '00000000-0000-0000-0000-000000000000';
        var fileIdHiddenField = $('#fileupload').parents('.input-group').find('#fileId');
        if (fileIdHiddenField.val() && fileIdHiddenField.val() != guidEmpty) {
            var progressBar = $('#fileupload').parents('.input-group').find('#progress .progress-bar');
            var progressPercentText = $('#fileupload').parents('.input-group').find('#progressPercentText');
            var textfield = $('#fileupload').parents('.input-group').find(':text');
            progressBar.css('width', 100 + '%');
            progressPercentText.text(100 + '%');
            textfield.val('Файл загружен.');
        }
    };

}(GlobalPrint.Shared.FileUpload));