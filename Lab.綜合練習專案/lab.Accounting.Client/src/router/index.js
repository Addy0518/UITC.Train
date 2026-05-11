import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { inject } from 'vue';

const showToastError = inject('showToastError');
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
      component: () => import('@/views/layout/layout.vue'),
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
              path: '/login',
              name: 'login',
              /*
                 meta 自訂屬性 , 用來讓守衛知道誰要驗證 token
              */
              meta: {
                isPermissionVerification: false,
              },
              component: () => import('@/views/auth/login.vue'),
            },
            {
              /*
                 註冊
              */
              path: '/create-account',
              name: 'create-account',
              meta: {
                isPermissionVerification: false,
              },
              component: () => import('@/views/auth/createAccount.vue'),
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
          component: () => import('@/views/layout/sidebar.vue'),
        },
        /*
            帳本區 ================================================================
        */
        {
          /*
            帳本回收桶
          */
          path: 'ledger-recycling',
          name: 'ledger-recycling',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/ledger/ledgerRecycling.vue'),
        },
        {
          /*
            帳本主畫面
          */
          path: 'ledger',
          name: 'ledger',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/ledger/ledger.vue'),
        },
        {
          /*
            帳本統計圖表
          */
          path: 'ledger-chart',
          name: 'ledger-chart',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/ledger/ledgerChart.vue'),
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
          component: () => import('@/views/ledger/ledgerCommand.vue'),
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
          component: () => import('@/views/ledger/ledgerCommand.vue'),
        },
        /*
            商城區 ================================================================
        */
        {
          /*
            商城
          */
          path: 'mall',
          name: 'mall',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/mall/mall.vue'),
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
          component: () => import('@/views/mall/shoppingCar.vue'),
        },
        /*
            商品區 ================================================================
        */
        {
          /*
             商品詳細
          */
          path: 'product-detail/:id',
          name: 'product-detail',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/product/productDetail.vue'),
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
          component: () => import('@/views/seller/sellerProductCommand.vue'),
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
          component: () => import('@/views/seller/sellerProductCommand.vue'),
        },
        {
          /*
            購買紀錄
          */
          path: 'purchase-orders',
          name: 'purchase-orders',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/product/purchaseOrders.vue'),
        },
        {
          /*
            單一購買紀錄
          */
          path: 'purchase-orders-details/:id',
          name: 'purchase-orders-details',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/product/purchaseOrderDetails.vue'),
        },

        /*
            賣家區 ================================================================
        */
        {
          /*
            賣家中心
          */
          path: 'seller-centre',
          name: 'seller-centre',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/seller/sellerCentre.vue'),
        },
        {
          /*
            商品回收桶
          */
          path: 'seller-product-recycling',
          name: 'seller-product-recycling',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/seller/sellerProductRecycling.vue'),
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
    showToastError('請先登入帳號!');
    return { name: 'login' };
  }
});

export default router;
