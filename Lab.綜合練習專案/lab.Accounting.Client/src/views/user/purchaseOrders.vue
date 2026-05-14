<script setup>
import { getUserOrder } from '@/api//orderService';
import { shippingEnum } from '@/common/enum';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
   變數名稱代表意義
   allOrders : 所有訂單
   baseUrl : 環境變數裡的圖片基底位址
   tableNow : 顯示目前頁面狀態
   router : 控制路由
   route : 抓取路由參數
*/
const allOrders = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const tableNow = ref(shippingEnum.PendingShipment.value);
const router = useRouter();
const route = useRoute();

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(() => {
  getUserAllOrder();

  // 交易成功跳轉回來接下來的動作
  if (route.query.status === 'success' && route.query.orderNo) {
    showToastSuccess('付款成功!');
    tableNow.value = shippingEnum.PendingShipment.value;
  }
  // 失敗則回到代付款
  if (route.query.status === 'fail') {
    showToastError('付款失敗，請重新嘗試!');
    tableNow.value = shippingEnum.PendingPayment.value;
  }
});

/*
   顯示目前頁面狀態
*/
const filtTable = computed(() => {
  if (!allOrders.value) return [];
  return allOrders.value.filter((order) => order.shippingStatus == tableNow.value);
});

/*
  查看全部訂單
*/
const getUserAllOrder = async () => {
  try {
    showLoading();
    const res = await getUserOrder();
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
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <!-- Tab 列 -->
        <div class="flex border-b border-gray-200">
          <button
            v-for="tab in shippingEnum"
            :key="tab.value"
            @click="tableNow = tab.value"
            class="flex-1 py-3 text-center text-sm transition-colors cursor-pointer"
            :class="
              tableNow === tab.value
                ? 'border-b-2 border-orange-500 text-orange-500 font-medium'
                : 'text-gray-500 hover:text-gray-700'
            "
          >
            {{ tab.description }}
          </button>
        </div>

        <!-- 訂單列表 -->
        <!-- 沒有訂單時顯示 -->
        <div
          v-if="filtTable.length === 0"
          class="flex justify-center items-center h-40 text-gray-400"
        >
          目前沒有訂單
        </div>
        <!-- 有訂單時顯示 -->
        <div v-for="order in filtTable">
          <div
            class="hover:shadow-xl hover:bg-gray-50 h-80 flex flex-row ps-10 cursor-pointer items-center"
            @click="router.push({ name: 'purchase-orders-details', params: { id: order.orderId } })"
          >
            <img :src="getProductsImg(order)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
            <span class="mt-3 ms-5 me-5">商品名稱 : {{ order.productsName }}</span>
            <span class="mt-3 ms-5 me-5">購買價格 : {{ order.unitPrice }}</span>
            <span class="mt-3 ms-5 me-5">購買數量 : {{ order.boughtQuantity }}</span>
            <span class="mt-3 ms-5 me-5">訂單金額 : ${{ order.accountPrice }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
