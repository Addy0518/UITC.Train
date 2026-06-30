<script setup>
import { logoutApi } from '@/api/userService';
import backendSidebar from '@/views/layout/backendSidebar.vue';

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   baseUrl : 圖片基底位址
*/
const authStore = useAuthStore();
const router = useRouter();
const baseUrl = import.meta.env.VITE_IMG_URL;

/*
   呼叫登出 API , 並退回登入頁面
*/
const logout = async () => {
  await logoutApi();
  authStore.clearAuth();
  router.push({ name: 'login' });
};

/*
   大頭照
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
</script>

<template>
  <div class="w-screen bg-surface-dark">
    <!-- #region  Layout 區-->
    <div class="container flex mx-auto h-20 items-center justify-center gap-6">
      <RouterLink :to="{ name: 'mall' }" class="text-ink-on-dark text-2xl">回到商城</RouterLink>

      <div class="flex-1 flex items-center justify-end gap-6">
        <!-- #region  已登入：顯示用戶名跟登出-->
        <template v-if="authStore.token">
          <div class="relative group">
            <div class="cursor-pointer flex items-center">
              <img :src="imgUrl" alt="頭貼" class="w-10 h-10 rounded-full object-cover" />
              <span class="text-ink-on-dark ps-3 text-xl">{{ authStore.userName }}</span>
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
        </template>
        <!-- #endregion -->
        <!-- #region  未登入：顯示註冊登入-->
        <template v-else>
          <RouterLink :to="{ name: 'login' }">
            <strong class="text-ink-on-dark text-xl">登入</strong>
          </RouterLink>
          <strong class="text-ink-on-dark-faint text-xl">/</strong>
          <RouterLink :to="{ name: 'create-account' }">
            <strong class="text-ink-on-dark text-xl">註冊</strong>
          </RouterLink>
        </template>
        <!-- #endregion -->
      </div>
    </div>
    <!-- #endregion -->
  </div>
  <div class="flex bg-page-bg">
    <backendSidebar class="w-80 h-screen" />

    <RouterView />
  </div>
</template>
