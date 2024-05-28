const importPlugin = require('eslint-plugin-import');
const tseslint = require('typescript-eslint');
const { FlatCompat } = require('@eslint/eslintrc');

const compat = new FlatCompat({
    baseDirectory: __dirname,
});

module.exports = tseslint.config(
    {
        ignores: [
            'node_modules/**',
            'dest/**',
            'eslint.config.js',
        ],
    },
    ...compat.extends('eslint-config-airbnb-typescript/base'),
    {
        languageOptions: {
            parser: tseslint.parser,
            parserOptions: {
                project: true,
            },
        },
        plugins: {
            import: importPlugin,
        },
        rules: {
        },
    },
);