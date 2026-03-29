<script setup>
import { ref, watch } from 'vue';
const account = ref();
const password = ref();
const name = ref();
const phone = ref();


</script>

<template>
  <div class="container mx-auto p-10">
    <p class="text-center mb-10 text-3xl font-bold">註冊帳號</p>

    <!-- 帳號欄位 -->
    <div class="card grid grid-cols-1 gap-4 gap-y-10">
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-user"></i>
        </InputGroupAddon>
        <InputText v-model="account" placeholder="帳號" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-unlock"></i>
        </InputGroupAddon>
        <InputText v-model="password" placeholder="密碼" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-id-card"></i>
        </InputGroupAddon>
        <InputNumber v-model="name" placeholder="姓名" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-phone"></i>
        </InputGroupAddon>
        <InputNumber v-model="phone" placeholder="電話" />
      </InputGroup>
    </div>
    <div class="justify-end flex mt-5">
      <button
        @click="confirm1($event)"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        註冊
      </button>
    </div>
  </div>
</template>
