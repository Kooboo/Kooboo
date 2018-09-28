
function getFieldName() {
    var commentdata = {
        fieldname: "content"
    };
    var repeater=Kooboo.dom.DomOperationRepeater({});
    var fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).to.eql("content");

    commentdata = {
        bindingvalue: "/chs/{userkey}"
    };
    fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).to.eql("userkey");

    commentdata = {
        bindingvalue: "/chs/{userkey1}"
    };
    fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).not.to.eql("userkey");

    commentdata = {
        bindingvalue: "List_Item.ct"
    };
    fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).to.eql("ct");

    commentdata = {
        bindingvalue: "{GetById.img}"
    };
    fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).to.eql("img");

    commentdata = {
        bindingvalue: "background-image: url({special1_Item.img})"
    };
    fieldname = repeater.getFieldName(commentdata);
    expect(fieldname).to.eql("img");

}
function modifyContentRepeater() {
    var doc = document.implementation.createHTMLDocument("");
    var htmlArr = [];
    htmlArr.push("<html><body kooboo-id='1'>");
    htmlArr.push("<!--#kooboo--objecttype='page'--nameorid='823d50ef-4917-44ac-9bdb-f5215e0b3ba8'-->");
    htmlArr.push("<!--#kooboo--objecttype='contentrepeater'--nameorid='b57b6101-5c08-2498-e2a2-c6640323da9b'--folderid='bfe6181b-45a5-11db-009a-bd2043c0548c'--bindingvalue='customer_Item'--boundary='220'-->")
    htmlArr.push("<div kooboo-id='1-1'>");
    htmlArr.push("<!--#kooboo--objecttype='content'--nameorid='b57b6101-5c08-2498-e2a2-c6640323da9b'--bindingvalue='customer_Item.Name'--fieldname='Name'--koobooid='1-0-1-1-1-1-3-1-1-1-1-1-3-1-1'-->");
    htmlArr.push('<h3 id="name" kooboo-id="1-0-1-1-1-1-3-1-1-1-1-1-3-1-1">Ejazul islam1</h3>');
    htmlArr.push("<!--#kooboo--objecttype='content'--nameorid='b57b6101-5c08-2498-e2a2-c6640323da9b'--bindingvalue='customer_Item.Job'--fieldname='Job'--koobooid='1-0-1-1-1-1-3-1-1-1-1-1-3-1-3'-->");
    htmlArr.push('<span id="job" class="text-uppercase m-b-1" kooboo-id="1-0-1-1-1-1-3-1-1-1-1-1-3-1-3">Ejazul islam123</span>');
    htmlArr.push("</div>");
    htmlArr.push("<!--#kooboo--end=true--objecttype='contentrepeater'--boundary='220'-->");
    htmlArr.push("</body></html>");
    doc.write(htmlArr.join(" "));

    var nameEl = $("#name", doc)[0];
    var context = Kooboo.elementReader.readObject(nameEl);

    expect($("#name", doc).html()).to.eql("Ejazul islam1");
    expect($("#job", doc).html()).to.eql("Ejazul islam123");

    var newRepeater = Kooboo.dom.DomOperationRepeater({ context: context, nameOrId: "b57b6101-5c08-2498-e2a2-c6640323da9b",action:Kooboo.SiteEditorTypes.ActionType.update });
    var data = {
        Job: "Ejazul islam123",
        Name: "Ejazul isla"
    };
    newRepeater.update(data);
    expect($("#name", doc).html()).to.eql(data.Name);
    expect($("#job", doc).html()).to.eql(data.Job);
}

function getFieldValue() {
    var data = {
        content: "test"
    }
    var fieldName = "content";
    var commentData = {
        bindingvalue: "List_Item.content"
    };
    var repeater=Kooboo.dom.DomOperationRepeater({});
    var fieldValue = repeater.getFieldValue(data, fieldName, commentData);
    expect(fieldValue).to.eql("test");

    commentData = {
        bindingvalue: "/chs/{content}"
    };
    fieldValue = repeater.getFieldValue(data, fieldName, commentData);
    expect(fieldValue).to.eql("/chs/test");

    // commentData = {
    //     bindingvalue: "{repeat:first}"
    // };
    // fieldValue = repeater._getFieldValue(data, fieldName, commentData);
    // expect(fieldValue).to.eql("");

    commentData = {
        bindingvalue: "background-image: url({special1_Item.img})"
    };
    fieldValue = repeater.getFieldValue(data, fieldName, commentData);
    expect(fieldValue).to.eql("background-image: url(test)");
}
