# AList

Ant Design Vue 4.2.6; native export `List`. Import `AList` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `bordered`: boolean; default false. Toggles rendering of the border around the list
- `dataSource`: any[]; default -. dataSource array for list
- `footer`: string|slot; default -. List footer renderer
- `grid`: object; default -. The grid type of list. You can set grid to something like {gutter: 16, column: 4}
- `header`: string|slot; default -. List header renderer
- `itemLayout`: string; default -. The layout of list, default is `horizontal`, If a vertical list is desired, set the itemLayout property to `vertical`
- `loading`: boolean|[object](https://www.antdv.com/components/spin/#api); default false. Shows a loading indicator while the contents of the list are being fetched
- `loadMore`: string|slot; default -. Shows a load more content
- `locale`: object; default emptyText: 'No Data' <br>. i18n text including empty text
- `pagination`: boolean | object; default false. Pagination [config](https://www.antdv.com/components/pagination/#api), hide it by setting it to false
- `renderItem`: ({item, index}) => vNode; default unspecified. Custom item renderer, #renderItem="{item, index}"
- `rowKey`: string|Function(record):string; default `key`. Item's unique key, could be a string or function that returns a string
- `split`: boolean; default true. Toggles rendering of the split under the list item
- `column`: number oneOf [ 1, 2, 3, 4, 6, 8, 12, 24]; default -. column of grid
- `gutter`: number; default 0. spacing between grid
- `size`: `default` | `middle` | `small`; default `default`. Size of list
- `xxxl`: number; default -. `≥2000px` column of grid
- `xs`: number; default -. `<576px` column of grid
- `sm`: number; default -. `≥576px` column of grid
- `md`: number; default -. `≥768px` column of grid
- `lg`: number; default -. `≥992px` column of grid
- `xl`: number; default -. `≥1200px` column of grid
- `xxl`: number; default -. `≥1600px` column of grid

## Events

No events listed in the pinned upstream metadata.

## Slots

- `footer`: List footer renderer
- `header`: List header renderer
- `loadMore`: Shows a load more content

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AList :data-source="rows" bordered><template #renderItem="{item}"><AListItem><AListItemMeta :title="item.name" :description="item.status"/></AListItem></template></AList>
```

Source: pinned upstream es/components.js, es/list, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
