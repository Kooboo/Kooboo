(function() {
  Vue.component("user-verify-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/UserVerifyModal.html"
    ),
    props: {
      isShow: Boolean, // is-show.sync
      email: String
    },
    data: function() {
      return {
        model: {
          email: ""
        },
        rules: {
          email: [
            {
              required: true,
              message: Kooboo.text.validation.required
            },
            {
              pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
              message: Kooboo.text.validation.emailInvalid
            }
          ]
        }
      };
    },
    methods: {
      onHide: function() {
        this.$refs.form.clearValid();
        this.$emit("update:isShow", false);
      },
      onSubmit: function() {
        var self = this;
        if (this.$refs.form.validate && this.$refs.form.validate()) {
          Kooboo.User.verifyEmail({
            email: self.model.email
          }).then(function(res) {
            if (res.success) {
              alert(Kooboo.text.alert.verificationEmailSent);
              self.onHide();
            }
          });
        }
      }
    },
    watch: {
      isShow: function(val) {
        if (val) {
          this.model.email = this.email;
        }
      }
    }
  });
})();
