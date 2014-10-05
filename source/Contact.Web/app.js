
/**
 * Module dependencies.
 */

var express = require('express');
//var routes = require('./routes/routes.js');


var http = require('http');
var path = require('path');

var port = process.env.PORT || 80;


var app = express();

// all environments
app.set('port', port);
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');
app.use(express.favicon());
app.use(express.logger('dev'));
app.use(express.json());
app.use(express.urlencoded());
app.use(express.methodOverride());


app.use(express.static(path.join(__dirname, 'public')));




app.use(app.router);
// routes ======================================================================
require('./routes/routes.js')(app); // load our routes and pass in our app and fully configured passport

// development only
if ('development' == app.get('env')) {
    app.use(express.errorHandler());
    app.locals.pretty = true;
}


http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});
