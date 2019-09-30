
var gulp = require('gulp');
var sass = require('gulp-sass');
var cssnano = require('gulp-cssnano');

sass.compiler = require('node-sass');

var siteConfig = {
    paths: {
        scss: {
            src: "./scss/**/*.scss",
            dest: "./css"
        },
        minify: {
            src: "./css/*.css",
            dest: "./dist"

        }
    }
}


gulp.task('sass', function () {
    return gulp.src(siteConfig.paths.scss.src)
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(siteConfig.paths.scss.dest))
});

gulp.task('cssminify', done => {
    gulp.src(siteConfig.paths.minify.src)
        .pipe(cssnano())
        .pipe(gulp.dest(siteConfig.paths.minify.dest));
    done();
});

gulp.task('default', gulp.series('sass', 'cssminify'));