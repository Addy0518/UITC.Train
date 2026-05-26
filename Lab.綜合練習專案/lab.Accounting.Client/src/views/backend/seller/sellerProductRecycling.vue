<script setup>
import {
  getSellerAllProduct,
  deleteProducts,
  updateProductsDeleteStatus,
} from '@/api/productsService';
import { isDeleteEnum } from '@/common/enum';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import Swal from 'sweetalert2';

/*
   變數名稱代表意義
   allproduct : 賣家所有商品
   baseUrl : 環境變數裡的圖片基底位址
   selectIds : 選取的商品
*/
const allproduct = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const selectIds = ref([]);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化拿到所有回收桶資料( isDelete === true 的)
*/
onMounted(() => {
  getSellerProduct();
});

/*
  拿到所有回收桶資料 ( isDelete === true 的)
*/
const getSellerProduct = async () => {
  try {
    const request = {
      pageIndex: 0,
      pageSize: 10,
      isDelete: isDeleteEnum.Delete.value,
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
  把後端傳回類別分開
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
};

/*
  硬刪除
*/
const deleteProduct = async () => {
  const result = await Swal.fire({
    title: '確定要刪除商品嗎？',
    text: '刪除後將無法復原！',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: '確定刪除',
    cancelButtonText: '取消',
  });

  if (result.isConfirmed) {
    try {
      for (const select of selectIds.value) {
        const res = await deleteProducts(select);
        if (res.data.codeStatus !== 2000) {
          showToastError('部分商品刪除失敗');
          return;
        }
      }
      showToastSuccess('已成功刪除!');
      selectIds.value = [];
      await getSellerProduct(0, 10, isDeleteEnum.Delete.value);
    } catch (err) {
      console.log(err);
    } finally {
    }
  }
};

/*
  復原選取商品
*/
const rollbackAll = async (target) => {
  // 確保進來的商品 ID 是陣列 ( Array ) , 再用解構把她轉成 js 的陣列 ( 正常要這樣 => [1,2]] ), 確保後端能接收到
  const productsId = Array.isArray(target) ? [...target] : [target];
  try {
    if (productsId.length === 0) {
      showToastError('請先勾選商品!');
      return;
    }
    const res = await updateProductsDeleteStatus(productsId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      selectIds.value = [];

      showToastSuccess('商品已復原');
      await getSellerProduct(0, 10, true);
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6">
    <!-- #region  標題列-->
    <div class="flex items-center justify-between mb-4">
      <p class="text-lg font-medium m-0">商品回收桶</p>
      <div class="flex gap-2" v-if="selectIds.length > 0">
        <button
          class="px-4 py-2 border border-gray-200 rounded-lg text-sm cursor-pointer hover:bg-gray-50"
          @click="rollbackAll(selectIds)"
        >
          復原商品
        </button>
        <button
          class="px-4 py-2 border border-red-200 rounded-lg text-sm text-red-500 cursor-pointer hover:bg-red-50"
          @click="deleteProduct()"
        >
          刪除商品
        </button>
      </div>
    </div>
    <!-- #endregion -->

    <!-- #region  所有商品-->
    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <!-- #region  欄位刊頭-->
      <div
        class="grid grid-cols-[40px_80px_1fr_100px_120px] px-5 py-2.5 bg-gray-50 border-b border-gray-100"
      >
        <span></span>
        <span class="text-xs text-gray-400">圖片</span>
        <span class="text-xs text-gray-400">商品名稱</span>
        <span class="text-xs text-gray-400">價格</span>
        <span class="text-xs text-gray-400">類別</span>
      </div>
      <!-- #endregion -->
      <!-- #region  商品列表-->
      <label
        v-for="product in allproduct"
        :key="product.productsId"
        class="grid grid-cols-[40px_80px_1fr_100px_120px] px-5 py-4 border-b border-gray-100 items-center cursor-pointer hover:bg-gray-50"
        :class="selectIds.includes(product.productsId) ? 'bg-orange-50' : ''"
      >
        <input
          type="checkbox"
          :value="product.productsId"
          v-model="selectIds"
          class="w-4 h-4 cursor-pointer"
        />
        <img
          :src="getProductsImg(product)"
          class="w-14 h-14 object-cover rounded-lg border border-gray-100"
        />
        <span class="text-sm font-medium">{{ product.productsName }}</span>
        <span class="text-sm text-gray-400">$ {{ product.productsPrice }}</span>
        <div class="flex flex-wrap gap-1">
          <span
            v-for="cate in productscategory(product.productCategoryName)"
            :key="cate"
            class="text-xs text-gray-400"
            >{{ cate }}</span
          >
        </div>
      </label>
      <!-- #endregion -->
    </div>
    <!-- #endregion -->

    <!-- #region  已選取提示-->
    <p class="text-xs text-gray-400 mt-3" v-if="selectIds.length > 0">
      已選取 {{ selectIds.length }} 件商品
    </p>
    <!-- #endregion -->
  </div>
</template>
