<script setup>
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { getSellerAllProduct } from '@/api/account-api';
import { ref } from 'vue';

/*
   變數名稱代表意義
   allproduct : 賣家所有商品
   baseUrl : 環境變數裡的圖片基底位址
   router : 控制路由
*/
const allproduct = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
/*
   初始化時
*/
onMounted(() => {
  getSellerProduct();
});

/*
   查看賣家所有商品
*/
const getSellerProduct = async () => {
  var res = await getSellerAllProduct();
  const { data } = res;

  if (data.codeStatus === 2000) {
    allproduct.value = data.returnData;
    console.log('allproduct', allproduct.value);
  }
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
  把後端傳回類別分開
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div v-for="product in allproduct">
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
              @click="router.push({ name: 'add-product' })"
            >
              新增商品
            </button>
            <button
              class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
              @click="router.push({ name: 'edit-product', params: { id: product.productsId } })"
            >
              編輯商品
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
