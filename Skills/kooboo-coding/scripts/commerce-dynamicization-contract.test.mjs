import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import test from "node:test";

const scriptsDir = dirname(fileURLToPath(import.meta.url));
const packageDir = resolve(scriptsDir, "..");

function read(relativePath) {
  return readFileSync(resolve(packageDir, relativePath), "utf8");
}

test("commerce dynamicization contract fixes the source of truth and completion boundary", () => {
  const source = read(
    "references/Scenario/site-building/commerce-dynamicization.md"
  );

  for (const entity of ["product", "category", "variant", "cart", "customer", "order"]) {
    assert.match(source.toLowerCase(), new RegExp(entity));
  }
  assert.match(source, /默认以 Commerce 为唯一事实源/);
  assert.match(source, /catalog-contact/);
  assert.match(source, /online-checkout/);
  assert.match(source, /原地动态化/);
  assert.match(source, /runtime verified/);
  assert.match(source, /business verified/);
});

test("product references use top-level minimum price and selected variant purchase semantics", () => {
  const productTypes = read("references/K-Script/commerce/types/product.md");
  const queryReference = read("references/Frontend/k-data/query.md");
  const productReference = read("references/K-Script/commerce/product.md");

  assert.match(productTypes, /`product\.price` 是变体价格最小值/);
  assert.match(productTypes, /不保证等于.*variants\[0\]\.price/s);
  assert.match(queryReference, /price\?: number/);
  assert.match(queryReference, /item\.price/);
  assert.match(productReference, /选定具体 variant/);
});

test("storefront creation and Products filtering require complete commerce data", () => {
  const contract = read(
    "references/Scenario/site-building/commerce-dynamicization.md"
  );
  const functionGuide = read("references/K-Script/commerce/function_guide.md");

  for (const field of ["title", "seoName", "featuredImage", "price", "inventory", "active", "categoryIds"]) {
    assert.match(contract, new RegExp("`" + field + "`"));
  }
  assert.match(contract, /All.*没有 category 条件/);
  assert.match(contract, /active = true/);
  assert.match(functionGuide, /AI storefront 创建时必传/);
});

test("commerce AI functions expose persisted receipts and typed admin destinations", () => {
  const productFunction = read("../../../Functions/BuiltIn/Commerce/Product/CreateProduct.cs");
  const categoryFunction = read("../../../Functions/BuiltIn/Commerce/Category/CreateProductCategory.cs");
  const actionButton = read("../../../Functions/BuiltIn/Frontend/ShowActionButton.cs");

  assert.match(productFunction, /CreateProductResult/);
  assert.match(productFunction, /FeaturedImage is empty/);
  assert.match(productFunction, /ProductCategoryModel/);
  assert.match(productFunction, /required string\[\] CategoryIds/);
  assert.match(productFunction, /CategoryIds == null/);
  assert.match(categoryFunction, /CreateProductCategoryResult/);
  assert.match(categoryFunction, /Image is empty/);
  assert.match(actionButton, /KoobooAdminTarget\.PaymentIntegration/);
  assert.match(actionButton, /group=Payment/);
  assert.match(actionButton, /Provide either url or koobooAdminTarget/);
});

test("Commerce login uses kb_commerce as the shared default state", () => {
  const loginFlow = read("references/Scenario/commerce-customer-login-flow.md");
  const customerReference = read("references/K-Script/commerce/customer.md");

  assert.match(loginFlow, /customer\.login\/currentId\/logout/);
  assert.match(loginFlow, /k\.commerce\.customer\.currentId/);
  assert.match(loginFlow, /k\.commerce\.customer\.logout/);
  assert.match(loginFlow, /不要默认再创建 `commerce_token`/);
  assert.match(customerReference, /默认不要再造第二套登录态/);
});

test("Commerce checkout does not expose direct paid-state mutation", () => {
  const orderReference = read("references/K-Script/commerce/order.md");
  const cartReference = read("references/K-Script/commerce/cart.md");
  const paymentReference = read("references/K-Script/payment/index.md");
  const rules = read("references/Preflight/rules/payment.yaml");

  assert.match(orderReference, /Storefront 禁止直接调用/);
  assert.doesNotMatch(orderReference, /k\.api\.(?:get|post)\([\s\S]{0,240}k\.commerce\.order\.pay/);
  assert.match(cartReference, /不要再维护第二套 `commerce_token`/);
  assert.match(paymentReference, /returnUrl.*不能调用 `k\.commerce\.order\.pay/s);
  assert.match(rules, /PF-COMMERCE-007/);
});

test("Commerce cart and client binding references use runtime field contracts", () => {
  const cartReference = read("references/K-Script/commerce/cart.md");
  const queryReference = read("references/Frontend/k-data/query.md");
  const frontendRules = read("references/Preflight/rules/template.yaml");
  const codeRules = read("references/Preflight/rules/commerce.yaml");

  assert.match(cartReference, /行单价：`line\.price`/);
  assert.match(cartReference, /行金额：`line\.amount`/);
  assert.match(queryReference, /不要在同一个 `<let>` 上重复声明 `from`/);
  assert.match(frontendRules, /PF-KOBOO-TEMPLATE-020/);
  assert.match(codeRules, /PF-COMMERCE-008/);
});
