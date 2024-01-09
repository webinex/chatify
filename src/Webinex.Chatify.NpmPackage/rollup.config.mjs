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

const ANALYZE_BUNDLE = false;

import packageJson from './package.json' assert { type: 'json' };

/** @type {Array.<import('rollup').RollupOptions>} */
const config = [
  {
    input: 'src/index.ts',
    output: [
      {
        file: packageJson.module,
        format: 'esm',
        sourcemap: true,
      },
      {
        file: packageJson.main,
        format: 'cjs',
        sourcemap: true,
      },
    ],
    plugins: [
      eslint(),
      peerDepsExternal(),
      resolve(),
      commonjs(),
      typescript({ tsconfig: './tsconfig.json' }),
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
    output: [{ file: 'dist/index.d.ts', format: 'esm' }],
    plugins: [dts()],
  },
  {
    input: 'src/ui/styles/index.scss',
    output: { file: 'dist/css/chatify.css' },
    onwarn: (warning, handler) => {
      if (warning.code !== 'FILE_NAME_CONFLICT' && warning.code !== 'EMPTY_BUNDLE') {
        handler(warning);
      }
    },
    plugins: [
      scss({
        failOnError: true,
        fileName: 'chatify.css',
      }),
    ],
  },
];

export default config;
