<script setup>
import { getSellerOrder } from '@/api//orderService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   allOrders : 所有訂單
   baseUrl : 環境變數裡的圖片基底位址
   tableNow : 目前顯示的購買紀錄
   router : 控制路由
*/
const allOrders = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const tableNow = ref();
const router = useRouter();

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化拿到所有訂單
*/
onMounted(() => {
  getOrders();
});

/*
   篩選訂單狀態
*/
const filtTable = computed(() => {
  if (!allOrders.value) return [];
  return allOrders.value.filter((order) => order.shippingStatus == tableNow.value);
});

/*
   拿到所有訂單
*/
const getOrders = async () => {
  try {
    showLoading();
    const res = await getSellerOrder();
    const { data } = res;
    if (data.codeStatus === 2000) {
      allOrders.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImg && product.productsImg.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="flex flex-col items-center">
      <!-- #region  訂單列表-->
      <div class="mt-8 w-300 rounded-card border border-border-soft bg-page-bg overflow-hidden">
        <!-- #region  Tab 列-->
        <div class="flex border-b border-border-soft">
          <button
            v-for="tab in shippingEnum"
            :key="tab.value"
            @click="tableNow = tab.value"
            class="flex-1 py-3 text-center text-sm transition-colors cursor-pointer"
            :class="
              tableNow === tab.value
                ? 'border-b-2 border-brand-500 text-brand-500 font-medium'
                : 'text-ink-500 hover:text-ink-900'
            "
          >
            {{ tab.description }}
          </button>
        </div>
        <!-- #endregion -->

        <!-- #region  沒有訂單時顯示 -->
        <div
          v-if="filtTable.length === 0"
          class="flex justify-center items-center h-40 text-ink-500"
        >
          目前沒有訂單
        </div>
        <!-- #endregion -->

        <!-- #region  有訂單時顯示-->
        <div
          v-for="order in filtTable"
          :key="order.orderId"
          class="border-b border-border-soft hover:bg-surface-muted transition-colors flex flex-row items-center px-6 py-4 cursor-pointer gap-5"
          @click="router.push({ name: 'seller-orders-details', params: { id: order.orderId } })"
        >
          <img
            :src="getProductsImg(order)"
            alt="商品圖片"
            class="w-16 h-16 object-cover rounded-card border border-border-soft flex-shrink-0"
          />
          <div class="flex-1 grid grid-cols-4 gap-3 items-center">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">訂單編號</p>
              <p class="text-sm text-ink-900 m-0 truncate">{{ order.orderNumber }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">商品名稱</p>
              <p class="text-sm text-ink-900 m-0 truncate">{{ order.productsName }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">購買數量</p>
              <p class="text-sm text-ink-900 m-0">
                {{ order.boughtQuantity }} 件 (單價 ${{ order.unitPrice }})
              </p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">訂單金額</p>
              <p class="text-sm font-medium text-brand-price m-0">$ {{ order.accountAmount }}</p>
            </div>
          </div>
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
