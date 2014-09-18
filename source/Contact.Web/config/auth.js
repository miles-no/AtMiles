module.exports = {
    
    'facebookAuth' : {
        'clientID' 		: 'your-secret-clientID-here', // your App ID
        'clientSecret' 	: 'your-client-secret-here', // your App Secret
        'callbackURL' 	: 'http://localhost:8080/auth/facebook/callback'
    },
    
    'twitterAuth' : {
        'consumerKey' 		: 'your-consumer-key-here',
        'consumerSecret' 	: 'your-client-secret-here',
        'callbackURL' 		: 'http://localhost:8080/auth/twitter/callback'
    },
    
    'googleAuth' : {
        'clientID' 		: '387201482859-4091mlp9nvru7lfhd6mr546hku4gue2q.apps.googleusercontent.com',
        'clientSecret' 	: 'pvJiPJkQTOI6LurhaWPRfyvt',
        'callbackURL' 	: 'http://localhost/auth/google/callback'
    }
};
