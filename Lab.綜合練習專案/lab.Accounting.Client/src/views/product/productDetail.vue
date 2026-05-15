<script setup>
import { getProduct } from '@/api/productsService';
import { addProductsInShoppingCar } from '@/api/shoppingcarService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路徑
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   authStore : pinia
   allRate : 所有評價
   boughtQuantity : 購買數量
*/
const route = useRoute();
const router = useRouter();
const product = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const allRate = ref(null);
const boughtQuantity = ref(1);
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   查看商品細節資訊
*/
const getProductDetail = async (id) => {
  try {
    showLoading();
    var res = await getProduct(id);
    const { data } = res;
    if (data.codeStatus === 2000) {
      product.value = data.returnData;
      allRate.value = data.returnData.productsAllRates;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
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
  去除後端傳回類別重複
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
};

/*
   載入頭貼
*/
const imgUrl = computed(() => {
  if (authStore.userHeadshot) {
    return `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
});

/*
  商品加入購物車
*/
const addProductsInCar = async (productId, boughtquantity) => {
  try {
    var res = await addProductsInShoppingCar(productId, boughtquantity);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('加入成功!');
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  直接購買
*/
const boughtProduct = async (id, boughtquantity) => {
  await addProductsInCar(id, boughtquantity);
  router.push({ name: 'shopping-car' });
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
          <span class="mt-3">商品名稱 : {{ product.productsName }}</span>
          <span class="mt-3">商品價格 : {{ product.productsPrice }}</span>
          <span class="mt-3">商品評分 : {{ product.productsAVGRate }}</span>
          <span class="mt-3">商品庫存 : {{ product.productsStock }}</span>
          <span class="mt-3">商品擁有者 ID : {{ product.userId }}</span>
          <!-- 購買數量 -->
          <label>購買數量</label>
          <div class="flex items-center border border-gray-300 rounded-lg overflow-hidden w-fit">
            <button
              class="px-3 py-2 text-gray-600 hover:bg-gray-100 cursor-pointer"
              @click="boughtQuantity = Math.max(1, boughtQuantity - 1)"
            >
              −
            </button>
            <span class="px-4 py-2 text-sm select-none">{{ boughtQuantity }}</span>
            <button
              class="px-3 py-2 text-gray-600 hover:bg-gray-100 cursor-pointer"
              @click="boughtQuantity = Math.min(product.productsStock, boughtQuantity + 1)"
            >
              +
            </button>
          </div>

          <span class="text-2xl">{{ product.productsStock > 0 ? '尚有庫存' : '已售罄' }}</span>
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
              @click="addProductsInCar(product.productsId, boughtQuantity)"
            >
              加入購物車
            </button>
            <button
              class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
              @click="boughtProduct(product.productsId, boughtQuantity)"
            >
              購買
            </button>
          </div>
          <div
            v-for="rate in allRate"
            class="hover:shadow-xl hover:bg-gray-50 h-20 flex flex-row ps-10 items-center"
          >
            <img :src="imgUrl" alt="頭貼" class="w-10 h-10 rounded-full object-cover me-5" />
            <span class="mt-3 me-5">評價者名稱 : {{ rate.userName }}</span>
            <span class="mt-3 me-5">評論 : {{ rate.comment }}</span>
            <span class="mt-3 me-5">評價時間 : {{ rate.createTime }}</span>
            <span class="mt-3 me-5">評分 : {{ rate.rating }}</span>
          </div>

          \
        </div>
      </div>
    </div>
  </div>
</template>
