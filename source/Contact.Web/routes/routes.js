
/*
 * GET home page.
 */

module.exports = function(app) {

    // route for home page
    app.get('/', function(req, res) {
        res.redirect("/miles"); // defaults to /miles 
    });
    app.get('/test', function(req, res) {
        res.render('test', { company: req.params.company, redirectUrl: req.url, notAuthenticated: true });
    });
    app.get('/:company', function(req, res) {
        res.render('index', { company: req.params.company, redirectUrl: req.url, notAuthenticated: true });
    });

    // route for logging out
    app.get('/logout', function(req, res) {
        req.logout();
        res.redirect('/');
    });


}