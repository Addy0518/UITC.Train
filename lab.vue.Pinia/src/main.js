import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { toRef, ref } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)
function SecretPiniaPlugin() {
  return { secret: 'the cake is a lie' }
}
const pinia = createPinia()

pinia.use(SecretPiniaPlugin)
pinia.use(({ store }) => {
  store.hello = 'world'

  if(!store.$state.hasOwnProperty('hasError'))
  {
    const hasError=ref(false)

    store.$state.hasError=hasError
  }

  store.hasError=toRef(store.$state,'hasError')
})

app.use(pinia)
app.use(router)

app.mount('#app')
