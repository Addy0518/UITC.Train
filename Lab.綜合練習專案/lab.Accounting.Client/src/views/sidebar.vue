<script setup>
import { userHeadShot } from '@/api/account-api';
import { useAuthStore } from '@/stores/auth';
import { onMounted, ref, watch } from 'vue';
/*
   變數名稱代表意義
   imgUrl : 大頭照圖片路徑
   baseUrl : 基底位址
   authStore : localstorage
*/
let imgUrl = ref();
const baseUrl = 'https://localhost:7124';
const authStore = useAuthStore();

onMounted(() => {
  imgUrl.value = `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
});

/*
   上傳檔案 ( 大頭照 ) 並在前端顯示
*/
const uploadFile = async (event) => {
  const file = event.target.files[0];
  if (!file) return;

  const formData = new FormData();
  formData.append('userFile', file);
  const res = await userHeadShot(formData);
  const { data } = res;
  if (data.codeStatus === 2000) {
    imgUrl.value = `${baseUrl}/UserHeadShot/${data.returnData.userHeadshot}`;
  }
};
</script>

/* 側邊攔 */
<template>
  <div class="w-80">
    <div class="w-80 h-full shadow-xl">
      <label class="relative cursor-pointer group flex justify-center pt-10 pb-5">
        <!-- 顯示照片 -->
        <img
          v-if="imgUrl"
          :src="imgUrl"
          alt="User Avatar"
          class="w-50 h-50 rounded-full object-cover border-2 border-gray-200 group-hover:opacity-75 transition-opacity"
        />
        <!-- 預設占位圖 (若無照片時) -->
        <div v-else class="w-50 h-50 rounded-full bg-gray-200 flex items-center justify-center">
          <i class="pi pi-user text-3xl text-gray-400"></i>
        </div>

        <!-- 提示文字 (選用：滑鼠移入時顯示「更換照片」) -->
        <div
          class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity"
        >
          <span class="bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded">更換照片</span>
        </div>

        <!-- 隱藏的檔案輸入框 -->
        <input type="file" @change="uploadFile" accept="image/*" class="hidden" />
      </label>
      <strong class="text-black text-2xl flex justify-center mb-5 mt-5"
        >歡迎 , {{ authStore.userName }} !</strong
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
        :to="{ name: 'accounting-practice' }"
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
        :to="{ name: 'purchase-records' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-trash px-5 ps-5"></i>購買紀錄</RouterLink
      >
      <RouterLink
        :to="{ name: 'chart' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-wallet px-5 ps-5"></i>帳本統計圖表</RouterLink
      >

      <RouterLink
        :to="{ name: 'recycling-ledger' }"
        class="w-auto p-3 text-xl flex align-center items-center rounded-lg hover:bg-gray-200 hover:text-2xl font-mono"
        ><i class="pi pi-trash px-5 ps-5"></i>資源回收桶</RouterLink
      >
    </div>
  </div>
</template>
