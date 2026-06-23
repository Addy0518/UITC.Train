<script setup>
import { getSellerAllCoupons } from '@/api/couponService';
import { couponTypeEnum } from '@/common/enum';
import CouponDialog from '@/dialog/sellerCouponCommandDialog.vue';

/*
   變數名稱代表意義
   allUser : 所有使用者
   router : 改變路由
   baseUrl : 環境變數裡的圖片基底位址
   currentPage : 目前所在頁數
   keyWords : 關鍵字查詢 ( 優惠卷名稱 )
   isActive : 是否啟用
   totalCount : 優惠卷數量
   isFiltering : 是否為第一次加載
   sortBy : 分類排序
   sortOrder : 排序方向
   search : 搜尋
   suggestions : 搜尋建議
   couponDialog : 控制新增 / 編輯的 Dialog
*/
const allCoupons = ref(null);
const router = useRouter();
const currentPage = ref();
const keyWords = ref();
const isActive = ref(null);
const totalCount = ref();
const isFiltering = ref(false);
const sortBy = ref('StartTime');
const sortOrder = ref('desc');
const search = ref();
const suggestions = ref([]);
const couponDialog = ref(null);
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
  getCoupons(true);
});

/*
   查看所有優惠卷
*/
const getCoupons = async (isFirstload = false) => {
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
      keyWords: keyWords.value ?? null,
      isActive: isActive.value ?? null,
      sortBy: sortBy.value,
      sortOrder: sortOrder.value,
    };
    const res = await getSellerAllCoupons(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCoupons.value = data.returnData;
      totalCount.value = data.returnData[0]?.totalCount ?? 0;
    } else if (data.codeStatus === 4001) {
      allCoupons.value = [];
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
   啟用狀態切換
*/
const toggleActiveFilter = () => {
  if (isActive.value === null || isActive.value === undefined) {
    isActive.value = true;
  } else if (isActive.value === true) {
    isActive.value = false;
  } else {
    isActive.value = null;
  }
  currentPage.value = 0;
  getCoupons();
};

/*
   排序切換
*/
const toggleSort = (field) => {
  if (sortBy.value === field) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortBy.value = field;
    sortOrder.value = 'desc';
  }
  getCoupons();
};

/*
   換頁
*/
const pageChange = (event) => {
  currentPage.value = event.page;
  getCoupons();
};

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const request = {
      keyWords: event.query,
      pageSize: 10,
      pageIndex: 0,
    };
    const res = await getSellerAllCoupons(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      suggestions.value = [...new Set(data.returnData.map((u) => u.name))];
    } else {
      suggestions.value = ['查無相關優惠卷資訊'];
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
  getCoupons();
};

/*
   打開 Dialog 的新增優惠卷
*/
const openCreateDialog = async () => {
  couponDialog.value.open();
};

