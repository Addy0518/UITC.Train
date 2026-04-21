<script setup>
import { computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getProduct, userBuyProductAndRate } from '@/api/account-api';
import { ref } from 'vue';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   route : 獲取路由資訊
   product : 商品資訊
   productsId : 商品 ID
   purchaseQuantity : 商品購買數量
   rating : 評分
   comment : 評論
   createTime : 現在時間
*/
const route = useRoute();
const product = ref(null);
const baseUrl = 'https://localhost:7124';
const purchaseQuantity = ref();
const rating = ref();
const comment = ref();
const createTime = Date.now;

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
  if (img && img.productsImg) {
    return `${baseUrl}/ProductsImg/${img.productsImg}`;
  }
  return defaultImgurl;
};

/*
  使用者購買跟評分
*/
const userBuy = async () => {
  const bought = {
    productsId: Number(route.params.id),
    purchaseQuantity: purchaseQuantity.value,
    rating: rating.value,
    comment: comment.value,
    createTime: new Date().toLocaleDateString('en-CA'),
  };

  const res = await userBuyProductAndRate(bought);
  const { data } = res;
  if (data.codeStatus === 2000) {
    alert('購買成功!');
  }
};

/*
  去除後端傳回類別重複
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
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
          <InputGroup>
            <InputNumber v-model="purchaseQuantity" placeholder="購買數量" />
          </InputGroup>
          <InputGroup>
            <InputNumber v-model="rating" placeholder="評分" />
          </InputGroup>
          <InputGroup>
            <InputText v-model="comment" placeholder="評論" />
          </InputGroup>
          <div class="flex gap-2 mt-3">
            <span
              v-for="cat in productscategory(product.productCategoryName)"
              :key="cat"
              class="bg-gray-100 px-2 py-1 rounded text-sm"
            >
              {{ cat }}
            </span>
            <button
              class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
              @click="userBuy()"
            >
              購買
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
