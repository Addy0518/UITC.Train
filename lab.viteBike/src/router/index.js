import { createRouter, createWebHistory } from 'vue-router';
import BikeDataTable from '@/views/BikeDataTable.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'bike',
      component: BikeDataTable,
    },
  ],
});

export default router;
