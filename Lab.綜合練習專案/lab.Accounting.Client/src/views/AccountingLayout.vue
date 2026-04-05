<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import { logoutApi } from '@/api/account-api';
// 引入 pinia 的 useAuthStore 來管理登入狀態
const authStore = useAuthStore();
const router = useRouter();
const visible = ref(false);
// 登出
const logout = async () => {
  const res = await logoutApi();
  console.log('登出', res);
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
  <RouterView></RouterView>
</template>
