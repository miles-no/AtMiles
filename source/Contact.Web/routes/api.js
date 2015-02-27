
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
 
 /*
 
 GET _search
{  "fields":["navn","_id","telefon","title","email","office_name"],

   "query": {
      "query_string": {
            "query": "Eurelia~ AND Stian~",
            "fields": ["navn",
            "key_qualifications.int_long_description",
            "key_qualifications.key_points.int_long_description",
            "key_qualifications.key_points.local_long_description",
            "key_qualifications.local_long_description",
            "technologies.int_tags",
            "technologies.local_tags",
            "technologies.technology_skills.tags.no",
            "technologies.technology_skills.tags.int",
            //"key_qualifications.int_long_description",
            "place_of_residence"]
            
        }
    
   }
}
 
 */
module.exports = function(app, esClient, config) {

    // route for home page
    app.get('/api/importCvPartner', function(req, res) {
      console.log(config);
    
      tokenGet(config.cvPartnerBaseUrl, config.cvPartnerUsersUrl,config.cvPartnerToken)
              .then(function(body){
                  var allUsers = JSON.parse(body);
                  var dic = {};
                  for (var i = 0; i < allUsers.length; i++) {
                    var user = allUsers[i];
                    dic[user.email] = user;
                //    console.log(user.user_id);
                    tokenGet(config.cvPartnerBaseUrl, config.cvPartnerCvUrl + user.user_id + "/" + user.default_cv_id,config.cvPartnerToken)
                             .then(function(cvData){
                                var cv = JSON.parse(cvData);
                                
                    //            console.log(cv._id);
                                var add = dic[cv.email];
                                if (add){
                                  console.log(add.office_name);
                                  cv.office_name = add.office_name;
                                  cv.office_id = add.office_id;
                                }
                                else{console.log('no add:(')}
                                
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
        var query = req.query.query;
        var phrases = query.match(/(["'])(?:(?=(\\?))\2.)*?\1/g);
        console.log('phrases: ');
        console.log(phrases);
        
        var searchString = '';
        
        if (phrases){
        for (var i = 0; i < phrases.length; i++) {
          
          if (searchString){
            searchString += " AND ";
          }
          
          query = query.replace(phrases[i],'');
          searchString += ' '+ phrases[i]   ;
          
        }}
        
        var terms = query.split(' ');
        console.log('terms');
        console.log(terms);
        
        for (var i = 0; i < terms.length; i++) {
          var term = terms[i];
          if (term){
            if (searchString){
              searchString += " AND ";
            } 
            searchString += "(" + term + "^3 OR "+ term + "~0.7 )";
          }
        }
        
    
        
        console.log('searchstring ' + searchString);
        
        esClient.search({
          index: config.companyId,
          type: 'cv',
          defaultOperator:'AND',
          "fields":["navn","_id","telefon","title","email","office_name","office_id"],

          body:{query: {
             query_string: {
                   "query":  searchString, 
                   "allow_leading_wildcard": true, 
                   "fields": ["navn^5",
                   "key_qualifications.int_long_description",
                   "key_qualifications.key_points.int_long_description",
                   "key_qualifications.key_points.local_long_description",
                   "key_qualifications.local_long_description",
                   "technologies.int_tags",
                   "technologies.local_tags",
                   "technologies.technology_skills.tags.no",
                   "technologies.technology_skills.tags.int",
                   //"key_qualifications.int_long_description",
                   "office_name",
                   "telefon"]
                   
               }
           
          }
        }
          
        }).then(function (resp) {
          //  console.log(resp);
            var hits = resp.hits.hits;
            res.json(hits);
        }, function (err) {
            console.trace(err.message);
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