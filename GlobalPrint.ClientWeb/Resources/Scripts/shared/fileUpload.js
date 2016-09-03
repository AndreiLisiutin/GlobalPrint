$(document).ready(function () {

    //Initialization of fileupload
    initSimpleFileUpload();
});

function initSimpleFileUpload() {
    'use strict';

    $('#fileupload').fileupload({
        url: '/File/UploadFile',
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        paramName: 'gpUserFile',
        maxFileSize: 1000000, // bytes
        autoUpload: true,
        dataType: 'json'
    }).on('fileuploadadd', function (e, data) {
        if (data.autoUpload || (data.autoUpload !== false &&
             $(this).fileupload('option', 'autoUpload'))) {
            data.process().done(function () {
                data.submit();
            });
        }
    }).on('fileuploadprogress', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('#progress .progress-bar').css(
            'width',
            progress + '%'
        );
    }).on('fileuploaddone', function (e, data) {
        if (data.result.isUploaded) {

        }
        else {

        }
        alert(data.result.message);
    }).on('fileuploadfail', function (e, data) {
        if (data.files[0].error) {
            alert(data.files[0].error);
        }
    });
}