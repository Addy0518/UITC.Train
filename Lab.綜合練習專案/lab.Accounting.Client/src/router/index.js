import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import AccountingLayout from '@/views/accountingLayout.vue';
import Swal from 'sweetalert2';

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
          component: () => import('@/views/home.vue'),
          children: [
            {
              // 登入
              path: '/Login',
              name: 'login',
              // meta 自訂屬性 , 用來讓守衛知道誰要驗證 token
              meta: {
                isPermissionVerification: false,
              },
              component: () => import('@/views/login.vue'),
            },
            {
              // 註冊
              path: '/CreateAccount',
              name: 'createaccount',
              meta: {
                isPermissionVerification: false,
              },
              component: () => import('@/views/createAccount.vue'),
            },
          ],
        },
        {
          // 記帳主畫面
          path: 'accounting-practice',
          name: 'accounting-practice',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/accountingPractice.vue'),
        },
        {
          // 統計圖表
          path: 'chart',
          name: 'chart',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/chart.vue'),
        },

        {
          // 新增帳本
          path: 'add-ledger',
          name: 'add-ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandLedger.vue'),
        },
        {
          // 編輯帳本
          path: 'edit-ledger/:id',
          name: 'edit-ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandLedger.vue'),
        },
        // 404 頁面
        {
          path: '/:pathMatch(.*)*',
          name: 'NotFound',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/notFoundPage.vue'),
        },
      ],
    },
  ],
});

router.beforeEach((to) => {
  const authStore = useAuthStore();
  if (to.meta.isPermissionVerification && !authStore.token) {
    Swal.fire({
      icon: 'error',
      title: '請先登入帳號!',
    });
    return { name: 'login' };
  }
});

export default router;
