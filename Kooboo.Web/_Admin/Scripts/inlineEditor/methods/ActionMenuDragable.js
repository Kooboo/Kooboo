function ActionMenuDragable(el,fixY){
    var id = 1,
    docEl = document.documentElement;
    return {
        el:el,
        $elem: null,
        isDragging: false,
        diffX: 0,
        diffY: 0,
        minX: 0,
        minY: 0,
        maxX: null,
        maxY: null,
        fixX: false,
        fixY: fixY,
        lockContainer: true,
        maxContainer: docEl,
        backMillisecond: 400,
        firstOffset: { left: 0, top: 0 },
        _idKey: "kb-dragable-id",
        init: function() {
            this.id = id++;
            this.$elem = $(this.el);
            this.$elem.data(this._idKey, this.id);

            this.initEvent();
            this.initMaxOffset();
        },
        initMaxOffset: function() {
            var offset = this.$elem.offset(),
                x = offset.left,
                y = offset.top;
            this.maxX = Math.max(this.maxContainer.clientWidth, this.maxContainer.scrollWidth) - this.$elem.width();
            this.maxY = Math.max(this.maxContainer.clientHeight, this.maxContainer.scrollHeight) - this.$elem.height();
            x < this.minX && (x = this.minX);
            y < this.minY && (y = this.minY);
            x > this.maxX && (x = this.maxX);
            y > this.maxY && (y = this.maxY);
            this.$elem.css({ left: x, top: y, right: "auto" });
        },
        OnResize: function() {
            this.initMaxOffset();
        },
        OnDragStart: function(e) {
            var udf;
            this.initDocEvent();
            this.isDragging = true;
            this.diffX = (e.clientX === udf ? e.originalEvent.changedTouches[0].clientX : e.clientX) - this.$elem.offset().left;
            this.diffY = (e.clientY === udf ? e.originalEvent.changedTouches[0].clientY : e.clientY) - this.$elem.offset().top;
            this.firstOffset = this.$elem.offset();
            this.initMaxOffset();
            e.preventDefault();
        },
        OnDraging: function(e) {
            var tmpX, tmpY, udf;
            if (this.$elem.length && this.isDragging) {
                this.$elem.addClass("draging");
                tmpX = (e.clientX === udf ? e.originalEvent.changedTouches[0].clientX : e.clientX) - this.diffX;
                tmpY = (e.clientY === udf ? e.originalEvent.changedTouches[0].clientY : e.clientY) - this.diffY;
                if (this.lockContainer) {
                    tmpX < this.minX && (tmpX = this.minX);
                    tmpY < this.minY && (tmpY = this.minY);
                    tmpX > this.maxX && (tmpX = this.maxX);
                    tmpY > this.maxY && (tmpY = this.maxY);
                }
                this.$elem.css({ left: tmpX, top: tmpY, right: "auto" });
            }
        },
        OnDragEnd: function(e) {
            this.destroyDocEvent();
            if (this.fixX) {
                this.$elem.animate({
                    left: this.firstOffset.left
                }, this.backMillisecond);
            }
            if (this.fixY) {
                this.$elem.animate({
                    top: this.firstOffset.top
                }, this.backMillisecond);
            }
            this.isDragging = false;
            this.$elem.removeClass("draging");
        },
        initDocEvent: function() {
            $(document).on("touchmove.dragable" + this.id, _.bind(this.OnDraging, this));
            $(document).on("touchend.dragable" + this.id, _.bind(this.OnDragEnd, this));
            $(document).on("mousemove.dragable" + this.id, _.bind(this.OnDraging, this));
            $(document).on("mouseup.dragable" + this.id, _.bind(this.OnDragEnd, this));
        },
        destroyDocEvent: function() {
            $(document).off("touchmove.dragable" + this.id);
            $(document).off("touchend.dragable" + this.id);
            $(document).off("mousemove.dragable" + this.id);
            $(document).off("mouseup.dragable" + this.id);
        },
        initEvent: function() {
            this.$elem.on("touchstart.dragable" + this.id, _.bind(this.OnDragStart, this));
            this.$elem.on("mousedown.dragable" + this.id, _.bind(this.OnDragStart, this));
            $(window).on("resize.dragable" + this.id, _.bind(this.OnResize, this));
        },
        destroyEvent: function() {
            this.$elem.off("touchstart.dragable" + this.id);
            this.$elem.off("mousedown.dragable" + this.id);
            $(window).off("resize.dragable" + this.id);
        },
        destroy: function() {
            this.$elem.removeData(this._idKey);
            this.destroyEvent(this.$elem);
            delete this.$elem;
            delete this.maxContainer;
            delete this.firstOffset;
            delete this.lockContainer;
        }
    }
}