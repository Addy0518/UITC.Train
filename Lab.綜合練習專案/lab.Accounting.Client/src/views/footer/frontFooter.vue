<script setup>
const currentYear = ref(new Date().getFullYear());
</script>

<template>
  <footer class="bg-[#f7f6f1] pt-12 pb-5 px-5 font-sans mt-60">
    <ScrollTop />
    <div class="max-w-5xl mx-auto">
      <div class="flex flex-wrap justify-between gap-y-8 mb-10">
        <div class="flex-1">
          <h4 class="text-base font-semibold text-gray-900 mb-5">關於我們</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >公司介紹</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >加入我們</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >聯絡資訊</router-link
              >
            </li>
          </ul>
        </div>

        <div class="flex-1">
          <h4 class="text-base font-semibold text-gray-900 mb-5">客戶服務</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >常見問題</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >退換貨政策</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >運送說明</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'mall' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >聯繫客服</router-link
              >
            </li>
          </ul>
        </div>

        <div class="flex-1 min-w-[150px]">
          <h4 class="text-base font-semibold text-gray-900 mb-5">會員專區</h4>
          <ul class="space-y-3">
            <li>
              <router-link
                :to="{ name: 'profile' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >我的訂單</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'wallet' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >我的優惠券</router-link
              >
            </li>
            <li>
              <router-link
                :to="{ name: 'help' }"
                class="text-sm text-gray-600 hover:text-black hover:font-medium transition-colors"
                >會員條款</router-link
              >
            </li>
          </ul>
        </div>

        <div class="flex-1 min-w-[150px]">
          <h4 class="text-base font-semibold text-gray-900 mb-5">追蹤我們</h4>
          <div class="flex gap-4">
            <a
              href="#"
              class="flex items-center justify-center w-10 h-10 bg-white rounded-full text-gray-800 font-bold text-sm shadow-sm hover:-translate-y-1 transition-transform"
              >f</a
            >
            <a
              href="#"
              class="flex items-center justify-center w-10 h-10 bg-white rounded-full text-gray-800 font-bold text-sm shadow-sm hover:-translate-y-1 transition-transform"
              >ig</a
            >
            <a
              href="#"
              class="flex items-center justify-center w-10 h-10 bg-white rounded-full text-gray-800 font-bold text-sm shadow-sm hover:-translate-y-1 transition-transform"
              >💬</a
            >
          </div>
        </div>
      </div>

      <hr class="border-t border-gray-300 mb-5" />

      <div class="flex justify-between items-center flex-wrap gap-4 text-sm text-gray-500">
        <p>&copy; {{ currentYear }} 購物商城. All rights reserved.</p>

        <div class="flex gap-3">
          <span
            class="bg-white px-4 py-1.5 rounded shadow-sm flex items-center justify-center text-sm"
            >💳</span
          >
          <span
            class="bg-white px-4 py-1.5 rounded shadow-sm flex items-center justify-center text-sm font-bold text-gray-800 tracking-widest"
            >綠界</span
          >
        </div>
      </div>
    </div>
  </footer>
</template>
