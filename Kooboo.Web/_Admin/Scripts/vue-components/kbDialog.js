Vue.component("kb-dialog", {
  template:
    '<div class="modal-dialog"><div class="modal-content"><slot></slot></div></div>'
});
Vue.component("kb-dialog-header", {
  template:
    '<div class="modal-header">' +
    '<button v-if="closeHandle" class="close" @click="closeHandle"><i class="fa fa-close"></i></button>' +
    '<h4 v-if="title" class="modal-title">{{title}}</h4>' +
    "<slot  v-else></slot></div>",
  props: {
    title: { type: String },
    closeHandle: {}
  }
});
Vue.component("kb-dialog-content", {
  template: '<div class="modal-body"><slot></slot></div>'
});
Vue.component("kb-dialog-footer", {
  template: '<div class="modal-footer"><slot></slot></div>'
});
