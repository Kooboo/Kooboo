# ElCarousel

Loop a series of images or texts in a limited space

Kind: component. Import `ElCarousel` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/carousel.html#carousel

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| height | string | '' | height of the carousel |
| initial-index | number | 0 | index of the initially active slide (starting from 0) |
| trigger | 'hover' \| 'click' | hover | how indicators are triggered |
| autoplay | boolean | true | whether automatically loop the slides |
| interval | number | 3000 | interval of the auto loop, in milliseconds |
| indicator-position | '' \| 'none' \| 'outside' | '' | position of the indicators |
| arrow | 'always' \| 'hover' \| 'never' | hover | when arrows are shown |
| type | '' \| 'card' | '' | type of the Carousel |
| card-scale | number | 0.83 | when type is card, scaled size of secondary cards |
| loop | boolean | true | display the items in loop |
| direction | 'horizontal' \| 'vertical' | horizontal | display direction |
| pause-on-hover | boolean | true | pause autoplay when hover |
| motion-blur | boolean | false | infuse dynamism and smoothness into the carousel |

## Events

- `change`: triggers when the active slide switches, it has two parameters, the one is the index of the new active slide, and other is index of the old active slide

## Slots

- `default`: customize default content
