import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite';
import vue from '@vitejs/plugin-vue';
import vueDevTools from 'vite-plugin-vue-devtools';
import AutoImport from 'unplugin-auto-import/vite';

// https://vite.dev/config/
export default defineConfig({
  base: '/shopping-frontend/',
  plugins: [
    vue(),
    vueDevTools(),
    tailwindcss(),
    // 自動 import , 包含 vue 本身還有像是我自訂的 commom 資料夾內部的東西 , 直接全域 import 了
    // 終端機輸入  npm i -D unplugin-auto-import
    AutoImport({
      imports: [
        'vue',
        'vue-router',
        'pinia',
        {
          '@vuelidate/core': ['useVuelidate'],
        },
      ], // vue 內建
      dirs: ['src/common', 'src/stores', 'src/validator', 'src/api'], // 我自訂的
      vueTemplate: true, // 確保 template 裡的也能自動 import
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
});
