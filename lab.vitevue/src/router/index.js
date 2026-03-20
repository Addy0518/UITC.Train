import { createRouter, createWebHistory } from 'vue-router';
import ParantsCompoment from '@/views/ParantsCompoment.vue';
import ChildCompoment from '@/views/ChildCompoment.vue';
import HomeView from '@/views/HomeView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '',
      name: 'home',
      component: HomeView,
      children: [
        {
          path: '/ParantsCompoment',
          name: 'ParantsCompoment',
          component: ParantsCompoment,
        },
        {
          path: '/ChildCompoment',
          name: 'ChildCompoment',
          component: ChildCompoment,
        },
      ],
    },
  ],
});

export default router;
