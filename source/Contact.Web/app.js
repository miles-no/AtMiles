
/**
 * Module dependencies.
 */

var express = require('express');

var os = require("os");
var hostname = os.hostname();
console.log(hostname);


//var routes = require('./routes/routes.js');
var elasticsearch = require('elasticsearch');
var client = new elasticsearch.Client({
  host: hostname === 'milescontact' ? 'localhost:9200' : 'milescontact.cloudapp.net:9200',
  //log: 'trace'
});


yaml = require('js-yaml');
fs   = require('fs');
var config = yaml.safeLoad(fs.readFileSync('config/config.yaml', 'utf8'));
console.log(config);

// create thumbs dir if not exists
var createDir = function(dir){
  try {
      fs.mkdirSync(dir);

    } catch(e) {
      if ( e.code != 'EEXIST' ) throw e;
    }
}

createDir('./data/' + config.companyId);
createDir('./data/' + config.companyId + '/thumbs');
createDir('./data/' + config.companyId + '/thumbs/small');
createDir('./data/' + config.companyId + '/thumbs/large');

var http = require('http');
var path = require('path');

var port = process.env.PORT || 80;


var app = express();

// all environments
app.set('port', port);
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');
app.use(express.favicon(path.join(__dirname, 'public','images','favicon.ico')));
app.use(express.logger('dev'));
app.use(express.json());
app.use(express.urlencoded());
app.use(express.methodOverride());


app.use(express.static(path.join(__dirname, 'public')));




app.use(app.router);
// routes ======================================================================
require('./routes/routes.js')(app, client, config); // load our routes and pass in our app and fully configured passport
require('./routes/api.js')(app, client, config); // load our routes and pass in our app and fully configured passport


// development only
if ('development' == app.get('env')) {
    app.use(express.errorHandler());
    app.locals.pretty = true;
}


http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});
