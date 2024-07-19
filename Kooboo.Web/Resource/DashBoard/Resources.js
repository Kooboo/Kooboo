const radarChart = k.site.dashBoard.createRadarChart();

radarChart
  .addLabel("Page")
  .addLabel("View")
  .addLabel("Layout")
  .addLabel("Script")
  .addLabel("Style");

radarChart.addMap("Resources", [
  k.site.pages.all().length,
  k.site.views.all().length,
  k.site.layouts.all().length,
  k.site.scripts.all().length,
  k.site.styles.all().length,
]);

const card = k.site.dashBoard.createCard().addRow(radarChart);

export default card;