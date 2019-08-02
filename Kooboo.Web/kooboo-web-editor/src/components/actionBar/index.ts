import { createContainer } from "./container";
import { createMoveButton } from "./moveButton";
import { createPreviousButton } from "./previousButton";
import { createNextButton } from "./nextButton";
import { createSaveButton } from "./saveButton";
import { createImageButton } from "./ImageButton";
import { createLinkButton } from "./linkButton";
import context from "@/common/context";

export function createActionBar() {
  let container = createContainer();
  container.appendChild(createMoveButton(container));
  container.appendChild(createImageButton());
  container.appendChild(createLinkButton());
  container.appendChild(createPreviousButton());
  container.appendChild(createNextButton());
  container.appendChild(createSaveButton());

  context.editableEvent.addEventListener(e => {
    container.style.display = e ? "none" : "block";
  });

  return container;
}
