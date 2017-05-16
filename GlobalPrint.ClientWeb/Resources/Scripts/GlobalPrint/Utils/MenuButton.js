GlobalPrint.namespace('GlobalPrint.Utils.MenuButton');
(function (MenuButton) {

    MenuButton.Initialize = function (element) {
        // based on Todd Motto functions
        // http://toddmotto.com/labs/reusable-js/

        // hasClass
        function hasClass(elem, className) {
            return new RegExp(' ' + className + ' ').test(' ' + elem.className + ' ');
        }
        // addClass
        function addClass(elem, className) {
            if (!hasClass(elem, className)) {
                elem.className += ' ' + className;
            }
        }
        // removeClass
        function removeClass(elem, className) {
            var newClass = ' ' + elem.className.replace(/[\t\r\n]/g, ' ') + ' ';
            if (hasClass(elem, className)) {
                while (newClass.indexOf(' ' + className + ' ') >= 0) {
                    newClass = newClass.replace(' ' + className + ' ', ' ');
                }
                elem.className = newClass.replace(/^\s+|\s+$/g, '');
            }
        }
        // toggleClass
        function toggleClass(elem, className) {
            var newClass = ' ' + elem.className.replace(/[\t\r\n]/g, " ") + ' ';
            if (hasClass(elem, className)) {
                while (newClass.indexOf(" " + className + " ") >= 0) {
                    newClass = newClass.replace(" " + className + " ", " ");
                }
                elem.className = newClass.replace(/^\s+|\s+$/g, '');
            } else {
                elem.className += ' ' + className;
            }
        }

        element.onclick = function () {
            toggleClass(this, 'on');
            return false;
        }
    };

    MenuButton.InitializeAllMenus = function () {
        var menus = Array.prototype.slice.call(document.querySelectorAll('.toggle-menu-button'));
        menus.forEach(function (item) {
            MenuButton.Initialize(item);
        });

        /**
         * Закрыть меню при клике на другую область окна.
         * @param {type} event
         */
        window.onclick = function (event) {
            if (!event.target.matches(".toggle-menu-button") && !event.target.matches(".toggle-menu-button-span")) {
                var dropdowns = Array.prototype.slice.call(document.querySelectorAll('.toggle-menu-button'));
                for (var i = 0; i < dropdowns.length; i++) {
                    var openDropdown = dropdowns[i];
                    if (openDropdown.classList.contains('on')) {
                        openDropdown.click();
                    }
                }
            }
        }
    };

})(GlobalPrint.Utils.MenuButton);
