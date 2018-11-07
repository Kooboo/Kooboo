$(function() {
    var debuggerModel = function() {
        var self = this;

        this.id = Kooboo.getQueryString("id");

        this.lines = ko.observableArray();

        this.isDebugMode = ko.observable(false);

        this.toggleBreakpoint = function(line) {
            if (!!line.message().trim()) {

                var breakLines = self.lines().filter(function(l) {
                    return l.isBreakpoint();
                }).map(function(l) {
                    return l.idx() + 1;
                })

                if (line.isBreakpoint()) {
                    var idx = breakLines.indexOf(line.idx() + 1);
                    breakLines.splice(idx, 1);
                } else {
                    breakLines.push(line.idx() + 1);
                    breakLines.sort();
                }

                Kooboo.Debugger.setBreakpoints({
                    codeId: self.id,
                    points: breakLines
                }).then(function(res) {
                    if (res.success) {
                        line.isBreakpoint(!line.isBreakpoint());

                        if (breakLines.length) {
                            queryState();
                        }
                    }
                });
            }
        }

        this.editUrl = function() {
            return Kooboo.Route.Get(Kooboo.Route.Code.EditPage, {
                id: self.id
            })
        }

        this.reloadCode = function() {
            location.reload();
        }

        this.goToLine = function(lineIdx) {

            self.lines().forEach(function(line) {
                line.isCurrent(false);
            })

            if (lineIdx) {
                var line = self.lines().find(function(line) {
                    return line.idx() == (lineIdx - 1);
                })

                line.isCurrent(true);
                // $('#code-line-' + (lineIdx - 1))[0].scrollIntoView();
            }
        }

        this.debugClick = function(action) {
            Kooboo.Debugger.step({
                codeId: self.id,
                action: action
            }).then(function(res) {
                queryState();
            })
        }

        this.startDebug = function() {
            Kooboo.Debugger.startSession({
                codeId: self.id
            }).then(function(res) {
                if (res.success) {
                    self.isDebugMode(true);
                }
            })
        }

        Kooboo.Debugger.startSession({
            codeId: self.id
        }).then(function(res) {
            if (res.success) {
                self.isDebugMode(true);
                self.lines(res.model.map(function(line, idx) {
                    return new lineModel({
                        idx: idx,
                        message: line
                    });
                }));
            }
        })

        this.executeHistories = ko.observableArray();

        this.command = ko.observable();

        this.handleEnter = function(m, e) {
            if (e.keyCode == 13) {
                self.executeCode();
            }
        }

        this.executeCode = function() {
            self.executeHistories.push({
                isCommand: true,
                content: self.command()
            })

            Kooboo.Debugger.execute({
                codeId: self.id,
                jsStatement: self.command()
            }).then(function(res) {
                if (res.success) {
                    self.executeHistories.push({
                        isCommand: false,
                        hasError: !res.model.success,
                        content: res.model.model
                    })
                    self.variablesData(syntaxHighlight(JSON.stringify(res.model.variables, undefined, 4)))
                } else {
                    self.executeHistories.push({
                        isCommand: false,
                        hasError: true,
                        content: res.messages.join('; ')
                    })
                }
                commandWrapperEl.scrollTop = commandWrapperEl.scrollHeight;
            })

            self.command('');
        }

        this.variablesData = ko.observable();

        function queryState() {
            Kooboo.Debugger.getInfo({
                codeId: self.id
            }).then(function(res) {
                if (res.success) {
                    var data = res.model;
                    if (data.hasValue) {
                        self.variablesData(syntaxHighlight(JSON.stringify(data.variables, undefined, 4)))
                        if (data.endOfExe) {
                            self.goToLine(null);

                            if (data.isException) {
                                self.variablesData(syntaxHighlight(JSON.stringify({
                                    "message": data.message
                                }, undefined, 4)))
                                self.isDebugMode(false);
                                self.lines().forEach(function(line) {
                                    line.isBreakpoint(false);
                                })
                                alert(Kooboo.text.alert.exceptionOccuredAndDebuggerClosed);
                            } else if (confirm(Kooboo.text.confirm.restartExecution)) {
                                Kooboo.Debugger.startSession({
                                    codeId: self.id
                                }).then(function(res) {
                                    if (res.success) {
                                        var breakLines = [];
                                        self.lines().forEach(function(line) {
                                            if (line.isBreakpoint()) {
                                                breakLines.push(line.idx() + 1);
                                            }
                                        })
                                        breakLines.sort();
                                        Kooboo.Debugger.setBreakpoints({
                                            codeId: self.id,
                                            points: breakLines
                                        }).then(function(res) {
                                            if (res.success) {
                                                if (breakLines.length) {
                                                    queryState();
                                                }
                                            }
                                        });
                                    }
                                });
                                self.isDebugMode(true);
                            } else {
                                self.isDebugMode(false);
                                self.lines().forEach(function(line) {
                                    line.isBreakpoint(false);
                                })
                            }
                        } else {
                            self.goToLine(data.start.line);
                            self.isDebugMode(true);
                        }
                    } else {
                        self.isDebugMode() && queryState();
                    }
                }
            })
        }
    }

    function lineModel(data) {
        var self = this;
        data.message = data.message.split('\t').join('&nbsp;&nbsp;&nbsp;&nbsp;')
        data.message = data.message.split(' ').join('&nbsp;');

        ko.mapping.fromJS(data, {}, self);

        this.index = ko.pureComputed(function() {
            if (self.idx() < 9) {
                return (self.idx() + 1) + '&nbsp;';
            } else {
                return self.idx() + 1;
            }
        })

        this.isBreakpoint = ko.observable(false);

        this.isCurrent = ko.observable(false);
    }

    function syntaxHighlight(json) {
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
    }

    var vm = new debuggerModel();

    ko.applyBindings(vm, document.getElementById('main'));

    var commandWrapperEl = $('.command-histories')[0];

    window.onbeforeunload = function() {
        var flag = "If this variable is undefined, window will close without confirmation.";

        Kooboo.Debugger.stopSession({
            codeId: Kooboo.getQueryString("id")
        }).then(function(res) {
            if (res.success) {
                flag = undefined
            }
        })

        return flag;
    }

    $(document).keydown(function(e) {
        switch (e.keyCode) {
            case 119:
                e.preventDefault();
                vm.debugClick('none');
                break;
            case 122:
                e.preventDefault();
                if (e.shiftKey) {
                    vm.debugClick('out');
                } else {
                    vm.debugClick('into');
                }
                break;
            case 121:
                e.preventDefault();
                vm.debugClick('over');
                break;
        }
    })
})