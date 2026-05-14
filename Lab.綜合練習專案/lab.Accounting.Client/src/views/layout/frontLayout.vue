<script setup>
import { logoutApi } from '@/api/userService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
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
   載入頭貼
*/
const imgUrl = computed(() => {
  if (authStore.userHeadshot) {
    return `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
});
</script>

<template>
  <div class="w-screen bg-black">
    <div class="container flex mx-auto h-20 items-center justify-center gap-6">
      <RouterLink :to="{ name: 'mall' }" class="text-white text-2xl">購物商城</RouterLink>
      <RouterLink :to="{ name: 'backend-layout' }" class="text-white text-2xl">賣家中心</RouterLink>

      <div class="flex-1 flex items-center justify-end gap-6">
        <!-- 已登入：顯示用戶名跟登出 -->
        <template v-if="authStore.token">
          <div class="relative group">
            <div class="cursor-pointer flex items-center">
              <!-- 顯示照片 -->
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

        <!-- 未登入：顯示註冊登入 -->
        <template v-else>
          <RouterLink :to="{ name: 'login' }">
            <strong class="text-white text-xl">登入</strong>
          </RouterLink>
          <strong class="text-white text-xl">/</strong>
          <RouterLink :to="{ name: 'create-account' }">
            <strong class="text-white text-xl">註冊</strong>
          </RouterLink>
        </template>
      </div>
    </div>
  </div>
  <div class="flex">
    <!-- <sidebar class="w-80 h-screen" /> -->

    <RouterView />
  </div>
</template>
