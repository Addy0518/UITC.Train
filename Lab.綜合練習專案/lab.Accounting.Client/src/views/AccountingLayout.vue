<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import { logoutApi } from '@/api/account-api';
import sidebar from '@/views/sidebar.vue';

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
*/
const authStore = useAuthStore();
const router = useRouter();

/*
   呼叫登出 API , 並退回登入頁面
*/
const logout = async () => {
  await logoutApi();
  authStore.clearAuth();
  router.push('login');
};
</script>

<template>
  <div class="w-full bg-black">
    <div class="container flex mx-auto h-20 items-center">
      <strong class="text-white text-2xl">記帳</strong>
      <div class="flex-1 flex items-center justify-end gap-6">
        <!-- 已登入：顯示用戶名跟登出 -->
        <template v-if="authStore.token">
          <strong class="text-white text-2xl">歡迎 : {{ authStore.userName }}</strong>
          <Button @click="logout">
            <strong class="text-white text-2xl">登出</strong>
          </Button>
        </template>

        <!-- 未登入：顯示註冊登入 -->
        <template v-else>
          <RouterLink :to="{ name: 'createaccount' }">
            <strong class="text-white text-2xl">註冊</strong>
          </RouterLink>
          <RouterLink :to="{ name: 'login' }">
            <strong class="text-white text-2xl">登入</strong>
          </RouterLink>
        </template>
      </div>
    </div>
  </div>
  <div class="flex">
    <sidebar v-if="authStore.token" class="w-80 h-screen" />

    <RouterView />
  </div>
</template>
