<script setup>
import { logoutApi } from '@/api/userService';
import { getAllProduct } from '@/api/productsService';
import { getAllNotifications, getNotification } from '@/api/notificationService';
import defaultImgurl from '@/img/預設圖片.jpg';
import FrontFooter from '@/views/footer/frontFooter.vue';

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   allProductsRaw : 初始資料 ( 全部購物車商品 )
   baseUrl : 圖片基底位址
   search : 搜尋
   suggestions : 搜尋建議
   notifications       : 預覽用通知（最新 5 筆）
   showNotification    : 通知 dropdown 開關
   notifPanelRef       : 通知面板 DOM ref，用於 click outside 判斷
*/
const authStore = useAuthStore();
const router = useRouter();
const allProductsRaw = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const search = ref();
const suggestions = ref([]);
const notifications = ref([]);
const showNotification = ref(false);
const notifPanelRef = ref(null);

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
  if (authStore.token) {
    loadNotifications();
  }
  document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
});

/*
   點擊 dropdown 外側時關閉
*/
const handleClickOutside = (e) => {
  if (notifPanelRef.value && !notifPanelRef.value.contains(e.target)) {
    showNotification.value = false;
  }
};

/*
   是否有未讀通知（控制鈴鐺紅點）
*/
const hasUnreadNotification = computed(() => notifications.value.some((n) => !n.isRead));

/*
   通知類型對應 badge 文字與樣式
*/
const notifBadge = (type) => {
  if ([5, 6].includes(type)) return { label: '訂單', style: 'background:#fff4ed; color:#ff6b35;' };
  if ([1, 2, 8].includes(type))
    return { label: '審核', style: 'background:#f1efe8; color:#888780;' };
  if ([3, 4].includes(type)) return { label: '商店', style: 'background:#f1efe8; color:#888780;' };
  if ([7].includes(type)) return { label: '評價', style: 'background:#fff4ed; color:#c9543f;' };
  return { label: '通知', style: 'background:#f1efe8; color:#888780;' };
};

/*
   載入最新 5 筆通知（預覽用）
*/
const loadNotifications = async () => {
  try {
    const res = await getAllNotifications({ pageIndex: 0, pageSize: 5 });
    const { data } = res;
    if (data.codeStatus === 2000) {
      notifications.value = data.returnData.notifications;
    }
  } catch (err) {
    console.log(err);
  }
};

/*
   切換通知 dropdown（同時重新拉取最新資料）
*/
const toggleNotification = async () => {
  showNotification.value = !showNotification.value;
  if (showNotification.value) {
    await loadNotifications();
  }
};

/*
   點擊單一通知：標記已讀 + 根據類型跳轉
*/
const readNotification = async (notif) => {
  try {
    await getNotification(notif.notificationId);
    notif.isRead = true;
    showNotification.value = false;

    // 根據通知類型導向對應頁面
    if ([5, 6].includes(notif.notificationType) && notif.relatedId) {
      router.push({ name: 'order-detail', params: { id: notif.relatedId } });
    } else if ([1, 2, 8].includes(notif.notificationType) && notif.relatedId) {
      router.push({ name: 'edit-product', params: { id: notif.relatedId } });
    } else if ([3, 4].includes(notif.notificationType)) {
      router.push({ name: 'store-edit' });
    }
  } catch (err) {
    console.log(err);
  }
};

