<script setup>
import { computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getProduct } from '@/api/account-api';
import { ref } from 'vue';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   route : 獲取路由資訊
   product : 商品資訊
*/
const route = useRoute();
const product = ref(null);
const baseUrl = 'https://localhost:7124';

/*
   查看商品細節資訊
*/
const getProductDetail = async (id) => {
  var res = await getProduct(id);
  const { data } = res;
  if (data.codeStatus === 2000) {
    product.value = data.returnData;
  }
};

/*
   初始化時從 url 拿取 商品 ID
*/
onMounted(() => {
  getProductDetail(route.params.id);
});

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (img) => {
  if (img && img.productsImgs) {
    return `${baseUrl}/ProductsImg/${img.productsImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full" v-if="product">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="justify-between flex flex-col">
          <div v-for="img in product.productsImgs">
            <img :src="getProductsImg(img)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
          </div>
          <span class="mt-3">{{ product.productsName }}</span>
          <span class="mt-3">{{ product.productsPrice }}</span>
          <div class="flex gap-2 mt-3">
            <span
              v-for="cat in product.productCategoryName?.split(',')"
              :key="cat"
              class="bg-gray-100 px-2 py-1 rounded text-sm"
            >
              {{ cat }}
            </span>
          </div>
          <div class="grid grid-cols-4 mt-5"></div>
        </div>
      </div>
    </div>
  </div>
</template>
