<script setup></script>

<template>
  <div class="w-80">
    <div class="w-80 h-full shadow-xl">
      <RouterLink
        :to="{ name: 'accounting-practice' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-home px-5 ps-5"></i>首頁</RouterLink
      >
      <RouterLink
        :to="{ name: 'add-ledger' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-plus px-5 ps-5"></i>新增帳本</RouterLink
      >
      <RouterLink
        :to="{ name: 'chart' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-wallet px-5 ps-5"></i>帳本統計圖表</RouterLink
      >
      <!-- <RouterLink
        :to="{ name: '' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-dollar px-5 ps-5"></i>實時金融資訊</RouterLink
      > -->
      <RouterLink
        :to="{ name: 'recyclingBin' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-trash px-5 ps-5"></i>資源回收桶</RouterLink
      >
    </div>
  </div>
</template>
