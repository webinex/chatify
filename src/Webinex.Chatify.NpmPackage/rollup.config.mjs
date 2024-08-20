import resolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import typescript from '@rollup/plugin-typescript';
import dts from 'rollup-plugin-dts';
import { terser } from 'rollup-plugin-terser';
import peerDepsExternal from 'rollup-plugin-peer-deps-external';
import image from '@rollup/plugin-image';
import postcss from 'rollup-plugin-postcss';
import eslint from '@rollup/plugin-eslint';
import scss from 'rollup-plugin-scss';
import { visualizer } from 'rollup-plugin-visualizer';
import watchGlobs from 'rollup-plugin-watch-globs';

const ANALYZE_BUNDLE = false;

/** @type {Array.<import('rollup').RollupOptions>} */
const config = [
  {
    input: 'src/index.ts',
    output: [
      {
        file: 'dist/esm/index.js',
        format: 'esm',
        sourcemap: true,
      },
    ],
    plugins: [
      eslint(),
      peerDepsExternal({
        includeDependencies: true,
      }),
      resolve(),
      typescript({
        tsconfig: 'tsconfig.json',
      }),
      commonjs(),
      postcss(),
      scss({
        failOnError: true,
        fileName: 'chatify.css',
      }),
      terser(),
      image(),
    ].concat(ANALYZE_BUNDLE ? [visualizer] : []),
  },
  {
    input: './dist/esm/types/index.d.ts',
    output: [{ file: 'dist/index.d.ts', format: 'cjs' }],
    plugins: [dts({})],
    onwarn: (warning, handler) => {
      if (warning.code !== 'CIRCULAR_DEPENDENCY') {
        handler(warning);
      } else {
        console.warn(`(!) ${warning.message}`);
      }
    },
  },
  {
    input: 'src/styles/index.scss',
    output: { file: 'dist/css/chatify.css' },
    onwarn: (warning, handler) => {
      if (warning.code !== 'FILE_NAME_CONFLICT' && warning.code !== 'EMPTY_BUNDLE') {
        handler(warning);
      }
    },
    plugins: [
      watchGlobs(['src/styles/**/*.scss']),
      scss({
        failOnError: true,
        fileName: 'chatify.css',
        include: 'src/styles/**/*',
      }),
    ],
  },
];

export default config;
