(function() {
    var dialogHtml = Kooboo.getTemplate("/_Admin/Scripts/viewEditor/components/modal.html");

    var Modal = function() {
        var self = this;
        var buttons = [];
        var id = '__modal_' + new Date().getTime();
        var $modal = $('#' + id);
        if ($modal.length === 0) {
            $modal = $(dialogHtml);
            $(document.body).append($modal);
            $modal.attr('id', id);
        }

        $modal.data('modal', this);
        $modal.on("click", function(e) {
            e.preventDefault();
            if ((e.target.tagName == "I" && $(e.target).hasClass("fa-close")) ||
                (e.target.tagName == "BUTTON" && $(e.target).hasClass("close"))) {
                $modal.modal('hide');
            }
        })

        $modal.on("hidden.bs.modal", function() {
            if ($(".modal.in", document.body).length && !$("body").hasClass("modal-open")) {
                $("body").addClass("modal-open");
            }
        })

        self.$modal = function() {
            return $modal;
        };

        self.find = function(selector) {
            return self.$modal().find(selector);
        };

        self.title = function(value) {
            $modal.find('.modal-title').html(value);
        };

        self.resize = function(options) {
            if (options.width) {
                $modal.find('.modal-dialog').width(options.width);
            }
            if (options.height) {
                $modal.find('.modal-dialog').height(options.height);
            }
            if (options.zIndex) {
                setTimeout(function() {
                    $modal.css("z-index", options.zIndex);
                }, 80);
            }
        };

        self.open = function(options) {
            self.title(options.title);

            if (options.url) {
                $modal.find('.modal-body').html('<iframe style="border:0;width:100%;"></iframe>');
                var $iframe = $modal.find('iframe');
                $iframe.attr('src', options.url);
            } else if (options.html) {
                $modal.find('.modal-body').html(options.html);
            }

            self.resize(options);
            $modal.modal('show');

            if (options.buttons) {
                for (var i = 0, button; button = options.buttons[i]; i++) {
                    self.addButton(button);
                }
            }
        };

        self.close = function() {
            $modal.modal('hide');
        };

        self.destroy = function() {
            $modal.modal("hide");
        }

        self.setFrameHeight = function(height) {
            var $iframe = $modal.find("iframe");
            $iframe.css('min-height', height);
        }

        self.addButton = function(button) {
            var $button = $('<button class="btn"></button>');
            $button.html(button.text);
            $button.addClass(button.cssClass);
            $modal.find('.modal-footer').append($button);

            if (button.click) {
                $button.on('click', function() {
                    button.click({
                        id: id,
                        modal: self
                    });
                });
            }

            var clone = $.extend(true, {}, button);
            clone.$element = $button;
            buttons.push(clone);

            if (button.visible === undefined) {
                button.visible = true;
            }

            if (!button.visible) {
                self.hideButton(button.id);
            }
        };

        self.removeButton = function(id) {
            for (var i = 0, button; button = buttons[i]; i++) {
                if (buttons[i].id === id) {
                    button.$element.remove();
                    buttons.splice(i, 1);
                    break;
                }
            }
        };

        self.clearButtons = function() {
            while (buttons.length > 0) {
                self.removeButton(buttons[buttons.length - 1].id);
            }
        };

        self.showButton = function(id) {
            for (var i = 0, button; button = buttons[i]; i++) {
                if (buttons[i].id === id) {
                    button.$element.show();
                    break;
                }
            }
        };

        self.hideButton = function(id) {
            for (var i = 0, button; button = buttons[i]; i++) {
                if (buttons[i].id === id) {
                    button.$element.hide();
                    break;
                }
            }
        };

        $modal.on("hidden.bs.modal", function(e) {
            $modal.remove();
        })

        Kooboo.EventBus.subscribe("kb/component/modal/set/height", function(data) {
            var height = data.height;
            self.setFrameHeight(height);

            if (this.parent !== this) {
                window.parent.Kooboo.EventBus.publish("kb/component/modal/set/height", {
                    height: Math.max(document.body.scrollHeight, height + 100)
                })
            }
        })
    };

    if (Kooboo.viewEditor &&
        Kooboo.viewEditor.component) {
        Kooboo.viewEditor.component.modal = {
            open: function(options) {
                var modal = new Modal();
                modal.open(options);
                return modal;
            }
        };
    }
})();