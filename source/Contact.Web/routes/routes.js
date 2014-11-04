
/*
 * GET home page.
 */

module.exports = function(app) {

    // route for home page
    app.get('/', function(req, res) {
        //res.redirect("/miles"); // defaults to /miles
        res.render('index', {redirectUrl: req.url, notAuthenticated: true })
    });

    app.get('/partial/:name', function(req, res) {
        var name = req.params.name;
        res.render(name);
    });
    /*app.get('/login', function(req, res) {
        res.render('login', { redirectUrl: req.url, notAuthenticated: true });
    });
    app.get('/search', function(req, res) {
        res.render('search', { redirectUrl: req.url, notAuthenticated: true });
    });*/

    // route for logging out
    app.get('/logout', function(req, res) {
        req.logout();
        res.redirect('/');
    });


};