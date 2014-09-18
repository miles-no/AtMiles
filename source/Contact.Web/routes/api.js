var http = require('http');

module.exports = function (app, passport) {
    
    // route for home page. Defaults to /miles
    app.get('/:company/api/search/fulltext', function (req, res) {
        
        var query = req.query.query;
        var company = req.params.company;
        var take = req.query.take || 15;
        var skip = req.query.skip || 0;
        
        if (req.session.company != company) {
            res.json({ Error: 'Yo you do not have access to this company' });
            return;
        }
        var resultData = [];
        var path = '/databases/Contact/indexes/EmployeeSearchModelIndex?'+ encodeURIComponent('query= Content:({0}*) AND Company:{1}&pageSize={2}&start={3}'.format(query, company, take, skip));
        console.log(path);
        var options = {
            host: app.rootHost,
            port: 8080,
            path: path
        }
         http.get(options, function (resp) {
            resp.on('data', function (chunk) {
                resultData.push(chunk);
              
            });
        }).on("error", function (e) {
            console.log(e);
            res.json({Error: e, Results: [], TotalResults :0 });

        }).on('end', function () {
            
            var response = JSON.parse(resultData.join(''));
            res.json(response);
        });
    });


    app.get('/:company/api/employee/:employeeId', function (req, res) {
        
        var company = req.params.company;
        var employeeId = req.params.employeeId;
        var resultData = [];
        if (req.session.company != company) {
            res.json({ Error: 'Yo you do not have access to this company.' });
            return;
        }
        
        var path = '/databases/Contact/docs/'+employeeId;
        console.log(path);
        var options = {
            host: app.rootHost,
            port: 8080,
            path: path
        }
        http.get(options, function (resp) {
            resp.on('data', function (chunk) {
                console.log('result from search' + chunk);
                resultData.push(chunk);
              
            });
        }).on("error", function (e) {
            console.log(e);
            res.json({ Error: e });

        }).on('end', function () {
            
            var response = JSON.parse(resultData.join(''));
            res.json(response);
        });
    });
     

};

String.prototype.format = function () {
    var s = this,
        i = arguments.length;
    
    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};