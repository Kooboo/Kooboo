var iframe = document.createElement('iframe');
iframe.setAttribute('src', 'about:blank');
iframe.setAttribute('id', 'test');
iframe.style.display = 'none';
document.body.appendChild(iframe);

var kbFrame = new KBFrame($('#test')[0], {
    type: 'test'
})

function getContent() {
    var content = '<!DOCTYPE html><html><head><base href="http://base.url"><title>test</title></head><body><div>testing</div></body></html>'
    kbFrame.setContent(content, function() {
        expect(kbFrame.getHTML()).to.be('<!DOCTYPE html><html><head><title>test</title></head><body><div>testing</div></body></html>');
    });
}

function setTitle() {
    kbFrame.setTitle('test2');
    expect(kbFrame.getTitle()).to.be('test2');
}

function hasResource() {
    expect(kbFrame.hasResource()).to.be(false);
}

function getScrollTop() {
    expect(kbFrame.getScrollTop()).to.be(0);
}

function setScrollTop() {
    var content = '<!DOCTYPE html><html><head><base href="http://base.url"><title>test</title></head><body><div style="margin-top: 500px">testing</div></body></html>'
    kbFrame.setContent(content, function() {
        $(iframe).show();
        kbFrame.setScrollTop(200);
        expect(kbFrame.getScrollTop()).to.be(200);
        $(iframe).hide();
    });
}