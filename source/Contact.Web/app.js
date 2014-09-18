
/**
 * Module dependencies.
 */

var express = require('express');
//var routes = require('./routes/routes.js');


var http = require('http');
var path = require('path');

var port = process.env.PORT || 80;
var passport = require('passport');
var flash = require('connect-flash');

var cookieParser = require('cookie-parser');
var session = require('express-session');


var app = express();

app.rootHost = 'milescontact.cloudapp.net';
// all environments
app.set('port', port);
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');
app.use(express.favicon());
app.use(express.logger('dev'));
app.use(express.json());
app.use(express.urlencoded());
app.use(express.methodOverride());

app.use(cookieParser()); // read cookies (needed for auth)

// required for passport
sessionStore = new express.session.MemoryStore;
app.use(express.session({ secret: 'humptydumptysatatmiles', store: sessionStore })); // session secret

require('./config/passport')(app, passport);


app.use(express.static(path.join(__dirname, 'public')));



app.use(passport.initialize());
app.use(passport.session()); // persistent login sessions
app.use(flash()); // use connect-flash for flash messages stored in session

app.use(app.router);
// routes ======================================================================
require('./routes/routes.js')(app, passport); // load our routes and pass in our app and fully configured passport
require('./routes/api.js')(app, passport);



// development only
if ('development' == app.get('env')) {
    app.use(express.errorHandler());
    app.locals.pretty = true;
}

// =====================================
// GOOGLE ROUTES =======================
// =====================================
// send to google to do the authentication
// profile gets us their basic information including their name
// email gets their emails
app.get('/auth/google', passport.authenticate('google', { scope : ['profile', 'email'] }));

// the callback after google has authenticated the user
app.get('/auth/google/callback',
            passport.authenticate('google', {
    successRedirect : '/profile',
    failureRedirect : '/'
}));





http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});
