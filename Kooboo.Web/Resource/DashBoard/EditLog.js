var logs = k.site.editLog.lastLog(0, 10);
var card;

if (logs.length) {
  var table = k.site.dashBoard
    .createTable()
    .addHeader("Name")
    .addHeader("Type")
    .addHeader("User")
    .addHeader("Ago");

  for (const log of logs) {
    table.addRow([log.storeName, log.actionType, log.userName, log.Ago]);
  }

  const button = k.site.dashBoard.createButton("View all");
  button.clickOpenUrl(`/_Admin/system/site-logs?SiteId=${k.site.webSite.id}`);

  card = k.site.dashBoard
    .createCard()
    .addTextRow("Editing Log", { align: "Center", fontSize: 16 })
    .addRow(table)
    .addRow(button)
}

export default card;
