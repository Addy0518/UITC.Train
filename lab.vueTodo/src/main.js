import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import TodoView from './views/TodoView.vue'
import router from './router'

const app = createApp(TodoView)

app.use(createPinia())
app.use(router)

app.mount('#app')
