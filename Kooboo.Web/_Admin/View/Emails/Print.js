$(function() {
    var $iframe = null;

    var printModel = function() {
        var self = this;

        this.currentMail = ko.observable();
        this.currentMail.subscribe(function(mail) {

            if (mail) {
                var setHTML = function(code) {
                    iframe.contentWindow.document.documentElement.innerHTML = code;
                    $("img", iframe.contentWindow.document).load(function() {
                        self.adjustIframe();
                    }).error(function() {
                        self.adjustIframe();
                    })
                    self.adjustIframe();
                }

                iframe = $("iframe.auto-height")[0];

                if (!iframe) {
                    setTimeout(function() {
                        iframe = $("iframe.auto-height")[0];
                        setHTML(mail.html);
                    }, 300);
                } else {
                    setHTML(mail.html);
                }
            }
        })

        this.adjustIframe = function() {
            $(iframe).removeAttr("style");
            $(iframe).height(iframe.contentWindow.document.body.scrollHeight + 20);
        }

        this.getDetailDate = ko.pureComputed(function() {
            var date = new Date(self.currentMail().date);
            return date.toDefaultLangString();
        })

        this.onPrint = function() {
            self.printing(true);

            if (!window.print()) {
                self.printing(false);
            }
        }

        this.printing = ko.observable(false);

        this.downloadAttachment = function(attachment) {
            window.open(Kooboo.EmailAttachment.downloadAttachment() +
                '/' + self.currentMail().id +
                (attachment ? '/' + attachment.fileName : ''))
        }
    }

    var vm = new printModel();

    ko.applyBindings(vm, document.getElementById("main"));

    var id = Kooboo.getQueryString("id"),
        folder = Kooboo.getQueryString("folder");

    Kooboo.EmailMessage.getContent({
        messageId: id,
        folder: folder
    }).then(function(res) {

        if (res.success) {
            vm.currentMail(res.model);
        }
    })
})