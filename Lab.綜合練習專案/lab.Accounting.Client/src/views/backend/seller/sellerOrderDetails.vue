<script setup>
import { getSellerOneOrder, updateShippingStatus } from '@/api/orderService';
import defaultImgurl from '@/img/預設圖片.jpg';

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
    const res = await getSellerOneOrder(id);
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

/*
  改變運輸狀態
*/
const updateStatus = async (id, status) => {
  try {
    const res = await updateShippingStatus(id, status);
    console.log('res', res);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('成功更新運輸狀態!');
      await getSellerProduct();
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6 mt-10" v-if="order">
    <div class="max-w-4xl mx-auto w-full">
      <div class="bg-page-bg rounded-card border border-border-soft overflow-hidden">
        <!-- #region  訂單編號 / 購買商品資訊-->
        <div class="px-6 py-4 border-b border-border-soft flex items-center justify-between">
          <div>
            <p class="text-xs text-ink-500 m-0">訂單編號</p>
            <p class="text-sm font-medium mt-1 m-0 text-ink-900">{{ order.orderNumber }}</p>
          </div>
          <span
            class="text-xs px-3 py-1 rounded-card"
            :class="{
              'bg-status-warning/10 text-status-warning': order.shippingStatus === 0,
              'bg-status-info/10 text-status-info': order.shippingStatus === 1,
              'bg-brand-50 text-brand-500': order.shippingStatus === 2,
              'bg-status-success/10 text-status-success': order.shippingStatus === 3,
            }"
          >
            {{ getEnumDescription(shippingEnum, order.shippingStatus) }}
          </span>
        </div>

        <div class="px-6 py-5 border-b border-border-soft flex gap-5">
          <img
            :src="getProductsImg(order)"
            class="w-20 h-20 rounded-card object-cover border border-border-soft"
          />
          <div class="flex-1">
            <p class="font-medium text-base m-0 mb-1 text-ink-900">{{ order.productsName }}</p>
            <p class="text-sm text-ink-500 m-0 mb-2">數量：{{ order.boughtQuantity }} 件</p>
            <p class="text-base font-medium text-brand-price m-0">$ {{ order.accountAmount }}</p>
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  訂單資訊-->
        <div class="px-6 py-5 border-b border-border-soft">
          <p class="text-xs font-medium text-ink-500 mb-3">訂單資訊</p>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">購買人</p>
              <p class="text-sm m-0 text-ink-900">{{ order.userName }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">購買時間</p>
              <p class="text-sm m-0 text-ink-900">{{ formatDateTimeString(order.paidTime) }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">付款方式</p>
              <p class="text-sm m-0 text-ink-900">{{ order.paidType }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">寄送地址</p>
              <p class="text-sm m-0 text-ink-900">{{ order.shippingAddress }}</p>
            </div>
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  更新狀態 / 總金額-->
        <div class="px-6 py-5 flex items-center justify-between">
          <div class="flex items-center gap-3">
            <p class="text-sm text-ink-500 m-0">更新運送狀態</p>
            <select
              class="border border-border-soft rounded-card px-3 py-1.5 text-sm cursor-pointer text-ink-900"
              :value="order.shippingStatus"
              @change="updateStatus(order.orderId, Number($event.target.value))"
            >
              <option v-for="status in shippingEnum" :key="status.value" :value="status.value">
                {{ status.description }}
              </option>
            </select>
          </div>
          <div class="text-right">
            <p class="text-xs text-ink-300 m-0 mb-1">訂單金額</p>
            <p class="text-xl font-medium text-brand-price m-0">$ {{ order.accountAmount }}</p>
          </div>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
