<script setup>
const currentYear = ref(new Date().getFullYear());
</script>

<template>
  <footer class="bg-page-bg-soft pt-12 pb-5 px-5 font-sans mt-60">
    <ScrollTop />
    <div class="max-w-7xl mx-auto">
      <!-- #region  四欄連結區 -->
      <div class="grid grid-cols-4 gap-8 mb-10">
        <!-- #region  關於我們 -->
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-900 mb-5">關於我們</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >公司介紹</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >加入我們</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >聯絡資訊</router-link
              >
            </li>
          </ul>
        </div>
        <!-- #endregion -->

        <!-- #region  客戶服務 -->
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-900 mb-5">客戶服務</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >常見問題</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >退換貨政策</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >運送說明</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >聯繫客服</router-link
              >
            </li>
          </ul>
        </div>
        <!-- #endregion -->

        <!-- #region  會員專區 -->
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-900 mb-5">會員專區</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'profile' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >我的訂單</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'wallet' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >我的優惠券</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'help' }"
                class="text-sm text-ink-500 hover:text-ink-900 hover:font-medium transition-colors"
                >會員條款</router-link
              >
            </li>
          </ul>
        </div>
        <!-- #endregion -->

        <!-- #region  追蹤我們 -->
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-900 mb-5">追蹤我們</h4>
          <div class="flex gap-3">
            <i class="pi pi-facebook" style="font-size: 1rem"></i>

            <i class="pi pi-instagram" style="font-size: 1rem"></i>

            <i class="pi pi-comments" style="font-size: 1rem"></i>
          </div>
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->

      <hr class="border-t border-border-soft mb-5" />

      <!-- #region  版權與付款方式列 -->
      <div class="flex justify-between items-center flex-wrap gap-4 text-sm text-ink-500">
        <p>&copy; {{ currentYear }} 購物商城. All rights reserved.</p>

        <div class="flex gap-3">
          <span
            class="bg-page-bg border border-border-soft px-4 py-1.5 rounded-card flex items-center justify-center text-sm"
          >
            <i class="pi pi-credit-card" style="font-size: 0.875rem"></i>
          </span>
          <span
            class="bg-page-bg border border-border-soft px-4 py-1.5 rounded-card flex items-center justify-center text-sm font-bold text-ink-900 tracking-widest"
            >綠界</span
          >
        </div>
      </div>
      <!-- #endregion -->
    </div>
  </footer>
</template>
