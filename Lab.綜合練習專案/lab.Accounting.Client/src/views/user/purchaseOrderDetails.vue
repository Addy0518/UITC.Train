<script setup>
import { getUserOneOrder } from '@/api/orderService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
   變數名稱代表意義
   order : 訂單
   baseUrl : 環境變數裡的圖片基底位址
   tableNow : 目前顯示的購買紀錄
   route : 獲取路由資訊
*/

const route = useRoute();
const order = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const tableNow = ref();

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(() => {
  if (route.params.id) {
    getOneOrder(route.params.id);
  }
});

const getOneOrder = async (id) => {
  try {
    showLoading();
    const res = await getUserOneOrder(id);
    const { data } = res;
    if (data.codeStatus === 2000) {
      order.value = data.returnData;

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
      <div class="mt-40 w-300 rounded-lg shadow-sm" v-if="order">
        <img :src="getProductsImg(order)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
        <span class="mt-3 ms-5 me-5">訂單金額 : ${{ order.accountPrice }}</span>
        <span class="mt-3 ms-5 me-5">購買數量 : {{ order.boughtQuantity }}</span>
        <span class="mt-3 ms-5 me-5">訂單編號 : {{ order.orderNumber }}</span>
        <span class="mt-3 ms-5 me-5">購買時間 : {{ order.paidTime }}</span>
        <span class="mt-3 ms-5 me-5">付款方式 : {{ order.paidType }}</span>
        <span class="mt-3 ms-5 me-5">寄送地址 : {{ order.shippingAddress }}</span>
        <span class="mt-3 ms-5 me-5"
          >運送狀態 : {{ getEnumDescription(shippingEnum, order.shippingStatus) }}</span
        >
        <span class="mt-3 ms-5 me-5">商品名稱 : {{ order.productsName }}</span>
      </div>
    </div>
  </div>
</template>
