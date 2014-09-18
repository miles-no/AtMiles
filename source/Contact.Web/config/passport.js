// config/passport.js
var NodeCache = require("node-cache");
var cache = new NodeCache();
var http = require('http');

// load all the things we need
//var LocalStrategy = require('passport-local').Strategy;
//var FacebookStrategy = require('passport-facebook').Strategy;
//var TwitterStrategy = require('passport-twitter').Strategy;
var GoogleStrategy = require('passport-google-oauth').OAuth2Strategy;

// load up the user model
//var User = require('../app/models/user');

// load the auth variables
var configAuth = require('./auth');

module.exports = function (passport) {
    
    // used to serialize the user for the session
    passport.serializeUser(function (user, done) {
        cache.set(user.GlobalId, user);
        done(null, user.GlobalId);
    });
    
    // used to deserialize the user
    passport.deserializeUser(function (id, done) {
        var user = cache.get(id);
        done(null, user);
        //User.findById(id, function (err, user) {
        //    done(err, user);
        //});
    });
    
    // code for login (use('local-login', new LocalStategy))
    // code for signup (use('local-signup', new LocalStategy))
    // code for facebook (use('facebook', new FacebookStrategy))
    // code for twitter (use('twitter', new TwitterStrategy))
    
    // =========================================================================
    // GOOGLE ==================================================================
    // =========================================================================
    passport.use(new GoogleStrategy({
        
        clientID        : configAuth.googleAuth.clientID,
        clientSecret    : configAuth.googleAuth.clientSecret,
        callbackURL     : configAuth.googleAuth.callbackURL,
    },
    function (token, refreshToken, profile, done) {

        var company = 'miles';
        var provider = 'Google';
        var id = profile.id;
        // make the code asynchronous
        // User.findOne won't fire until we have all our data back from Google
        process.nextTick(function () {
            var path = '/databases/Contact/indexes/UserLookupIndex?&query=GlobalProviderId%3A' + company + '%2F' + provider + '%2F' + id + '&pageSize=1';
            console.log(path); 
            http.get({
                host: 'milescontact.cloudapp.net',
                port: 8080,
                path: path
            }, function (resp) {
                resp.on('data', function (chunk) {
                    console.log('data:' + chunk);
                    var response = JSON.parse(chunk);
                    if (response.Results.length > 0) {
                        return done(null, response.Results[0]);
                    } else {
                        return done('User doesnt exist', null);
                    }

                });
            }).on("error", function (e) {
                return done(e, null);
                
            });
            
            //TODO: Search raven
           // return done(null, { id: profile.id, email: profile.emails[0].value });

            // try to find the user based on their google id
            //User.findOne({ 'google.id' : profile.id }, function (err, user) {
            //    if (err)
            //        return done(err);
                
            //    if (user) {
                    
            //        // if a user is found, log them in
            //        return done(null, user);
            //    } else {
            //        // if the user isnt in our database, create a new user
            //        var newUser = new User();
                    
            //        // set all of the relevant information
            //        newUser.google.id = profile.id;
            //        newUser.google.token = token;
            //        newUser.google.name = profile.displayName;
            //        newUser.google.email = profile.emails[0].value; // pull the first email
                    
            //        // save the user
            //        newUser.save(function (err) {
            //            if (err)
            //                throw err;
            //            return done(null, newUser);
            //        });
            //    }
            //});
        });

    }));

};
