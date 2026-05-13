<script setup>
import { useAuthStore } from '@/stores/auth';
import { computed, onMounted, ref, watch } from 'vue';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義

   baseUrl : 基底位址
   authStore : localstorage
*/

const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();

onMounted(() => {});

const imgUrl = computed(() => {
  if (authStore.userHeadshot) {
    return `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
});
</script>

/* 側邊攔 */
<template>
  <div class="w-80">
    <div class="w-80 h-full shadow-xl">
      <label class="relative group flex justify-center pt-10 pb-5">
        <!-- 顯示照片 -->
        <img
          :src="imgUrl"
          alt="User Avatar"
          class="w-50 h-50 rounded-full object-cover border-2 border-gray-200 transition-opacity"
        />
      </label>
      <strong class="text-black text-2xl flex justify-center mb-5 mt-5"
        >歡迎 , {{ authStore.userName }} !</strong
      >
      <RouterLink
        :to="{ name: 'profile' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-user px-5 ps-5"></i>用戶中心</RouterLink
      >
      <RouterLink
        :to="{ name: 'mall' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-shop px-5 ps-5"></i>商城</RouterLink
      >
      <RouterLink
        :to="{ name: 'seller-centre' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-shop px-5 ps-5"></i>賣家中心</RouterLink
      >
      <RouterLink
        :to="{ name: 'ledger' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-dollar px-5 ps-5"></i>記帳</RouterLink
      >
      <RouterLink
        :to="{ name: 'add-ledger' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-plus px-5 ps-5"></i>新增帳本</RouterLink
      >

      <RouterLink
        :to="{ name: 'shopping-car' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-shopping-cart px-5 ps-5"></i>購物車</RouterLink
      >
      <RouterLink
        :to="{ name: 'purchase-orders' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-shopping-bag px-5 ps-5"></i>購買紀錄</RouterLink
      >
      <RouterLink
        :to="{ name: 'seller-product-order' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-book px-5 ps-5"></i>訂單管理</RouterLink
      >
      <RouterLink
        :to="{ name: 'ledger-chart' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-wallet px-5 ps-5"></i>帳本統計圖表</RouterLink
      >
    </div>
  </div>
</template>
