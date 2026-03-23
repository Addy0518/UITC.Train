import './assets/main.css';

import { createApp, inject } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router'; // 引用剛剛導出的路由

const app = createApp(App);

app.provide('global', 'hello injections');
app.use(createPinia());
app.use(router); // 使用他
// 路由守衛 , to 代表要去的網址 , from 則是來自哪個網址
router.beforeEach((to, from) => {
  if (to.meta.required) {
    console.log('可以登入');
    return { path: '/:pathMatch(.*)*' };
  } else {
    console.log('不能登入');
  }
});

router.beforeResolve(async (to) => {
  // console.log(`我是beforeResolve=>${to}`);
});
app.mount('#app');
