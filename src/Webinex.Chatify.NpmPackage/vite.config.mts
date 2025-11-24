import { defineConfig } from 'vite';
import { resolve } from 'node:path';
import react from '@vitejs/plugin-react';
import dts from 'vite-plugin-dts';
// @ts-expect-error no types
import peerDepsExternal from 'rollup-plugin-peer-deps-external';

import { libInjectCss } from 'vite-plugin-lib-inject-css';
import pkg from './package.json';
import { fileURLToPath } from 'node:url';

const __dirname = fileURLToPath(new URL('.', import.meta.url));

export default defineConfig({
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5072/',
        rewriteWsOrigin: true,
      },
    },
  },
  plugins: [
    react(),
    libInjectCss(),
    peerDepsExternal({ includeDependencies: true }),
    dts({
      include: ['src'],
      pathsToAliases: true,
      tsconfigPath: resolve(__dirname, './tsconfig.lib.json'),
      outDir: resolve(__dirname, './dist/types'),
    }),
  ],
  css: {
    preprocessorOptions: {
      scss: {
        api: 'modern-compiler',
      },
    },
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
      '@webinex/chatify': resolve(__dirname, 'src/index.ts'),
    },
  },
  build: {
    copyPublicDir: false,
    minify: false,
    sourcemap: true,
    rollupOptions: {
      external: [...Object.keys(pkg.peerDependencies), ...Object.keys(pkg.devDependencies)],
    },
    lib: {
      entry: { index: resolve(__dirname, 'src/index.ts'), audit: resolve(__dirname, 'src/audit/index.tsx') },
      formats: ['es'],
    },
  },
});
