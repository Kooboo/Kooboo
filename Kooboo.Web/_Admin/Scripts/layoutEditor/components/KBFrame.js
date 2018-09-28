(function() {
    var Script, Style, BindingStore;

    var ATTR_RES_TAG_ID = 'kb-res-tag-id';

    if (Kooboo.layoutEditor || Kooboo.pageEditor) {
        Script = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Script;
        Style = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].viewModel.Style;
        BindingStore = Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].store.BindingStore;
    } else {
        Script = null;
        Style = null;
        BindingStore = null;
    }

    var KBFrame = function(frame, config) {
        var self = this;
        this._iframe = frame || null;
        this._window = frame ? frame.contentWindow : null;
        this._document = frame ? frame.contentWindow.document : null;
        this._baseTagHTML = null;
        this._title = null;
        this.type = null;
        this.resources = null;
        this._scripts = null;
        this._styles = null;
        this._defaultLang = null;

        var _cacheContent = $("<textarea>");
        $(_cacheContent).attr("id", "__kb_frame_cache_content_container__")
            .css("display", "none");
        $(_cacheContent).insertAfter(frame);
        this._cacheContentContainer = _cacheContent;
        this._cacheContentVirtualDOM = null;

        if (config) {
            this.type = config.type;
        }

        this.setContentVirtualDOM = function(html) {
            if (this._baseTagHTML && html.indexOf(this._baseTagHTML) == -1) {
                html = html.split('<head>').join('<head>' + this._baseTagHTML);
            }

            html = html.split('http-equiv').join('http_equiv');

            var parser = new DOMParser();
            var dom = parser.parseFromString(html, "text/html");

            prehandleHead();
            prehandleBody();

            this._cacheContentVirtualDOM = dom;

            this._cacheContentContainer.val(html);

            function prehandleHead() {
                var headDOM = dom.head,
                    metas = $('meta', headDOM),
                    scripts = $('script', headDOM),
                    styles = $('link, style', headDOM);
                $(metas).each(function(idx, meta) {
                    $(meta).attr(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('meta'));
                })

                $(scripts).each(function(idx, script) {
                    $(script).attr(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('script-head'));
                })

                $(styles).each(function(idx, style) {
                    $(style).attr(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('style-head'));
                })

                var baseTag = $('base', headDOM);

                if (!this._baseTagHTML) {
                    if (baseTag.length) {
                        self._baseTagHTML = baseTag[0].outerHTML;
                    }
                }
            }

            function prehandleBody() {
                var bodyDOM = dom.body,
                    scripts = $('script', bodyDOM),
                    styles = $('link, style', bodyDOM);

                $(scripts).each(function(idx, script) {
                    $(script).attr(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('script-body'));
                })

                $(styles).each(function(idx, style) {
                    $(style).attr(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('style-body'));
                })
            }
        }

        this.getDocType = function() {
            if (this._cacheContentVirtualDOM) {
                var docType = this._cacheContentVirtualDOM.doctype,
                    result = '<!DOCTYPE html';

                if (docType) {
                    result += (docType.publicId ? ' PUBLIC "' + docType.publicId + '"' : '') +
                        (docType.systemId ? ' "' + docType.systemId + '"' : '');
                }

                result += '>';
                return result;
            } else {
                return "<!DOCTYPE html>";
            }
        }

        this.getOriginalHTML = function() {
            if (this._cacheContentVirtualDOM) {
                var dom = this._cacheContentVirtualDOM.cloneNode(true);

                var html = "";
                html += this.getDocType();
                html += '<html>';
                html += getHeadHTML();
                html += getBodyHTML();
                html += '</html>'

                html = removeEmptyLine(html);

                return html;
            } else {
                return this._cacheContentContainer.val();
            }

            function getHeadHTML() {
                return dom.head.outerHTML.split('http_equiv').join('http-equiv');
            }

            function getBodyHTML() {
                return dom.body.outerHTML;
            }

            function removeEmptyLine(html) {
                var newLines = [];
                html.split('\n').forEach(function(line) {
                    if (!!line.trim()) {
                        newLines.push(line);
                    }
                })
                return newLines.join('\n');
            }
        }

        this.getHTML = function() {
            var html = this.getOriginalHTML();

            var baseTag = $('base', this._cacheContentVirtualDOM.head);
            if (baseTag.length) {
                html = html.split(baseTag[0].outerHTML).join('');
            }

            var resTagRegex = new RegExp(ATTR_RES_TAG_ID + '="\\S*"');
            html = html.split(resTagRegex).join('');

            return html;
        }

        this.getPreviewHTML = function() {
            var html = '';
            html += this.getDocType();
            html += this._cacheContentVirtualDOM.documentElement.outerHTML;
            return html;
        }

        this.updateCacheContainer = function() {
            this._cacheContentContainer.val(this.getOriginalHTML());
        }

        this.getResourceTagSelectorById = function(id) {
            return '[' + ATTR_RES_TAG_ID + '="' + id + '"]'
        }

        this.setContent = function(html, callback) {
            this.setContentWithoutResource(html, function() {
                setTimeout(function() {
                    self._setScripts(self._scripts);
                    self._setStyles(self._styles);
                    Kooboo.EventBus.publish("kb/html/previewer/rootElem", self._document.body);
                    Kooboo.EventBus.publish("kb/lighter/holder", self._document.body);
                }, 500);
                callback();
            });
        };

        this.setContentWithoutResource = function(html, callback) {
            self.setContentVirtualDOM(html);

            html = self.getPreviewHTML();

            self._window = self._iframe.contentWindow;
            self._document = self._window.document;

            $(".page-loading").show();
            $.when(setHtml(html)).done(function() {
                $(".page-loading").hide();
                bindEvent();
                self._title = self._document.title;
                setTimeout(function() {
                    Kooboo.EventBus.publish("kb/frame/loaded");
                }, 200);
                callback();
            })

            function setHtml(html) {
                var drd = $.Deferred();
                self._document.open();
                self._document.write(html);
                self._document.close();
                var task = function() {
                    //wait for document loaded
                    //if body is null,document is blocked by script
                    if (!self._document.body) {
                        setTimeout(task, 200);
                    } else {
                        drd.resolve();
                    }
                }
                setTimeout(task, 500) //document js is blocked by ko.so add settimeout
                return drd.promise();
            }

            function bindEvent() {
                $('img', self._document).load(function() {
                    self.resize();
                }).error(function() {
                    self.resize();
                })

                $(self.getDocumentElement()).on('click', function(e) {
                    e.preventDefault();
                    if (e.target.tagName == 'BODY' || $(e.target).parents('body').length) {
                        Kooboo.EventBus.publish('kb/lighter/holder', e.target);
                    }
                })

                $(self.getDocumentElement()).on('mouseover', function(e) {
                    e.preventDefault();
                    if (e.target.tagName == 'BODY' || $(e.target).parents('body').length) {
                        Kooboo.EventBus.publish('kb/preview/elem/hover', e.target);
                    }
                });

                self._window.onscroll = function() {
                    self.resize();
                }

                $(self._window).load(function() {
                    $(self).trigger('loaded')
                })
            }
        };

        this.setExistResource = function(obj) {
            self._scripts = obj.scripts;
            self._styles = obj.styles;
        }

        this.setResource = function(resources) {
            self.resources = resources;
        }

        this.hasResource = function() {
            return !!self.resources;
        }

        this.setTitle = function(title) {
            self._document.title = title;
            self._title = title;
            self._cacheContentVirtualDOM && (self._cacheContentVirtualDOM.title = title);
            self.updateCacheContainer();
        }

        this.getTitle = function() {
            return self._title;
        }

        this._setScripts = function(scripts) {
            var resources = self.resources;
            if (!scripts) {
                reloadScriptAndAddToStore(true);
            } else {
                reloadScriptAndAddToStore(false);
                scripts.forEach(function(url) {
                    var tag = self._document.createElement('script');
                    tag.setAttribute('src', url);
                    self._document.body.appendChild(tag);

                    var find = _.find(_.concat(resources.scripts, resources.scriptGroup), function(script) {
                        return script.url == url;
                    })

                    find && BindingStore.add(new Script({
                        id: find.id,
                        elem: tag,
                        head: false,
                        url: find.url,
                        displayName: find.text
                    }))
                });
                self.resize();
            }

            function reloadScriptAndAddToStore(addToStore) {
                var resources = self.resources;
                handleScripts(self._document.head.getElementsByTagName('script'), true);
                handleScripts(self._document.body.getElementsByTagName('script'), false);

                function handleScripts(scripts, inHead) {
                    if (scripts && scripts.length) {
                        _.forEach(scripts, function(script, idx) {
                            var src = script.getAttribute('src');
                            if (src) {
                                var find = _.find(_.concat(resources.scripts, resources.scriptGroup), function(s) {
                                    return s.url == src;
                                })

                                if (addToStore) {
                                    if (find) {
                                        BindingStore.add(new Script({
                                            id: find.id,
                                            elem: script,
                                            head: inHead,
                                            pullToHead: inHead,
                                            url: find.url,
                                            displayName: find.text
                                        }))
                                    } else {
                                        BindingStore.add(new Script({
                                            id: Kooboo.Guid.NewGuid(),
                                            elem: script,
                                            head: inHead,
                                            pullToHead: inHead,
                                            url: src,
                                            displayName: src
                                        }))
                                    }
                                }
                            } else {
                                addToStore && BindingStore.add(new Script({
                                    id: Math.ceil(Math.random() * Math.pow(2, 53)),
                                    elem: script,
                                    head: inHead,
                                    pullToHead: inHead,
                                    text: script.text
                                }))
                            }

                            if (!script.getAttribute(ATTR_RES_TAG_ID)) {
                                script.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('script-' + (inHead ? 'head' : 'body')));
                                Kooboo.EventBus.publish('kb/frame/resource/add', {
                                    type: 'script',
                                    tag: $(script).clone()[0],
                                    isAppendToHead: inHead
                                })
                            }
                        })
                        self.resize();
                    }
                }
            }
        };

        this._setStyles = function(styles) {
            var resources = self.resources;
            if (!styles) {
                $('link, style', self._document).each(function(idx, el) {


                    switch (el.tagName.toLowerCase()) {
                        case 'link':
                            var rel = el.getAttribute('rel'),
                                href = el.getAttribute('href');

                            if (rel !== 'stylesheet' || !href) {
                                return;
                            }

                            var find = _.find(_.concat(resources.styles, resources.styleGroup), function(style) {
                                return style.url == href;
                            });

                            find && BindingStore.add(new Style({
                                id: find.id,
                                elem: el,
                                displayName: find.text,
                                url: find.url
                            }))
                            break;
                        case 'style':
                            if (el.innerHTML.trim()) {
                                BindingStore.add(new Style({
                                    id: Math.ceil(Math.random() * Math.pow(2, 53)),
                                    elem: el,
                                    text: el.innerHTML
                                }))
                            }
                            break;
                    }

                    if (!el.getAttribute(ATTR_RES_TAG_ID)) {
                        el.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('style'));
                        Kooboo.EventBus.publish('kb/frame/resource/add', {
                            type: 'style',
                            tag: $(el).clone()[0]
                        })
                    }
                })
            } else {
                styles.forEach(function(url) {
                    var tag = self._document.createElement('link');
                    tag.setAttribute('rel', 'stylesheet');
                    tag.setAttribute('href', url);
                    tag.setAttribute(ATTR_RES_TAG_ID, Kooboo.getResourceTagId('style'));

                    var existStyle = $('link,style', self._document);
                    if (existStyle.length) {
                        $(tag).insertAfter(existStyle.last());
                    } else {
                        self._document.head.appendChild(tag);
                    }

                    var find = _.find(_.concat(resources.styles, resources.styleGroup), function(style) {
                        return style.url == url;
                    })

                    find && BindingStore.add(new Style({
                        id: find.id,
                        elem: tag,
                        url: find.url,
                        displayName: find.text
                    }))

                    Kooboo.EventBus.publish('kb/frame/resource/add', {
                        type: 'style',
                        tag: $(tag).clone()[0]
                    })
                });
            }
        };

        this.getDocumentElement = function() {
            return self._document.documentElement;
        }

        this.getScrollTop = function() {
            return $(self._document).scrollTop();
        }

        this.setScrollTop = function(top) {
            $(self._document).scrollTop(top);
        }

        this.resize = function() {
            $(window).trigger('resize');
        }

        Kooboo.EventBus.subscribe("kb/frame/resource/add", function(data) {
            var vdom = self._cacheContentVirtualDOM;

            switch (data.type) {
                case 'style':
                    var styles = $('link, style', vdom.head);
                    if (styles.length) {
                        $(data.tag).insertAfter(styles.last());
                    } else {
                        vdom.head.appendChild(data.tag);
                    }

                    break;
                case 'script':
                    vdom[data.isAppendToHead ? 'head' : 'body'].appendChild(data.tag);
                    break;
                case 'meta':
                    vdom.head.appendChild(data.tag);
                    break;
            }
            self.updateCacheContainer();
        })

        Kooboo.EventBus.subscribe("kb/frame/resource/update", function(data) {
            var vdom = self._cacheContentVirtualDOM,
                rdom = self._document;
            var selector = self.getResourceTagSelectorById(data.resTagId),
                vElem = $(selector, vdom),
                rElem = $(selector, rdom);
            switch (data.type) {
                case 'style':
                case 'script':
                    vElem.length && vElem.text(data.content);
                    rElem.length && rElem.text(data.content);
                    break;
                case 'meta':
                    [vElem, rElem].forEach(function(elem) {
                        if (elem.length) {
                            var attrs = ['name', 'http_equiv', 'charset'],
                                idx = attrs.indexOf(data.content.attr.key);

                            attrs.forEach(function(attr) {
                                elem.removeAttr(attr);
                            });

                            elem.removeAttr('content');

                            attrs.splice(idx, 1);

                            if (data.content.attr.key == 'charset') {
                                elem.attr('charset', data.content.attr.value);
                            } else {
                                elem.attr(data.content.attr.key, data.content.attr.value);
                                elem.attr('content', data.content.content);
                            }
                        }
                    })
                    break;
            }
            self.updateCacheContainer();
        })

        Kooboo.EventBus.subscribe('kb/frame/resource/remove', function(data) {
            var selector = self.getResourceTagSelectorById($(data.tag).attr(ATTR_RES_TAG_ID));
            $(selector, self._cacheContentVirtualDOM).remove();
            $(selector, self._document).remove();
            self.updateCacheContainer();
        })

        Kooboo.EventBus.subscribe('kb/frame/resource/sort', function(data) {
            var movedEl = data.elem,
                elTagId = $(movedEl).attr(ATTR_RES_TAG_ID);

            var selector = self.getResourceTagSelectorById(elTagId),
                $virEl = $(selector, self._cacheContentVirtualDOM),
                $realEl = $(selector, self._document),
                $cloneVir = null,
                $cloneReal = null;

            if ($virEl.length) {
                $cloneVir = $virEl.clone();
                $virEl.remove();
            }

            if ($realEl.length) {
                $cloneReal = $realEl.clone();
                $realEl.remove();
            }

            if (data.targetIdx > -1) {
                insertElement(data.targetIdx == 0, data);
            }

            self.updateCacheContainer();

            function insertElement(isBefore, data) {

                if (data.list.length == 1) {
                    var isAppendToHead = data.list[0].head;
                    $(isAppendToHead ? 'head' : 'body', self._cacheContentVirtualDOM).append(data.elem);
                    $(isAppendToHead ? 'head' : 'body', self._document).append($(data.elem).clone());
                } else {
                    var siblingEl = data.list[data.targetIdx + (isBefore ? 1 : -1)].elem,
                        siblingElTagId = $(siblingEl).attr(ATTR_RES_TAG_ID),
                        selector = self.getResourceTagSelectorById(siblingElTagId),
                        $siblingVirEl = $(selector, self._cacheContentVirtualDOM),
                        $siblingRealEl = $(selector, self._document);

                    if ($siblingVirEl.length) {
                        if ($cloneVir) {
                            $cloneVir['insert' + (isBefore ? 'Before' : 'After')]($siblingVirEl);
                        } else {
                            $(data.elem)['insert' + (isBefore ? 'Before' : 'After')]($siblingVirEl);
                        }

                        if ($cloneReal) {
                            $cloneReal['insert' + (isBefore ? 'Before' : 'After')]($siblingRealEl);
                        } else {
                            $(data.elem).clone()['insert' + (isBefore ? 'Before' : 'After')]($siblingRealEl);
                        }
                    }
                }
            }
        })

        Kooboo.EventBus.subscribe('kb/frame/dom/update', function(data) {
            self._cacheContentVirtualDOM.body = self._document.body.cloneNode(true);
        })
    }

    if (Kooboo.layoutEditor || Kooboo.pageEditor) {
        Kooboo[Kooboo.layoutEditor ? 'layoutEditor' : 'pageEditor'].component.KBFrame = KBFrame;
    } else if (Kooboo.viewEditor) {
        Kooboo.viewEditor.component.KBFrame = KBFrame;
    } else {
        window.KBFrame = KBFrame;
    }
})()