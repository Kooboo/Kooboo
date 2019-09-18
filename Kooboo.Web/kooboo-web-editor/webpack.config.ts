import webpack from "webpack";
import path from "path";

export default (env: any): webpack.Configuration => ({
  entry: {
    index: "./src/index.ts",
    richEditor: "./src/components/richEditor/tinymce.ts"
  },
  output: {
    path: path.resolve("../_Admin/Scripts/kooboo-web-editor"),
    filename: "[name].min.js"
  },
  watch: env.NODE_ENV == "development",
  mode: env.NODE_ENV,
  externals: {
    "tinymce-declaration": "tinymce"
  },
  resolve: {
    extensions: [".ts", ".tsx", ".js"],
    alias: {
      "@": path.resolve(__dirname, "./src")
    }
  },
  // devtool: env.NODE_ENV == "development" ? "inline-source-map" : undefined,
  module: {
    rules: [
      { test: /\.(tsx?)|(js)$/, loader: "ts-loader" },
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
      },
      {
        test: /\.css$/,
        use: ["style-loader", "css-loader"]
      }
    ]
  }
});
