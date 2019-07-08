import { createContainer } from "./container";
import { createMoveButton } from "./moveButton";
import { createPreviousButton } from "./previousButton";
import { createNextButton } from "./nextButton";
import { createSaveButton } from "./saveButton";
import { getEditorContainer } from "@/dom/utils";
import { createImageButton } from "./ImageButton";
import { createLinkButton } from "./linkButton";

export function createActionBar(document: Document) {
  let container = createContainer(document);
  container.appendChild(createMoveButton(document, container));
  container.appendChild(createImageButton(document));
  container.appendChild(createLinkButton(document));
  container.appendChild(createPreviousButton(document));
  container.appendChild(createNextButton(document));
  container.appendChild(createSaveButton(document));
  getEditorContainer().appendChild(container);
}
