const path = require('path');

module.exports = {
  // The entry point file described above
  entry: './src/domino-train-firebase-js.js',
  // The location of the build folder described above
  output: {
    path: path.resolve(__dirname, '../domino-train/wwwroot'),
    filename: 'npmjs.bundle.js'
  },
  // Optional and for development only. This provides the ability to
  // map the built code back to the original source format when debugging.
  devtool: 'eval-source-map',
};
