const logs = k.site.visitor.Top500();
var card;

if (logs.length) {
  const lineChart = k.site.dashBoard.createLineChart();
  const dates = [];

  function formatDate(value) {
    return `${value.getMonth() + 1}/${value.getDate()}`;
  }

  for (let i = 0; i < 5; i++) {
    const date = new Date();
    date.setDate(new Date().getDate() - i);
    dates.unshift(formatDate(date));
  }

  for (const date of dates) {
    lineChart.addLabel(date);
  }

  const datas = [];

  for (const log of logs) {
    const date = formatDate(new Date(log.begin));

    if (dates.includes(date)) {
      let page = datas.find((f) => f.name == log.url && f.date == date);
      if (!page) {
        page = { name: log.url, date, count: 1 };
        datas.push(page);
      } else {
        page.count++;
      }
    }
  }

  const pages = Array.from(new Set(datas.map((m) => m.name))).map((m) => ({
    name: m,
    points: [],
  }));

  for (const date of dates) {
    for (const page of pages) {
      const found = datas.find((f) => f.date == date && f.name == page.name);
      page.points.push(found ? found.count : 0);
    }
  }

  for (const page of pages) {
    lineChart.addLine(page.name, page.points);
  }

  card = k.site.dashBoard.createCard();
  card.autoRefresh = true;

  card
    .addTextRow("Trend", { align: "Center", fontSize: 16 })
    .addRow(lineChart)
    .addTextRow("Based on last 500 visitors", { align: "Center" });


}

export default card;
