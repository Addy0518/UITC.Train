<script setup>
import { getProductsReview, getAllProductsReview } from '@/api/reviewService';
import { reviewStatusEnum } from '@/common/enum';

/*
   變數名稱代表意義
   allreview : 所有審查表
   router : 改變路由
   currentPage : 目前所在頁數
   currentSort : 現在的排序
   sortBy : 分類排序
   sortOrder : 排序方向
   reviewStatus : 審查狀態
   keyWords : 關鍵字查詢 ( 商品名稱或賣家名稱 )
   sellerId : 賣家 ID
   isFiltering : 是否為第一次加載
   totalCount : 審查表數量
   search : 搜尋
   suggestions : 搜尋建議
   searchType : 搜尋類型
*/
const allreview = ref(null);
const router = useRouter();
const currentPage = ref();
const currentSort = ref({ type: 'CreateTime', order: 'desc' });
const sortBy = ref('CreateTime');
const sortOrder = ref('desc');
const reviewStatus = ref();
const keyWords = ref();
const sellerId = ref();
const isFiltering = ref(false);
const totalCount = ref();
const search = ref();
const suggestions = ref([]);
const searchType = ref('ProductsName');

/*
   搜尋類型選項
*/
const searchTypeOptions = [
  { label: '商品名稱', value: 'ProductsName' },
  { label: '賣家名稱', value: 'SellerName' },
  { label: '審核編號', value: 'ProductsReviewId' },
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
  getAllReview(true);
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
  getAllReview();
};

