const path = require('path');

module.exports = {
  entry: {
    AdminIndex: './js/AdminIndex.js'
  },
  output: {
    filename: '[name].bundle.js',
    path: path.resolve(__dirname, '../dist'),
    clean: true
  },
};