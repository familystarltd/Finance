/* Gulp Configuration */
var gulp = require('gulp'), jsuglify = require("gulp-uglify");
/* CSS */
var sourcemaps = require('gulp-sourcemaps'), autoprefixer = require('gulp-autoprefixer'), sass = require('gulp-sass'), cssClean = require('gulp-clean-css'), rimraf = require("rimraf");
/* JS & TS */
var typescript = require('gulp-typescript'), tsProject = typescript.createProject('./Client_Dev/tsconfig.json');
var paths = { webroot: "wwwroot/", scss: "Client_Dev/scss/", devTS: "Client_Dev/App/", appProd: 'wwwroot/App', npmSrc: "/node_modules/", npmLibs: "wwwroot/lib/" };
gulp.task('deploy-packages', function () {
    gulp.src([
        paths.npmSrc + '/@angular/**/bundles/*.js',
        paths.npmSrc + '/angular2-in-memory-web-api/*.js',
        paths.npmSrc + '/rxjs/**/bundles/*.js',
        paths.npmSrc + '/systemjs/dist/*.js',
        paths.npmSrc + '/zone.js/dist/*.js',
        paths.npmSrc + '/core-js/client/*.js',
        paths.npmSrc + '/reflect-metadata/reflect.js',
        paths.npmSrc + '/jquery/dist/*.js',
        "/Client_Dev/systemjs.config.js"
    ]).pipe(gulp.dest('./wwwroot/libs'));
});
gulp.task('build-css', function () {
    return gulp.src(paths.scss + '*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({}).on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(autoprefixer({
            browsers: ['last 3 versions'],
            cascade: false
        }))
        .pipe(cssClean({ compatibility: 'ie8' }))
        .pipe(gulp.dest(paths.webroot + 'css/'));
});
gulp.task('build-ts', function () {
    return gulp.src(paths.devTS + '**/*.ts')
        .pipe(sourcemaps.init())
        .pipe(typescript(tsProject))
        .on('error', function (error) { console.error(error); })
        .pipe(sourcemaps.write())
        .pipe(jsuglify())
        .pipe(gulp.dest(paths.appProd));
});
gulp.task('build-js', function () {
    return gulp.src(paths.devTS + '**/*.js')
        .pipe(sourcemaps.init())
        .on('error', function (error) { console.error(error); })
        .pipe(sourcemaps.write())
        .pipe(jsuglify())
        .pipe(gulp.dest(paths.appProd));
});
gulp.task('bundle-ts', function () {
    var path = require("path");
    var Builder = require('systemjs-builder');
    // optional constructor options
    // sets the baseURL and loads the configuration file
    var builder = new Builder('', './Client_Dev/systemjs.config.js');
    builder.buildStatic('./Client_Dev/App/main.js', 'wwwroot/bundle.js', { minify: true, sourceMaps: true })
        .then(function () {
            console.log('Bundle complete');
        })
        .catch(function (err) {
            console.log('******** Bundle error ********');
            console.log(err);
        });
});
gulp.task('watch', function () {
    gulp.watch(paths.devTS + '**/*.ts', ['build-ts']);
    gulp.watch(paths.scss + '**/*.scss', ['build-css']);
});
gulp.task('default', ['watch', 'build-js', 'build-ts', 'bundle-ts', 'build-css']);