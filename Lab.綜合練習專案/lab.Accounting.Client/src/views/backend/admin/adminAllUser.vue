<script setup>
import { getAllUser } from '@/api/admin/userService';
import defaultImgurl from '@/img/預設圖片.jpg';
import { genderEnum } from '@/common/enum';
/*
   變數名稱代表意義
   allUser : 所有使用者
   router : 改變路由
   baseUrl : 環境變數裡的圖片基底位址
   currentPage : 目前所在頁數
   currentSort : 現在的排序
   sortBy : 分類排序
   sortOrder : 排序方向
   reviewStatus : 審查狀態
   keyWords : 關鍵字查詢 ( 商品名稱或賣家名稱 )
   sellerId : 賣家 ID
   isFiltering : 是否為第一次加載
   totalCount : 用戶數量
   search : 搜尋
   suggestions : 搜尋建議
   searchType : 搜尋類型
   userRole : 角色
*/
const allUser = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const currentPage = ref();
const currentSort = ref({ type: 'CreateTime', order: 'desc' });
const sortBy = ref('CreateTime');
const sortOrder = ref('desc');
const keyWords = ref();
const isFiltering = ref(false);
const totalCount = ref();
const search = ref();
const suggestions = ref([]);
const userRole = ref();

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
  getUserAll(true);
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
  getUserAll();
};

/*
   查看所有用戶
*/
const getUserAll = async (isFirstload = false) => {
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
      userGender: genderEnum.value?.value ?? null,
      keyWords: keyWords.value ?? null,
      userRole: userRole.value ?? null,
      isDelete: isDeleteEnum.value?.value ?? null,
    };
    const res = await getAllUser(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allUser.value = data.returnData;
      totalCount.value = data.returnData[0]?.totalCount ?? 0;
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
  getUserAll();
};

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getAllUser({
      keyWords: event.query,
      pageSize: 10,
      pageIndex: 0,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      suggestions.value = [...new Set(data.returnData.map((u) => u.userName))];
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
  getUserAll();
};

/*
  讀取用戶圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getUserImg = (user) => {
  const headshot = user.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
  return `${baseUrl}/UserHeadShot/${headshot}`;
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allUser">
    <!-- #region  標題列-->
    <div class="flex items-center gap-4 mb-4">
      <p class="text-2xl font-bold m-0 text-ink-900">用戶管理</p>

      <div class="flex flex-1 items-center justify-center">
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

    <!-- #region  欄位標頭-->
    <div class="bg-page-bg rounded-card border border-border-soft overflow-hidden">
      <div
        class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-2.5 bg-surface-muted border-b border-border-soft"
      >
        <span class="text-xs text-ink-500"></span>
        <span class="text-xs text-ink-500">用戶 ID</span>
        <span class="text-xs text-ink-500">用戶名稱</span>
        <span class="text-xs text-ink-500">帳號</span>
        <span class="text-xs text-ink-500">性別</span>
        <span class="text-xs text-ink-500">角色</span>
        <button
          @click="toggleSort('CreateTime')"
          :class="
            currentSort.type === 'CreateTime' ? 'text-brand-500 font-semibold' : 'text-ink-500'
          "
          class="text-xs text-left cursor-pointer hover:text-ink-900 flex items-center gap-1 focus:outline-none"
        >
          註冊時間
          <i
            v-if="currentSort.type === 'CreateTime' && currentSort.order === 'asc'"
            class="pi pi-arrow-up text-[10px]"
          />
          <i
            v-if="currentSort.type === 'CreateTime' && currentSort.order === 'desc'"
            class="pi pi-arrow-down text-[10px]"
          />
        </button>
        <span class="text-xs text-ink-500">狀態</span>
      </div>
      <!-- #endregion -->
      <!-- #region  商品-->

      <template v-if="isFiltering">
        <div
          v-for="n in 6"
          :key="n"
          class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-4 border-b border-border-soft gap-4 items-center"
        >
          <Skeleton height="40px" border-radius="50%" />
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
          v-if="allUser.length === 0"
          class="flex flex-col items-center justify-center py-16 text-ink-500"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的用戶</span>
        </div>
        <div
          v-for="user in allUser"
          :key="user.UserId"
          class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-4 border-b border-border-soft items-center hover:bg-surface-muted cursor-pointer"
          @click="router.push({ name: 'admin-user-details', params: { id: user.userId } })"
        >
          <img
            :src="getUserImg(user)"
            class="w-10 h-10 object-cover rounded-card border border-border-soft cursor-pointer"
          />
          <span class="text-sm font-medium text-ink-900 truncate">{{ user.userId }}</span>
          <span class="text-sm text-ink-900 truncate">{{ user.userName }}</span>
          <span class="text-sm text-ink-900 truncate">{{ user.userAccount }}</span>
          <span class="text-sm">
            <span class="px-2 py-0.5 rounded-full text-xs bg-surface-muted text-ink-500">
              {{ user.userGender === 0 ? '女性' : user.userGender === 1 ? '男性' : '其他' }}
            </span>
          </span>
          <span class="px-2 py-0.5 rounded-full text-xs w-fit bg-surface-muted text-ink-500">
            {{
              user.userRole === 'Seller'
                ? '賣家'
                : user.userRole === 'Admin'
                  ? '管理員'
                  : '一般用戶'
            }}
          </span>
          <span class="text-sm text-ink-500">{{ formatDateTimeString(user.createTime) }}</span>
          <span class="text-sm">
            <span
              class="px-2 py-0.5 rounded-full text-xs"
              :class="
                user.isDelete === 0
                  ? 'bg-status-success/10 text-status-success'
                  : 'bg-surface-muted text-status-neutral'
              "
            >
              {{ user.isDelete === 0 ? '正常' : '停用' }}
            </span>
          </span>
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
          :rows="10"
          :totalRecords="totalCount"
          @page="pageChange"
        />
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
