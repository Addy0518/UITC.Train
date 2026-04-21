<script setup>
import { ref, onMounted, compile, computed, watch } from 'vue';
import { getAllProduct, addProductsInShoppingCar } from '@/api/account-api';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
  變數名稱代表意義
  categorys : 商品類別
  products : 全部商品
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  baseUrl : 基底位址
*/
const categorys = ref();
const products = ref([]);
const allProductsRaw = ref([]);
const baseUrl = 'https://localhost:7124';

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
  const res = await getAllProduct(0, 11);
  const { data } = res;
  console.log('data', data);
  if (data.codeStatus === 2000) {
    allProductsRaw.value = data.returnData;
    products.value = data.returnData;

    /*
       使用 flatmap 把後端的類別攤平去重 , 跟 map 的差別是
       map 會把 ['男士,男士', '鞋子'] + split(',') 變成 [['男士', '男士'], ['鞋子']] （ 陣列裡面包陣列 ）, 系統就會分不出來
       flatmap 則會 ['男士', '男士', '鞋子'] + split(',') 變成 ['男士', '男士', '鞋子'] ( 自動攤平成一個大陣列 ）, 這樣就能比對重複
    */
    const allNames = products.value.flatMap((x) =>
      x.productCategoryName ? x.productCategoryName.split(',') : [],
    );
    /*
       最後在用 new Set 把剛剛 flatmap 攤平的陣列去重複塞進類別
    */
    categorys.value = [...new Set(allNames)];
  }
};

/*
  依據點擊類別篩選商品
*/
const categoryFilter = (cate) => {
  if (cate !== null) {
    products.value = allProductsRaw.value.filter(
      (x) => x.productCategoryName && x.productCategoryName.split(',').includes(cate),
    );
  }
};

/*
  去除後端傳回類別重複
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
  商品加入購物車
*/
const addProductsInCar = async (productId) => {
  var res = await addProductsInShoppingCar(productId);
  const { data } = res;
  if (data.codeStatus === 2000) {
    alert('加入成功!');
  }
};


</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <span class="text-2xl m-5">分類</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="categoryname in categorys">
              <div
                @click="categoryFilter(categoryname)"
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
            <div v-for="product in products">
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
                  <!-- 迴圈讀取商品類別並去重複 ( 因為這裡是拿沒去重複的 product 資料 , 所以類別會多重複一次 ) -->

                  <span
                    v-for="cate in productscategory(product.productCategoryName)"
                    :key="cate"
                    class="mt-3 ms-2 me-2 text-sm text-gray-500"
                  >
                    {{ cate }}
                  </span>
                </RouterLink>
                <button
                  class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
                  @click="addProductsInCar(product.productsId)"
                >
                  加入購物車
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
