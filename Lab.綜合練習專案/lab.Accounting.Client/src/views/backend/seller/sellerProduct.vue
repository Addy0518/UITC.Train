<script setup>
import { getSellerAllProduct, deleteProducts } from '@/api/productsService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   allproduct : 賣家所有商品
   currentPage : 目前所在頁數
   baseUrl : 環境變數裡的圖片基底位址
   router : 控制路由
   currentSort : 現在的排序
   sortBy : 分類排序
   sortOrder : 排序方向
   maxPrice : 最大價格
   minPrice = 最少價格
   rate = 評分
   keyWords : 關鍵字搜尋
   visible : Dialog 開關
   isFiltering : 是否為第一次加載
   totalCount : 商品數量
   search : 搜尋
   suggestions : 搜尋建議
*/
const allproduct = ref(null);
const currentPage = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const currentSort = ref({ type: 'CreateTime', order: 'desc' });
const sortBy = ref('CreateTime');
const sortOrder = ref('desc');
const maxPrice = ref();
const minPrice = ref();
const rate = ref();
const keyWords = ref();
const visible = ref(false);
const isFiltering = ref(false);
const totalCount = ref();
const search = ref();
const suggestions = ref([]);

/*
   評價選項
*/
const rateOptions = [
  { label: '全部', value: null },
  { label: '5星', value: 5 },
  { label: '4星以上', value: 4 },
  { label: '3星以上', value: 3 },
  { label: '2星以上', value: 2 },
  { label: '1星以上', value: 1 },
];

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
  getSellerProduct(true);
});

/*
   切換排序類型
*/
const toggleSort = (type) => {
  if (currentSort.value.type === type) {
    if (currentSort.value.order === 'asc') {
      currentSort.value.order = 'desc';
    } else {
      currentSort.value = { type: null, order: null };
    }
  } else {
    currentSort.value = { type: type, order: 'asc' };
  }
  sortBy.value = currentSort.value.type;
  sortOrder.value = currentSort.value.order;
  getSellerProduct();
};

