<script setup>
import { getSellerAllProduct, deleteProducts, updateProductsDeleteStatus } from '@/api/account-api';
import { computed, onMounted, inject } from 'vue';
import { ref } from 'vue';
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

onMounted(() => {
  getSellerProduct();
});

/*
  拿到所有回收桶資料 ( isDelete === true 的)
*/
const getSellerProduct = async () => {
  try {
    const res = await getSellerAllProduct(0, 10, true);
    const { data } = res;
    if (data.codeStatus === 2000) {
      allproduct.value = data.returnData;
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
      let res = null;
      for (const select of selectIds.value) {
        res = await deleteProducts(select);
      }
      const { data } = res;
      if (data.codeStatus === 2000) {
        showToastSuccess('已成功刪除!');
        selectIds.value = [];
        await getSellerProduct(0, 10, true);
      }
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
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="flex justify-end">
          <button
            class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
            @click="rollbackAll(selectIds)"
            v-if="selectIds.length > 0"
          >
            復原商品
          </button>
          <button
            class="bg-red-800 text-white p-3 rounded-2xl cursor-pointer font-bold ms-5"
            @click="deleteProduct()"
            v-if="selectIds.length > 0"
          >
            刪除商品
          </button>
        </div>
        <div v-for="product in allproduct" :key="product.productsId">
          <label
            class="hover:shadow-xl hover:bg-gray-50 h-80 flex flex-row ps-10 cursor-pointer items-center"
          >
            <input
              type="checkbox"
              :value="product.productsId"
              v-model="selectIds"
              class="cursor-pointer me-3 w-8 h-8"
            />
            <img :src="getProductsImg(product)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
            <span class="mt-3 ms-5 me-5">{{ product.productsName }}</span>
            <span class="mt-3 ms-5 me-5">{{ product.productsPrice }}</span>
            <!-- 依照原始資料篩選出同個商品的類別  -->
            <div v-for="cate in productscategory(product.productCategoryName)" :key="cate">
              <span class="mt-3 ms-5 me-5">{{ cate }}</span>
            </div>
          </label>
        </div>
      </div>
    </div>
  </div>
</template>
