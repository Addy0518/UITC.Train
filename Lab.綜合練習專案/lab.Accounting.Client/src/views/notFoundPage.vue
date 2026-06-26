<script setup></script>
/* 404 導向頁 */
<template>
  <!--#region 主容器 -->
  <div class="min-h-screen w-full bg-page-bg flex items-center justify-center">
    <div class="container mx-auto md:flex-row items-center px-10">
      <!--#region Logo 圖片區 -->
      <div class="hidden md:flex flex-1 justify-center items-center">
        <img src="@/img/404.png" alt="Logo" class="max-w-2xl object-contain" />
      </div>
      <!-- #endregion -->

      <!--#region 返回按鈕區 -->
      <div class="flex-1 flex justify-center">
        <div class="bg-page-bg border border-border-soft rounded-card p-5">
          <RouterLink :to="{ name: 'home' }">
            <button
              class="bg-ink-900 hover:opacity-90 text-white px-8 py-2.5 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              回到登入頁
            </button>
          </RouterLink>
        </div>
      </div>
      <!-- #endregion -->
    </div>
  </div>
  <!-- #endregion -->
</template>
