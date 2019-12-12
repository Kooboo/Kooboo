var getKViewSuggestions = function() {
  var suggestions = [
    {
      label: "k-attributes",
      documentation:
        "k-attributes will render the value as defined attribute name inside the open tag.",
      insertText: 'k-attributes="attributeKey attributeValue"'
    },
    {
      label: "k-content",
      documentation:
        "k-content will replace the inner html of that tag with the value of that key",
      insertText: 'k-content="obj"'
    },
    {
      label: "k-for",
      documentation: "Loop the item collection.",
      insertText: 'k-for="1, 10, i"'
    },
    {
      label: "k-foreach",
      documentation: "Loop the item collection",
      insertText: 'k-foreach="item itemList"'
    },
    {
      label: "k-href",
      documentation:
        "Bind the value into href attribute of the tag. You can also set the value in the href attribute of a tag, only in the later case, you must use the {} to mark the field name that will be used to retrieve value from kooboo data context.",
      insertText: 'k-href="url"'
    },
    {
      label: "k-if",
      documentation:
        "When the if condition return true, the element will be rendered.",
      insertText: 'k-if="true"'
    },
    {
      label: "k-ignore",
      documentation:
        "Ignore action on that tag. This includes all tags that have kview attributes or A tag that has href attribute.",
      insertText: "k-ignore"
    },
    {
      label: "k-label",
      documentation:
        "Used to get or set value from content label repository. If the key exists, it will get the value from label repository and replace the tag innerhtml. If the key is not exists, it will be created with the tag innerhtml as the default value.",
      insertText: 'k-label="value"'
    },
    {
      label: "k-omit",
      documentation: "Omit the element open tag.",
      insertText: 'k-omit="true"'
    },
    {
      label: "k-placeholder",
      documentation:
        "Used to mark an element as a placeholder/container in the layout. Components can be added into that placeholder from page designer.",
      insertText: 'k-placeholder="positionName"'
    },
    {
      label: "k-repeat",
      documentation: "Loop the item collection.",
      insertText: 'k-repeat="item itemlist"'
    },
    {
      label: "k-repeat-self",
      documentation: "Repeating the container element as well.",
      insertText: "k-repeat-self"
    },
    {
      label: "k-replace",
      documentation: "Replace the element with the value.",
      insertText: 'k-replace="htmlText"'
    }
  ];
  return suggestions;
};
