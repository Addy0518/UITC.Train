<script setup>
const router = useRouter();

const menuItems = ref([
  {
    label: '帳本管理',
    icon: 'pi pi-user',
    command: () => router.push('/ledger-centre/ledger'),
    items: [
      { label: '新增帳本', command: () => router.push('/ledger-centre/add-ledger') },
      { label: '帳本回收', command: () => router.push('/ledger-centre/ledger-recycling') },
      { label: '帳本圖表', command: () => router.push('/ledger-centre/ledger-chart') },
    ],
  },
]);
</script>

<template>
  <div class="container mx-auto">
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
  </div>
</template>
