<script setup>
import { getAllProduct } from '@/api/productsService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import { useRoute } from 'vue-router';

/*
  變數名稱代表意義
  route : 獲取路由資訊
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  baseUrl : 環境變數裡的圖片基底位址
  selectedCategory : 選擇的類別區塊
*/
const route = useRoute();
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
  if (route.params.id) {
    loadproducts(route.params.id);
  }
});

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async (parentId) => {
  try {
    showLoading();

    const res = await getAllProduct({ productCategoryId: parentId });
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData.products;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  根據所有商品類別取出不重複的各個子類別
*/
const allCategories = computed(() => {
  if (!allProducts.value) return [];
  const seen = new Set();
  // 創建新的 Set 來記錄類別 id 跟名稱
  return allProducts.value
    .filter((p) => {
      if (seen.has(p.productCategoryName)) return false;
      seen.add(p.productCategoryName);
      return true;
    })
    .map((p) => ({ id: p.productCategoryId, name: p.productCategoryName }));
});

/*
  再根據類別分區塊
*/
const filterProducts = computed(() => {
  if (!allProducts.value) return [];
  if (!selectedCategory.value) return allProducts.value;
  return allProducts.value.filter((p) => p.productCategoryId === selectedCategory.value);
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
    <div class="flex flex-col items-center">
      <div class="mt-20 w-300 rounded-lg shadow-sm">
        <!-- 類別 + 商品 左右並排 -->
        <div class="flex gap-4">
          <!-- 左邊：類別 -->
          <div class="w-50 shrink-0">
            <span class="text-2xl m-5 font-bold">分類</span>
            <div class="flex flex-col mt-5 gap-2">
              <div v-for="categoryname in allCategories" :key="categoryname.id">
                <span
                  @click="
                    selectedCategory = selectedCategory === categoryname.id ? null : categoryname.id
                  "
                  class="cursor-pointer m-5"
                  >{{ categoryname.name }}</span
                >
              </div>
            </div>
          </div>

          <!-- 右邊：商品 -->
          <div class="flex-1">
            <span class="text-2xl m-5">商品</span>
            <div class="grid grid-cols-3 mt-5 gap-4">
              <div v-for="product in filterProducts" :key="product.productsId">
                <div
                  class="hover:shadow-xl hover:bg-gray-50 flex flex-col items-center rounded-lg p-3"
                >
                  <RouterLink
                    :to="{ name: 'product-detail', params: { id: product.productsId } }"
                    class="flex flex-col items-center cursor-pointer w-full"
                  >
                    <img
                      :src="getProductsImg(product)"
                      alt="Logo"
                      class="w-full max-w-40 max-h-40 mt-4 object-cover"
                    />
                    <span class="mt-3">{{ product.productsName }}</span>
                    <span class="mt-3">{{ product.productsPrice }}</span>
                    <span class="mt-3 ms-2 me-2 text-sm text-gray-500">{{
                      product.productCategoryName
                    }}</span>
                  </RouterLink>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
