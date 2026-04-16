<script setup>
import { ref, onMounted, compile, computed, watch } from 'vue';
import {
  getProduct,
  getAllProduct,
  createProducts,
  productsImgUpload,
  productsImgDelete,
} from '@/api/account-api';
/*
  變數名稱代表意義
  productscategory : 商品類別
  products : 商品
*/
const categorys = ref();
const products = ref([]);
const filterproducts = ref([]);

onMounted(() => {
  loadproducts();
});

const loadproducts = async () => {
  const res = await getAllProduct();
  const { data } = res;
  if (data.codeStatus === 2000) {
    products.value = data.returnData;
    filterproducts.value = products.value;
    /*
       在解構的陣列 products 裡面再建立一個陣列 [x.productCategoryName, x] , 為 key 跟 value
       用 map 去除重複的 key 再把陣列轉回 values 陣列
    */
    categorys.value = [...new Map(products.value.map((x) => [x.productCategoryName, x])).values()];
  }
};

/*
  依據點擊類別篩選商品
*/
const categoryFilter = (cate) => {
  console.log('CATE', cate.productCategoryName);
  if (cate !== null) {
    filterproducts.value = products.value.filter(
      (x) => x.productCategoryName === cate.productCategoryName,
    );
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
            <div v-for="category in categorys">
              <div
                @click="categoryFilter(category)"
                class="hover:shadow-xl hover:bg-gray-50 h-80 cursor-pointer flex flex-col items-center"
              >
                <img
                  src="@/img/oguri-cap-chibi.png"
                  alt="Logo"
                  class="w-full max-w-40 max-h-40 mt-4"
                />
                <span class="mt-15">{{ category.productCategoryName }}</span>
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
            <div v-for="product in filterproducts">
              <div
                class="hover:shadow-xl hover:bg-gray-50 h-80 cursor-pointer flex flex-col items-center"
              >
                <img
                  src="@/img/oguri-cap-chibi.png"
                  alt="Logo"
                  class="w-full max-w-40 max-h-40 mt-4"
                />
                <span class="mt-3">{{ product.productsName }}</span>
                <span class="mt-3">{{ product.productsPrice }}</span>
                <span class="mt-3">{{ product.productCategoryName }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
