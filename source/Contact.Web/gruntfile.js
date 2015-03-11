
module.exports = function(grunt) {
  grunt.initConfig({
    compass: {                  // Task
      dist: {                   // Target
        options: {              // Target options
          sassDir: 'public/stylesheets',
          cssDir: 'public/stylesheets',
        }
      },
    }
  });
  
  grunt.loadNpmTasks('grunt-contrib-compass');
  
  grunt.registerTask('default', ['compass']);
  
};
