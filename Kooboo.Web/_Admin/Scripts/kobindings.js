(function() {
    ko.bindingHandlers.upload = {
        init: function(element, valueAccessor, allBindings, bindingContext) {
            /*
             *config = {
             *  allowMultiple: true || false,
             *  acceptTypes: [Array],
             *  acceptSuffix: [Array],
             *  callback: function
             *}
             * 
             */
            var config = valueAccessor();
            config.allowMultiple ? $(element).attr("multiple", true) : $(element).removeAttr("multiple");

            if (config.acceptTypes && config.acceptTypes.length) {
                $(element).attr("accept", config.acceptTypes.join(","));
            }

            $(element).change(function() {
                var files = this.files,
                    len = files.length,
                    acceptableFilesLength = 0;

                var availableFiles = [];

                if (len) {
                    var data = new FormData();

                    var errors = {
                        size: [],
                        type: [],
                        suffix: []
                    };

                    _.forEach(files, function(file, idx) {
                        var fileName = file.name;

                        if (!config.acceptSuffix || !config.acceptSuffix.length) {
                            if (!config.acceptTypes || !config.acceptTypes.length) {
                                alert("Upload failed: please init the acceptType first.");
                            } else {
                                if (config.acceptTypes.indexOf(file.type) > -1 ||
                                    config.acceptTypes.indexOf("*/*") > -1) {
                                    if (file.size) {
                                        data.append("file_" + idx, file);
                                        availableFiles.push(file);
                                        acceptableFilesLength++;
                                    } else {
                                        errors.size.push(file.name);
                                    }
                                } else {
                                    errors.type.push(file.name);
                                }
                            }
                        } else {
                            if (fileName.indexOf(".") > -1) {
                                var suffix = fileName.split(".").reverse()[0].toLowerCase();

                                if (config.acceptSuffix.indexOf(suffix) > -1) {
                                    if (file.size) {
                                        data.append("file_" + idx, file);
                                        availableFiles.push(file);
                                        acceptableFilesLength++;
                                    } else {
                                        errors.size.push(file.name);
                                    }
                                } else {
                                    errors.suffix.push(file.name);
                                }
                            }
                        }
                    });

                    config.callback(data, availableFiles);
                    resetValue(element);

                    var errorString = getErrorString();
                    errorString && alert(errorString);

                    function getErrorString() {
                        var string = "";
                        if (errors.size.length) {
                            string += Kooboo.text.common.File + ' ' + errors.size.join(', ') + ' ' + Kooboo.text.alert.fileUpload.emptyFile + '\n'
                        }
                        if (errors.type.length) {
                            string += Kooboo.text.common.File + ' ' + errors.type.join(', ') + ' ' + Kooboo.text.alert.fileUpload.invalidSuffix + '\n'
                        }
                        if (errors.suffix.length) {
                            string += Kooboo.text.common.File + ' ' + errors.suffix.join(', ') + ' ' + Kooboo.text.alert.fileUpload.invalidType + '\n'
                        }
                        return string;
                    }
                }
            })

            function resetValue(el) {
                $(el).wrap("<form>").parent("form").trigger("reset");
                $(el).unwrap();
            }
        }
    };
    ko.bindingHandlers.autoFocus = {
        init: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            ko.bindingHandlers.autoFocus.update(element, valueAccessor, allBindingsAccessor);
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                setTimeout(function() {
                    element.focus();
                }, 0);
            }
        }
    };
    ko.bindingHandlers.collapsein = {
        init: function(element, valueAccessor) {
            $(element).addClass('collapse');
        },
        update: function(element, valueAccessor) {
            $(element).collapse(ko.utils.unwrapObservable(valueAccessor()) ? 'show' : 'hide');
        }
    };
    ko.validation = {
        rules: {
            required: function(value, params) {
                if (typeof value !== 'number' && !_.trim(value || '')) {
                    var msg = null;
                    if (typeof(params) === 'string') {
                        msg = params;
                    } else {
                        msg = params.message;
                    }
                    return {
                        valid: false,
                        message: msg || Kooboo.text.validation.required
                    };
                }
                return { valid: true };
            },
            regex: function(value, params) {
                if (!params.pattern.test(value)) {
                    return {
                        valid: false,
                        message: params.message
                    };
                }
                return { valid: true };
            },
            range: function(value, params) {
                if (!(parseFloat(value) == value && value >= params.from && value <= params.to)) {
                    return {
                        valid: false,
                        message: params.message
                    };
                }
                return { valid: true };
            },
            stringlength: function(value, params) {
                if (!(value.length >= params.min && value.length <= params.max)) {
                    return {
                        valid: false,
                        message: params.message
                    };
                }
                return { valid: true };
            },
            minLength: function(value, params) {
                if (value.length >= params.value) {
                    return { valid: true }
                } else {
                    return { valid: false, message: params.message }
                }
            },
            maxLength: function(value, params) {
                if (value.length <= params.value) {
                    return { valid: true }
                } else {
                    return { valid: false, message: params.message }
                }
            },
            minChecked: function(value, params) {
                if (value.length >= params.value) {
                    return { valid: true }
                } else {
                    return { valid: false, message: params.message }
                }
            },
            maxChecked: function(value, params) {
                if (value.length <= params.value) {
                    return { valid: true }
                } else {
                    return { valid: false, message: params.message }
                }
            },
            remote: function(value, params) {
                params = params || {};
                var data = {},
                    response = { valid: false, message: params.message || "invalid" };
                if (params.value !== undefined) {
                    var oldValue = params.value;
                    if (typeof oldValue === "function") {
                        oldValue = oldValue();
                    }
                    if (value === oldValue) {
                        return { valid: true };
                    }
                }

                if (params.data) {
                    for (var i in params.data) {
                        var p = params.data[i];
                        if (typeof p === "function") {
                            data[i] = p();
                        } else {
                            data[i] = p;
                        }
                    }
                }
                $.ajax(params.url, {
                    type: params.type || "get",
                    data: data,
                    async: false
                }).then(function(res) {
                    if (typeof res === "object") {
                        if (res.success === "true" || res.success === true) {
                            response.valid = true;
                        } else {
                            response["message"] = response["message"];
                        }
                    } else {
                        if (res === "true" || res === true) {
                            response.valid = true;
                        } else {
                            response.message = res.message || response.message;
                        }
                    }
                    return response;
                });
                return response;
            },
            name: function(target, params) {
                var msg = null;
                if (params) {
                    if (typeof(params) === 'string') {
                        msg = params;
                    } else if (typeof(params) === 'function') {
                        msg = params();
                    } else {
                        msg = params.message;
                    }
                }
                return ko.validation.rules.regex(target, {
                    pattern: /^[a-zA-Z]\w*$/,
                    message: msg || Kooboo.text.validation.nameInvalid
                });
            },
            equals: function(value, params) {
                if (params.value() !== value) {
                    return {
                        valid: false,
                        message: params.message
                    }
                } else {
                    return { valid: true };
                }
            },
            exclude: function(value, params) {
                if (params.target().indexOf(value) > -1) {
                    return {
                        valid: false,
                        message: params.message
                    }
                } else {
                    return {
                        valid: true
                    }
                }
            },
            dataType: function(value, params) {
                var value = value.toString(),
                    type = params.type,
                    message = params.message || (Kooboo.text.validation.dataTypeInvalid + Kooboo.text.validation.dataType[type]);
                var valid = false
                switch (type) {
                    case "Integer":
                        if (!!value.match(/^\d+$/)) {
                            valid = true;
                        }
                        break;
                    case "DateTime":
                        //"2008-07-22"
                        if (!!value.match(/^(\d{4})-(\d{1,2})-(\d{1,2})$/)) {
                            valid = true;
                        }
                        break;
                    case "Decimal":
                        if (parseFloat(value) == value || !!value.match(/^[\.\d]+$/)) {
                            valid = true;
                        }
                        break;
                    case "Guid":
                        if (!!value.match(/^\d+$/)) {
                            valid = true;
                        }
                        break;
                    case "String":
                        if (typeof value === "string") {
                            valid = true;
                        }
                        break;
                    case "Bool":
                        if (value == true || value == false || value == "true" || value == "false") {
                            valid = true;
                        }
                        break;
                    case "Number":
                        var val = Number(value);
                        valid = !isNaN(Number(value));
                        break;
                }
                if (!valid) {
                    return {
                        valid: false,
                        message: message
                    }
                } else {
                    return {
                        valid: true
                    }
                }
            },
            localUnique: function(value, params) {
                var find = _.filter(_.compact(params.compare()), function(c) {
                    return c.toLowerCase() == value.toLowerCase();
                })
                if (find.length > 1) {
                    return {
                        valid: false,
                        message: params.message || Kooboo.text.validation.taken
                    }
                } else {
                    return { valid: true }
                }
            },
            /** 
             * if the value compared is a observable value,
             * use lessThan/greaterThan
             * if the value is a static value,
             * use min/max
             */
            min: function(value, params) {
                var val = Number(value);
                if (!isNaN(val)) {
                    if (params.value > val) {
                        return {
                            valid: false,
                            message: params.message || Kooboo.text.validation.greaterThan + params.value
                        }
                    } else {
                        return { valid: true };
                    }
                } else {
                    return {
                        valid: false,
                        message: Kooboo.text.error.dataType + ", " + Kooboo.text.correct.dataType.number
                    }
                }
            },
            max: function(value, params) {
                var val = Number(value);
                if (!isNaN(val)) {
                    if (params.value < val) {
                        return {
                            valid: false,
                            message: params.message || Kooboo.text.validation.lessThan + params.value
                        }
                    } else {
                        return { valid: true };
                    }
                } else {
                    return {
                        valid: false,
                        message: Kooboo.text.error.dataType + ", " + Kooboo.text.correct.dataType.number
                    }
                }
            },
            lessThan: function(value, params) {
                var max = Number(params.value() || value),
                    min = Number(value);

                if (!isNaN(max) && !isNaN(min)) {
                    if (min <= max) {
                        return {
                            valid: true
                        }
                    } else {
                        return {
                            valid: false,
                            message: params.message()
                        }
                    }
                } else {
                    return {
                        valid: false,
                        message: Kooboo.text.error.dataType + ", " + Kooboo.text.correct.dataType.number
                    }
                }
            },
            greaterThan: function(value, params) {
                var min = Number(params.value() || value),
                    max = Number(value);

                if (!isNaN(min) && !isNaN(max)) {
                    if (max >= min) {
                        return {
                            valid: true
                        }
                    } else {
                        return {
                            valid: false,
                            message: params.message()
                        }
                    }
                } else {
                    return {
                        valid: false,
                        message: Kooboo.text.error.dataType + ", " + Kooboo.text.correct.dataType.number
                    }
                }
            },
            kbDomain: function(value, params) {
                if (value) {
                    return ko.validation.rules.regex(value, {
                        pattern: /^[^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\>|\/|\?|\.|\-|\_][^\s|\~|\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\||\\|\[|\]|\{|\}|\;|\:|\"|\'|\,|\<|\>|\/|\?]*\w$/,
                        message: params || (Kooboo.text.validation.startWithNonPunctuation + ", " + Kooboo.text.validation.onlyFollowingPunctuationAllowed + ". - _")
                    })
                } else {
                    return { valid: true }
                }
            },
            domain: function(value, params) {
                if (value.indexOf(".") > -1) {
                    var temp = value.split(".");
                    // TODO: verify every part of domain.
                    // each part's length must less than 63 char
                    // and obey the regex rule
                } else {
                    return ko.validation.rules.stringlength(value, {
                        min: 2,
                        max: 63,
                        message: Kooboo.text.validation.minLength + 2 + ", " + Kooboo.text.validation.maxLength + 63
                    })
                }
            }
        }
    };
    ko.extenders.validate = function(target, options) {
        target.isValid = ko.observable(false);
        target.isShow = ko.observable(false);
        target.error = ko.observable();
        target.valid = function() {
            validate(target());
            return target.isValid();
        };

        target.valid();

        function validate(newValue) {
            var allValid = true;
            var msg = null;
            if (typeof(options) === 'function') {
                var result = options(newValue);
                if (result === undefined) {
                    allValid = true;
                } else if (typeof(result) === 'string') {
                    if (result) {
                        allValid = false;
                        msg = result;
                    }
                } else {
                    allValid = result.valid;
                    msg = result.message;
                }
            } else {
                for (var name in options) {
                    var rule = ko.validation.rules[name];
                    var params = options[name];
                    if (rule) {
                        var result = rule(newValue, params);
                        if (!result.valid) {
                            allValid = false;
                            msg = result.message;
                            break;
                        }
                    }
                }
            }
            if (allValid) {
                target.isValid(true);
                target.isShow(false);
                target.error('');
            } else {
                target.isValid(false);
                target.isShow(true);
                target.error(msg || Kooboo.text.error.validationError);
            }
        }
        //validate(target());
        target.subscribe(validate);
        return target;
    };
    ko.extenders.stringlength = function(target, params) {
        return ko.extenders.validate(target, {
            stringlength: params
        });
    };

    ko.extenders.range = function(target, params) {
        return ko.extenders.validate(target, {
            range: params
        });
    };

    ko.extenders.required = function(target, params) {
        return ko.extenders.validate(target, {
            required: params
        });
    };

    ko.extenders.regex = function(target, params) {
        return ko.extenders.validate(target, {
            regex: params
        });
    }

    ko.extenders.dataType = function(target, params) {
        return ko.extenders.validate(target, {
            dataType: params
        });
    }

    ko.bindingHandlers.tooltip = {
        init: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            ko.bindingHandlers.tooltip.update(element, valueAccessor, allBindingsAccessor);
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                // This will be called when the element is removed by Knockout or
                // if some other part of your code calls ko.removeNode(element)
                $(element).tooltip("destroy");
            });
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                $el = $(element);
            $el.tooltip('destroy');
            if (value) {

                var allBinding = allBindingsAccessor();
                setTimeout(function() {
                    $el.tooltip({
                        title: value,
                        container: "body",
                        placement: allBinding.tooltipPlacement || 'top'
                    });
                }, 50);
            }
        }
    };
    ko.bindingHandlers.error = {
        init: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            ko.bindingHandlers.error.update(element, valueAccessor, allBindingsAccessor, bindingContext);
            // ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
            //     // This will be called when the element is removed by Knockout or
            //     // if some other part of your code calls ko.removeNode(element)
            //     $(element).tooltip("destroy");
            // });
        },
        update: function(element, valueAccessor, allBindingsAccessor, bindingContext) {

            function isCoverByHeader(el) {
                var headerRect = $("#header")[0].getBoundingClientRect(),
                    elemRect = el.getBoundingClientRect();

                return ((elemRect.top + elemRect.height) < (headerRect.top + headerRect.height));
            }

            function getProperScrollingElement($el) {
                if ($el.parents(".modal").length) {
                    return $el.parents(".modal")[0];
                } else {
                    if (document.body.scrollTop) {
                        return document.body;
                    } else if (document.documentElement.scrollTop) {
                        return document.documentElement;
                    } else {
                        if (document.body.scrollTop + 1 == 0) {
                            return document.documentElement;
                        } else {
                            return document.body;
                        }
                    }
                }
            }

            var isShow = bindingContext.showError(),
                value = valueAccessor(),
                error = value.error(),
                $el = $(element),
                placement = allBindingsAccessor().errorPlacement,
                delay = allBindingsAccessor().errorDelay;

            var errorContainer = allBindingsAccessor().errorContainer || element.getAttribute("kb-error-container");

            if ($el.closest(".kb-error-holder").length > 0) {
                $el.closest(".kb-error-holder").removeClass('has-error');
            } else if ($el.closest(".input-group").length > 0) {
                $el.closest(".input-group").removeClass('has-error');
            } else {
                $el.closest('.form-group').removeClass('has-error');
            }
            if (error && isShow && $el.is(':visible')) {

                if (!$el.data().hasOwnProperty('bs.tooltip')) {

                    if ($el[0].getBoundingClientRect().top < 0) {
                        var scrollElement = getProperScrollingElement($el);
                        scrollElement.scrollTop = isCoverByHeader($el[0]) ? 68 : $el[0].getBoundingClientRect().top + 20;
                    } else if (($el[0].getBoundingClientRect().top + $el[0].getBoundingClientRect().height) > (window.innerHeight - 53)) {
                        $el[0].scrollIntoView();
                    }

                    $el.tooltip({
                        container: errorContainer || 'body',
                        placement: 'auto ' + (placement || 'right'),
                        trigger: 'manual',
                        template: '<div class="tooltip error" role="tooltip" style="' + (errorContainer ? 'position: absolute' : 'z-index:199999') + '"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
                    });
                }
                if ($el.closest(".kb-error-holder").length > 0) {
                    $el.closest(".kb-error-holder").addClass('has-error');
                } else if ($el.closest(".input-group").length > 0) {
                    $el.closest(".input-group").addClass('has-error');
                } else {
                    $el.closest('.form-group').addClass('has-error');
                }
                $el.data('bs.tooltip').options.title = error;
                if (delay) {
                    setTimeout(function() {
                        $el.tooltip('show');
                    }, delay);
                } else { $el.tooltip('show'); }
            } else {

                if ($el.data().hasOwnProperty('bs.tooltip')) {
                    // $el.data('bs.tooltip').options.title = "";
                    $el.tooltip('hide');
                    delete $el.data()["bs.tooltip"];
                }
            }
        }
    };
    ko.observableArray.fn.pausable = function() {
        var _isPaused = false;
        this.notifySubscribers = function() {
            if (!_isPaused) {
                ko.subscribable.fn.notifySubscribers.apply(this, arguments);
            }
        };
        this.pause = function() {
            _isPaused = true;
        };
        this.resume = function() {
            _isPaused = false;
        };
        return this;
    };
    ko.observableArray.fn.first = function(fn) {
        return _.find(this(), fn);
    };
    ko.observableArray.fn.each = function(callback) {
        _.forEach(this(), callback);
        return this;
    };
    ko.observableArray.fn.sortable = function(callback) {
        this.onSorted = callback;
        return this;
    };
    var prepareTemplateOptions = function(valueAccessor, dataName) {
        var result = {},
            options = ko.utils.unwrapObservable(valueAccessor()) || {};
        //build our options to pass to the template engine
        if (options.data) {
            result[dataName] = options.data;
            result.name = options.template;
        } else {
            result[dataName] = valueAccessor();
        }
        //return options to pass to the template binding
        return result;
    };
    //去除模板的前后空格
    var stripTemplateWhitespace = function(element, name) {
        var templateSource, templateElement;
        //process named templates
        if (name) {
            templateElement = document.getElementById(name);
            if (templateElement) {
                templateSource = new ko.templateSources.domElement(templateElement);
                templateSource.text($.trim(templateSource.text()));
            }
        } else {
            //remove leading/trailing non-elements from anonymous templates
            $(element).contents().each(function() {
                if (this && this.nodeType !== 1) {
                    element.removeChild(this);
                }
            });
        }
    };

    ko.bindingHandlers.modal = {
        init: function(element, valueAccessor, allBindingsAccessor, bindingContext) {
            ko.bindingHandlers.modal.update(element, valueAccessor, allBindingsAccessor);
            $(element).on("hidden.bs.modal", function() {
                valueAccessor()(false);
                if ($("body").find(".modal.in").length) {
                    if (!$("body").hasClass("modal-open")) {
                        $("body").addClass("modal-open");
                    }
                }
            });
            $(element).on("show.bs.modal", function() {
                setTimeout(function() {
                    if ($(element).hasClass("media-dialog") ||
                        $(element).hasClass("category-dialog")) {
                        $(".modal-backdrop:last").css("z-index", 200001);
                    }
                }, 80)
            })
            $(element).on("shown.bs.modal", function() {
                Kooboo.EventBus.publish("ko/binding/modal/shown", {
                    elem: element
                })
            })
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor()),
                $el = $(element);
            $el.modal(value ? 'show' : 'hide');
        }
    };

    function toTimeAgo(dt) {
        var secs = ((+new Date - dt.getTime()) / 1000),
            days = Math.floor(secs / 86400);
        return days === 0 && (secs < 60 && "just now" ||
                secs < 120 && "a minute ago" ||
                secs < 3600 && Math.floor(secs / 60) + " minutes ago" ||
                secs < 7200 && "an hour ago" ||
                secs < 86400 && Math.floor(secs / 3600) + " hours ago") ||
            days === 1 && "yesterday" ||
            days < 31 && days + " days ago" ||
            days < 60 && "one month ago" ||
            days < 365 && Math.ceil(days / 30) + " months ago" ||
            days < 730 && "one year ago" || Math.ceil(days / 365) + " years ago";
    }

    ko.bindingHandlers.timeAgo = {
        update: function(element, valueAccessor) {
            var val = valueAccessor(),
                date = new Date(val), // WARNING: this is not compatibile with IE8
                timeAgo = toTimeAgo(date);
            return ko.bindingHandlers.html.update(element, function() {
                return '<time datetime="' + encodeURIComponent(val) + '">' + timeAgo + '</time>';
            });
        }
    };

    ko.validateField = function(val, condition) {

        if (arguments.length === 1) {
            if (typeof val === "object") {
                condition = val;
                val = null;
            }
        };

        var validateCondition = {};
        _.forEach(Object.keys(condition), function(key) {
            validateCondition[key] = condition[key];
        });

        return ko.observable(val).extend({
            validate: validateCondition
        });

    }
})()