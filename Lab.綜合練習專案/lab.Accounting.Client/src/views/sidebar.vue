<script setup>
import { userHeadShot } from '@/api/account-api';
import { useAuthStore } from '@/stores/auth';
import { onMounted, ref, watch } from 'vue';
/*
   變數名稱代表意義
   imgUrl : 大頭照圖片路徑
   baseUrl : 基底位址
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
      <img
        v-if="imgUrl"
        :src="imgUrl"
        alt=""
        style="width: 200px; height: 200px"
        class="container w-50 h-50 mx-auto flex justify-items-center align-items-center"
      />
      <InputText @change="uploadFile" type="file" placeholder="大頭照上傳" class="mx-auto w-50" />
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
