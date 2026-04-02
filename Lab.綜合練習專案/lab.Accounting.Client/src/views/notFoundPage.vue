<script setup>
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
</script>

<template>
  <div
    class="min-h-screen w-full bg-center flex items-center justify-center"
    style="background-color: white"
  >
    <div class="container mx-auto md:flex-row items-center px-10">
      <div class="hidden md:flex flex-1 justify-center items-center">
        <img src="@/img/404.png" alt="Logo" class="max-w-2xl object-contain" />
      </div>
      <div class="flex-1 flex justify-center">
        <div class="border-4 rounded-xl p-5">
          <RouterLink :to="{ name: 'home' }" :class="['py-3 text-center']">
            <button class="cursor-pointer">回到首頁</button>
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>
