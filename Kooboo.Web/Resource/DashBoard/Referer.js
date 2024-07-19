const last500 = k.site.visitor.topSource();
var card;

if (last500.length) {
  const pie = k.site.dashBoard.createPieChart();

  for (const i of last500) {
    pie.addItem(i.source, i.counter);
  }

  card = k.site.dashBoard
    .createCard()
    .addTextRow("Referer", { align: "Center", fontSize: 16 })
    .addRow(pie)
    .addTextRow("Based on last 500 visitors", { align: "Center" });

}

export default card;
