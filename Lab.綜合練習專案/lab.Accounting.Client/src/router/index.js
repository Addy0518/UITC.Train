import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

/*
    路由設定
*/
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      /*
         前台 layout
      */
      path: '/',
      name: 'front-layout',
      component: () => import('@/views/layout/frontLayout.vue'),
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

        /*
            帳本區 ================================================================
        */
        {
          /*
            帳本中心
          */
          path: 'ledger-centre',
          name: 'ledger-centre',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/ledger/ledgerCentre.vue'),
          redirect: { name: 'ledger' },
          children: [
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
                帳本統計圖表
              */
              path: 'ledger-chart',
              name: 'ledger-chart',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/ledger/ledgerChart.vue'),
            },
          ],
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
             商品購買
          */
          path: 'product-bought',
          name: 'product-bought',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/product/productBought.vue'),
        },
        {
          /*
             買家查看的賣場
          */
          path: 'seller-store/:id',
          name: 'seller-store',
          meta: {
            isPermissionVerification: false,
          },
          component: () => import('@/views/product/sellerStore.vue'),
        },
        /*
            用戶區 ================================================================
        */
        {
          /*
            用戶中心
          */
          path: 'user-centre',
          name: 'user-centre',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/user/userCentre.vue'),
          redirect: { name: 'profile' },
          children: [
            {
              /*
                個人檔案
              */
              path: 'profile',
              name: 'profile',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/user/profile.vue'),
            },
            {
              /*
                更改密碼
              */
              path: 'update-password',
              name: 'update-password',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/user/updatePassword.vue'),
            },
            {
              /*
                買家訂單管理
              */
              path: 'purchase-orders',
              name: 'purchase-orders',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/user/purchaseOrders.vue'),
            },
            {
              /*
                買家訂單詳細
              */
              path: 'purchase-orders-details/:id',
              name: 'purchase-orders-details',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/user/purchaseOrderDetails.vue'),
            },
            {
              /*
                買家評價
              */
              path: 'purchaseOrderRate/:id',
              name: 'purchaseOrderRate',
              meta: {
                isPermissionVerification: true,
              },
              component: () => import('@/views/user/purchaseOrderRate.vue'),
            },
          ],
        },
      ],
    },
    {
      /*
         後台 layout
      */
      path: '/backend-layout',
      name: 'backend-layout',
      component: () => import('@/views/layout/backendLayout.vue'),
      redirect: { name: 'seller-product' },
      children: [
        /*
            賣家區 ================================================================
        */
        {
          /*
            後臺側邊欄
          */
          path: 'backend-sidebar',
          name: 'backend-sidebar',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/layout/backendSidebar.vue'),
        },
        {
          /*
            賣家查看的賣場
          */
          path: 'seller-product',
          name: 'seller-product',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/backend/seller/sellerProduct.vue'),
        },
        {
          /*
                新增商品
              */
          path: 'add-product',
          name: 'add-product',
          meta: {
            isPermissionVerification: true,
            isSeller: true,
          },
          component: () => import('@/views/backend/seller/sellerProductCommand.vue'),
        },
        {
          /*
                編輯商品
              */
          path: 'edit-product/:id',
          name: 'edit-product',
          meta: {
            isPermissionVerification: true,
            isSeller: true,
          },
          component: () => import('@/views/backend/seller/sellerProductCommand.vue'),
        },
        {
          /*
                賣家訂單管理
              */
          path: 'seller-product-order',
          name: 'seller-product-order',
          meta: {
            isPermissionVerification: true,
          },
          component: () => import('@/views/backend/seller/sellerProductOrder.vue'),
        },
        {
          /*
                賣家訂單詳細
              */
          path: 'seller-orders-details/:id',
          name: 'seller-orders-details',
          meta: {
            isPermissionVerification: true,
            isSeller: true,
          },
          component: () => import('@/views/backend/seller/sellerOrderDetails.vue'),
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
          component: () => import('@/views/backend/seller/sellerProductRecycling.vue'),
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
    return { name: 'login' };
  }
  if (to.meta.isSeller && authStore.userRole !== 'Seller') {
    return { name: 'mall' };
  }
});

export default router;
