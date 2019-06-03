import webpack from "webpack";
import path from "path";
export default {
  entry: "./src/index.ts",
  output: {
    path: path.resolve("./"),
    filename: "kooboo-web-editor.min.js"
  },
  watch: process.env.NODE_ENV == "development",
  mode: process.env.NODE_ENV,
  resolve: {
    extensions: [".ts", ".tsx", ".js"]
  },
  module: {
    rules: [
      { test: /\.tsx?$/, loader: "ts-loader" },
      {
        test: /\.js$/,
        loader: "eslint-loader",
        include: [path.join(__dirname, "src")],
        options: {
          fix: true
        }
      }
    ]
  }
} as webpack.Configuration;
