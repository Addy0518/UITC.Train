<script setup>
import { getAllProduct } from '@/api/productsService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
  變數名稱代表意義
  categorys : 商品類別
  products : 全部商品
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  baseUrl : 環境變數裡的圖片基底位址
  selectedCategory : 選擇的類別區塊
*/
const allProducts = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const selectedCategory = ref(null);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化加載所有商品
*/
onMounted(() => {
  loadproducts();
});

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async () => {
  try {
    showLoading();
    
    const res = await getAllProduct();
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  根據所有商品類別取出不重複的各個類別
*/
const allCategories = computed(() => {
  if (!allProducts.value) return [];
  return [...new Set(allProducts.value.map((x) => x.productCategoryName))];
});

/*
  再根據類別分區塊
*/
const filterProducts = computed(() => {
  if (!allProducts.value) return [];
  if (!selectedCategory.value) return allProducts.value;
  return allProducts.value.filter((p) => p.productCategoryName === selectedCategory.value);
});

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImgs && product.productsImgs.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImgs[0].productsImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <span class="text-2xl m-5">分類</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="categoryname in allCategories">
              <div
                @click="selectedCategory = selectedCategory === categoryname ? null : categoryname"
                class="hover:shadow-xl hover:bg-gray-50 h-80 cursor-pointer flex flex-col items-center"
              >
                <img :src="defaultImgurl" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
                <span class="mt-15">{{ categoryname }}</span>
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
            <div v-for="product in filterProducts">
              <div class="hover:shadow-xl hover:bg-gray-50 h-100 flex flex-col items-center">
                <RouterLink
                  :to="{ name: 'product-detail', params: { id: product.productsId } }"
                  class="flex flex-col items-center cursor-pointer"
                >
                  <img
                    :src="getProductsImg(product)"
                    alt="Logo"
                    class="w-full max-w-40 max-h-40 mt-4"
                  />
                  <span class="mt-3">{{ product.productsName }}</span>
                  <span class="mt-3">{{ product.productsPrice }}</span>

                  <span class="mt-3 ms-2 me-2 text-sm text-gray-500">
                    {{ product.productCategoryName }}
                  </span>
                </RouterLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
