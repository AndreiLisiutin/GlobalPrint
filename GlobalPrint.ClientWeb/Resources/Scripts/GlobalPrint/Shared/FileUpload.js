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

    FileUpload.initFileUpload = function () {
        $('#fileupload').fileupload({
            url: '/Order/UploadFile',
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png|pdf|doc|docx)$/i,
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

            $('#progressbarError').hide();
            $('#fileName').text(file.name + ' (' + formatBytes(file.size) + ')');
            
            // File validation
            if (data.autoUpload || (data.autoUpload !== false &&
                    $(this).fileupload('option', 'autoUpload'))) {
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
                $('#progressbarError').show();
                $('#progressbarError').text(data.files[0].error);
            }
        }).on('fileuploadprogress', function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width', progress + '%'
            );
            $('#percent').text(progress + '%');
        }).on('fileuploaddone', function (e, data) {
            if (data.result.isUploaded) {
                $('#fileId').val(data.result.fileId);
            } else {
                if (data.files[0].error) {
                    $('#progress .progress-bar').css(
                        'width', 0 + '%'
                    );
                    $('#progressbarError').show();
                    $('#progressbarError').text(data.files[0].error);
                }
            }
        }).on('fileuploadfail', function (e, data) {
            if (data.files[0].error) {
                $('#progress .progress-bar').css(
                    'width', 0 + '%'
                );
                $('#progressbarError').show();
                $('#progressbarError').text(data.files[0].error);
            }
        });
    };

}(GlobalPrint.Shared.FileUpload));