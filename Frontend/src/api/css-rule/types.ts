export interface InlineItem {
  dateModifiedString: string;
  id: string;
  lastModified: string;
  name: string;
  ownerName: string;
  ownerType: string;
  source: string;
}

export interface InlineStyle {
  declarations: Declaration[];
  id: string;
  selector: string;
}

export interface Declaration {
  cmsCssRuleId: string;
  constType: number;
  creationDate: string;
  id: string;
  important: boolean;
  lastModified: string;
  lastModifyTick: number;
  name: string;
  parentStyleId: string;
  propertyName: string;
  value: string;
}
