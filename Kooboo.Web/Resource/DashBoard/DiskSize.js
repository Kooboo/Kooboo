const pie = k.site.dashBoard.createPieChart();
const storesSize = k.site.diskSpace.getCommonStoreSize();
const stores = storesSize.storeItem;

for (const store of stores) {
    pie.addItem(store.name, store.size)
}

var panel = k.site.dashBoard.createCard()
    .addRow(pie)
    .addTextRow(`Total Size: ${storesSize.totalSize}`, { align: "Center" })

export default panel;