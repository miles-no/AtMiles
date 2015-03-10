
/**
 * Module dependencies.
 */

var express = require('express');
var os = require("os");
var hostname = os.hostname();

console.log(hostname);


yaml = require('js-yaml');
fs   = require('fs');

// download config file if not exists
if (!fs.existsSync('config/config.yaml')){
  console.log('config not found. Downloading')
  var finished = false;
  var location = yaml.safeLoad(fs.readFileSync('config/config_location.yaml', 'utf8')).yamlLocation;
  var request = require('sync-request');
  var res = request('GET', location);
  console.log(res.getBody());
  fs.writeFileSync('config/config.yaml', res.getBody());
}

var config = yaml.safeLoad(fs.readFileSync('config/config.yaml', 'utf8'));
console.log(config);

var elasticsearch = require('elasticsearch');
var client = new elasticsearch.Client({
  host: config.searchUrl,
  log: 'trace'
});


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

/* Auth0*/
var passport = require('passport');

// Session and cookies middlewares to keep user logged in
var cookieParser = require('cookie-parser');
var session = require('express-session');

var Auth0Strategy = require('passport-auth0');

var strategy = new Auth0Strategy({
    domain:       config.auth0Issuer,
    clientID:     config.auth0Audience,
    clientSecret: config.auth0Secret,
    callbackURL:  '/api/callback'
  }, function(accessToken, refreshToken, extraParams, profile, done) {
    // accessToken is the token to call Auth0 API (not needed in the most cases)
    // extraParams.id_token has the JSON Web Token
    // profile has all the information from the user
    return done(null, profile);
  });

passport.use(strategy);

passport.serializeUser(function(user, done) {
  done(null, user);
});

passport.deserializeUser(function(user, done) {
  done(null, user);
});


app.use(cookieParser());
app.use(session({ secret: config.appSecret}));
app.use(passport.initialize());
app.use(passport.session());


app.use(app.router);
// routes ======================================================================

require('./routes/api.js')(app, client, config, passport); // load our routes and pass in our app and fully configured passport
require('./routes/routes.js')(app, client, config); // load our routes and pass in our app and fully configured passport


// development only
if ('development' == app.get('env')) {
    app.use(express.errorHandler());
    app.locals.pretty = true;
}


http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});
