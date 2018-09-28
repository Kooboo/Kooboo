
var editLog = Kooboo.EditLog,
    SiteEditorTypes = Kooboo.SiteEditorTypes;

function editLogAddItem() {
    expect(editLog.count()).to.be(0);
    editLog.add({
        editorType: SiteEditorTypes.EditorType.htmlblock,
        action: SiteEditorTypes.ActionType.update,
    });
    expect(editLog.count()).to.be(1);
    editLog.clear();
}

function editLogGetPrevious() {
    expect(editLog.count()).to.be(0);
    editLog.add({
        editorType: SiteEditorTypes.EditorType.htmlblock,
        action: SiteEditorTypes.ActionType.update,
    });
    editLog.add({
        editorType: SiteEditorTypes.EditorType.label,
        action: SiteEditorTypes.ActionType.update,
    });
    var item = editLog.getPrevious();
    expect(item.editorType).to.be(SiteEditorTypes.EditorType.label);
    item = editLog.getPrevious();
    expect(item.editorType).to.be(SiteEditorTypes.EditorType.htmlblock);
    editLog.clear();
}

function editLogGetNext() {
    editLog.add({
        editorType: SiteEditorTypes.EditorType.htmlblock,
        action: SiteEditorTypes.ActionType.update
    });
    editLog.add({
        editorType: SiteEditorTypes.EditorType.label,
        action: SiteEditorTypes.ActionType.update
    })
    var nextitem = editLog.getPrevious();
    nextitem = editLog.getPrevious();
    nextitem = editLog.getNext();
    expect(nextitem.editorType).to.be(SiteEditorTypes.EditorType.htmlblock);
    nextitem = editLog.getNext();
    expect(nextitem.editorType).to.be(SiteEditorTypes.EditorType.label);
    nextitem = editLog.getNext();
    expect(nextitem).to.be(null);

    editLog.clear();
}

function editLogHasPrevious() {
    expect(editLog.hasPrevious()).to.be(false);
    editLog.add({
        editorType: SiteEditorTypes.EditorType.label,
        action: SiteEditorTypes.ActionType.update
    });
    expect(editLog.hasPrevious()).to.be(true);

    editLog.clear();
}

function editLogHasNext() {
    expect(editLog.hasNext()).to.be(false);
    editLog.add({
        editorType: SiteEditorTypes.EditorType.label,
        action: SiteEditorTypes.ActionType.update
    });
    editLog.getPrevious();
    expect(editLog.hasNext()).to.be(true);

    editLog.clear();
}

