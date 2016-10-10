window.GlobalPrint = window.GlobalPrint || {};
(function(GlobalPrint) {
    //creates manespace by its string representation
    GlobalPrint.namespace = function (namespace) {
        var nsparts = namespace.split(".");
        var parent = GlobalPrint;

        // we want to be able to include or exclude the root namespace so we strip
        // it if it's in the namespace
        if (nsparts[0] === "GlobalPrint") {
            nsparts = nsparts.slice(1);
        }

        // loop through the parts and create a nested namespace if necessary
        for (var i = 0; i < nsparts.length; i++) {
            var partname = nsparts[i];
            // check if the current parent already has the namespace declared
            // if it isn't, then create it
            if (typeof parent[partname] === "undefined") {
                parent[partname] = {};
            }
            // get a reference to the deepest element in the hierarchy so far
            parent = parent[partname];
        }
        // the parent is now constructed with empty namespaces and can be used.
        // we return the outermost namespace
        return parent;
    };

    GlobalPrint.initializeValidation = function () {
        jQuery.extend(jQuery.validator.messages, {
            required: "Это поле обязательное.",
            remote: "Значение этого поля некорректно.",
            email: "Укажите корректный адрес электронной почты.",
            url: "Укажите корректный URL.",
            date: "Укажите корректную дату.",
            dateISO: "Укажите корректную дату в формате ISO.",
            number: "Это поле допускает только числовые значения.",
            digits: "Это поле допускает только целочисленные значения.",
            creditcard: "Please enter a valid credit card number.",
            equalTo: "Please enter the same value again.",
            accept: "Please enter a value with a valid extension.",
            maxlength: jQuery.validator.format("Please enter no more than {0} characters."),
            minlength: jQuery.validator.format("Please enter at least {0} characters."),
            rangelength: jQuery.validator.format("Укажите значение длиной от {0} до {1} символов."),
            range: jQuery.validator.format("Укажите значение между {0} и {1}."),
            max: jQuery.validator.format("Укажите значение меньшее или равное {0}."),
            min: jQuery.validator.format("Укажите значение большее или равное {0}.")
        });
    };


    GlobalPrint.initializeValidation();

})(window.GlobalPrint);