<script setup>
import { getAllProduct } from '@/api/productsService';

import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
   變數名稱代表意義
   allproduct : 賣家所有商品
   userInfo : 賣家資訊
   baseUrl : 環境變數裡的圖片基底位址
   router : 控制路由
   selectedCategory : 選擇的類別區塊
*/
const allproduct = ref(null);
const userInfo = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const route = useRoute();
const selectedCategory = ref(null);
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

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
  try {
    showLoading();
    const request = {
      pageIndex: 0,
      pageSize: 10,
      sellerId: route.params.id,
    };
    const res = await getAllProduct(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allproduct.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
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
  根據所有商品類別取出不重複的各個類別
*/
const allCategories = computed(() => {
  if (!allproduct.value) return [];
  return [...new Set(allproduct.value.map((x) => x.productCategoryName))];
});

/*
  再根據類別分區塊
*/
const filterProducts = computed(() => {
  if (!allproduct.value) return [];
  if (!selectedCategory.value) return allproduct.value;
  return allproduct.value.filter((p) => p.productCategoryName === selectedCategory.value);
});
</script>

<template>
  <div class="flex flex-col w-full" v-if="allproduct">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <h2 class="text-2xl">賣場</h2>
        <!-- 類別 tab -->
        <div class="flex gap-2 mb-4">
          <button
            v-for="cat in allCategories"
            :key="cat"
            @click="selectedCategory = selectedCategory === cat ? null : cat"
            class="px-3 py-1 rounded-full text-sm cursor-pointer"
            :class="selectedCategory === cat ? 'bg-black text-white' : 'bg-gray-100'"
          >
            {{ cat }}
          </button>
        </div>
        <div v-for="product in filterProducts">
          <div class="hover:shadow-xl hover:bg-gray-50 h-80 flex flex-row ps-10 items-center">
            <img
              :src="getProductsImg(product)"
              alt="Logo"
              class="w-full max-w-40 max-h-40 mt-4 cursor-pointer"
              @click="router.push({ name: 'product-detail', params: { id: product.productsId } })"
            />
            <span class="mt-3 ms-5 me-5">{{ product.productsName }}</span>
            <span class="mt-3 ms-5 me-5">{{ product.productsPrice }}</span>
            <!-- 依照原始資料篩選出同個商品的類別  -->
            <span class="mt-3 ms-5 me-5">{{ product.productCategoryName }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
