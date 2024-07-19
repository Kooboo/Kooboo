import { monaco } from "./userWorker";
let initialized = false;

export default function registerJsonSchema() {
  if (initialized) return;
  initialized = true;

  monaco.languages.json.jsonDefaults.setDiagnosticsOptions({
    validate: true,
    schemas: [
      {
        uri: "http://www.kooboo.com/module-setting-schema.json",
        fileMatch: ["*/module.config"],
        schema: {
          type: "object",
          properties: {
            name: {
              type: "string",
            },
            version: {
              type: "string",
            },
            description: {
              type: "string",
            },
            settingDefines: {
              type: "array",
              items: {
                $ref: "http://www.kooboo.com/module-setting-define-schema.json",
              },
            },
            setting: {
              type: "object",
            },
          },
          required: ["name", "version"],
        },
      },
      {
        uri: "http://www.kooboo.com/module-setting-define-schema.json", // id of the second schema
        schema: {
          type: "object",
          properties: {
            name: { type: "string" },
            type: {
              enum: ["input", "textarea", "switch", "number", "select"],
            },
            defaultValue: {
              type: ["string", "number", "null", "boolean"],
            },
            description: {
              type: "string",
            },
            options: {
              type: "array",
              items: {
                type: ["string", "number", "boolean"],
              },
            },
            display: {
              type: "string",
            },
          },
          required: ["name", "type"],
        },
      },
    ],
  });
}
