<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { loginApi } from '@/api/account-api';
import Swal from 'sweetalert2';
// 引入 pinia 的 useAuthStore 來管理登入狀態
const authStore = useAuthStore();
const route = useRouter();
const account = ref(null);
const password = ref(null);

// 測試用帳號
const testUser = () => {
  account.value = 'aaa@gmail.com';
  password.value = 'Andy1111';
};

const userLogin = async () => {
  try {
    const userlogin = { userAccount: account.value, userPassword: password.value };
    const res = await loginApi(userlogin);
    const { data } = res;
    if (data.codeStatus === 2000) {
      authStore.setAuth(data.returnData);
      Swal.fire({
        icon: 'success',
        title: '登入成功!',
      });
      route.push('accounting-practice');
    }
    if (data.codeStatus === 4001) {
      Swal.fire({
        icon: 'error',
        title: data.message,
      });
    }
  } catch (error) {
    console.error('使用者登入錯誤 ', error.response);
  }
};
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
        <InputText v-model="account" placeholder="帳號" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-unlock"></i>
        </InputGroupAddon>
        <InputText v-model="password" placeholder="密碼" />
      </InputGroup>
    </div>
    <div class="justify-end flex mt-5">
      <button
        @click="testUser"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer me-4"
      >
        測試帳號
      </button>
      <button
        @click="userLogin"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        登入
      </button>
    </div>
  </div>
</template>
