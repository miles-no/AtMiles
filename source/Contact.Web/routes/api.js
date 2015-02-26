
/*
 * GET home page.
 */
 var https = require('https');
 var Promise = require("promise");
 var rp = require('request-promise');
 var fs = require('fs');
 
 var tokenGet = function(host, path, token){
   var options = {
     accept: '*/*',
     uri: host + path,
     headers: {
         'Authorization': 'Token token="' + token + '"'
     }   
   };
  // console.log('GETTING ' + options.uri);
   return rp(options);
   
 }
 
 var download = function(uri, filename, callback){

  rp.head(uri, function(err, res, body){

  var r = rp(uri).pipe(fs.createWriteStream(filename));
    //r.on('close', callback);
    r.on('close', function(){console.log('Donwloaded to ' + filename)});
    r.on('error', function(err){console.log(err);});
  });
};

var getImageTypeFromUri = function(uri){
  var extTmp = uri;
  if (uri.indexOf("?")> 0){
    extTmp = uri.substring(0, uri.indexOf('?'));
  }
  var extTmp = extTmp.substring(uri.lastIndexOf('.'));
  
  //console.log(extTmp);
  return extTmp;
}
 
module.exports = function(app, esClient, config) {

    // route for home page
    app.get('/api/importCvPartner', function(req, res) {
      console.log(config);
    
      tokenGet(config.cvPartnerBaseUrl, config.cvPartnerUsersUrl,config.cvPartnerToken)
              .then(function(body){
                  var allUsers = JSON.parse(body);
                  for (var i = 0; i < allUsers.length; i++) {
                    var user = allUsers[i];
                    tokenGet(config.cvPartnerBaseUrl, config.cvPartnerCvUrl + user.user_id + "/" + user.default_cv_id,config.cvPartnerToken)
                             .then(function(cvData){
                                var cv = JSON.parse(cvData);
                                
                              
                                if (cv.image && cv.image.url){
                                  var largeExt = getImageTypeFromUri(cv.image.url);
                                  cv.imageLargeExt = largeExt;
                                  download(cv.image.url, './data/' + config.companyId + '/thumbs/large/' + cv._id + largeExt, function(){ console.log(cv.image.url + " downloaded")});
                                }
                                if (cv.image && cv.image.small_thumb && cv.image.small_thumb.url){
                                  var smallExt = getImageTypeFromUri(cv.image.small_thumb.url);
                                  cv.imageSmallExt = smallExt;
                                  
                                  download(cv.image.small_thumb.url, './data/' + config.companyId + '/thumbs/small/' + cv._id + smallExt, function(){ console.log(cv.image.url + " downloaded")});
                                }
                                
                                esClient.index({
                                  index: config.companyId,
                                  type: 'cv',
                                  id: cv._id,
                                  body: cv
                                }, function (error, response) {
                                   if (error){
                                     console.log(error);
                                   }
                                   else {
                                     console.log(cv.email + ' indexed');
                                   }

                                  });


                             });
                  }
                  
              })
              .catch(function(reason){
                console.log(reason);
              });
        
    
        return res.json({status:'started'});    
    });
    app.get('/api/search', function(req, res) {
        var query = req.params.query;
        
        esClient.search({
          q: query
        })
        
        .then(function (body) {
          console.log(body);
          return res.json(body.hits.hits);
        }, 
        
        function (error) {
          console.trace(error.message);
        });
        
    });

    // route for logging out
    app.get('api/logout', function(req, res) {
        req.logout();
        res.redirect('/');
    });
    
    // route for logging out
    app.get('api/login', function(req, res) {
        req.logout();
        res.redirect('/');
    });

  


};