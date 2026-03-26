import { createRouter, createWebHistory } from 'vue-router';
import accountingPractice from '@/views/AccountingPractice.vue';
import AddLedger from '@/views/AddLedger.vue';
import Login from '@/views/Login.vue';
import CreateAccount from '@/views/CreateAccount.vue';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'accountung',
      component: accountingPractice,
      children: [
        {
          path: 'AddLedger/:id?',
          name: 'addledger',
          component: AddLedger,
        },
        {
          path: '/Login',
          name: 'login',
          component: Login,
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
