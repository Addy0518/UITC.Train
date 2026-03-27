<script setup>
import { ref } from 'vue';

const visible = ref(false);
</script>

<template>
  <!-- 導覽列 -->
  <div class="w-full bg-black">
    <div class="container flex mx-auto h-20 items-center">
      <strong class="text-white text-2xl">記帳</strong>
      <div class="flex-1 flex items-center justify-end gap-6">
        <RouterLink :to="{ name: 'login' }">
          <strong class="text-white text-2xl">登入</strong>
        </RouterLink>
        <RouterLink :to="{ name: 'createaccount' }">
          <strong class="text-white text-2xl">註冊</strong>
        </RouterLink>
      </div>
    </div>
  </div>

  <RouterView></RouterView>
</template>
