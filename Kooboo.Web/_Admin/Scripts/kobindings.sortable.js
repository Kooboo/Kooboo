(function() {
    ko.bindingHandlers.connect = {};

    ko.bindingHandlers.sortable = {
        init: function(element, valueAccessor, allBindingsAccessor, data, context) {
            var list = valueAccessor(),
                tmplOpts = prepareTemplateOptions(valueAccessor, 'foreach'),
                $el = $(element);
            stripTemplateWhitespace(element, tmplOpts.name);
            ko.bindingHandlers.template.init(element, function() { return tmplOpts; }, allBindingsAccessor, data, context);
            var tid = setTimeout(function() {
                $el.sortable({
                    connectWith: allBindingsAccessor().connect,
                    handle: '.sortable',
                    update: function(ev, ui) {
                        var el = ui.item[0];

                        if ($(document).find(el).length) {
                            var item = ko.dataFor(el),
                                sourceIndex = list.indexOf(item),
                                targetIndex = ko.utils.arrayIndexOf(ui.item.parent().children(), el);

                            list.splice(sourceIndex, 1);
                            if (ui.item.parent().length) {
                                var targetParentGroup = ui.item.parent().attr("id").split("-")[0],
                                    isGroupChanged = Boolean((targetParentGroup === "head") ^ item.head);
                                isGroupChanged ? targetIndex = -1 : list.splice(targetIndex, 0, item);
                            } else {
                                list.splice(targetIndex, 0, item);
                            }
                            list.onSorted && list.onSorted(list(), {
                                sourceElement: el,
                                viewModel: item,
                                sourceIndex: sourceIndex,
                                targetIndex: targetIndex
                            });
                            Kooboo.EventBus.publish("ko/binding/sorted");
                        }
                    },
                    receive: function(event, ui) {
                        var el = ui.item[0],
                            item = ko.dataFor(el),
                            sourceIndex = list.indexOf(item),
                            targetIndex = ko.utils.arrayIndexOf(ui.item.parent().children(), el);
                        el.remove();
                        list.splice(targetIndex, 0, item);
                        list.onReceive && list.onReceive(list(), {
                            sourceElement: el,
                            viewModel: item,
                            sourceIndex: sourceIndex,
                            targetIndex: targetIndex
                        });
                    },
                    remove: function(event, ui) {
                        //debugger;
                        //var el = ui.item[0], item = ko.dataFor(el), sourceIndex = list.indexOf(item), targetIndex = ko.utils.arrayIndexOf(ui.item.parent().children(), el);
                        //list.splice(sourceIndex, 1);
                    }
                });
            }, 0);
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                clearTimeout(tid);
                if ($el.data("uiSortable")) {
                    $el.sortable("destroy");
                }
            });
            return { 'controlsDescendantBindings': true };
        },
        update: function(element, valueAccessor, allBindingsAccessor, data, context) {
            var tmplOpts = prepareTemplateOptions(valueAccessor, 'foreach');
            ko.bindingHandlers.template.update(element, function() {
                return tmplOpts;
            }, allBindingsAccessor, data, context);
        }
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

    return ko;
})();