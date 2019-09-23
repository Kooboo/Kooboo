import { Config, ConfigOptions } from "karma";
import webpack from "webpack";
import getWebpackConfig from "./webpack.config";

const WebpackConfig = getWebpackConfig({ NODE_ENV: "development" });

export default function(config: Config) {
  let configOptions = {
    webpackMiddleware: {},
    basePath: "",
    frameworks: ["mocha", "chai"],
    reporters: ["progress"],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ["ChromeHeadless"],
    singleRun: false,
    concurrency: Infinity,
    mime: {
      "text/x-typescript": ["ts"]
    },
    files: ["test/index.test.ts"],
    preprocessors: {
      "test/index.test.ts": ["webpack", "sourcemap"]
    },
    webpack: {
      mode: "development",
      resolve: WebpackConfig.resolve,
      module: WebpackConfig.module,
      devtool: "inline-source-map",
      externals: WebpackConfig.externals
    } as webpack.Configuration
  } as ConfigOptions;

  config.set(configOptions);
}
