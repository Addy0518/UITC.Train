<script setup>
import { ref } from 'vue';
const account = ref();
const password = ref();
</script>

<template>
  <div class="container mx-auto p-10">
    <p class="text-center mb-10 text-3xl font-bold">登入帳號</p>

    <!-- 帳號欄位 -->
    <div class="card grid grid-cols-1 gap-4 gap-y-10">
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-user"></i>
        </InputGroupAddon>
        <InputText v-model="text1" placeholder="帳號" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-unlock"></i>
        </InputGroupAddon>
        <InputNumber v-model="number" placeholder="密碼" />
      </InputGroup>
    </div>
    <div class="justify-end flex mt-5">
      <button
        @click="confirm1($event)"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        登入
      </button>
    </div>
  </div>
</template>
