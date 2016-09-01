$(document).ready(function () {
    $('.delete-printer').click(function (event) {
        var printerId = event.target;

        $.ajax({
            url: '/Printer/Delete',
            type: 'POST',
            data: {
                printerID: printerId
            },
        }).done(function (json) {
            if (!json) {
                console.log('Error: ajax response is empty.');
                return;
            }


        }).fail(function () {
            console.log('Error: ajax call failed.');
        });

    });
});