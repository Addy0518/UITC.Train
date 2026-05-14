<script setup>
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   baseUrl : 基底位址
   authStore : localstorage
*/

const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();

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
      <RouterLink
        :to="{ name: 'seller-product' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-plus px-5 ps-5"></i>商品管理</RouterLink
      >

      <RouterLink
        :to="{ name: 'add-product' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-book px-5 ps-5"></i>新增商品</RouterLink
      >

      <RouterLink
        :to="{ name: 'seller-product-order' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-wallet px-5 ps-5"></i>訂單管理</RouterLink
      >
      <RouterLink
        :to="{ name: 'seller-product-recycling' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-wallet px-5 ps-5"></i>商品回收</RouterLink
      >
    </div>
  </div>
</template>
