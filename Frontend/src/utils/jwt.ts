export function getPayload(accessToken: string) {
  if (!accessToken) return {};
  const json = decodeBase64Url(accessToken.split(".")[1]);
  return JSON.parse(json) as Record<string, unknown>;
}

const decodeBase64Url = function (input: string) {
  input = input.replace(/-/g, "+").replace(/_/g, "/");
  const pad = input.length % 4;
  if (pad) {
    if (pad === 1) {
      throw new Error(
        "InvalidLengthError: Input base64url string is the wrong length to determine padding"
      );
    }
    input += new Array(5 - pad).join("=");
  }

  return atob(input);
};