/*
   查看所有審查表
*/
const getAllReview = async (isFirstload = false) => {
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
      reviewStatus: reviewStatus.value ?? null,
      searchType: searchType.value,
      keyWords: keyWords.value ?? null,
      sellerId: sellerId.value,
    };
    const res = await getAllProductsReview(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allreview.value = data.returnData.productsReview;
      totalCount.value = data.returnData.totalCount;
    } else if (data.codeStatus === 4001) {
      allreview.value = [];
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
  currentPage.value = event.page;
  getAllReview();
};

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getAllProductsReview({
      keyWords: event.query,
      searchType: searchType.value,
      pageSize: 10,
      pageIndex: 0,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      const fieldMap = {
        ProductsName: (r) => r.productsName,
        SellerName: (r) => r.sellerName,
        ProductsReviewId: (r) => String(r.productsReviewId),
      };
      suggestions.value = [
        ...new Set(data.returnData.productsReview.map(fieldMap[searchType.value])),
      ];
    } else {
      suggestions.value = ['查無相關審查資訊'];
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

  const keyword = search.value;
  keyWords.value = keyword;
  currentPage.value = 0;
  search.value = null;
  getAllReview();
};

const changeReviewStatus = (status) => {
  reviewStatus.value = status;
  currentPage.value = 0;
  getAllReview();
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allreview">
    <!-- #region  標題列-->
    <div class="flex items-center gap-4 mb-4">
      <p class="text-2xl font-bold m-0">審查表管理</p>
      <button
        @click="toggleSort('CreateTime')"
        :class="currentSort.type === 'CreateTime' ? 'text-orange-500' : 'text-gray-700'"
        class="w-28 h-9 cursor-pointer hover:bg-gray-100 rounded-lg text-sm"
      >
        申請時間
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
        @click="toggleSort('ReviewTime')"
        :class="currentSort.type === 'ReviewTime' ? 'text-orange-500' : 'text-gray-700'"
        class="w-28 h-9 cursor-pointer hover:bg-gray-100 rounded-lg text-sm"
      >
        審查時間
        <i
          v-if="currentSort.type === 'ReviewTime' && currentSort.order === 'asc'"
          class="pi pi-arrow-up text-xs"
        />
        <i
          v-if="currentSort.type === 'ReviewTime' && currentSort.order === 'desc'"
          class="pi pi-arrow-down text-xs"
        />
      </button>

      <button
        v-for="option in reviewStatusEnum"
        :key="option.label"
        @click="changeReviewStatus(option.value)"
        :class="
          reviewStatus === option.value
            ? 'bg-orange-500 text-white border-orange-500'
            : 'text-gray-600 border-gray-200 hover:bg-gray-100'
        "
        class="px-3 py-1.5 border rounded-lg text-xs cursor-pointer transition-colors"
      >
        {{ option.description }}
      </button>

      <div class="flex flex-1 items-center justify-center">
        <Select
          v-model="searchType"
          :options="searchTypeOptions"
          optionLabel="label"
          optionValue="value"
          class="w-36"
        />
        <AutoComplete
          v-model="search"
          :suggestions="suggestions"
          @complete="searchSuggestions"
          @keyup.enter="goSearch"
          @item-select="goSearch"
          placeholder="搜尋"
          style="width: 500px"
          fluid
        />
      </div>
    </div>

    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <!-- #endregion -->
    </div>
    <!-- #endregion -->
    <!-- #region  欄位標頭-->
    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <div
        class="grid grid-cols-[100px_100px_1fr_110px_150px_150px] px-5 py-2.5 bg-gray-50 border-b border-gray-100"
      >
        <span class="text-xs text-gray-400">賣家</span>
        <span class="text-xs text-gray-400">審核人員</span>
        <span class="text-xs text-gray-400">商品名稱</span>
        <span class="text-xs text-gray-400">審核狀態</span>
        <span class="text-xs text-gray-400">申請時間</span>
        <span class="text-xs text-gray-400">審核時間</span>
      </div>

      <!-- #endregion -->
      <!-- #region  商品-->

      <template v-if="isFiltering">
        <div
          v-for="n in 6"
          :key="n"
          class="grid grid-cols-[100px_100px_1fr_110px_150px_150px] px-5 py-4 border-b border-gray-100 gap-4 items-center"
        >
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
        </div>
      </template>
      <template v-else>
        <div
          v-if="allreview.length === 0"
          class="flex flex-col items-center justify-center py-16 text-gray-400"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的審查表</span>
        </div>
        <div
          v-for="review in allreview"
          :key="review.productsReviewId"
          class="grid grid-cols-[100px_100px_1fr_110px_150px_150px] px-5 py-4 border-b border-gray-100 items-center hover:bg-gray-50 cursor-pointer"
          @click="
            router.push({ name: 'admin-review-details', params: { id: review.productsReviewId } })
          "
        >
          <span class="text-sm font-medium truncate">{{ review.sellerName }}</span>
          <span class="text-sm text-gray-700 truncate">{{ review.adminName ?? '—' }}</span>
          <span class="text-sm text-gray-800 truncate">{{ review.productsName }}</span>
          <span class="text-sm">
            <span
              class="px-2 py-0.5 rounded-full text-xs"
              :class="{
                'bg-yellow-100 text-yellow-700': review.reviewStatus === 0,
                'bg-green-100 text-green-700': review.reviewStatus === 1,
                'bg-red-100 text-red-700': review.reviewStatus === 2,
              }"
            >
              {{
                review.reviewStatus === 0
                  ? '待審核'
                  : review.reviewStatus === 1
                    ? '已通過'
                    : '已駁回'
              }}
            </span>
          </span>
          <span class="text-sm text-gray-500">{{ formatDateTimeString(review.createTime) }}</span>
          <span class="text-sm text-gray-500">{{
            review.reviewTime ? formatDateTimeString(review.reviewTime) : '—'
          }}</span>
        </div>
      </template>
      <!-- #endregion -->
      <!-- #region  頁碼按鈕-->
      <div class="flex items-center justify-center gap-4 py-4">
        <span class="text-sm text-gray-400">總筆數：{{ totalCount }}</span>
        <Paginator
          :template="{
            '640px': 'PrevPageLink CurrentPageReport NextPageLink',
            '960px': 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink',
            '1300px': 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
            default:
              'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageDropdown JumpToPageInput',
          }"
          :rows="10"
          :totalRecords="totalCount"
          @page="pageChange"
        />
      </div>

      <!-- #endregion -->
    </div>
  </div>
</template>
