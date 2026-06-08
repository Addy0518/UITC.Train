<script setup>
import { getAllCategories, deleteCategory } from '@/api/admin/categoryService';
import { getOneFatherCategory } from '@/api/categoryService';
import defaultImgurl from '@/img/預設圖片.jpg';
import Swal from 'sweetalert2';
/*
   變數名稱代表意義
   allCategories : 所有商品類別
   router : 改變路由
   baseUrl : 環境變數裡的圖片基底位址
   currentPage : 目前所在頁數
   keyWords : 關鍵字查詢 ( 類別名稱 )
   isFiltering : 是否為第一次加載
   totalCount : 類別數量
   search : 搜尋
   suggestions : 搜尋建議
   searchType : 搜尋類型
   parentCategories : 父類別
   selectParent : 選擇的父類別
*/
const allCategories = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const currentPage = ref();
const keyWords = ref();
const isFiltering = ref(false);
const totalCount = ref();
const search = ref();
const suggestions = ref([]);
const parentCategories = ref([]);
const selectParent = ref();

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
  getCategoriesAll(true);
  getFatherCategories();
});

/*
   查看所有類別
*/
const getCategoriesAll = async (isFirstload = false) => {
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
      parentId: selectParent.value ?? null,
    };
    const res = await getAllCategories(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
      totalCount.value = data.returnData[0]?.totalCount ?? 0;
    } else if (data.codeStatus === 4001) {
      allCategories.value = [];
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
   查看所有父類別
*/
const getFatherCategories = async () => {
  try {
    const res = await getOneFatherCategory();
    const { data } = res;

    if (data.codeStatus === 2000) {
      parentCategories.value = data.returnData;
    } else if (data.codeStatus === 4001) {
      parentCategories.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   切換父類別下拉選單時
*/
const changeParentCategories = () => {
  currentPage.value = 0;
  getCategoriesAll();
};

/*
   換頁
*/
const pageChange = (event) => {
  currentPage.value = event.page;
  getCategoriesAll();
};

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getAllCategories({
      keyWords: event.query,
      pageSize: 10,
      pageIndex: 0,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      suggestions.value = [...new Set(data.returnData.map((u) => u.productCategoryName))];
    } else {
      suggestions.value = ['查無相關類別資訊'];
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
  getCategoriesAll();
};

/*
  讀取類別圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getCategoryImg = (category) => {
  if (category.productCategoryImg) {
    return `${baseUrl}/CategoryImg/${category.productCategoryImg}`;
  }
  return defaultImgurl;
};

/*
   刪除類別
*/
const deleteOneCategory = async (id) => {
  const result = await Swal.fire({
    title: '確定要刪除這項類別嗎？',
    text: '刪除後將無法復原！',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: '確定刪除',
    cancelButtonText: '取消',
  });

  try {
    if (result.isConfirmed) {
      const res = await deleteCategory(id);
      const { data } = res;

      if (data.codeStatus === 2000) {
        showToastSuccess('成功 !');
        getCategoriesAll(false);
      } else if (data.codeStatus === 4001) {
        showToastError('刪除失敗');
      }
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="allCategories">
    <!-- #region  標題列-->
    <div class="flex items-center gap-4 mb-4">
      <p class="text-2xl font-bold m-0">類別管理</p>

      <div class="flex flex-1 items-center justify-center">
        <p class="me-5">父類別搜尋 :</p>
        <Select
          v-model="selectParent"
          :options="[{ productCategoryName: '全部', productCategoryId: null }, ...parentCategories]"
          optionLabel="productCategoryName"
          optionValue="productCategoryId"
          placeholder="選擇父類別"
          @change="changeParentCategories"
          class="w-40 me-20"
        />
        <AutoComplete
          v-model="search"
          :suggestions="suggestions"
          @complete="searchSuggestions"
          @keyup.enter="goSearch"
          @item-select="goSearch"
          placeholder="類別名稱搜尋"
          style="width: 500px"
          fluid
        />
        <button
          class="flex items-center gap-1.5 px-4 py-2 ms-2 bg-orange-500 hover:bg-orange-600 text-white rounded-lg text-sm cursor-pointer whitespace-nowrap"
          @click="router.push({ name: 'admin-addcategory' })"
        >
          <i class="pi pi-plus text-xs" />
          新增類別
        </button>
      </div>
    </div>

    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <!-- #endregion -->
    </div>

    <!-- #region  欄位標頭-->
    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <div
        class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-2.5 bg-gray-50 border-b border-gray-100"
      >
        <span class="text-xs text-gray-400"></span>
        <span class="text-xs text-gray-400">類別 ID</span>
        <span class="text-xs text-gray-400">類別名稱</span>
        <span class="text-xs text-gray-400">父類別 ID</span>
        <span class="text-xs text-gray-400"></span>
      </div>

      <!-- #endregion -->
      <!-- #region  類別-->
      <template v-if="isFiltering">
        <div
          v-for="n in 6"
          :key="n"
          class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-4 border-b border-gray-100 gap-4 items-center"
        >
          <Skeleton height="40px" border-radius="50%" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
          <Skeleton height="1rem" />
        </div>
      </template>
      <template v-else>
        <div
          v-if="allCategories.length === 0"
          class="flex flex-col items-center justify-center py-16 text-gray-400"
        >
          <i class="pi pi-inbox text-4xl mb-3" />
          <span class="text-sm">沒有符合條件的類別</span>
        </div>
        <div
          v-for="cate in allCategories"
          :key="cate.ProductCategoryId"
          class="grid grid-cols-[56px_56px_1fr_1fr_80px_90px_150px_80px] px-5 py-4 border-b border-gray-100 items-center hover:bg-gray-50 cursor-pointer"
        >
          <img
            :src="getCategoryImg(cate)"
            class="w-10 h-10 object-cover rounded-lg border border-gray-100 cursor-pointer"
          />
          <span class="text-sm font-medium truncate">{{ cate.productCategoryId }}</span>
          <span class="text-sm font-medium truncate">{{ cate.productCategoryName }}</span>
          <span class="text-sm text-gray-700 truncate">{{ cate.productParentId }}</span>

          <button
            @click="deleteOneCategory(cate.productCategoryId)"
            class="px-3 py-1.5 border border-red-200 rounded-lg text-xs text-red-500 cursor-pointer hover:bg-red-50"
          >
            刪除
          </button>
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
