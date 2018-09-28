$(function() {

    var cropper;

    var editModel = function() {
        var self = this;

        this.filename = ko.observable();

        this.link = ko.observable();

        this.linkText = ko.observable();

        this.altText = ko.observable();
        this._altText = ko.observable();

        this.src = ko.observable();
        this._src = ko.observable();

        this.imageEditing = ko.observable(false);

        this.toggleMode = function() {

            if (!self.imageEditing()) {
                self.imageEditing(true);
                var image = document.getElementById("editable-img");

                cropper = new Cropper(image, {
                    aspectRatio: self.aspectRatio(),
                    viewMode: 1,
                    preview: '.thumbnail',
                    crop: function(e) {
                        self.width(Math.round(e.detail.width));
                        self.height(Math.round(e.detail.height));
                        self.rotate(Math.round(e.detail.rotate));
                    }
                });

            } else {
                if (confirm(Kooboo.text.confirm.exit)) {
                    self.imageEditing(false);
                    cropper.destroy();
                }
            }
        }

        this.actionClick = function(action) {
            switch (action.name) {
                case "zoom":
                    cropper.zoom(action.value);
                    break;
                case "move":
                    cropper.move(action.value[0], action.value[1]);
                    break;
                case "rotate":
                    cropper.rotate(action.value);
                    break;
                case "scaleX":
                    cropper.scaleX(-cropper.getData().scaleX);
                    break;
                case "scaleY":
                    cropper.scaleY(-cropper.getData().scaleY);
                    break;
            }
        }

        this.aspectRatio = ko.observable(0);
        this.aspectRatio.subscribe(function(ratio) {
            cropper.setAspectRatio(ratio);
        });

        this.aspectRatioArray = ko.observableArray([{
            text: Kooboo.text.site.images.free,
            ratio: 0
        }, {
            text: "16 : 9",
            ratio: 16 / 9
        }, {
            text: "4 : 3",
            ratio: 4 / 3
        }, {
            text: "1 : 1",
            ratio: 1 / 1
        }, {
            text: "2 : 3",
            ratio: 2 / 3
        }])

        this.changeRatio = function(m, e) {
            e.preventDefault();
            if (m.ratio !== self.aspectRatio()) {
                self.aspectRatio(m.ratio);
            } else {
                e.preventDefault();
            }
        }

        this.saveEditing = function() {
            var canvas = cropper.getCroppedCanvas(),
                base64Image = canvas.toDataURL();

            self.src(base64Image);
            cropper.destroy();
            $(".thumbnail").removeAttr("style");
            self.imageEditing(false);
            self.isImageChange(true);
        }

        this.width = ko.observable();
        this.width.subscribe(function(width) {
            self.resetCropper();
        })

        this.height = ko.observable();
        this.height.subscribe(function(height) {
            self.resetCropper();
        })

        this.rotate = ko.observable();
        this.rotate.subscribe(function(rotate) {
            self.resetCropper();
        })

        this.resetCropper = function() {
            var data = cropper.getData();
            data.width = Number(self.width());
            data.height = Number(self.height());
            data.rotate = Number(self.rotate());
            cropper.setData(data);
        }

        this.isImageChange = ko.observable(false);

        this.attrAction = function(action) {
            switch (action.name) {
                case 'width':
                    action.op == '-' ? self.width(self.width() - 1) : self.width(self.width() + 1);
                    break;
                case 'height':
                    action.op == '-' ? self.height(self.height() - 1) : self.height(self.height() + 1);
                    break;
                case 'rotate':
                    action.op == '-' ? self.rotate(self.rotate() - 1) : self.rotate(self.rotate() + 1);
                    break;
            }
        }

        this.submitEdit = function() {

            Kooboo.Media.imageUpdate(self.getUpdateData()).then(function(res) {

                if (res.success) {
                    self.goBack();
                }
            })
        }

        this.getUpdateData = function() {
            var data = {
                id: id,
                alt: self.altText()
            }

            if (self.isImageChange()) {
                data["base64"] = self.src()
            }

            return data;
        }

        this.userCancel = function() {

            if (!self.isChanged()) {
                self.goBack();
            } else {

                if (confirm(Kooboo.text.confirm.beforeReturn)) {
                    self.goBack();
                }
            }
        }

        self.isChanged = function() {
            return (self.altText() !== self._altText()) || (self.src() !== self._src());
        }

        this.goBack = function() {
            location.href = Kooboo.Route.Get(Kooboo.Route.Image.ListPage) + location.hash;
        }
    }

    var vm = new editModel();

    ko.applyBindings(vm, document.getElementById("main"));

    var id = Kooboo.getQueryString("Id");

    id && Kooboo.Media.Get({
        Id: id
    }).then(function(res) {

        if (res.success) {
            vm.filename(res.model.name);
            vm.linkText(res.model.url);
            vm.link(res.model.fullUrl);
            vm.altText(res.model.alt || "");
            vm._altText(vm.altText());
            vm.src(res.model.siteUrl);
            vm._src(vm.src());
        }
    })
})