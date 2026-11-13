import { defineConfig } from 'oxlint';

export default defineConfig({
    categories: {
        correctness: 'error',
        pedantic: 'error',
        perf: 'warn',
        restriction: 'warn',
        style: 'warn',
        suspicious: 'error',
    },
    env: {
        builtin: true,
    },
    plugins: ['typescript', 'unicorn', 'oxc'],
    rules: {
        'func-style': ['warn', 'declaration', { allowArrowFunctions: true }],
        'max-statements': ['warn', 100],
        'no-magic-numbers': 'off',
        'no-ternary': 'off',
        'no-undefined': 'off',
        'one-var': 'off',
        'oxc/no-async-await': 'off',
        radix: ['warn', 'as-needed'],
        'switch-case-braces': ['warn', 'avoid'],
        'typescript/explicit-function-return-type': 'off',
        'unicorn/prefer-ternary': ['warn', 'only-single-line'],
    },
});
