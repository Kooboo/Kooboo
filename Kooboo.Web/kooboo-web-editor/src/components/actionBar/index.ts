import { createContainer } from "./container";
import { createMoveButton } from "./moveButton";
import { createPreviousButton } from "./previousButton";
import { createNextButton } from "./nextButton";
import { createSaveButton } from "./saveButton";
import { createImageButton } from "./ImageButton";
import { createLinkButton } from "./linkButton";

export function createActionBar() {
  let container = createContainer();
  container.appendChild(createMoveButton(container));
  container.appendChild(createImageButton());
  container.appendChild(createLinkButton());
  container.appendChild(createPreviousButton());
  container.appendChild(createNextButton());
  container.appendChild(createSaveButton());
  return container;
}
