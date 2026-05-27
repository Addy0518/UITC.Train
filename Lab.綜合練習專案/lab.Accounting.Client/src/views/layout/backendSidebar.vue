<script setup></script>

<template>
  <div class="w-80">
    <!-- #region  賣家後台區-->
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
    <!-- #endregion -->
  </div>
</template>
