import { createRouter, createWebHistory } from 'vue-router';
import BikeDataTable from '@/views/BikeDataTable.vue';
import TestView from '@/views/TestView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'bike',
      component: BikeDataTable,
    },
    {
      path: '/test',
      name: 'test',
      component: TestView,
    },
  ],
});

export default router;
