(function(kb) {
    kb.EventBus = {
        events: {},
        remains: {},
        subscribe: function(event, listener) {
            // create the topic if not yet created
            if (!this.events[event]) this.events[event] = [];

            // add the listener
            this.events[event].push(listener);
        },

        publish: function(event, data) {
            // return if the topic doesn't exist, or there are no listeners
            if (!this.events[event] || this.events[event].length < 1) return;

            // send the event to all listeners
            this.events[event].forEach(function(listener) {
                listener(data);
            });
        },

        unsubscribe: function(event, listener) {
            if (!this.events[event]) return;

            this.events[event].pop();
        },

        keepEvents: function(keep) {
            var self = this;
            keep.forEach(function(ev) {
                if (self.events[ev]) {
                    if (!self.remains[ev]) self.remains[ev] = [];
                    self.remains[ev] = self.remains[ev].concat(self.events[ev]);
                }
            })
        },

        reset: function() {
            this.events = {};

            if (Object.keys(this.remains).length > 0) {
                var self = this;
                Object.keys(this.remains).forEach(function(key) {
                    self.events[key] = self.remains[key];
                })

                this.remains = {};
            }
        }
    };
})(Kooboo)