$(function () {
    var self;
    function lineModel(data) {
        data.message = data.message.split('\t').join('&nbsp;&nbsp;&nbsp;&nbsp;');
        data.message = data.message.split(' ').join('&nbsp;');
        data.isBreakpoint  = false;
        data.isCurrent = false;
        return data
    }
    new Vue({
        el: "#main",
        data: function () {
            return {
                id:Kooboo.getQueryString("id"),
                isDebugMode: false,
                lines:[],
                variablesData:'',
                command:undefined,
                executeHistories:[]
            };
        },
        created: function () {
            self = this;
        },
        mounted:function() {
            window.onbeforeunload = function() {
                var flag = "If this variable is undefined, window will close without confirmation.";

                Kooboo.Debugger.stopSession({
                    codeId: Kooboo.getQueryString("id")
                }).then(function(res) {
                    if (res.success) {
                        flag = undefined
                    }
                });

                return flag;
            };
            $(document).keydown(function(e) {
                switch (e.keyCode) {
                    case 119://F8
                        e.preventDefault();
                        self.debugClick(null,'none');
                        break;
                    case 122://F11
                        e.preventDefault();
                        if (e.shiftKey) {
                            self.debugClick(null,'out');
                        } else {
                            self.debugClick(null,'into');
                        }
                        break;
                    case 121://F10
                        e.preventDefault();
                        self.debugClick(null,'over');
                        break;
                }
            })
            this.startSession()
        },
        methods: {
            startSession: function () {
                Kooboo.Debugger.startSession({
                    codeId: self.id
                }).then(function(res) {

                    if (res.success) {
                        var temp = res.model.map(function(line, idx) {
                            return new lineModel({
                                idx: idx,
                                message: line
                            });
                        })
                        self.lines = temp;
                        self.isDebugMode = true;
                    }
                })
            },

            getPlaceholder:function(index) {
                var lastIndexStr = (this.lines.length-1).toString();
                var len = lastIndexStr.length - index.toString().length;
                var str ='';
                for (var i= 0; i < len;i++) {
                    str = str + '&nbsp;'
                }
                return str
            },
            toggleBreakpoint:function (event,item,index) {
                if (!!item.message.trim()) {
                    var breakLines = self.lines.filter(function(l) {
                        return l.isBreakpoint;
                    }).map(function(l) {
                        return l.idx + 1;
                    });

                    if (item.isBreakpoint) {
                        var idx = breakLines.indexOf(item.idx + 1);
                        breakLines.splice(idx, 1);
                    } else {
                        breakLines.push(item.idx + 1);
                        breakLines.sort();
                    }

                    Kooboo.Debugger.setBreakpoints({
                        codeId: self.id,
                        points: breakLines
                    }).then(function(res) {
                        if (res.success) {
                            item.isBreakpoint = !item.isBreakpoint;
                            if (breakLines.length) {
                                self.queryState();
                            }
                        }
                    });
                }
            },
            syntaxHighlight: function (json) {
                json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
                return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function(match) {
                    var cls = 'number';
                    if (/^"/.test(match)) {
                        if (/:$/.test(match)) {
                            cls = 'key';
                        } else {
                            cls = 'string';
                        }
                    } else if (/true|false/.test(match)) {
                        cls = 'boolean';
                    } else if (/null/.test(match)) {
                        cls = 'null';
                    }
                    return '<span class="' + cls + '">' + match + '</span>';
                });
            },
            queryState:function () {
                Kooboo.Debugger.getInfo({
                    codeId: self.id
                }).then(function(res) {
                    if (res.success) {
                        var data = res.model;
                        if (data.hasValue) {
                            self.variablesData = self.syntaxHighlight(JSON.stringify(data.variables, undefined, 4))
                            if (data.endOfExe) {
                                self.goToLine(null);

                                if (data.isException) {
                                    self.variablesData = self.syntaxHighlight(JSON.stringify({
                                        "message": data.message
                                    }, undefined, 4));
                                    self.isDebugMode = false;
                                    self.lines.forEach(function(line) {
                                        line.isBreakpoint = false;
                                    });
                                    alert(Kooboo.text.alert.exceptionOccuredAndDebuggerClosed);
                                } else if (confirm(Kooboo.text.confirm.restartExecution)) {
                                    Kooboo.Debugger.startSession({
                                        codeId: self.id
                                    }).then(function(res) {
                                        if (res.success) {
                                            var breakLines = [];
                                            self.lines.forEach(function(line) {
                                                if (line.isBreakpoint) {
                                                    breakLines.push(line.idx + 1);
                                                }
                                            });
                                            breakLines.sort();
                                            Kooboo.Debugger.setBreakpoints({
                                                codeId: self.id,
                                                points: breakLines
                                            }).then(function(res) {
                                                if (res.success) {
                                                    if (breakLines.length) {
                                                        self.queryState();
                                                    }
                                                }
                                            });
                                        }
                                    });
                                    self.isDebugMode = true;
                                } else {
                                    self.isDebugMode = false;
                                    self.lines.forEach(function(line) {
                                        line.isBreakpoint = false;
                                    })
                                }
                            } else {
                                self.goToLine(data.start.line);
                                self.isDebugMode = true;
                            }
                        } else {
                            self.isDebugMode && self.queryState();
                        }
                    }

                })
            },
            goToLine:function(lineIdx) {

                self.lines.forEach(function(line) {
                    line.isCurrent = false;
                });

                if (lineIdx) {
                    var line = self.lines.find(function(line) {
                        return line.idx === (lineIdx - 1);
                    });

                    line.isCurrent= true;
                }
            },
            executeCode:function() {
                self.executeHistories.push({
                    isCommand: true,
                    content: self.command
                });

                Kooboo.Debugger.execute({
                    codeId: self.id,
                    jsStatement: self.command
                }).then(function(res) {
                    if (res.success) {
                        self.executeHistories.push({
                            isCommand: false,
                            hasError: !res.model.success,
                            content: res.model.model
                        });
                        self.variablesData = self.syntaxHighlight(JSON.stringify(res.model.variables, undefined, 4))
                    } else {
                        self.executeHistories.push({
                            isCommand: false,
                            hasError: true,
                            content: res.messages.join('; ')
                        })
                    }
                    var commandWrapperEl = $('.command-histories')[0];
                    commandWrapperEl.scrollTop = commandWrapperEl.scrollHeight;
                });

                self.command = '';
            },
            startDebug:function () {
                Kooboo.Debugger.startSession({
                    codeId: self.id
                }).then(function(res) {
                    if (res.success) {
                        self.isDebugMode = true;
                    }
                })
            },
            debugClick:function (event,action) {
                Kooboo.Debugger.step({
                    codeId: self.id,
                    action: action
                }).then(function(res) {
                    self.queryState();
                })
            },
            reloadCode:function () {
                window.location.reload();
            }

        }
    })
})
