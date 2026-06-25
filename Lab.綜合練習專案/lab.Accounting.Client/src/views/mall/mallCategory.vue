<script setup>
import { getFatherCategories, getOneSonCategory } from '@/api/categoryService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
  變數名稱代表意義
  route : 獲取路由資訊
  router : 改變路徑
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  totalCount : 商品數量
  allCategories : 所有子類別
  baseUrl : 環境變數裡的圖片基底位址
  selectedCategory : 選擇的類別區塊
  breadCrumCategories : 麵包屑的類別
  currentSort : 現在的排序
  sortBy : 分類排序
  sortOrder : 排序方向
  maxPrice : 最大價格
  minPrice = 最少價格
  rate = 評分
  visible : Dialog 開關
  isFiltering : 是否為第一次加載
*/
const route = useRoute();
const router = useRouter();
const allProducts = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const selectedCategory = ref(null);
const totalCount = ref();
const allCategories = ref();
const breadCrumCategories = ref([]);
const currentSort = ref({ type: 'CreateTime', order: 'desc' });
const sortBy = ref('CreateTime');
const sortOrder = ref('desc');
const maxPrice = ref();
const minPrice = ref();
const rate = ref();
const visible = ref(false);
const isFiltering = ref(false);

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
   初始化加載所有商品 / 類別 / 麵包屑
*/
onMounted(() => {
  if (route.params.id) {
    loadproducts(route.params.id, 0, true);
    loadCategory(route.params.id);
    loadBreadCrumb(route.params.id);
  }
});

/*
   監聽路由變化 , 隨時加載類別跟商品
*/
watch(
  () => route.params.id,
  (newId) => {
    loadproducts(newId, 0, true);
    loadCategory(newId);
    loadBreadCrumb(newId);
  },
);

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
  loadproducts(selectedCategory.value ?? route.params.id);
};

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async (parentId, page = 0, isFirstload = false) => {
  try {
    // 判斷是不是第一次加載
    if (isFirstload) {
      showLoading();
    } else {
      isFiltering.value = true;
    }

    const res = await getAllProduct({
      productCategoryId: parentId,
      pageIndex: page,
      pageSize: 12,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
      rate: rate.value,
      maxPrice: maxPrice.value,
      minPrice: minPrice.value,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData.products;
      totalCount.value = data.returnData.totalCount;
    } else if (data.codeStatus === 4001) {
      allProducts.value = [];
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
   換頁
*/
const pageChange = (event) => {
  loadproducts(route.params.id, event.page);
};

/*
  根據所有商品類別取出不重複的各個子類別
*/
const loadCategory = async (id) => {
  try {
    const res = await getOneSonCategory(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  點擊類別區分商品
*/
const selectCategory = async (categoryId) => {
  if (selectedCategory.value === categoryId) {
    // 再點一次取消，回到當前路由的全部商品
    selectedCategory.value = null;
    loadproducts(route.params.id);
  } else {
    selectedCategory.value = categoryId;
    loadproducts(categoryId); // 打 API 拿該類別商品
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
  拿到麵包屑類別
*/
const loadBreadCrumb = async (id) => {
  try {
    const res = await getFatherCategories(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      breadCrumCategories.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  麵包屑 , 回主頁
*/
const home = ref({
  icon: 'pi pi-home',
  command: () => router.push({ name: 'mall' }),
});

/*
  麵包屑 , 動態讀取所有父類別
*/
const breadCrumbItem = computed(() => {
  if (!allProducts.value) return [];

  return breadCrumCategories.value.map((category, index) => ({
    label: category.productCategoryName,
    command: () =>
      router.push({
        name: 'mall-category',
        params: { id: category.productCategoryId },
        query:
          // 不是第一層才要帶 parentId
          index > 0 ? { parentId: breadCrumCategories.value[index - 1].productCategoryId } : {},
      }),
  }));
});
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="flex flex-col items-center">
      <!-- #region  麵包屑-->
      <div class="card flex justify-start">
        <Breadcrumb :home="home" :model="breadCrumbItem" />
      </div>
      <!-- #endregion -->
      <div class="mt-8 w-300 rounded-card border border-border-soft bg-page-bg">
        <!-- #region  類別 / 商品-->
        <div class="flex gap-4">
          <!-- #region  類別-->
          <div class="w-50 shrink-0 p-10">
            <span class="text-2xl m-5 font-bold text-ink-900">分類</span>
            <div class="flex flex-col mt-5 gap-2">
              <div v-for="category in allCategories" :key="category.productCategoryId">
                <span
                  @click="selectCategory(category.productCategoryId)"
                  class="cursor-pointer m-5"
                  :class="
                    selectedCategory === category.productCategoryId
                      ? 'text-brand-500 font-medium'
                      : 'text-ink-500'
                  "
                  >{{ category.productCategoryName }}</span
                >
              </div>
            </div>
          </div>
          <!-- #endregion -->

          <div class="flex-1 p-7">
            <div class="flex flex-1 items-center">
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
                          loadproducts(selectedCategory ?? route.params.id);
                        }
                      "
                    />
                  </div>
                </template>
              </Dialog>
              <!-- #endregion -->
            </div>
            <!-- #region  商品-->
            <span class="text-2xl m-5 font-bold text-ink-900">商品</span>
            <div class="grid grid-cols-3 mt-5 gap-4">
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
                  v-if="allProducts.length === 0"
                  class="col-span-3 flex flex-col items-center justify-center py-16 text-ink-500"
                >
                  <i class="pi pi-inbox text-4xl mb-3" />
                  <span class="text-sm">沒有符合條件的商品</span>
                </div>
                <div v-for="product in allProducts" :key="product.productsId">
                  <div
                    class="border border-border-soft hover:border-ink-300 transition-colors flex flex-col items-center rounded-card p-3"
                  >
                    <RouterLink
                      :to="{ name: 'product-detail', params: { id: product.productsId } }"
                      class="flex flex-col items-center cursor-pointer w-full"
                    >
                      <div
                        class="w-full aspect-square bg-surface-muted rounded-card overflow-hidden mt-1"
                      >
                        <img
                          :src="getProductsImg(product)"
                          alt="Logo"
                          class="w-full h-full object-cover"
                        />
                      </div>
                      <span class="mt-3 text-sm font-bold text-ink-900">{{
                        product.productsName
                      }}</span>
                      <div v-if="product.isDiscount" class="flex items-center gap-2">
                        <p class="text-base font-bold text-brand-price">
                          $ {{ product.finalPrice }}
                        </p>
                        <p class="text-sm font-medium line-through text-ink-300">
                          $ {{ product.productsPrice }}
                        </p>
                      </div>
                      <div v-else>
                        <p class="text-base font-bold text-brand-price">
                          $ {{ product.productsPrice }}
                        </p>
                      </div>
                      <span class="mt-3 ms-2 me-2 text-sm text-ink-500">{{
                        product.productCategoryName
                      }}</span>
                    </RouterLink>
                  </div>
                </div>
              </template>

              <!-- #endregion -->
            </div>
          </div>
          <!-- #endregion -->
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
    </div>
  </div>
</template>
