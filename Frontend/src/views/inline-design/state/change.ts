import type { KoobooBinding } from "../binding";
import { hasStringFormat } from "../binding";
import { configSources } from "../binding";
import { dbSources } from "../binding";
import { contentSources } from "../binding";
import { keyValueSources } from "../binding";
import { getBinding } from "../binding";
import { labelSources } from "../binding";
import { clearKoobooMark } from "../utils/dom";

export interface Change {
  source: string;
  id: string;
}

export interface DomChange extends Change {
  koobooId: string;
  value?: string;
  attribute?: string;
  action: "copy" | "delete" | "update";
}

export interface StyleChange extends Change {
  koobooId?: string;
  url?: string;
  value: string;
  action: "styleSheet";
  selector: string;
  property: string;
  important?: string;
  mediarulelist?: string;
}

export interface AttributeChange extends Change {
  koobooId?: string;
  value: string;
  action: "update";
  attribute: string;
}

export interface ConfigChange extends Change {
  value: string;
  attribute: string;
}

export interface ObjectChange extends Change {
  value?: string;
}

export interface ContentChange extends Change {
  value?: string;
  path: string;
  action: "copy" | "delete" | "update";
}

export interface DbChange extends Change {
  value?: string;
  table: string;
  path: string;
  action: "copy" | "delete" | "update";
}

export function getLabelChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, labelSources);
  if (!binding || hasStringFormat(bindings)) return;
  if (el && binding.reference == el) return;

  return {
    source: binding.source,
    id: binding.id,
    value: clearKoobooMark(binding.reference!.innerHTML),
  } as ObjectChange;
}

export function getKeyValueChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, keyValueSources, undefined, "content");
  if (!binding || hasStringFormat(bindings)) return;
  if (el && binding.reference == el) return;

  return {
    source: binding.source,
    id: binding.key,
    value: clearKoobooMark(binding.reference!.innerHTML),
  } as ObjectChange;
}

export function getKeyValueAttributeChange(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  const binding = getBinding(bindings, keyValueSources, attribute);
  if (
    !binding ||
    binding.reference !== el ||
    hasStringFormat(bindings, "attribute")
  )
    return;

  return {
    source: binding.source,
    id: binding.key,
    value: el.getAttribute(attribute),
  } as ObjectChange;
}

export function getConfigChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, configSources, "innerHtml");
  if (!binding || hasStringFormat(bindings)) return;
  if (el && binding.reference == el) return;

  return {
    source: binding.source,
    id: binding.key,
    value: clearKoobooMark(binding.reference!.innerHTML),
    attribute: "innerHtml",
  } as ConfigChange;
}

export function getConfigAttributeChange(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  const binding = getBinding(bindings, configSources, attribute);
  if (
    !binding ||
    binding.reference != el ||
    hasStringFormat(bindings, attribute)
  )
    return;

  return {
    source: binding.source,
    id: binding.key,
    value: el.getAttribute(attribute),
    attribute: attribute,
  } as ConfigChange;
}

export function getContentChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, contentSources, undefined, "content");
  if (!binding || hasStringFormat(bindings)) return;
  if (el && binding.reference == el) return;

  return {
    source: binding.source,
    id: binding.id,
    path: binding.fieldName,
    action: "update",
    value: clearKoobooMark(binding.reference!.innerHTML),
  } as ContentChange;
}

export function getContentAttributeChange(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  const binding = getBinding(bindings, contentSources, attribute);

  if (
    !binding ||
    binding.reference != el ||
    hasStringFormat(bindings, attribute)
  )
    return;

  return {
    source: binding.source,
    id: binding.id,
    path: binding.fieldName,
    action: "update",
    value: el.getAttribute(attribute),
  } as ContentChange;
}

export function getDbChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const binding = getBinding(bindings, dbSources, undefined, "content");
  if (!binding || hasStringFormat(bindings)) return;
  if (el && binding.reference == el) return;

  return {
    source: binding.source,
    id: binding.id,
    path: binding.fieldName,
    table: binding.table,
    action: "update",
    value: clearKoobooMark(binding.reference!.innerHTML),
  } as DbChange;
}

export function getDbAttributeChange(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  const binding = getBinding(bindings, dbSources, attribute);
  if (
    !binding ||
    binding.reference != el ||
    hasStringFormat(bindings, attribute)
  )
    return;

  return {
    source: binding.source,
    id: binding.id,
    path: binding.fieldName,
    table: binding.table,
    action: "update",
    value: el.getAttribute(attribute),
  } as DbChange;
}

export function getDynamicChange(bindings: KoobooBinding[], el?: HTMLElement) {
  const labelChange = getLabelChange(bindings, el);
  if (labelChange) return labelChange;
  const keyValueChange = getKeyValueChange(bindings, el);
  if (keyValueChange) return keyValueChange;
  const configChange = getConfigChange(bindings, el);
  if (configChange) return configChange;
  const contentChange = getContentChange(bindings, el);
  if (contentChange) return contentChange;
  const dbChange = getDbChange(bindings, el);
  if (dbChange) return dbChange;
}

export function getDynamicAttributeChange(
  bindings: KoobooBinding[],
  el: Element,
  attribute: string
) {
  const keyValueChange = getKeyValueAttributeChange(bindings, el, attribute);
  if (keyValueChange) return keyValueChange;
  const configChange = getConfigAttributeChange(bindings, el, attribute);
  if (configChange) return configChange;
  const contentChange = getContentAttributeChange(bindings, el, attribute);
  if (contentChange) return contentChange;
  const dbChange = getDbAttributeChange(bindings, el, attribute);
  if (dbChange) return dbChange;
}
