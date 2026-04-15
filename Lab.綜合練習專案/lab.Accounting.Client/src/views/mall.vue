<script setup>
import { ref, onMounted, compile, computed, watch } from 'vue';
/*
  變數名稱代表意義
  productscategory : 商品類別
  products : 商品
*/
const categorys = ref(8);
const products = ref(32);

/*
  依據點擊類別篩選商品
*/
const category = (cate) => {
  if (cate !== null) {
    products.value = cate;
  }
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="bg-amber-600 h-100">1221</div>

    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <span class="text-2xl m-5">分類</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="value in categorys">
              <div
                @click="category(value)"
                class="hover:shadow-xl hover:bg-gray-50 h-50 cursor-pointer flex justify-center items-center"
              >
                <img src="@/img/oguri-cap-chibi.png" alt="Logo" class="w-full max-w-40 max-h-40" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-10 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <span class="text-2xl m-5">商品</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="value in products">
              <div
                class="hover:shadow-xl hover:bg-gray-50 h-80 cursor-pointer flex flex-col items-center"
              >
                <img
                  src="@/img/oguri-cap-chibi.png"
                  alt="Logo"
                  class="w-full max-w-40 max-h-40 mt-5"
                />
                <span class="mt-5">商品名稱</span>
                <span class="mt-5">商品價格</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
