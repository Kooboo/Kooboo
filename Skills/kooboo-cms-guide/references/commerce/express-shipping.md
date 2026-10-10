# E-commerce - Physical Shipping

## Page Purpose

- Admin pages: `/commerce/shippings?name=express`, `/commerce/shippings/create`, `/commerce/shippings/edit`
- A physical shipping method defines the base shipping fee, surcharge rules, supported countries, estimated delivery days, and carrier-identification information.
- It is used for cart shipping calculations and admin shipping assistance; it does not purchase shipping labels, schedule carrier pickups, or query real tracking routes.

## Shipping Calculation

- Physical shipping is selected and calculated only when the cart contains physical products; an order containing only digital products does not use physical shipping fees.
- If the specified shipping method cannot be found, calculation can fall back to the current default shipping method. Setting a new default clears the old default status.
- Shipping starts with the base fee and then checks surcharge rules one by one. All matching rules are accumulated; only the first match is not used.
- Current surcharge conditions use the product subtotal and total weight. Total weight comes from each variant's weight multiplied by its quantity; the weight unit is defined by Commerce Settings.
- A free-shipping discount can offset an already-calculated shipping fee in later calculation, but does not change the shipping method's own base fee or rules.

## Country and Carrier Information

- Supported countries and estimated delivery days are saved with the shipping method and provided to the frontend, but the current shipping calculation does not automatically filter countries from them or verify actual transit times.
- Company Code is the shipping method's business identifier. Tracking Number Matching is a regular-expression matching hint that can fill the carrier name from a tracking number during admin shipment processing.
- These fields do not automatically connect to a logistics API and cannot prove that a tracking number is valid, a package has been collected, or an estimated date can be met.

## Modification and Deletion Boundaries

- A successful save proves only that the shipping configuration was persisted. Recalculate with a specific cart, currency, product weight, and subtotal to verify the final fee.
- Changing fees, rules, countries, or the default item does not recalculate already-created orders or modify shipping amounts and shipment information already saved in them.
- The current list does not allow the default shipping method to be selected directly for bulk deletion. Deleting another shipping method also does not repair references to it in frontend code, carts, or historical orders.
