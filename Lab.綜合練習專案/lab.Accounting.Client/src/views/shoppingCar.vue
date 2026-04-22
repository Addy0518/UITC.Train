<script setup>
import { ref, onMounted, compile, computed, watch } from 'vue';
import { getAllProductsInShoppingCar, deleteProductsInShoppingCar } from '@/api/account-api';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import { value } from 'valibot';
/*
  變數名稱代表意義
  allProductsRaw : 初始資料 ( 全部商品 )
  products : 全部商品
  baseUrl : 環境變數裡的圖片基底位址
*/

const products = ref([]);
const allProductsRaw = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;

/*
   初始化時加載購物車商品
*/
onMounted(() => {
  loadproducts();
});

/*
   初始化時加載購物車商品
*/
const loadproducts = async () => {
  const res = await getAllProductsInShoppingCar();
  const { data } = res;

  if (data.codeStatus === 2000) {
    allProductsRaw.value = data.returnData;
    /*
        在解構的陣列 products 裡面再建立一個陣列 [x.productCategoryName, x] , 為 key 跟 value
        用 map 去除重複的 key 再把陣列轉回 values 陣列
    */
    products.value = [...new Map(allProductsRaw.value.map((x) => [x.productsName, x])).values()];
  } else {
    allProductsRaw.value = [];
    products.value = [];
  }
};

/*
  把後端傳回類別分開
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
};

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImgs && product.productsImgs.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImgs[0].productsImg}`;
  }
  return defaultImgurl;
};

/*
  移除購物車
*/
const deleteProductsInCar = async (productId) => {
  await deleteProductsInShoppingCar(productId);
  await loadproducts();
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div v-for="product in products">
          <div
            class="hover:shadow-xl hover:bg-gray-50 h-80 flex flex-row ps-10 cursor-pointer items-center"
          >
            <img :src="getProductsImg(product)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
            <span class="mt-3 ms-5 me-5">{{ product.productsName }}</span>
            <span class="mt-3 ms-5 me-5">{{ product.productsPrice }}</span>
            <!-- 依照原始資料篩選出同個商品的類別  -->
            <div v-for="cate in productscategory(product.productCategoryName)" :key="cate">
              <span class="mt-3 ms-5 me-5">{{ cate }}</span>
            </div>
            <button
              class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
              @click="deleteProductsInCar(product.productsId)"
            >
              移除購物車
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
