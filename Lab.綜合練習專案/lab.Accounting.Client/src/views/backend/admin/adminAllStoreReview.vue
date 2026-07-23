<script setup>
import { getAllStoreReview } from '@/api/admin/reviewService';

/*
   變數名稱代表意義
   allreview : 所有賣場審查表
   router : 改變路由
   currentPage : 目前所在頁數
   currentSort : 現在的排序
   sortBy : 分類排序
   sortOrder : 排序方向
   reviewStatus : 審查狀態
   keyWords : 關鍵字查詢 ( 賣場名稱或統編 )
   isFiltering : 是否為第一次加載
   totalCount : 審查表數量
   search : 搜尋
   suggestions : 搜尋建議
   searchType : 搜尋類型
   searchTypeOptions : 搜尋類型選項
*/
const allreview = ref(null);
const router = useRouter();
const currentPage = ref();
const currentSort = ref({ type: 'CreateTime', order: 'desc' });
const sortBy = ref('CreateTime');
const sortOrder = ref('desc');
const reviewStatus = ref(null);
const keyWords = ref();
const isFiltering = ref(false);
const totalCount = ref();
const search = ref();
const suggestions = ref([]);
const searchType = ref('StoreCompanyName');
const pageSize = ref(10);

const searchTypeOptions = [
  { label: '公司名稱', value: 'StoreCompanyName' },
  { label: '統一編號', value: 'StoreUnifiedNumber' },
  { label: '審核編號', value: 'StoreCompanyReviewId' },
];

const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');

/*
   初始化
*/
onMounted(() => {
  getAllReview(true);
});

/*
   切換顯示順序類型
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
   切換審查狀態顯示
*/
const toggleReviewStatusFilter = () => {
  if (reviewStatus.value === null || reviewStatus.value === undefined) {
    reviewStatus.value = 0;
  } else if (reviewStatus.value === 0) {
    reviewStatus.value = 1;
  } else if (reviewStatus.value === 1) {
    reviewStatus.value = 2;
  } else {
    reviewStatus.value = null;
  }
  currentPage.value = 0;
  getAllReview();
};

