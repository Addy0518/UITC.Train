<script setup>
/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   isLogin && isRegister : 判斷註冊還是登入 , 改變按鈕樣式
*/
const authStore = useAuthStore();
const route = useRoute();
const isLogin = computed(() => route.name === 'login' || route.name === 'home');
const isRegister = computed(() => route.name === 'create-Account');

/*
   首頁清空登入狀態
*/
onMounted(() => {
  authStore.clearAuth();
});
</script>
<template>
  <div
    class="min-h-screen w-full bg-center flex items-center justify-center"
    style="background-color: white"
  >
    <div
      class="container mx-auto flex flex-col md:flex-row items-center justify-start px-10 md:pl-24"
    >
      <div class="hidden md:flex flex-1 justify-center items-center">
        <img src="@/img/記帳.png" alt="Logo" class="max-w-2xl object-contain" />
      </div>
      <div class="flex-1 flex justify-center">
        <div
          class="w-full max-w-xl backdrop-blur-sm p-8 border rounded-2xl shadow-2xl overflow-hidden"
        >
          <div class="flex mb-8 border-b w-full">
            <RouterLink
              :to="{ name: 'login' }"
              :class="[
                'flex-1 py-3 text-center transition-all',
                isLogin ? 'border-b-4 border-black font-bold text-xl' : 'text-gray-400',
              ]"
            >
              登入
            </RouterLink>

            <RouterLink
              :to="{ name: 'create-account' }"
              :class="[
                'flex-1 py-3 text-center transition-all',
                isRegister ? 'border-b-4 border-black font-bold text-xl' : 'text-gray-400',
              ]"
            >
              註冊
            </RouterLink>
          </div>

          <RouterView></RouterView>
        </div>
      </div>
    </div>
  </div>
</template>
