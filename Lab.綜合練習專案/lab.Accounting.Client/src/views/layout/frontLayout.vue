<script setup>
import { logoutApi } from '@/api/userService';
import { getAllProduct } from '@/api/productsService';
import defaultImgurl from '@/img/預設圖片.jpg';
import FrontFooter from '@/views/footer/frontFooter.vue';

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   allProductsRaw : 初始資料 ( 全部購物車商品 )
   products : 篩選完的全部購物車商品
   baseUrl : 圖片基底位址
   search : 搜尋
   suggestions : 搜尋建議
*/
const authStore = useAuthStore();
const router = useRouter();
const products = ref([]);
const allProductsRaw = ref();
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
   初始化
*/
onMounted(() => {
  loadShopingCarProducts();
});
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
  const headshot = authStore.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
  return `${baseUrl}/UserHeadShot/${headshot}`;
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

/*
   初始化時加載購物車商品
*/
const loadShopingCarProducts = async () => {
  try {
    showLoading();
    const res = await getAllProductsInShoppingCar();
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProductsRaw.value = data.returnData;

      /*
        在解構的陣列 products 裡面再建立一個陣列 [x.productCategoryName, x] , 為 key 跟 value
        用 map 去除重複的 key 再把陣列轉回 values 陣列
    */
      products.value = [...new Map(allProductsRaw.value.map((x) => [x.productsId, x])).values()];
    } else {
      allProductsRaw.value = [];
      products.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <header class="w-screen bg-surface-dark">
    <!-- #region   主導覽列 -->
    <div class="container flex mx-auto h-20 items-center">
      <div class="flex-1 flex items-center gap-6">
        <RouterLink :to="{ name: 'mall' }" class="text-ink-on-dark text-2xl font-bold">
          購物商城
        </RouterLink>
        <RouterLink
          v-if="authStore.userRole == 'Seller'"
          :to="{ name: 'backend-layout' }"
          class="text-ink-on-dark-muted text-base hover:text-ink-on-dark transition-colors"
        >
          賣家中心
        </RouterLink>
        <RouterLink
          v-if="authStore.userRole == 'Admin'"
          :to="{ name: 'admin-allreview' }"
          class="text-ink-on-dark-muted text-base hover:text-ink-on-dark transition-colors"
        >
          管理中心
        </RouterLink>
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
              <span class="text-ink-on-dark ps-3 text-base">{{ authStore.userName }}</span>
            </div>
            <div
              class="invisible group-hover:visible z-50 absolute bg-page-bg w-40 -mt-1 rounded-card border border-border-soft overflow-hidden"
            >
              <RouterLink
                :to="{ name: 'user-centre' }"
                class="block px-4 py-3 hover:bg-surface-muted text-sm text-ink-900"
              >
                用戶中心
              </RouterLink>
              <RouterLink
                :to="{ name: 'ledger-centre' }"
                class="block px-4 py-3 hover:bg-surface-muted text-sm text-ink-900"
              >
                帳本管理
              </RouterLink>
              <button
                @click="logout"
                class="block w-full text-left px-4 py-3 hover:bg-surface-muted text-sm text-ink-900 cursor-pointer"
              >
                登出
              </button>
            </div>
          </div>

          <RouterLink :to="{ name: 'shopping-car' }" class="relative text-ink-on-dark">
            <i class="pi pi-shopping-cart" style="font-size: 1.5rem"></i>
            <span
              v-if="products.length > 0"
              class="absolute -top-1.5 -right-2 bg-brand-tag text-white text-[10px] font-bold rounded-full flex items-center justify-center px-1"
            >
              {{ products.length > 99 ? '99+' : products.length }}
            </span>
          </RouterLink>

          <button class="relative text-ink-on-dark cursor-pointer" aria-label="通知">
            <i class="pi pi-bell" style="font-size: 1.5rem"></i>
            <span
              v-if="hasUnreadNotification"
              class="absolute -top-0.5 -right-0.5 bg-brand-tag w-2 h-2 rounded-full"
            ></span>
          </button>
        </template>
        <!-- #endregion -->

        <!-- #region   未登入：顯示註冊登入-->
        <template v-else>
          <RouterLink :to="{ name: 'login' }">
            <strong class="text-ink-on-dark text-base">登入</strong>
          </RouterLink>
          <strong class="text-ink-on-dark-faint text-base">/</strong>
          <RouterLink :to="{ name: 'create-account' }">
            <strong class="text-ink-on-dark text-base">註冊</strong>
          </RouterLink>
        </template>
        <!-- #endregion -->
      </div>
    </div>
    <!-- #endregion -->
  </header>
  <main class="flex bg-page-bg">
    <RouterView />
  </main>
  <FrontFooter />
</template>