/*
   拿取所有審查表
*/
const getAllReview = async (isFirstload = false) => {
  try {
    if (isFirstload) {
      showLoading();
    } else {
      isFiltering.value = true;
    }

    const request = {
      pageIndex: currentPage.value,
      pageSize: pageSize.value,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
      reviewStatus: reviewStatus.value ?? null,
      searchType: searchType.value,
      keyWords: keyWords.value ?? null,
    };
    const res = await getAllStoreReview(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allreview.value = data.returnData.storeReviews;
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
   頁面切換
*/
const pageChange = (event) => {
  currentPage.value = event.page;
  getAllReview();
};

/*
   搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getAllStoreReview({
      keyWords: event.query,
      searchType: searchType.value,
      pageSize: 10,
      pageIndex: 0,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      const fieldMap = {
        StoreCompanyName: (r) => r.storeCompanyName,
        StoreUnifiedNumber: (r) => r.storeUnifiedNumber,
        StoreCompanyReviewId: (r) => String(r.storeCompanyReviewId),
      };
      suggestions.value = [
        ...new Set(data.returnData.storeReviews.map(fieldMap[searchType.value])),
      ];
    } else {
      suggestions.value = ['查無相關審查資訊'];
    }
  } catch (err) {
    console.log(err);
  }
};

/*
   搜尋
*/
const goSearch = () => {
  if (!search.value) return;
  keyWords.value = search.value;
  currentPage.value = 0;
  search.value = null;
  getAllReview();
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allreview">
    <!-- #region  標題列-->
    <div class="flex items-center gap-4 mb-4">
      <p class="text-2xl font-bold m-0 text-ink-900">企業賣場審核</p>

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
    <!-- #endregion -->

    <div class="bg-page-bg rounded-card border border-border-soft overflow-hidden">
      <!-- #region  欄位標頭-->
      <div
        class="grid grid-cols-[100px_100px_1fr_110px_110px_150px_150px] px-5 py-2.5 bg-surface-muted border-b border-border-soft"
      >
        <span class="text-xs text-ink-500">賣家</span>
        <span class="text-xs text-ink-500">審核人員</span>
        <span class="text-xs text-ink-500">公司名稱</span>
        <span class="text-xs text-ink-500">統一編號</span>
        <button
          @click="toggleReviewStatusFilter"
          class="text-xs text-left cursor-pointer hover:text-ink-900 flex items-center gap-1 focus:outline-none"
          :class="
            reviewStatus !== null && reviewStatus !== undefined
              ? 'text-ink-900 font-semibold'
              : 'text-ink-500'
          "
        >
          審核狀態
          <span
            v-if="reviewStatus === 0"
            class="px-1 py-0.5 rounded bg-status-warning/10 text-status-warning text-[10px]"
            >待審核</span
          >
          <span
            v-else-if="reviewStatus === 1"
            class="px-1 py-0.5 rounded bg-status-success/10 text-status-success text-[10px]"
            >已通過</span
          >
          <span
            v-else-if="reviewStatus === 2"
            class="px-1 py-0.5 rounded bg-action-danger-50 text-action-danger text-[10px]"
            >已駁回</span
          >
          <span v-else class="text-[10px] text-ink-300">(全部)</span>
        </button>
        <button
          @click="toggleSort('CreateTime')"
          :class="
            currentSort.type === 'CreateTime' ? 'text-brand-500 font-semibold' : 'text-ink-500'
          "
          class="text-xs text-left cursor-pointer hover:text-ink-900 flex items-center gap-1 focus:outline-none"
        >
          申請時間
          <i
            v-if="currentSort.type === 'CreateTime' && currentSort.order === 'asc'"
            class="pi pi-arrow-up text-[10px]"
          />
          <i
            v-if="currentSort.type === 'CreateTime' && currentSort.order === 'desc'"
            class="pi pi-arrow-down text-[10px]"
          />
        </button>
        <button
          @click="toggleSort('ReviewTime')"
          :class="
            currentSort.type === 'ReviewTime' ? 'text-brand-500 font-semibold' : 'text-ink-500'
          "
          class="text-xs text-left cursor-pointer hover:text-ink-900 flex items-center gap-1 focus:outline-none"
        >
          審核時間
          <i
            v-if="currentSort.type === 'ReviewTime' && currentSort.order === 'asc'"
            class="pi pi-arrow-up text-[10px]"
          />
          <i
            v-if="currentSort.type === 'ReviewTime' && currentSort.order === 'desc'"
            class="pi pi-arrow-down text-[10px]"
          />
        </button>
      </div>
      <!-- #endregion -->

      <!-- #region  列表-->
      <template v-if="isFiltering">
        <div
          v-for="n in 6"
          :key="n"
          class="grid grid-cols-[100px_100px_1fr_110px_110px_150px_150px] px-5 py-4 border-b border-border-soft gap-4 items-center"
        >
          <Skeleton height="1rem" />
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
          class="flex flex-col items-center justify-center py-16 text-ink-500"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的審查表</span>
        </div>
        <div
          v-for="review in allreview"
          :key="review.storeCompanyReviewId"
          class="grid grid-cols-[100px_100px_1fr_110px_110px_150px_150px] px-5 py-4 border-b border-border-soft items-center hover:bg-surface-muted cursor-pointer"
          @click="
            router.push({
              name: 'admin-store-review-details',
              params: { id: review.storeCompanyReviewId },
            })
          "
        >
          <span class="text-sm font-medium text-ink-900 truncate">{{ review.sellerName }}</span>
          <span class="text-sm text-ink-900 truncate">{{ review.adminName ?? '—' }}</span>
          <span class="text-sm text-ink-900 truncate">{{ review.storeCompanyName }}</span>
          <span class="text-sm text-ink-500 truncate">{{ review.storeUnifiedNumber }}</span>
          <span class="text-sm">
            <span
              class="px-2 py-0.5 rounded-full text-xs"
              :class="{
                'bg-status-warning/10 text-status-warning': review.reviewStatus === 0,
                'bg-status-success/10 text-status-success': review.reviewStatus === 1,
                'bg-action-danger-50 text-action-danger': review.reviewStatus === 2,
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
          <span class="text-sm text-ink-500">{{ formatDateTimeString(review.createTime) }}</span>
          <span class="text-sm text-ink-500">{{
            review.reviewTime ? formatDateTimeString(review.reviewTime) : '—'
          }}</span>
        </div>
      </template>
      <!-- #endregion -->

      <!-- #region  頁碼按鈕-->
      <div class="flex items-center justify-center gap-4 py-4">
        <span class="text-sm text-ink-500">總筆數：{{ totalCount }}</span>
        <Paginator
          :template="{
            '640px': 'PrevPageLink CurrentPageReport NextPageLink',
            '960px': 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink',
            '1300px': 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
            default:
              'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageDropdown JumpToPageInput',
          }"
          :rows="pageSize"
          :totalRecords="totalCount"
          @page="pageChange"
        />
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
