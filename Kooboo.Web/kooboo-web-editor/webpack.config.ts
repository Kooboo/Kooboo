import webpack from "webpack";
import path from "path";
export default {
  entry: "./src/index.ts",
  output: {
    path: path.resolve("../_Admin/Scripts/kooboo-web-editor"),
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
      },
      {
        test: /\.(png|jpg|gif|svg)$/,
        use: [
          {
            loader: "url-loader",
            options: {
              limit: 8192
            }
          }
        ]
      }
    ]
  }
} as webpack.Configuration;
