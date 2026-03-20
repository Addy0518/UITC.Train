import './assets/main.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router'; // 引用剛剛導出的路由

const app = createApp(App);

app.use(createPinia());
app.use(router); // 使用他

app.mount('#app');
