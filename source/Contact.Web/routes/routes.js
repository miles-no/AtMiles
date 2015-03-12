
/*
 * GET home page.
 */

module.exports = function(app, esClient, config) {

    // route for home page
    app.get('/', function(req, res) {
        //res.redirect("/miles"); // defaults to /miles
        res.render('index', {redirectUrl: req.url, notAuthenticated: true })
    });

    app.get('/partial/:name', function(req, res) {
        var name = req.params.name;
        res.render(name);
    });
  
    // route for logging out
    app.get('/logout', function(req, res) {
        req.logout();
        res.redirect('/');
    });
    
    app.get('/thumb/:size/:user', function(req, res) {
        res.sendfile('./data/' + config.companyId +'/thumbs/' + req.params.size + "/" +req.params.user);
    });
    
    app.get('/vcard/:user', function(req, res) {
        res.setHeader("Content-Type", "text/vcard");;
        res.setHeader('Content-disposition', 'attachment; filename='+req.params.user);
        res.sendfile('./data/'+config.companyId + '/' + req.params.user);
        
    });
    

    app.use(function(req,res) {
       res.render('index');
    });

// 
};