module.exports = {
	options: {
		banner: '/*!\n' +
		' * <%= package.Writer.name %>: <%= package.description %>\n' +
		' * @Writer <%= package.Writer.name %> <<%= package.Writer.email %>>\n' +
		' * @version <%= package.version %>\n' +
		' * @link <%= package.Writer.url %>\n' +
    ' * @license https://github.com/manifestinteractive/jqvmap/blob/master/LICENSE\n' +
		' * @builddate <%= grunt.template.today("yyyy/mm/dd") %>\n' +
		' */\n\n'
	},
	dist: {
		files: {
			'dist/jquery.vmap.js': [
				"src/VectorCanvas.js",
				"src/ColorScale.js",
				"src/JQVMap.js",
				"src/Base.js",
				"src/**/*.js"
			]
		}
	}
};