/*
   查看賣家所有商品
*/
const getSellerProduct = async (isFirstload = false) => {
  try {
    // 判斷是不是第一次加載
    if (isFirstload) {
      showLoading();
    } else {
      isFiltering.value = true;
    }

    const request = {
      pageIndex: currentPage.value,
      pageSize: 10,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
      rate: rate.value,
      maxPrice: maxPrice.value,
      minPrice: minPrice.value,
      keyWords: keyWords.value ?? null,
    };
    const res = await getSellerAllProduct(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allproduct.value = data.returnData.products;
      totalCount.value = data.returnData.totalCount;
    } else if (data.codeStatus === 4001) {
      allproduct.value = [];
      totalCount.value = 0;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
    isFiltering.value = false;
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

/*
   換頁
*/
const pageChange = (event) => {
  currentPage.value = event.page;
  getSellerProduct();
};

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getSellerAllProduct({ keyWords: event.query, pageSize: 10, pageIndex: 0 });
    const { data } = res;
    if (data.codeStatus === 2000) {
      suggestions.value = data.returnData.products.map((p) => p.productsName);
    } else {
      suggestions.value = ['查無相關商品'];
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
   前往搜尋
*/
const goSearch = () => {
  if (!search.value) return;
  // 這是判斷使用者是選推薦選單的選項還是直接打字
  // 因為選單可能會選成物件所以取物件裡的 productsname , 直接打字就沒差直接取值就好
  const keyword = typeof search.value === 'object' ? search.value.productsName : search.value;
  keyWords.value = keyword;
  getSellerProduct();
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allproduct">
    <!-- #region  標題列-->
    <div class="flex items-center justify-between mb-4">
      <p class="text-2xl font-bold m-0 text-ink-900">商品管理</p>
      <!-- #region  排序-->
      <button
        @click="toggleSort('CreateTime')"
        :class="currentSort.type === 'CreateTime' ? ' text-brand-500' : ' text-ink-500'"
        class="me-5 w-30 h-15 cursor-pointer hover:bg-surface-muted rounded-card"
      >
        上架時間
        <i
          v-if="currentSort.type === 'CreateTime' && currentSort.order === 'asc'"
          class="pi pi-arrow-up text-xs"
        />
        <i
          v-if="currentSort.type === 'CreateTime' && currentSort.order === 'desc'"
          class="pi pi-arrow-down text-xs"
        />
      </button>

      <button
        @click="toggleSort('Rate')"
        :class="currentSort.type === 'Rate' ? ' text-brand-500' : ' text-ink-500'"
        class="me-5 w-30 h-15 cursor-pointer hover:bg-surface-muted rounded-card"
      >
        評價
        <i
          v-if="currentSort.type === 'Rate' && currentSort.order === 'asc'"
          class="pi pi-arrow-up text-xs"
        />
        <i
          v-if="currentSort.type === 'Rate' && currentSort.order === 'desc'"
          class="pi pi-arrow-down text-xs"
        />
      </button>

      <button
        @click="toggleSort('ProductsPrice')"
        :class="currentSort.type === 'ProductsPrice' ? ' text-brand-500' : ' text-ink-500'"
        class="me-5 w-30 h-15 cursor-pointer hover:bg-surface-muted rounded-card"
      >
        價格
        <i
          v-if="currentSort.type === 'ProductsPrice' && currentSort.order === 'asc'"
          class="pi pi-arrow-up text-xs"
        />
        <i
          v-if="currentSort.type === 'ProductsPrice' && currentSort.order === 'desc'"
          class="pi pi-arrow-down text-xs"
        />
      </button>
      <!-- #endregion -->
      <!-- #region  篩選-->
      <button
        @click="visible = true"
        class="flex items-center gap-1 px-3 py-1.5 rounded-card text-sm cursor-pointer transition-colors bg-surface-muted text-ink-900 hover:bg-brand-50"
      >
        <i class="pi pi-filter text-xs" />
        篩選
      </button>

      <Dialog v-model:visible="visible" modal header="篩選條件" :style="{ width: '40rem' }">
        <div class="flex flex-col gap-4 py-2">
          <div>
            <label class="text-sm text-ink-500 mb-1 block">評分</label>
            <Select
              v-model="rate"
              :options="rateOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="選擇評分"
              class="w-full"
            />
          </div>

          <div>
            <label class="text-sm text-ink-500 mb-1 block">價格區間</label>
            <div class="flex items-center gap-2">
              <InputNumber
                v-model="minPrice"
                placeholder="最小價格"
                :min="0"
                :max="maxPrice ?? undefined"
                class="w-full"
              />
              <span class="text-ink-300">—</span>
              <InputNumber
                v-model="maxPrice"
                placeholder="最大價格"
                :min="minPrice ?? 0"
                class="w-full"
              />
            </div>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end gap-2">
            <Button
              label="清除"
              severity="secondary"
              @click="
                () => {
                  rate = null;
                  minPrice = null;
                  maxPrice = null;
                }
              "
            />
            <Button
              label="搜尋"
              @click="
                () => {
                  visible = false;
                  getSellerProduct();
                }
              "
            />
          </div>
        </template>
      </Dialog>
      <!-- #endregion -->
      <div class="flex flex-1 items-center justify-center">
        <AutoComplete
          v-model="search"
          :suggestions="suggestions"
          @complete="searchSuggestions"
          @keyup.enter="goSearch"
          @item-select="goSearch"
          placeholder="搜尋商品"
          style="width: 500px"
          fluid
        />
      </div>
      <button
        class="bg-brand-500 hover:opacity-90 text-white px-5 py-2 rounded-card text-sm cursor-pointer"
        @click="router.push({ name: 'add-product' })"
      >
        + 新增商品
      </button>
    </div>
    <!-- #endregion -->
    <!-- #region  欄位標頭-->
    <div class="bg-page-bg rounded-card border border-border-soft overflow-hidden">
      <div
        class="grid grid-cols-[80px_1fr_100px_100px_120px_160px] px-5 py-2.5 bg-surface-muted border-b border-border-soft"
      >
        <span class="text-xs text-ink-500">圖片</span>
        <span class="text-xs text-ink-500">商品名稱</span>
        <span class="text-xs text-ink-500">價格</span>
        <span class="text-xs text-ink-500">類別</span>
        <span class="text-xs text-ink-500">庫存</span>
        <span class="text-xs text-ink-500 text-right">操作</span>
      </div>
      <!-- #endregion -->
      <!-- #region  商品-->

      <template v-if="isFiltering">
        <div v-for="n in 12" :key="n" class="flex flex-col items-center rounded-card p-3">
          <Skeleton width="100%" height="160px" class="rounded-card" />
          <Skeleton width="80%" height="1rem" class="mt-3" />
          <Skeleton width="40%" height="1rem" class="mt-2" />
          <Skeleton width="60%" height="0.8rem" class="mt-2" />
        </div>
      </template>

      <template v-else>
        <div
          v-if="allproduct.length === 0"
          class="col-span-3 flex flex-col items-center justify-center py-16 text-ink-500"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的商品</span>
        </div>
        <div
          v-for="product in allproduct"
          :key="product.productsId"
          class="grid grid-cols-[80px_1fr_100px_100px_120px_160px] px-5 py-4 border-b border-border-soft items-center hover:bg-surface-muted"
        >
          <img
            :src="getProductsImg(product)"
            class="w-14 h-14 object-cover rounded-card border border-border-soft cursor-pointer"
            @click="router.push({ name: 'product-detail', params: { id: product.productsId } })"
          />
          <span class="text-sm font-medium text-ink-900">{{ product.productsName }}</span>
          <span class="text-sm text-brand-price font-medium">$ {{ product.productsPrice }}</span>
          <span class="text-sm text-ink-500">{{ product.productCategoryName }}</span>
          <span class="text-sm text-ink-900">{{ product.productsStock }} 件</span>
          <div class="flex gap-2 justify-end">
            <button
              class="px-3 py-1.5 border border-border-soft rounded-card text-xs cursor-pointer hover:bg-surface-muted text-ink-900"
              @click="router.push({ name: 'edit-product', params: { id: product.productsId } })"
            >
              編輯
            </button>
            <button
              class="px-3 py-1.5 border border-action-danger/30 rounded-card text-xs text-action-danger cursor-pointer hover:bg-action-danger-50"
              @click="deleteProduct(product.productsId)"
            >
              刪除
            </button>
          </div>
        </div>
      </template>
      <!-- #endregion -->
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
  </div>
</template>
