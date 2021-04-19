(function () {
  Kooboo.loadJS([
    "/_Admin/Scripts/kooboo/Guid.js",
    "/_Admin/Scripts/components/kbForm.js",
  ]);

  Vue.component("kb-options-editor", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/ECommerce/kb-options-editor.html"
    ),
    props: ["list", "hasDependent"],
    data() {
      return {
        innerList: [],
        newOption: null,
        validateModel: {
          count: { valid: true, msg: "" },
        },
        validRules: [
          { required: Kooboo.text.validation.required },
          {
            max: 31,
            message: Kooboo.text.validation.maxLength + 31,
          },
        ],
      };
    },
    mounted() {
      this.innerList = this.list.map((m) => ({
        key: m.key,
        value: m.value,
        editing: false,
        editingValue: m.value,
      }));

      for (const i of this.innerList) {
        Vue.set(this.validateModel, i.key, { valid: true, msg: "" });
      }
    },
    methods: {
      saveOption(item) {
        if (!item.editingValue) return;
        item.value = item.editingValue;
        item.editing = false;
        this.update();
      },
      cancelEdit(item) {
        item.editingValue = item.value;
        item.editing = false;
      },
      removeOption(item) {
        this.innerList = this.innerList.filter((f) => f != item);
        this.update();
      },
      editOption(item) {
        if (this.innerList.some((s) => s.editing) || this.newOption != null) {
          return;
        }

        item.editing = true;
      },
      addOption() {
        if (this.innerList.some((s) => s.editing)) return;
        this.newOption = {
          key: Kooboo.Guid.NewGuid(),
          value: "",
          editing: true,
          editingValue: "",
        };

        Vue.set(this.validateModel, this.newOption.key, {
          valid: true,
          msg: "",
        });
      },
      saveNewOption() {
        if (!this.newOption.editingValue) return;
        this.newOption.value = this.newOption.editingValue;
        this.newOption.editing = false;
        this.innerList.push(this.newOption);
        this.newOption = null;
        this.update();
      },
      update() {
        this.$emit(
          "update:list",
          this.innerList.map((m) => ({
            key: m.key,
            value: m.value,
          }))
        );
      },
      valid() {
        var valid = true;

        this.validateModel.count = Kooboo.validField(this.innerList, [
          { required: Kooboo.text.validation.required },
        ]);

        if (!this.validateModel.count.valid) valid = false;

        for (const i of this.innerList) {
          this.validateModel[i.key] = Kooboo.validField(
            i.value,
            this.validRules
          );

          if (!this.validateModel[i.key].valid) valid = false;
        }

        return valid;
      },
    },
  });
})();