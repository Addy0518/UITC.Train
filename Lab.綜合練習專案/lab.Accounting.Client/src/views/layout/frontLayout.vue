<script setup>
import { logoutApi } from '@/api/userService';
import { getAllProduct, getCategory } from '@/api/productsService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   baseUrl : 圖片基底位址
   search : 搜尋
   suggestions : 搜尋建議
*/
const authStore = useAuthStore();
const router = useRouter();
const baseUrl = import.meta.env.VITE_IMG_URL;
const search = ref();
const suggestions = ref([]);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   呼叫登出 API , 並退回登入頁面
*/
const logout = async () => {
  await logoutApi();
  authStore.clearAuth();
  router.push({ name: 'login' });
};

/*
   載入頭貼
*/
const imgUrl = computed(() => {
  if (authStore.userHeadshot) {
    return `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
});

/*
   載入搜尋建議
*/
const searchSuggestions = async (event) => {
  if (!event.query) return [];

  try {
    const res = await getAllProduct({ keyWords: event.query, pageSize: 12 });
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
  router.push({ name: 'mall', query: { keyword } });
  search.value = null;
};
</script>

<template>
  <div class="w-screen bg-black">
    <div class="container flex mx-auto h-20 items-center">
      <div class="flex-1 flex items-center gap-6">
        <RouterLink :to="{ name: 'mall' }" class="text-white text-2xl">購物商城</RouterLink>
        <RouterLink :to="{ name: 'backend-layout' }" class="text-white text-2xl"
          >賣家中心</RouterLink
        >
      </div>

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

      <div class="flex-1 flex items-center justify-end gap-6">
        <!-- #region   已登入：顯示用戶名跟登出-->
        <template v-if="authStore.token">
          <div class="relative group">
            <div class="cursor-pointer flex items-center">
              <img :src="imgUrl" alt="頭貼" class="w-10 h-10 rounded-full object-cover" />
              <span class="text-white ps-3 text-xl">{{ authStore.userName }}</span>
            </div>
            <div class="invisible group-hover:visible z-50 absolute bg-white w-40 -mt-1">
              <RouterLink
                :to="{ name: 'user-centre' }"
                class="block px-4 py-3 hover:bg-gray-100 text-sm"
              >
                用戶中心
              </RouterLink>
              <RouterLink
                :to="{ name: 'ledger-centre' }"
                class="block px-4 py-3 hover:bg-gray-100 text-sm"
              >
                帳本管理
              </RouterLink>
              <button
                @click="logout"
                class="block w-full text-left px-4 py-3 hover:bg-gray-100 text-sm"
              >
                登出
              </button>
            </div>
          </div>
          <RouterLink :to="{ name: 'shopping-car' }" class="text-white text-2xl"
            ><i class="pi pi-shopping-cart px-5 ps-5" style="font-size: 1.5rem"></i
          ></RouterLink>
        </template>
        <!-- #endregion -->

        <!-- #region   未登入：顯示註冊登入-->
        <template v-else>
          <RouterLink :to="{ name: 'login' }">
            <strong class="text-white text-xl">登入</strong>
          </RouterLink>
          <strong class="text-white text-xl">/</strong>
          <RouterLink :to="{ name: 'create-account' }">
            <strong class="text-white text-xl">註冊</strong>
          </RouterLink>
        </template>
        <!-- #endregion -->
      </div>
    </div>
  </div>
  <div class="flex">
    <RouterView />
  </div>
</template>
