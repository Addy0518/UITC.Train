import { createRouter, createWebHistory } from 'vue-router';
import accountingPractice from '@/views/accountingPractice.vue';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'accountung',
      component: accountingPractice,
    },
  ],
});

export default router;
