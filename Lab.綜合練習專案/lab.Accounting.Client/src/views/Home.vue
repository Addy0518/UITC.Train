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
  <div class="min-h-screen w-full bg-page-bg flex items-center justify-center">
    <!--#region 首頁 -->
    <div
      class="container mx-auto flex flex-col md:flex-row items-center justify-start px-10 md:pl-24"
    >
      <div class="hidden md:flex flex-1 justify-center items-center">
        <!-- 圖片預留位置，建議放品牌插圖或購物主題視覺，比例可參考規範的廣告 Banner 16:6 或自由方形構圖 -->
        <div
          class="w-full max-w-lg aspect-square rounded-card bg-surface-muted flex items-center justify-center"
        >
          <span class="text-sm text-ink-500">圖片預留位置</span>
        </div>
      </div>
      <div class="flex-1 flex justify-center">
        <!--#region 登入註冊 -->
        <div
          class="w-full max-w-xl p-8 border border-border-soft rounded-card overflow-hidden bg-page-bg"
        >
          <div class="flex mb-8 border-b border-border-soft w-full">
            <RouterLink
              :to="{ name: 'login' }"
              :class="[
                'flex-1 py-3 text-center transition-all',
                isLogin
                  ? 'border-b-4 border-brand-500 font-bold text-xl text-ink-900'
                  : 'text-ink-300',
              ]"
            >
              登入
            </RouterLink>

            <RouterLink
              :to="{ name: 'create-account' }"
              :class="[
                'flex-1 py-3 text-center transition-all',
                isRegister
                  ? 'border-b-4 border-brand-500 font-bold text-xl text-ink-900'
                  : 'text-ink-300',
              ]"
            >
              註冊
            </RouterLink>
          </div>

          <RouterView></RouterView>
        </div>
        <!-- #endregion -->
      </div>
    </div>
    <!-- #endregion -->
  </div>
</template>
