(function () {
    Date.prototype.toDefaultLangString = function () {
        return this.toLocaleString(Kooboo.LanguageManager.getDateLanguage());
    }

    Date.prototype.toDefaultLangString = function () {
        // return this.toLocaleString(Kooboo.LanguageManager.getDateLanguage());
        //'2018-11-23 23:59'
        var year = this.getFullYear(),
            month = (this.getMonth() + 1),
            date = this.getDate();

        var hour = this.getHours(),
            minute = this.getMinutes();

        return [year, getTwoDigit(month), getTwoDigit(date)].join('-') + ' ' + [getTwoDigit(hour), getTwoDigit(minute)].join(':');

        function getTwoDigit(num) {
            var str = num + '';
            return str.length == 2 ? str : ('0' + str)
        }
    }

    Date.prototype.toKBDateString = function () {
        let year = this.getFullYear(),
            month = (this.getMonth() + 1),
            date = this.getDate();

        return [year, month, date].join('-');
    }

    String.prototype.toCamelCase = function () {
        return this[0].toLocaleLowerCase() + this.slice(1);
    }

    $(document).on('mouseover', '[data-toggle="popover"]', function (e) {
        e.preventDefault();
        $(e.target).popover({ html: true }).popover("show");
    });

    $(document).on('click.kooboo.menu', '.block-menu [data-toggle="expand"]', function () {
        $(this).parent().toggleClass('active');
    });

    $.fn.extend({
        readOnly: function (readOnly) {
            var $container = this;
            $container.css('position', 'relative');

            var $overlay = $container.find('>#__readonly_overlay');
            if ($overlay.length === 0) {
                $overlay = $('<div id="__readonly_overlay" style="position:absolute;z-index:100;left:0;top:0;width:100%;height:100%;background:rgba(255,255,255,.46);display:none;"></div>');
                $container.append($overlay);
            }

            $container.find('>#__readonly_overlay')[readOnly ? "show" : "hide"]()
        }
    })
})()

$(function () {
    $('[kb-target]').click(function (e) {
        e.preventDefault();
        var $target = $($(this).attr('kb-target'));
        if ($target.hasClass('in')) {
            $target.animate({
                right: -400
            }, 200, function () {
                $target.removeClass('in')
            })
        } else {
            $target.animate({
                right: 0
            }, 200, function () {
                $target.addClass('in');
            })
        }
    })

    $(window).on('resize', function () {
        var $target = $($('[kb-target]').attr('kb-target'));
        if ($target.length) {
            $($target).removeClass('in').removeAttr('style')
        }
    })

    toastr.options = {
        closeButton: true,
        progressBar: true
    };

    window.info = {
        show: function (message, success) {
            if (success) {
                toastr.success(message);
            } else {
                toastr.error(message);
            }
        },
        done: function (msg) {
            toastr.success(msg);
        },
        fail: function (msg) {
            toastr.error(msg);
        }
    };
})