<script setup>
import { getSellerAllProduct, deleteProducts } from '@/api/productsService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

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
    const request = {
      pageIndex: 0,
      pageSize: 10,
    };
    const res = await getSellerAllProduct(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allproduct.value = data.returnData.products;
    }
  } catch (err) {
    console.log(err);
  } finally {
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
  軟刪除
*/
const deleteProduct = async (productId) => {
  try {
    const res = await deleteProducts(productId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('成功加入回收桶!');
      await getSellerProduct();
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allproduct">
    <!-- #region  標題列-->
    <div class="flex items-center justify-between mb-4">
      <p class="text-2xl font-bold m-0">商品管理</p>
      <button
        class="bg-orange-500 hover:bg-orange-600 text-white px-5 py-2 rounded-lg text-sm cursor-pointer"
        @click="router.push({ name: 'add-product' })"
      >
        + 新增商品
      </button>
    </div>
    <!-- #endregion -->
    <!-- #region  欄位標頭-->
    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <div
        class="grid grid-cols-[80px_1fr_100px_100px_120px_160px] px-5 py-2.5 bg-gray-50 border-b border-gray-100"
      >
        <span class="text-xs text-gray-400">圖片</span>
        <span class="text-xs text-gray-400">商品名稱</span>
        <span class="text-xs text-gray-400">價格</span>
        <span class="text-xs text-gray-400">類別</span>
        <span class="text-xs text-gray-400">庫存</span>
        <span class="text-xs text-gray-400 text-right">操作</span>
      </div>
      <!-- #endregion -->
      <!-- #region  商品-->
      <div
        v-for="product in allproduct"
        :key="product.productsId"
        class="grid grid-cols-[80px_1fr_100px_100px_120px_160px] px-5 py-4 border-b border-gray-100 items-center hover:bg-gray-50"
      >
        <img
          :src="getProductsImg(product)"
          class="w-14 h-14 object-cover rounded-lg border border-gray-100 cursor-pointer"
          @click="router.push({ name: 'product-detail', params: { id: product.productsId } })"
        />
        <span class="text-sm font-medium">{{ product.productsName }}</span>
        <span class="text-sm text-orange-500 font-medium">$ {{ product.productsPrice }}</span>
        <span class="text-sm text-gray-400">{{ product.productCategoryName }}</span>
        <span class="text-sm">{{ product.productsStock }} 件</span>
        <div class="flex gap-2 justify-end">
          <button
            class="px-3 py-1.5 border border-gray-200 rounded-lg text-xs cursor-pointer hover:bg-gray-50"
            @click="router.push({ name: 'edit-product', params: { id: product.productsId } })"
          >
            編輯
          </button>
          <button
            class="px-3 py-1.5 border border-red-200 rounded-lg text-xs text-red-500 cursor-pointer hover:bg-red-50"
            @click="deleteProduct(product.productsId)"
          >
            刪除
          </button>
        </div>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
