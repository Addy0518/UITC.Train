<script setup>
import { getAllProduct } from '@/api/productsService';
import { getStore } from '@/api/storeService';
import { getOneUser } from '@/api/userService';
import defaultImgurl from '@/img/預設圖片.jpg';

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
  const headshot = user.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
  return `${baseUrl}/UserHeadShot/${headshot}`;
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
  <div v-if="allproduct" class="min-h-screen bg-page-bg-soft py-6 px-4 w-full">
    <div class="max-w-5xl mx-auto flex flex-col gap-4">
      <!-- #region 賣場資訊 -->
      <div class="bg-page-bg border border-border-soft rounded-card p-6">
        <div class="flex flex-col md:flex-row gap-6">
          <!-- 頭像 -->
          <div class="shrink-0">
            <img
              :src="sellerImg(seller)"
              class="w-24 h-24 rounded-full object-cover border border-border-soft"
            />
          </div>

          <!-- 賣場主資訊 -->
          <div class="flex-1">
            <div class="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4">
              <div>
                <h1 class="text-xl font-bold text-ink-900 m-0">
                  {{ store.storeName }}
                </h1>

                <div class="flex items-center gap-2 mt-2">
                  <span class="text-brand-price font-bold text-base">
                    {{ sellerAVGRate }}
                  </span>

                  <Rating
                    :modelValue="sellerAVGRate"
                    :stars="5"
                    :readonly="true"
                    :pt="{
                      onIcon: { class: 'text-brand-price' },
                      offIcon: { class: 'text-slate-300' },
                    }"
                  />

                  <span class="text-xs text-ink-500"> {{ sellerAllRate }} 則評價 </span>
                </div>

                <p class="text-xs text-ink-500 mt-2 mb-0">共 {{ allproduct.length }} 件商品</p>
              </div>
            </div>

            <div
              class="mt-6 pt-6 border-t border-border-soft grid grid-cols-2 lg:grid-cols-4 gap-4"
            >
              <div>
                <p class="text-xs text-ink-500 mb-1">商品數量</p>
                <p class="text-brand-price font-bold m-0">
                  {{ store.allProductsCount }}
                </p>
              </div>

              <div>
                <p class="text-xs text-ink-500 mb-1">加入時間</p>
                <p class="text-ink-900 m-0">
                  {{ formatDateOnly(store.createTime) }}
                </p>
              </div>

              <div>
                <p class="text-xs text-ink-500 mb-1">公司名稱</p>
                <p class="text-ink-900 m-0 truncate">
                  {{ store.storeCompanyName }}
                </p>
              </div>

              <div>
                <p class="text-xs text-ink-500 mb-1">統一編號</p>
                <p class="text-ink-900 m-0">
                  {{ store.storeUnifiedNumber }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- #endregion -->

      <!-- #region 商品分類 -->
      <div class="bg-page-bg border border-border-soft rounded-card p-4">
        <div class="flex flex-wrap gap-2">
          <button
            @click="selectedCategory = null"
            class="px-4 py-2 text-sm rounded-full border cursor-pointer transition-all"
            :class="
              !selectedCategory
                ? 'border-selection bg-selection-50 text-ink-900'
                : 'border-border-soft bg-page-bg text-ink-500 hover:bg-surface-muted'
            "
          >
            全部
          </button>

          <button
            v-for="cat in allCategories"
            :key="cat"
            @click="selectedCategory = selectedCategory === cat ? null : cat"
            class="px-4 py-2 text-sm rounded-full border cursor-pointer transition-all"
            :class="
              selectedCategory === cat
                ? 'border-selection bg-selection-50 text-ink-900'
                : 'border-border-soft bg-page-bg text-ink-500 hover:bg-surface-muted'
            "
          >
            {{ cat }}
          </button>
        </div>
      </div>
      <!-- #endregion -->

      <!-- #region 商品列表標題 -->
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-bold text-ink-900 m-0">商品列表</h2>

        <span class="text-sm text-ink-500"> 共 {{ filterProducts.length }} 件商品 </span>
      </div>
      <!-- #endregion -->

      <!-- #region 商品列表 -->
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-[14px]">
        <div
          v-for="product in filterProducts"
          :key="product.productsId"
          @click="
            router.push({
              name: 'product-detail',
              params: { id: product.productsId },
            })
          "
          class="bg-page-bg border border-border-soft rounded-card overflow-hidden cursor-pointer transition-colors hover:border-brand-500"
        >
          <!-- 商品圖片 -->
          <div class="aspect-square bg-surface-muted overflow-hidden">
            <img :src="getProductsImg(product)" class="w-full h-full object-cover" />
          </div>

          <!-- 商品資訊 -->
          <div class="p-4">
            <p class="text-sm font-bold text-ink-900 mb-2 line-clamp-2 min-h-[40px]">
              {{ product.productsName }}
            </p>

            <p class="text-base font-bold text-brand-price mb-2">NT$ {{ product.productsPrice }}</p>

            <p class="text-xs text-ink-500">
              {{ product.productCategoryName }}
            </p>
          </div>
        </div>
      </div>
      <!-- #endregion -->
    </div>
    ```
  </div>
</template>
