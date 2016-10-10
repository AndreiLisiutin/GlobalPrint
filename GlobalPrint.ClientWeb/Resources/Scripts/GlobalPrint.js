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
            creditcard: "Укажите корректные данные банковской карты.",
            equalTo: "Указанные значения не совпадают.",
            accept: "Файлы хэтого расширения не поддерживаются.",
            maxlength: jQuery.validator.format("Укажите значение длиной не менее {0} символов."),
            minlength: jQuery.validator.format("Укажите значение длиной не более {0} символов."),
            rangelength: jQuery.validator.format("Укажите значение длиной от {0} до {1} символов."),
            range: jQuery.validator.format("Укажите значение между {0} и {1}."),
            max: jQuery.validator.format("Укажите значение меньшее или равное {0}."),
            min: jQuery.validator.format("Укажите значение большее или равное {0}.")
        });


        $.validator.addMethod('phone', function (value, element) {
            return this.optional(element) || /\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})/.test(value);
        }, "Укажите корректный номер телефона.");
        jQuery.validator.addMethod("decimal", function (value, element) {
            return this.optional(element) || /^(\d+([\.,]\d{1,9})?)$/.test(value);
        }, "Это поле допускает только числовые значения.");
        $.validator.addMethod('custom', function (value, element) {
            return false;
        }, "Значение этого поля некорректно.");
        $.validator.addMethod('time', function (value, element) {
            return this.optional(element) || /\d\d:\d\d/.test(value)
                && parseInt(value.substring(0, 2)) <= 23 && parseInt(value.substring(3, 5)) <= 59;
        }, "Укажите корректное время дня в формате ЧЧ:ММ.");


        $.validator.setDefaults({
            errorElement: "em",
            errorClass: "has-error",
            validClass: "has-success",
            errorPlacement: function (error, element) {
                error.addClass("help-block");
                $(element).closest('.input-wrapper').append(error);
            },
            highlight: function (element, errorClass, validClass) {
                $(element).closest(".input-wrapper").addClass(errorClass).removeClass(validClass);
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).closest(".input-wrapper").addClass(validClass).removeClass(errorClass);
            }
        });
    };

    GlobalPrint.initializeValidation();

})(window.GlobalPrint);