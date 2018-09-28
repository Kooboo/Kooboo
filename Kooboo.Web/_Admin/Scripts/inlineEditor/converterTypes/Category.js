function Category(){
    return {
        name: "Category",
        displayName: "Category",
        execute: function(element) {
            var clonedElement = element.cloneNode(true);
            var result = {
                convertToType: this.name,
                name: this.name,
                koobooId: $(element).attr("kooboo-id")
            }
            Kooboo.NewDomService.RemoveKoobooAttribute(clonedElement);
            //ToDo Verify logic
            var tree = Kooboo.Category.FindCategoryTree(clonedElement);

            if (tree == null) {
                return result;
            }

            if (tree.ItemContainer != null && !tree.ItemContainer.isEqualNode(clonedElement)) {
                tree.ItemContainer = clonedElement;
            }

            result.htmlBody = Kooboo.Category.GetTemplate(tree);
            result.data = Kooboo.Category.GetData(tree);
            result.hasResult = true;
        },
    }
}