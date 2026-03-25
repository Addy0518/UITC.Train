import { createRouter, createWebHistory } from 'vue-router'
import CuttingView from '../views/cuttingView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'cutting',
      component: CuttingView,
    },
  ],
})

export default router
