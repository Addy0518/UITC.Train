<script setup>
const currentYear = ref(new Date().getFullYear());
</script>

<template>
  <footer class="bg-surface-dark pt-12 pb-5 px-5 font-sans">
    <ScrollTop />
    <div class="max-w-7xl mx-auto">
      <div class="grid grid-cols-4 gap-8 mb-10">
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-on-dark mb-5">關於我們</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >公司介紹</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >加入我們</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >聯絡資訊</router-link
              >
            </li>
          </ul>
        </div>
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-on-dark mb-5">客戶服務</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >常見問題</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >退換貨政策</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >運送說明</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >聯繫客服</router-link
              >
            </li>
          </ul>
        </div>
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-on-dark mb-5">會員專區</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'profile' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >我的訂單</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'wallet' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >我的優惠券</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'help' }"
                class="text-sm text-ink-on-dark-muted hover:text-ink-on-dark hover:font-medium transition-colors"
                >會員條款</router-link
              >
            </li>
          </ul>
        </div>
        <div class="flex-1">
          <h4 class="text-base font-semibold text-ink-on-dark mb-5">追蹤我們</h4>
          <div class="flex gap-3 text-ink-on-dark-muted">
            <i
              class="pi pi-facebook hover:text-ink-on-dark transition-colors cursor-pointer"
              style="font-size: 1.25rem"
            ></i>
            <i
              class="pi pi-instagram hover:text-ink-on-dark transition-colors cursor-pointer"
              style="font-size: 1.25rem"
            ></i>
            <i
              class="pi pi-comments hover:text-ink-on-dark transition-colors cursor-pointer"
              style="font-size: 1.25rem"
            ></i>
          </div>
        </div>
      </div>
      <hr class="border-t border-ink-on-dark-faint mb-5" />

      <div class="flex justify-between items-center flex-wrap gap-4 text-sm text-ink-on-dark-muted">
        <p>&copy; {{ currentYear }} 購物商城. All rights reserved.</p>

        <div class="flex gap-3">
          <span
            class="bg-page-bg text-ink-900 px-4 py-1.5 rounded-card flex items-center justify-center text-sm shadow-none"
          >
            <i class="pi pi-credit-card" style="font-size: 0.875rem"></i>
          </span>
          <span
            class="bg-page-bg text-ink-900 px-4 py-1.5 rounded-card flex items-center justify-center text-sm font-bold tracking-widest shadow-none"
            >綠界</span
          >
        </div>
      </div>
    </div>
  </footer>
</template>
