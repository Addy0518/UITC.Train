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
   初始化時首頁清空登入狀態
*/
onMounted(() => {
  authStore.clearAuth();
});
</script>
<template>
  <div class="min-h-screen w-full bg-page-bg-soft flex items-center justify-center font-sans">
    <div
      class="container mx-auto flex flex-col md:flex-row items-center justify-start px-10 md:pl-24 gap-10 md:gap-0"
    >
      <div class="hidden md:flex flex-1 justify-center items-center">
        <img
          src="@/img/登入頁面插圖.png"
          alt="品牌插圖"
          class="w-full  aspect-square object-contain"
        />
      </div>
      <div class="flex-1 flex justify-center">
        <div
          class="w-full max-w-xl p-8 border border-border-soft rounded-card overflow-hidden bg-page-bg shadow-none"
        >
          <div class="flex mb-8 border-b border-border-soft w-full">
            <RouterLink
              :to="{ name: 'login' }"
              :class="[
                'flex-1 py-3 text-center transition-all cursor-pointer',
                isLogin
                  ? 'border-b-4 border-brand-500 font-bold text-20px text-ink-900'
                  : 'text-ink-300 hover:text-ink-500',
              ]"
            >
              登入
            </RouterLink>

            <RouterLink
              :to="{ name: 'create-account' }"
              :class="[
                'flex-1 py-3 text-center transition-all cursor-pointer',
                isRegister
                  ? 'border-b-4 border-brand-500 font-bold text-20px text-ink-900'
                  : 'text-ink-300 hover:text-ink-500',
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
