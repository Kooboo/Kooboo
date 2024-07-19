var pages = k.site.visitor.top5Pages();
const barChart = k.site.dashBoard.createBarChart();

for (const page of pages) {
  barChart.addLabel(page.name);
  barChart.addBar(page.count, {
    category: "Top 5 pages",
  });
}

const card = k.site.dashBoard
  .createCard()
  .addRow(barChart)
  .addTextRow("Based on last 500 visitors", { align: "Center" });

export default card;
