import { createRouter, createWebHistory } from 'vue-router';
import AccountingLayout from '@/views/AccountingLayout.vue';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      // 最外層 layout
      path: '/',
      name: 'layout',
      component: AccountingLayout,
      children: [
        {
          // 主頁登入畫面
          path: '',
          name: 'home',
          redirect: { name: 'login' },
          component: () => import('@/views/Home.vue'),
          children: [
            {
              // 登入
              path: '/Login',
              name: 'login',
              component: () => import('@/views/Login.vue'),
            },
            {
              // 註冊
              path: '/CreateAccount',
              name: 'createaccount',
              component: () => import('@/views/CreateAccount.vue'),
            },
          ],
        },
        {
          // 記帳主畫面
          path: 'accounting-practice',
          name: 'accounting-practice',
          component: () => import('@/views/accountingPractice.vue'),
        },
        {
          // 新增帳本
          path: 'add-ledger',
          name: 'add-ledger',
          component: () => import('@/views/CommandLedger.vue'),
        },
        {
          // 編輯帳本
          path: 'edit-ledger/:id',
          name: 'edit-ledger',
          component: () => import('@/views/CommandLedger.vue'),
        },
      ],
    },
  ],
});

export default router;
