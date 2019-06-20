import { createContainer } from "./container";
import { createBlank } from "./blank";
import { createMoveButton } from "./moveButton";
import { createPreviousButton } from "./previousButton";
import { createNextButton } from "./nextButton";
import { createSaveButton } from "./saveButton";
import { getEditorContainer } from "../../dom/utils";

export function createActionBar(document: Document) {
  let container = createContainer(document);
  container.appendChild(createMoveButton(document, container));
  container.appendChild(createBlank(document));
  container.appendChild(createPreviousButton(document));
  container.appendChild(createBlank(document));
  container.appendChild(createNextButton(document));
  container.appendChild(createBlank(document));
  container.appendChild(createSaveButton(document));
  getEditorContainer(document).appendChild(container);
}
