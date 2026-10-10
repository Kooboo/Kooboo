var result = k.ai.callFunction("read_skill_resource", {
    skillName: "kooboo-coding",
    resourceName: "assets/view.template.html"
})

k.site.views.add({
    name: k.request.viewName,
    body: result
});

return "create view success!"