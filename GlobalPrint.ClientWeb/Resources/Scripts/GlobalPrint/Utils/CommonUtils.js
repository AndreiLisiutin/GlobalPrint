GlobalPrint.namespace('GlobalPrint.Utils.CommonUtils');
(function (CommonUtils) {

    CommonUtils.isUserAuthenticated = function () {
        var isAuthenticated = $("body").data('isAuthenticated');
        return isAuthenticated && isAuthenticated.toLowerCase() == 'true';
    };

    CommonUtils.getUserID = function () {
        var userId = $("body").data('userId');
        return userId && parseInt(userId);
    };

    CommonUtils.geolocate = function (successCallback, failureCallback) {
        /// <summary>
        /// Geolocate user's position and perform callback with it.
        /// </summary>
        /// <param name="successCallback" type="type">Callback on successful geolocaion. 
        /// Parameter is an object of type { coords { latitude, longitude, accuracy } }.</param>
        /// <param name="failureCallback" type="type">Callback on unsuccessful geolocaion. Parameter is an object of type Error.</param>
        failureCallback = failureCallback || function (error) {
            console.log("Ошибка определения текущего положения." + error.message);
        };
        successCallback = successCallback || function () { };


        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(successCallback, failureCallback, {
                enableHighAccuracy: true,
                timeout: 5000,
                maximumAge: 0
            });
        } else {
            failureCallback({ message: "Отключено определение текущего положения." });
        }
    };

    CommonUtils.makeProgressBar = function () {
        /// <summary>
        /// Configuration of NProgress progress bar on AJAX and so on.
        /// </summary>
        //NProgress.configure({ showSpinner: false });
        $(document).ready(function () {
            GlobalPrint.Utils.CommonUtils.startProgressBar();
        });
        $(window).load(function () {
            GlobalPrint.Utils.CommonUtils.finishProgressBar();
        });
        $.ajaxSetup({
            beforeSend: function () {
                GlobalPrint.Utils.CommonUtils.startProgressBar();
            },
            complete: function () {
                GlobalPrint.Utils.CommonUtils.finishProgressBar();
            },
            success: function () {
                GlobalPrint.Utils.CommonUtils.finishProgressBar();
            }
        });
    };

    CommonUtils.startProgressBar = function () {
        NProgress.start();
    };

    CommonUtils.finishProgressBar = function () {
        NProgress.done();
    };

    CommonUtils.makeValidation = function () {
        /// <summary>
        /// Configuration of client jQuery validation.
        /// </summary>
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
        $.validator.addMethod("decimal", function (value, element) {
            return this.optional(element) || /^(-?\d+([\.,]\d{1,7})?)$/.test(value) && !isNaN(parseFloat(value.replace(',', '.')));
        }, "Это поле допускает только числовые значения с плавающей точкой.");
        $.validator.addMethod("integer", function (value, element) {
            return this.optional(element) || /^(-?\d+)$/.test(value) && !isNaN(parseInt(value));
        }, "Это поле допускает только целочисленные значения.");
        $.validator.addMethod("positive", function (value, element) {
            return this.optional(element) || parseFloat(value.replace(',', '.')) > 0;
        }, "Это поле допускает только положительные числовые значения.");
        $.validator.addMethod("money", function (value, element) {
            return this.optional(element) || /^(\d+([\.,]\d{2})?)$/.test(value) && parseFloat(value.replace(',', '.')) > 0;
        }, "Укажите корректную денежную сумму.");
        $.validator.addMethod('custom', function (value, element) {
            return false;
        }, "Значение этого поля некорректно.");
        $.validator.addMethod("notEqual", function (value, element, param) {
            return this.optional(element) || value != $(param).val();
        }, "Значение этого поля не может быть таким же. Задайте другое значение.");
        $.validator.addMethod('time', function (value, element) {
            return this.optional(element) || /^(\d{2}:\d{2}(:\d{2})?)$/.test(value)
                && parseInt(value.substring(0, 2)) <= 23 && parseInt(value.substring(3, 5)) <= 59;
        }, "Укажите корректное время дня в формате ЧЧ:ММ или ЧЧ:ММ:СС.");


        $.validator.setDefaults({
            ignore: [],
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

    CommonUtils.makeLookups = function () {
        $('.lookup-input-group').each(function (index, item) {
            var lookupType = $(item).attr('data-lookup-type');
            var id = GlobalPrint.Utils.CommonUtils.markupModalLookup(lookupType, function (event, data) {
                $(item).find('.lookup-value-id').val(data.id);
                $(item).find('.lookup-value-name').val(data.name);
            });
            $(item).find('.lookup-show-button').click(function () {
                GlobalPrint.Utils.CommonUtils.showLookup(id);
            });

            GlobalPrint.Utils.CommonUtils.initializeLookupValue(item);
        });
    };

    CommonUtils.makeClockPickers = function () {
        $('.clockpicker').clockpicker();
    };

    CommonUtils.markupModalLookup = function (lookupTypeID, setValueCallback) {
        /// <summary>
        /// Create necessary markup for lookup implementation.
        /// </summary>
        /// <param name="lookupTypeID" type="type">Lookup type enumeration member (GlobalPrint.Utils.CommonUtils.LookupType).</param>
        /// <param name="setValueCallback" type="type">Callback for set value event. Parameter is object of type { id, name }.</param>
        /// <returns type="">id attribute of the created markup.</returns>
        var url = "/Lookup/Lookup?lookupType=" + lookupTypeID;
        var modalWindowId = "modal_" + Math.floor(Math.random() * 100000000000);
        var modalHtml =
            '<div class="modal modal-fullscreen fade lookup-wrapper" id="' + modalWindowId + '" tabindex="-1" role="dialog" aria-hidden="true">' +
            '    <div class="modal-dialog">                                                                                                    ' +
            '        <div class="modal-content">                                                                                               ' +
            '            <div class="modal-body">                                                                                              ' +
            '                <div class="modal-body-content">                                                                                  ' +
            '                </div>                                                                                                            ' +
            '            </div>                                                                                                                ' +
            '        </div>                                                                                                                    ' +
            '    </div>                                                                                                                        ';
        $('body').append(modalHtml);
        $('#' + modalWindowId).on('show.bs.modal', function () {
            setTimeout(function () {
                $('.modal-backdrop').addClass("modal-backdrop-fullscreen");
                GlobalPrint.Utils.CommonUtils.startProgressBar();
                GlobalPrint.Utils.CommonUtils.setLoading('#' + modalWindowId + ' .modal-body-content')
                $('#' + modalWindowId + ' .modal-body-content').load(url, function () {
                    GlobalPrint.Utils.CommonUtils.finishProgressBar();
                });
            }, 0);
        });
        $('#' + modalWindowId).on('hidden.bs.modal', function () {
            $('#' + modalWindowId + ' .modal-backdrop').addClass("modal-backdrop-fullscreen");
        });
        $('#' + modalWindowId).on('setValue', function (event, data) {
            if (setValueCallback) {
                setValueCallback(event, data);
            }
        });
        return modalWindowId;
    };

    CommonUtils.showLookup = function (lookupID) {
        /// <summary>
        /// Show lookup modal window.
        /// </summary>
        /// <param name="lookupID" type="type">Lookup modal window markup id attribute. 
        /// It is an output of GlobalPrint.Utils.CommonUtils.markupModalLookup.</param>
        $('#' + lookupID).modal('toggle');
    };

    CommonUtils.initializeLookupValue = function (lookup) {
        /// <summary>
        /// Set lookup text value according to its selected identifier value.
        /// </summary>
        /// <param name="lookup" type="type">Lookup element.</param>
        var idValue = parseInt($(lookup).find('.lookup-value-id').val());
        var lookupType = $(lookup).attr('data-lookup-type');
        if (idValue) {
            $.ajax({
                url: '/Lookup/GetValue',
                type: "GET",
                data: {
                    lookupType: lookupType,
                    id: parseInt(idValue)
                }
            })
            .done(function (response) {
                $(lookup).find('.lookup-value-name').val(response);
            });
        }
    };

    CommonUtils.LookupType = {
        User: 1
    };

    /// <summary>
    /// Initialize swithers like in iOS.
    /// </summary>
    CommonUtils.initializeSwitchers = function () {
        var smallSwithcers = Array.prototype.slice.call(document.querySelectorAll('.js-switch-small'));
        var mediumSwithcers = Array.prototype.slice.call(document.querySelectorAll('.js-switch-medium'));
        var largeSwithcers = Array.prototype.slice.call(document.querySelectorAll('.js-switch-large'));

        var processSwitchers = function (array, size) {
            if (array && array.length > 0) {
                array.forEach(function (html) {
                    var switchery = new Switchery(html, { size: size || 'default' });

                    var observer = new MutationObserver(function (mutations) {
                        for (var i = 0, mutation; mutation = mutations[i]; i++) {
                            if (mutation.attributeName == 'disabled') {
                                if (mutation.target.disabled) {
                                    switchery.disable();
                                } else {
                                    switchery.enable();
                                }
                            }
                        };
                    });

                    // Observe attributes change
                    observer.observe(html, { attributes: true });

                    html.onchange = function () {
                        switchery.setPosition();
                    };
                });
            }
        }
        processSwitchers(smallSwithcers, 'small');
        processSwitchers(mediumSwithcers);
        processSwitchers(largeSwithcers, 'large');
    };

    CommonUtils.setLoading = function (elementSelector) {
        /// <summary>
        /// Create loading mask on element by CSS selector of elementSelector.
        /// </summary>
        /// <param name="elementSelector" type="type">Selector for loading mask base element.</param>
        var loadingHtml =
           '<div class="loading">                                                          ' +
           '   <img class="img-responsive img-centred" src="/Resources/Images/loading_pony.gif" />' +
           '   <h2 class="text-center">Идет загрузка<span class="one">.</span><span class="two">.</span><span class="three">.</span> </h2>                                     ' +
           '</div>                                                                                ';


        $(elementSelector).html(loadingHtml);
    };

    CommonUtils.setCheckboxValueById = function (id, checked) {
        var elm = document.getElementById(id.replace('#', ''));
        CommonUtils.setCheckboxValue(elm, checked);
    };
    CommonUtils.setCheckboxValue = function (elm, checked) {
        if (elm && checked != elm.checked) {
            elm.click();
        }
    };

})(GlobalPrint.Utils.CommonUtils);