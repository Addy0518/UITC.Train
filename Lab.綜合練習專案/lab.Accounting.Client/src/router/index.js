import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import AccountingLayout from '@/views/accountingLayout.vue';
import Swal from 'sweetalert2';

/*
    路由設定
*/
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      /*
         最外層 layout
      */
      path: '/',
      name: 'layout',
      component: AccountingLayout,
      children: [
        {
          /*
            主頁 ( 預設導向登入畫面 )
          */
          path: '',
          name: 'home',
          redirect: { name: 'login' },
          component: () => import('@/views/home.vue'),
          children: [
            {
              /*
                 登入
              */
              path: '/Login',
              name: 'login',
              /*
                 meta 自訂屬性 , 用來讓守衛知道誰要驗證 token
              */
              meta: {
                isPermissionVerification: false,
              },
              component: () => import('@/views/login.vue'),
            },
            {
              /*
                 註冊
              */
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
          /*
            側邊欄
          */
          path: 'sidebar',
          name: 'sidebar',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/sidebar.vue'),
        },
        {
          /*
            帳本回收桶
          */
          path: 'recycling-ledger',
          name: 'recycling-ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/recyclingLedger.vue'),
        },
        {
          /*
            帳本主畫面
          */
          path: 'accounting-practice',
          name: 'accounting-practice',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/accountingPractice.vue'),
        },
        {
          /*
            帳本統計圖表
          */
          path: 'chart',
          name: 'chart',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/chart.vue'),
        },

        {
          /*
            新增帳本
          */
          path: 'add-ledger',
          name: 'add-ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandLedger.vue'),
        },
        {
          /*
            編輯帳本
          */
          path: 'edit-ledger/:id',
          name: 'edit-ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandLedger.vue'),
        },
        {
          /*
            商城
          */
          path: 'mall',
          name: 'mall',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/mall.vue'),
        },
        {
          /*
             商品詳細
          */
          path: 'product-detail/:id',
          name: 'product-detail',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/productDetail.vue'),
        },
        {
          /*
            新增商品
          */
          path: 'add-product',
          name: 'add-product',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandProducts.vue'),
        },
        {
          /*
            編輯商品
          */
          path: 'edit-product/:id',
          name: 'edit-product',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/commandProducts.vue'),
        },
         {
          /*
            商品回收桶
          */
          path: 'recycling-products',
          name: 'recycling-products',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/recyclingProducts.vue'),
        },
        {
          /*
            購物車
          */
          path: 'shopping-car',
          name: 'shopping-car',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/shoppingCar.vue'),
        },
        {
          /*
            賣家中心
          */
          path: 'seller-centre',
          name: 'seller-centre',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/sellerCentre.vue'),
        },
      ],
    },
    /*
       404 畫面
    */
    {
      path: '/:pathMatch(.*)*',

      name: 'NotFound',
      meta: {
        isPermissionVerification: false,
      },
      component: () => import('@/views/notFoundPage.vue'),
    },
  ],
});

/*
   路由守衛 ( to 是 目標路由 , from 是來自哪個路由 )
*/
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
