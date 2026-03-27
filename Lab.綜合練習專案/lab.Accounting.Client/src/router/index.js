import { createRouter, createWebHistory } from 'vue-router';
import CreateAccount from '@/views/CreateAccount.vue';
import AccountingLayout from '@/views/AccountingLayout.vue';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'layout',
      component: AccountingLayout,
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/views/Home.vue'),
        },
        {
          path: 'accounting-practice',
          name: 'accounting-practice',
          component: () => import('@/views/AccountingPractice.vue'),
        },
        {
          path: 'add-ledger',
          name: 'add-ledger',
          component: () => import('@/views/CommandLedger.vue'),
        },
        {
          path: 'edit-ledger/:id',
          name: 'edit-ledger',
          component: () => import('@/views/CommandLedger.vue'),
        },
        {
          path: '/Login',
          name: 'login',
          component: () => import('@/views/Login.vue'),
        },
        {
          path: '/CreateAccount',
          name: 'createaccount',
          component: CreateAccount,
        },
      ],
    },
  ],
});

export default router;
