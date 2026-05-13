<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const menuItems = ref([
  {
    label: '我的帳戶',
    icon: 'pi pi-user',
    items: [
      { label: '個人檔案', command: () => router.push('/user-centre/profile') },
      { label: '更改密碼', command: () => router.push('/user-centre/update-password') },
    ],
  },
  {
    label: '購買清單',
    icon: 'pi pi-list',
    command: () => router.push('/user-centre/purchase-orders'),
  },
]);
</script>

<template>
  <div class="flex gap-6 p-6">
    <!-- 側邊欄 -->
    <aside class="w-60 shrink-0">
      <PanelMenu :model="menuItems" />
    </aside>

    <!-- 內容區 -->
    <main class="flex-1">
      <RouterView />
    </main>
  </div>
</template>
