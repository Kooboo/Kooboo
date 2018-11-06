(function() {
    var languageManager = Kooboo.LanguageManager;

    var SITE_ID = Kooboo.getQueryString("SiteId"),
        SITE_ID_STRING = "?SiteId=" + SITE_ID;

    ko.bindingHandlers.richeditor = {
        init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
            if (!ko.isWriteableObservable(valueAccessor())) {
                throw 'valueAccessor must be writeable and observable';
            }

            // Get custom configuration object from the 'wysiwygConfig' binding, more settings here... http://www.tinymce.com/wiki.php/Configuration
            var options = allBindings.has('editorConfig') ? allBindings.get('editorConfig') : {},
                isMailEditor = allBindings.has('mailConfig') ? allBindings.get('mailConfig') : false,
                // Set up a minimal default configuration
                defaults = {
                    "language": languageManager.getTinyMceLanguage(), // params needed
                    'branding': false,
                    'plugins': 'autoresize link textcolor lists codemirror image ' + (isMailEditor ? '' : 'codesample'),
                    'toolbar': "undo redo | " +
                        "bold italic forecolor backcolor formatselect | " +
                        "indent outdent | " +
                        "alignleft aligncenter alignright alignjustify | " +
                        "bullist numlist | " +
                        "image link | " +
                        "code" + (isMailEditor ? '' : ' codesample'),
                    'menubar': false,
                    'statusbar': false,
                    'remove_script_host': false,
                    'convert_urls': false,
                    'extended_valid_elements': 'style',
                    'valid_children': '+body[style]',
                    'autoresize_min_height': 300,
                    'autoresize_max_height': 600,
                    'setup': function(editor) {
                        // Ensure the valueAccessor state to achieve a realtime responsive UI.
                        editor.on('change keyup nodechange', function(args) {
                            // Update the valueAccessor
                            if (SITE_ID) {
                                valueAccessor()(editor.getContent().split(SITE_ID_STRING).join(""));
                            } else {
                                valueAccessor()(editor.getContent());
                            }
                        });

                        editor.on('NodeChange', function(e) {
                            // if (e && e.element.nodeName.toLowerCase() == 'img') {
                            //     tinyMCE.DOM.setAttribs(e.element, { 'width': null, 'height': null });
                            // }
                        });

                        editor.on('init', function(e) {
                            Kooboo.EventBus.publish("kb/tinymce/initiated", editor);
                        })
                    },
                    codesample_dialog_height: 500,
                    codesample_languages: [
                        { text: 'HTML/XML', value: 'markup' },
                        { text: 'JavaScript', value: 'javascript' },
                        { text: 'CSS', value: 'css' },
                        { text: 'PHP', value: 'php' },
                        { text: 'Ruby', value: 'ruby' },
                        { text: 'Python', value: 'python' },
                        { text: 'Java', value: 'java' },
                        { text: 'C', value: 'c' },
                        { text: 'C#', value: 'csharp' },
                        { text: 'C++', value: 'cpp' }
                    ],
                    content_style: 'html { overflow-x: hidden; } .mce-content-body img { max-width: 100%; height: auto; }',
                    verify_html: false,
                    file_browser_callback_types: 'image',
                    codemirror: {
                        indentOnInit: true, // Whether or not to indent code on init.
                        path: '/_Admin/Scripts/lib/codemirror', // Path to CodeMirror distribution
                        config: { // CodeMirror config object
                            mode: 'htmlmixed',
                            lineNumbers: true,
                            indentUnit: 4,
                            tabSize: 4
                        },
                        width: 800, // Default value is 800
                        height: 400, // Default value is 550
                        // saveCursorPosition: true, // Insert caret marker
                        jsFiles: [ // Additional JS files to load
                            "mode/htmlmixed/htmlmixed",
                        ]
                    }
                };

            if (!isMailEditor) {
                defaults.file_browser_callback = function(field_name, url, type, win, isPicked) {
                    if (!isPicked) {
                        Kooboo.EventBus.publish("ko/style/list/pickimage/show", {
                            settings: tinymce.editors[0].settings,
                            field_name: field_name,
                            type: type,
                            win: win,
                            from: "RICHEDITOR"
                        });
                    } else {
                        win.document.getElementById(field_name).value = url;
                    }
                }
            } else {
                defaults.file_browser_callback = function(field_name, url, type) {
                    document.getElementById(field_name).value = url;
                }
                defaults.file_picker_types = 'image';
                defaults.file_picker_callback = function(cb, value, meta) {
                    var input = document.createElement('input');
                    input.setAttribute('type', 'file');
                    input.setAttribute('accept', 'image/*');
                    input.onchange = function() {
                        var files = this.files;
                        if (files && files.length) {
                            var data = new FormData();
                            data.append("fileName", files[0].name);
                            data.append("file", files[0]);

                            Kooboo.EmailAttachment.ImagePost(data).then(function(res) {
                                if (res.success) {
                                    cb(res.model);
                                }
                            })
                        }
                        $(this).val("");
                    }
                    input.click();
                }
            }

            // Apply custom configuration over the defaults
            defaults = $.extend(defaults, options);
            // Ensure the valueAccessor's value has been applied to the underlying element, before instanciating the tinymce plugin

            if (valueAccessor()()) {
                var _tempParent = $("<div>");
                $(_tempParent).append(valueAccessor()());
                var imgDoms = $(_tempParent).find('img');
                imgDoms.each(function(idx, el) {
                    $(el).attr("src", $(el).attr("src") + SITE_ID_STRING);
                })

                $(element).text($(_tempParent).html());
            }
            // Tinymce will not be able to calculate the textarea height without this delay
            setTimeout(function() {
                if (!element.id) {
                    element.id = tinymce.DOM.uniqueId();
                }
                tinyMCE.init(defaults);
                tinymce.execCommand("mceAddEditor", true, element.id);
            }, 50);
            // To prevent a memory leak, ensure that the underlying element's disposal destroys it's associated editor.
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                element && $(element).attr('id') && tinyMCE.get($(element).attr('id')) && tinyMCE.get($(element).attr('id')).remove();
            });
        },
        update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
            // Implement the 'value' binding
            return ko.bindingHandlers['value'].update(element, valueAccessor, allBindings, viewModel, bindingContext);
        }
    };
})();