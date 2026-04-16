<script setup>
import { useAuthStore } from '@/stores/auth';
import { onMounted, ref, watch } from 'vue';
import { productsImgUpload, productsImgDelete, createProducts } from '@/api/account-api';
import { id } from 'zod/v4/locales';
import Chips from 'primevue/chips';
import { url } from 'zod';
/*
   變數名稱代表意義
   imgUrl : 大頭照圖片路徑
   baseUrl : 基底位址
   authStore : localstorage
*/
let imgs = ref([]);
const baseUrl = 'https://localhost:7124';
const authStore = useAuthStore();
let formData = ref();
const productCategoryName = ref([]);
const productName = ref();
const productPrice = ref();
/*
   新增商品
*/
const createProduct = async () => {
  const createData = {
    productCategoryName: productCategoryName.value,
    productsName: productName.value,
    productsPrice: productPrice.value,
  };

  const res = await createProducts(createData);
  const { data } = res;
  if (data.codeStatus === 2000) {
    for (const file of imgs.value) {
      const fd = new FormData();
      fd.append('productsImgsFiles', file.file);
      fd.append('productId', data.returnData);
      await productsImgUpload(fd);
    }
  }
};

/*
   上傳商品圖片並在前端顯示
*/
const uploadFile = async (event) => {
  const files = Array.from(event.target.files);
  if (files.length === 0) return;

  for (const file of files) {
    const previewUrl = URL.createObjectURL(file);

    imgs.value.push({
      file: file,
      url: previewUrl,
    });
  }

  event.target.value = '';
};

const removeImage = (index) => {
  imgs.value.splice(index, 1);
};
</script>

<template>
  <div>
    <div class="flex flex-wrap gap-4 p-5">
      <!-- 1. 顯示已上傳的圖片預覽 -->
      <div v-for="(img, index) in imgs" :key="index" class="relative w-100 h-100">
        <img :src="img.url" class="w-full h-full object-cover rounded-lg shadow" />
        <!-- 刪除按鈕 -->
        <button
          @click="removeImage(index)"
          class="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs cursor-pointer"
        >
          ✕
        </button>
      </div>

      <!-- 2. 上傳按鈕 (永遠在最後面) -->
      <label
        class="w-100 h-100 border-2 border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center cursor-pointer hover:bg-gray-50 transition"
      >
        <i class="pi pi-plus text-gray-400"></i>
        <span class="text-xs text-gray-400 mt-1">上傳照片</span>
        <input type="file" @change="uploadFile" accept="image/*" class="hidden" multiple />
      </label>
    </div>

    <InputGroup>
      <InputGroupAddon>
        <!-- <i class="pi pi-user"></i> -->
      </InputGroupAddon>
      <InputText v-model="productName" placeholder="商品名稱" />
    </InputGroup>
    <InputGroup>
      <InputGroupAddon>類別</InputGroupAddon>
      <!-- 使用 Chips 元件，它會自動將內容存成陣列 -->
      <Chips v-model="productCategoryName" placeholder="輸入後按 Enter 分類" />
    </InputGroup>
    <InputGroup>
      <InputGroupAddon>
        <!-- <i class="pi pi-user"></i> -->
      </InputGroupAddon>
      <InputNumber v-model="productPrice" placeholder="商品價格" />
      <InputGroupAddon>.00</InputGroupAddon>
    </InputGroup>
    <!-- 按鈕區 -->
    <div class="justify-end flex mt-5">
      <button
        @click="createProduct()"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        儲存
      </button>
    </div>
  </div>
</template>
