<script setup>
import { getAllProduct } from '@/api/productsService';
import { getStore } from '@/api/storeService';
import { getOneUser } from '@/api/userService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
   變數名稱代表意義
   allproduct : 賣家所有商品
   userInfo : 賣家資訊
   baseUrl : 環境變數裡的圖片基底位址
   router : 控制路由
   selectedCategory : 選擇的類別區塊
   seller : 賣家
   store : 賣場
   sellerAllRate : 賣家的所有評價
   sellerAVGRate : 賣家評分
*/
const allproduct = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const route = useRoute();
const selectedCategory = ref(null);
const seller = ref();
const store = ref({});
const sellerAllRate = ref(null);
const sellerAVGRate = ref();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時查看賣家所有商品
*/
onMounted(() => {
  getSellerInfo(route.params.id);
  getStoreInfo(route.params.id);
  getSellerProduct();
});

/*
   拿到賣家資訊
*/
const getSellerInfo = async (id) => {
  try {
    showLoading();
    const res = await getOneUser(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      seller.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   載入賣家頭貼
*/
const sellerImg = (user) => {
  if (user) {
    return `${baseUrl}/UserHeadShot/${user.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
};

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
      allproduct.value = data.returnData.products;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   拿到賣場資訊
*/
const getStoreInfo = async (id) => {
  try {
    showLoading();
    const res = await getStore(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      store.value = data.returnData;
      sellerAllRate.value = data.returnData.allProductsRateCount;
      sellerAVGRate.value = data.returnData.countAVGAllProductRate;
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
  <div class="flex flex-col w-full p-6" v-if="allproduct">
    <!-- 賣家資訊 -->
    <div class="bg-gray-50 rounded-lg p-6 mb-5 flex items-center gap-5">
      <img
        :src="sellerImg(seller)"
        class="w-18 h-18 rounded-full object-cover border-2 border-gray-200"
      />
      <div>
        <p class="text-lg font-medium m-0 mb-1">{{ store.storeName }}</p>
        <p class="text-sm text-gray-400 m-0">共 {{ allproduct.length }} 件商品</p>
      </div>
      <!--#region  統計資訊 -->
      <div class="grid grid-cols-3 gap-x-10 gap-y-2.5 flex-1 text-sm">
        <div class="flex items-center gap-2">
          <span class="text-gray-400">商品</span>
          <span class="text-orange-500 font-medium">{{ store.allProductsCount }}</span>
        </div>

        <div class="flex items-center gap-2">
          <span class="text-gray-400">加入時間</span>
          <span class="text-gray-700 font-medium">{{ formatDateOnly(store.createTime) }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-gray-400">評價</span>
          <span class="text-orange-500 font-medium">{{ sellerAllRate }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-gray-400">賣場評分</span
          ><span class="text-orange-500 font-medium">{{ sellerAVGRate }}</span>
          <Rating :modelValue="sellerAVGRate" :stars="5" :readonly="true" />
        </div>
        <div class="flex items-center gap-2">
          <span class="text-gray-400">公司名稱</span>
          <span class="text-gray-700 font-medium">{{ store.storeCompanyName }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-gray-400">公司統編</span>
          <span class="text-gray-700 font-medium">{{ store.storeUnifiedNumber }}</span>
        </div>
      </div>
      <!-- #endregion -->
    </div>

    <!-- 類別篩選 -->
    <div class="flex gap-2 mb-5 flex-wrap">
      <button
        @click="selectedCategory = null"
        class="px-4 py-1.5 rounded-full text-sm cursor-pointer transition-colors"
        :class="
          !selectedCategory
            ? 'bg-orange-500 text-white'
            : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
        "
      >
        全部
      </button>
      <button
        v-for="cat in allCategories"
        :key="cat"
        @click="selectedCategory = selectedCategory === cat ? null : cat"
        class="px-4 py-1.5 rounded-full text-sm cursor-pointer transition-colors"
        :class="
          selectedCategory === cat
            ? 'bg-orange-500 text-white'
            : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
        "
      >
        {{ cat }}
      </button>
    </div>

    <!-- 商品格狀列表 -->
    <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
      <div
        v-for="product in filterProducts"
        :key="product.productsId"
        class="bg-white rounded-lg border border-gray-100 overflow-hidden cursor-pointer hover:border-orange-200 hover:shadow-sm transition-all"
        @click="router.push({ name: 'product-detail', params: { id: product.productsId } })"
      >
        <div class="h-36 bg-gray-50 flex items-center justify-center overflow-hidden">
          <img :src="getProductsImg(product)" class="w-full h-full object-cover" />
        </div>
        <div class="p-3">
          <p class="text-sm font-medium m-0 mb-1 truncate">{{ product.productsName }}</p>
          <p class="text-base font-medium text-orange-500 m-0 mb-1">
            $ {{ product.productsPrice }}
          </p>
          <p class="text-xs text-gray-400 m-0">{{ product.productCategoryName }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
