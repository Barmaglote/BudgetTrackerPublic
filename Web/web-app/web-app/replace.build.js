var replace = require('replace-in-file');
var package = require("./package.json");
var environment = process.argv[3];
var file = process.argv[2];
var buildVersion = getBuildVersion();
const options = {
  files: file,
  from: /version: '(.*)'/g,
  to: "version: '"+ buildVersion + "'",
  allowEmptyPaths: false,
};

try {
    let changedFiles = replace.sync(options);
    if (changedFiles == 0) {
      throw "Please make sure that file '" + options.files + "' has \"version: ''\"";
    }
    console.log('Build version set: ' + buildVersion);
}
catch (error) {
    console.error('Error occurred:', error);
}

function getBuildVersion() {
  var packageVersion = package.version;
  if (environment) {
    packageVersion += '-' + environment
  }
  return packageVersion;
}

function getDate() {
  var d = new Date(),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

  if (month.length < 2)
      month = '0' + month;
  if (day.length < 2)
      day = '0' + day;

  var dateString = [year, month, day].join('.');
  if (environment == 'dev') {
    dateString += '-Dev'
  } else if (environment == 'test'){
    dateString += '-Test'
  }
  return dateString;
}

