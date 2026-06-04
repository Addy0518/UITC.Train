<script setup>
import { useAuthStore } from '@/stores/auth';
import { onMounted } from 'vue';

/*
   變數名稱代表意義
   authStore : pinia
*/
const authStore = useAuthStore();

/*
   初始化拿到角色權限
*/
onMounted(() => {});
</script>

<template>
  <div class="w-80">
    <div class="w-80 h-full shadow-xl">
      <!-- #region  賣家後台區-->
      <template v-if="authStore.userRole == 'Seller'">
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
      </template>
      <!-- #endregion -->

      <!-- #region  管理者後台區-->
      <template v-if="authStore.userRole == 'Admin'">
        <RouterLink
          :to="{ name: 'admin-review' }"
          class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
          ><i class="pi pi-plus px-5 ps-5"></i>審查列表</RouterLink
        >
      </template>
      <!-- #endregion -->
    </div>
  </div>
</template>
