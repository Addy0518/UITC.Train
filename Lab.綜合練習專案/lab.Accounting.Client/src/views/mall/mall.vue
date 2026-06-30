<script setup>
import defaultImgurl from '@/img/預設圖片.jpg';
import advertise1 from '@/img/廣告1.jpg';
import advertise2 from '@/img/廣告2.jpg';
import advertise3 from '@/img/廣告3.jpg';
import { getAllProductsInShoppingCar } from '@/api/shoppingcarService';

/*
  變數名稱代表意義
  route : 獲取路由資訊
  router :　改變路由
  allProductsRaw : 初始資料 ( 全部商品 )
  products : 全部商品
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  baseUrl : 環境變數裡的圖片基底位址
  totalCount : 商品數量
  allCategories : 所有類別
  advertiseImg : 廣告圖片
*/
const route = useRoute();
const router = useRouter();
const products = ref([]);
const allProductsRaw = ref();
const allProducts = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const totalCount = ref();
const allCategories = ref();
const advertiseImg = ref([
  { itemImageSrc: advertise1 },
  { itemImageSrc: advertise2 },
  { itemImageSrc: advertise3 },
]);

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
  if (route.query.forbidden) {
    showToastError('你沒有訪問權限');
  }
  // 載入 Layout 的搜尋參數
  loadproducts();
  loadCategory();
});

/*
   監聽搜尋欄位變化
*/
watch(
  () => route.query.keyword,
  () => {
    loadproducts();
  },
);

/*
   監聽路由守衛擋下的錯誤
*/
watch(
  () => route.query.forbidden,
  (val) => {
    if (val) showToastError('你沒有訪問權限');
  },
);

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async (page = 0) => {
  try {
    showLoading();
    allProducts.value = [];
    const res = await getAllProduct({
      pageIndex: page,
      pageSize: 12,
      keyWords: route.query.keyword ?? null,
    });
    const { data } = res;
    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData.products;
      totalCount.value = data.returnData.totalCount;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   換頁
*/
const pageChange = (event) => {
  loadproducts(event.page);
};

/*
  根據所有商品類別取出不重複的各個父類別
*/
const loadCategory = async () => {
  try {
    showLoading();
    const res = await getOneFatherCategory();
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
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
  拿取類別圖片
*/
const getCategoryImg = (category) => {
  if (category.productCategoryImg) {
    return `${baseUrl}/CategoryImg/${category.productCategoryImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full items-center bg-page-bg">
    <!-- #region  廣告-->
    <div class="w-300 mt-8">
      <div class="rounded-card overflow-hidden border border-border-soft">
        <!-- #region 廣告 -->
        <Galleria
          :value="advertiseImg"
          :showThumbnails="false"
          :showIndicators="true"
          :showItemNavigators="true"
          :showIndicatorsOnItem="true"
          indicatorsPosition="bottom"
        >
          <template #item="slotProps">
            <img
              :src="slotProps.item.itemImageSrc"
              style="width: 100%; height: 400px; display: block; object-fit: cover"
            />
          </template>
        </Galleria>
        <!-- #endregion -->
      </div>
    </div>
    <!-- #endregion -->

    <!-- #region  類別 / 商品 -->
    <!-- #region 類別區 -->
    <div class="w-300 mt-8">
      <span class="text-xl font-bold text-ink-900 mb-5 block">分類</span>
      <div class="grid grid-cols-4 gap-4">
        <RouterLink
          v-for="category in allCategories"
          :key="category.productCategoryId"
          :to="{ name: 'mall-category', params: { id: category.productCategoryId } }"
          class="bg-page-bg border border-border-soft rounded-card hover:border-ink-300 transition-colors cursor-pointer flex flex-col items-center p-4"
        >
          <div class="aspect-square w-full bg-surface-muted rounded-card overflow-hidden">
            <img :src="getCategoryImg(category)" alt="Logo" class="w-full h-full object-cover" />
          </div>
          <span class="mt-3 text-sm text-ink-900">{{ category.productCategoryName }}</span>
        </RouterLink>
      </div>
    </div>
    <!-- #endregion -->

    <!-- #region 商品區 -->
    <div class="w-300 mt-8">
      <span class="text-xl font-bold text-ink-900 mb-5 block">商品</span>
      <div
        v-if="allProducts.length === 0"
        class="col-span-3 flex flex-col items-center justify-center py-16 text-ink-500"
      >
        <i class="pi pi-inbox text-4xl mb-3" />
        <span class="text-sm">沒有符合條件的商品</span>
      </div>
      <div class="grid grid-cols-4 gap-4">
        <RouterLink
          v-for="product in allProducts"
          :key="product.productsId"
          :to="{ name: 'product-detail', params: { id: product.productsId } }"
          class="block bg-page-bg border border-border-soft rounded-card overflow-hidden hover:border-ink-300 transition-colors cursor-pointer"
        >
          <div class="aspect-square bg-surface-muted overflow-hidden">
            <img :src="getProductsImg(product)" alt="" class="w-full h-full object-cover" />
          </div>
          <div class="p-3 flex flex-col gap-1.5">
            <p class="text-sm font-bold text-ink-900 truncate">{{ product.productsName }}</p>
            <div v-if="product.isDiscount" class="flex items-center gap-2">
              <p class="text-base font-bold text-brand-price">$ {{ product.finalPrice }}</p>
              <p class="text-sm font-medium line-through text-ink-300">
                $ {{ product.productsPrice }}
              </p>
            </div>
            <div v-else>
              <p class="text-base font-bold text-brand-price">$ {{ product.productsPrice }}</p>
            </div>

            <Rating
              :modelValue="product.productsAVGRate"
              :stars="5"
              :readonly="true"
              :pt="{
                onIcon: { class: 'text-brand-price' } /* 已點亮星星的顏色 */,
                offIcon: { class: 'text-slate-300' } /* 未點亮星星的顏色 */,
              }"
            />
            <div class="flex items-center justify-between mt-1">
              <span class="text-xs text-ink-500 bg-surface-muted px-2 py-0.5 rounded-full">
                {{ product.productCategoryName }}
              </span>
              <span class="text-xs text-ink-500">已售 {{ product.boughtQuantity }} 件</span>
            </div>
          </div>
        </RouterLink>
      </div>

      <!-- #region  頁碼按鈕-->
      <div class="flex flex-1 items-center justify-center mt-5 mb-30">
        <span class="text-ink-500">總筆數 : {{ totalCount }}</span>
        <Paginator
          :template="{
            '640px': 'PrevPageLink CurrentPageReport NextPageLink',
            '960px': 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink',
            '1300px': 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
            default:
              'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageDropdown JumpToPageInput',
          }"
          :rows="12"
          :totalRecords="totalCount"
          @page="pageChange"
        >
        </Paginator>
      </div>
      <!-- #endregion -->
    </div>
    <!-- #endregion -->
    <!-- #endregion -->
  </div>
</template>
