<script setup>
import { ref, onMounted, computed, watch, inject } from 'vue';
import { getUserOrder } from '@/api//orderService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import { shippingEnum } from '../common/enum';

/*
   變數名稱代表意義
   orders : 所有訂單
   baseUrl : 環境變數裡的圖片基底位址
   tableNow : 目前顯示的購買紀錄
*/
const allOrders = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const tableNow = ref();

const table = [
  { label: '待付款', status: 0 },
  { label: '待出貨', status: 1 },
  { label: '運送中', status: 2 },
  { label: '已抵達', status: 3 },
];

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(() => {
  getOrders();
});

const filtTable = computed(() => {
  if (!allOrders.value) return [];
  return allOrders.value.filter((order) => order.shippingStatus == tableNow.value);
});

const getOrders = async () => {
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
            v-for="tab in table"
            :key="tab.status"
            @click="tableNow = tab.status"
            class="flex-1 py-3 text-center text-sm transition-colors cursor-pointer"
            :class="
              tableNow === tab.status
                ? 'border-b-2 border-orange-500 text-orange-500 font-medium'
                : 'text-gray-500 hover:text-gray-700'
            "
          >
            {{ tab.label }}
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
          >
            <img :src="getProductsImg(order)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
            <span class="mt-3 ms-5 me-5">訂單金額 : ${{ order.accountPrice }}</span>
            <span class="mt-3 ms-5 me-5">購買數量 : {{ order.boughtQuantity }}</span>
            <span class="mt-3 ms-5 me-5">訂單編號 : {{ order.orderNumber }}</span>
            <span class="mt-3 ms-5 me-5">購買時間 : {{ order.paidTime }}</span>
            <span class="mt-3 ms-5 me-5">付款方式 : {{ order.paidType }}</span>
            <span class="mt-3 ms-5 me-5">寄送地址 : {{ order.shippingAddress }}</span>
            <span class="mt-3 ms-5 me-5">運送狀態 : {{ order.shippingStatus }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