/*
   全部標為已讀
*/
const markAllRead = async () => {
  try {
    await Promise.all(
      notifications.value.filter((n) => !n.isRead).map((n) => getNotification(n.notificationId)),
    );
    notifications.value.forEach((n) => (n.isRead = true));
  } catch (err) {
    console.log(err);
  }
};
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
    <!--#region 主導覽列 -->
    <div class="container flex mx-auto h-20 items-center">
      <div class="flex-1 flex items-center gap-6">
        <RouterLink :to="{ name: 'mall' }" class="text-ink-on-dark text-2xl font-bold">
          購物商城
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
        <!--#region 已登入 -->
        <template v-if="authStore.token">
          <!--#region 用戶選單 -->
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
                v-if="authStore.userRole == 'Seller'"
                :to="{ name: 'backend-layout' }"
                class="block px-4 py-3 hover:bg-surface-muted text-sm text-ink-900"
              >
                賣家中心
              </RouterLink>
              <RouterLink
                v-else-if="authStore.userRole != 'Admin'"
                :to="{ name: 'store-register' }"
                class="block px-4 py-3 hover:bg-surface-muted text-sm text-ink-900"
              >
                成為賣家
              </RouterLink>
              <RouterLink
                v-if="authStore.userRole == 'Admin'"
                :to="{ name: 'admin-allreview' }"
                class="block px-4 py-3 hover:bg-surface-muted text-sm text-ink-900"
              >
                管理中心
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
          <!-- #endregion -->

          <!--#region 購物車 -->
          <RouterLink :to="{ name: 'shopping-car' }" class="relative text-ink-on-dark">
            <i class="pi pi-shopping-cart" style="font-size: 1.5rem"></i>
            <span
              v-if="allProductsRaw.length > 0"
              class="absolute -top-1.5 -right-2 bg-brand-tag text-white text-[10px] font-bold rounded-full flex items-center justify-center px-1"
            >
              {{ allProductsRaw.length > 99 ? '99+' : allProductsRaw.length }}
            </span>
          </RouterLink>
          <!-- #endregion -->

          <!--#region 通知鈴鐺 + dropdown -->
          <div class="relative" ref="notifPanelRef">
            <button
              class="relative text-ink-on-dark cursor-pointer"
              aria-label="通知"
              @click.stop="toggleNotification"
            >
              <i class="pi pi-bell" style="font-size: 1.5rem"></i>
              <span
                v-if="hasUnreadNotification"
                class="absolute -top-0.5 -right-0.5 bg-brand-tag w-2 h-2 rounded-full"
              ></span>
            </button>

            <!--#region 通知預覽面板 -->
            <div
              v-if="showNotification"
              class="absolute right-0 top-10 z-50 w-80 bg-page-bg border border-border-soft rounded-card overflow-hidden"
              style="box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1)"
            >
              <div class="px-4 py-3 border-b border-border-soft flex items-center justify-between">
                <span class="text-sm font-medium text-ink-900">通知</span>
                <button
                  v-if="hasUnreadNotification"
                  @click="markAllRead"
                  class="text-xs text-brand-500 cursor-pointer hover:opacity-80"
                >
                  全部標為已讀
                </button>
              </div>

              <div v-if="notifications.length === 0" class="py-8 text-center text-xs text-ink-500">
                目前沒有通知
              </div>

              <!--#region 通知項目 -->
              <div
                v-for="notif in notifications"
                :key="notif.notificationId"
                @click="readNotification(notif)"
                class="px-4 py-3 border-b border-border-soft flex gap-3 items-start cursor-pointer transition-colors"
                :class="notif.isRead ? 'hover:bg-surface-muted' : 'bg-[#FFF4ED] hover:bg-[#FFE8DA]'"
              >
                <div
                  class="w-2 h-2 rounded-full mt-1.5 shrink-0"
                  :class="notif.isRead ? 'bg-transparent' : 'bg-brand-500'"
                ></div>
                <div class="flex-1 min-w-0">
                  <span
                    class="inline-block text-[10px] px-2 py-0.5 rounded-full mb-1"
                    :style="notifBadge(notif.notificationType).style"
                  >
                    {{ notifBadge(notif.notificationType).label }}
                  </span>
                  <p class="text-xs font-medium text-ink-900 m-0 truncate">
                    {{ notif.title }}
                  </p>
                </div>
              </div>
              <!-- #endregion -->

              <div class="px-4 py-2.5 text-center border-t border-border-soft">
                <RouterLink
                  :to="{ name: 'notifications' }"
                  class="text-xs text-brand-500 hover:opacity-80"
                  @click="showNotification = false"
                >
                  查看全部通知
                </RouterLink>
              </div>
            </div>
            <!-- #endregion -->
          </div>
          <!-- #endregion -->
        </template>
        <!-- #endregion -->

        <!--#region 未登入 -->
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
