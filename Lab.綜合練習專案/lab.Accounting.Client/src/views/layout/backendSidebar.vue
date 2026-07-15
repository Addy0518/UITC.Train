<script setup>
import { useAuthStore } from '@/stores/auth';
import { onMounted } from 'vue';

/*
   變數名稱代表意義
   authStore : pinia
*/
const authStore = useAuthStore();

/*
   初始化拿到角色權限
*/
onMounted(() => {});
</script>

<template>
  <div class="w-80">
    <div class="w-80 h-full border-r border-border-soft bg-page-bg p-3 flex flex-col gap-1">
      <!-- #region  賣家後台區-->
      <template v-if="authStore.userRole == 'Seller'">
        <RouterLink
          :to="{ name: 'seller-store-edit' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-shop"></i>賣場管理
        </RouterLink>
        <RouterLink
          :to="{ name: 'seller-data' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-chart-scatter"></i>賣場數據
        </RouterLink>
        <RouterLink
          :to="{ name: 'seller-product' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-box"></i>商品管理
        </RouterLink>

        <RouterLink
          :to="{ name: 'add-product' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-plus"></i>新增商品
        </RouterLink>

        <RouterLink
          :to="{ name: 'seller-product-order' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-wallet"></i>訂單管理
        </RouterLink>
        <RouterLink
          :to="{ name: 'seller-coupons' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-ticket"></i>優惠卷列表
        </RouterLink>
        <RouterLink
          :to="{ name: 'seller-product-recycling' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-trash"></i>商品回收
        </RouterLink>
      </template>
      <!-- #endregion -->

      <!-- #region  管理者後台區-->
      <template v-if="authStore.userRole == 'Admin'">
        <RouterLink
          :to="{ name: 'admin-allreview' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-list-check"></i>審查列表
        </RouterLink>
        <RouterLink
          :to="{ name: 'admin-alluser' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-user"></i>用戶列表
        </RouterLink>
        <RouterLink
          :to="{ name: 'admin-allCategories' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-sitemap"></i>類別列表
        </RouterLink>
        <RouterLink
          :to="{ name: 'admin-allcoupons' }"
          class="w-auto p-3 text-base flex items-center gap-3 rounded-card text-ink-500 hover:bg-surface-muted hover:text-ink-900 transition-colors"
          active-class="bg-brand-50 text-brand-500 font-medium"
        >
          <i class="pi pi-ticket"></i>優惠卷列表
        </RouterLink>
      </template>
      <!-- #endregion -->
    </div>
  </div>
</template>
