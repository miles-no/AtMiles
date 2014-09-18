
/*
 * GET home page.
 */

module.exports = function (app, passport) {
    
    // route for home page
    app.get('/', function (req, res) {
        res.redirect("/miles"); // defaults to /miles 
    });
    app.get('/:company', function (req, res) {
        res.render('index', { company: req.params.company, redirectUrl: req.url, notAuthenticated: !req.isAuthenticated() }); 
    });
    
    // route for logging out
    app.get('/logout', function (req, res) {
        req.logout();
        res.redirect('/');
    });
    
    
    // =====================================
    // GOOGLE ROUTES =======================
    // =====================================
    // send to google to do the authentication
    // profile gets us their basic information including their name
    // email gets their emails
    app.get('/:company/auth/google',  function(req, res, next) {
        req.session.redirect = req.query.redirect;
        req.session.company = req.params.company;
        next();
    }, passport.authenticate('google', { scope: ['profile', 'email'], accessType: 'offline', approvalPrompt: 'force' }));
    
    // the callback after google has authenticated the user

    app.get('/auth/google/callback', passport.authenticate('google',
        { failureRedirect: '/' }
    ), function(req, res) {
        res.redirect(req.session.redirect || '/');
        
        delete req.session.redirect;
    });

  
};

// route middleware to make sure a user is logged in
function isLoggedIn(req, res, next) {
    
    // if user is authenticated in the session, carry on
    if (req.isAuthenticated())
        return next();
    
    // if they aren't redirect them to the home page
    res.redirect('/');
};
