import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
// import UserView from '@/views/UserView.vue';
import NotFound from '@/views/NotFound.vue';
import UserProfile from '@/views/UserProfile.vue';
// import UserRight from '@/views/UserRight.vue';
const UserRight = () => import('@/views/UserRight.vue');
// 建立一個名為 router 的路由實體
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      // 路徑
      path: '/',
      // 名稱
      name: 'home',
      // 元件
      component: HomeView,
    },
    {
      path: '/User/:id(\\d+)?',
      props: true,
      name: 'UserById',
      component: () => import('@/views/UserView.vue'),
      beforeEnter: (to, from) => {
        console.log('進來home');
      },
      meta: { required: false },
      children: [
        {
          path: '',
          name: 'userprofile',
          components: { Left: UserProfile, Right: UserRight },
        },
      ],
    },
    {
      path: '/User/:Name',
      name: 'UserName',
      component: () => import('@/views/UserView.vue'),
      meta: { required: true },
    },
    { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
  ],
});

// 導出路由來引用
export default router;