/*
   打開 Dialog 的編輯優惠卷
*/
const openEditDialog = async (coupon) => {
  couponDialog.value.open(coupon);
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allCoupons">
    <!-- #region  標題列-->
    <div class="flex items-center gap-4 mb-4">
      <p class="text-2xl font-bold m-0">優惠券管理</p>

      <div class="flex flex-1 items-center justify-center">
        <AutoComplete
          v-model="search"
          :suggestions="suggestions"
          @complete="searchSuggestions"
          @keyup.enter="goSearch"
          @item-select="goSearch"
          placeholder="搜尋優惠券名稱"
          style="width: 500px"
          fluid
        />
      </div>

      <button
        class="flex items-center gap-1.5 bg-black text-white px-4 h-9 rounded-lg text-sm cursor-pointer"
        @click="openCreateDialog"
      >
        <i class="pi pi-plus text-xs" />
        新增優惠券
      </button>
    </div>
    <!-- #endregion -->

    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <!-- #region  欄位標頭-->
      <div
        class="grid grid-cols-[90px_1.4fr_1fr_0.9fr_1fr_1.4fr_0.8fr_110px] px-5 py-2.5 bg-gray-50 border-b border-gray-100"
      >
        <span class="text-xs text-gray-400">優惠碼</span>
        <span class="text-xs text-gray-400">名稱</span>
        <span class="text-xs text-gray-400">類型</span>
        <span class="text-xs text-gray-400">折扣</span>
        <span class="text-xs text-gray-400">最低消費</span>
        <button
          @click="toggleSort('StartTime')"
          :class="sortBy === 'StartTime' ? 'text-orange-500' : 'text-gray-400'"
          class="text-xs text-left cursor-pointer hover:text-gray-700"
        >
          有效期限
          <i v-if="sortBy === 'StartTime' && sortOrder === 'asc'" class="pi pi-arrow-up text-xs" />
          <i
            v-if="sortBy === 'StartTime' && sortOrder === 'desc'"
            class="pi pi-arrow-down text-xs"
          />
        </button>
        <button
          @click="toggleActiveFilter"
          class="text-xs text-left cursor-pointer hover:text-gray-700 flex items-center gap-1 focus:outline-none"
          :class="isActive !== null ? 'text-black font-semibold' : 'text-gray-400'"
        >
          狀態
          <span
            v-if="isActive === true"
            class="px-1 py-0.5 rounded bg-green-50 text-green-700 text-[10px]"
            >已啟用</span
          >
          <span
            v-else-if="isActive === false"
            class="px-1 py-0.5 rounded bg-gray-100 text-gray-500 text-[10px]"
            >未啟用</span
          >
          <span v-else class="text-[10px] text-gray-300">(全部)</span>
        </button>
        <span class="text-xs text-gray-400 text-right">操作</span>
      </div>
      <!-- #endregion -->

      <!-- #region  優惠券列表-->
      <template v-if="isFiltering">
        <div
          v-for="n in 6"
          :key="n"
          class="grid grid-cols-[90px_1.4fr_1fr_0.9fr_1fr_1.4fr_0.8fr_110px] px-5 py-4 border-b border-gray-100 gap-4 items-center"
        >
          <Skeleton height="1rem" />
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
          v-if="allCoupons.length === 0"
          class="flex flex-col items-center justify-center py-16 text-gray-400"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的優惠券</span>
        </div>
        <div
          v-for="coupon in allCoupons"
          :key="coupon.couponId"
          class="grid grid-cols-[90px_1.4fr_1fr_0.9fr_1fr_1.4fr_0.8fr_110px] px-5 py-4 border-b border-gray-100 items-center hover:bg-gray-50"
        >
          <span class="font-mono text-xs text-gray-500 truncate">{{ coupon.code }}</span>
          <span class="text-sm truncate">{{ coupon.name }}</span>
          <span>
            <span class="px-2 py-0.5 rounded-full text-xs bg-blue-50 text-blue-700">
              {{ getEnumDescription(couponTypeEnum, coupon.type) }}
            </span>
          </span>
          <span class="text-sm text-orange-500 font-medium">
            {{
              coupon.type === couponTypeEnum.百分比折扣.value
                ? `${coupon.discount} 折`
                : `$${coupon.discount} 元`
            }}
          </span>
          <span class="text-sm">
            {{ coupon.minimunSpend > 0 ? `$${coupon.minimunSpend}` : '無限制' }}
          </span>
          <span class="text-xs text-gray-500">
            {{ formatDateTimeString(coupon.startTime) }} ～
            {{ formatDateTimeString(coupon.endTime) }}
          </span>
          <span>
            <span
              class="px-2 py-0.5 rounded-full text-xs"
              :class="coupon.isActive ? 'bg-green-50 text-green-700' : 'bg-gray-100 text-gray-500'"
            >
              {{ coupon.isActive ? '啟用中' : '未啟用' }}
            </span>
          </span>
          <div class="flex gap-1 justify-end">
            <button
              class="w-8 h-8 flex items-center justify-center rounded-md hover:bg-gray-100 cursor-pointer"
              @click="openEditDialog(coupon)"
            >
              <i class="pi pi-pencil text-sm text-gray-600" />
            </button>
          </div>
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

  <CouponDialog ref="couponDialog" @refresh="getCoupons(false)" />
</template>
