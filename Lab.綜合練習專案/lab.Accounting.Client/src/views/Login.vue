<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { loginApi } from '@/api/account-api';

import Swal from 'sweetalert2';

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   account : 帳號
   password : 密碼
   errorAccount : 帳號錯誤警告
   errorPassword :　密碼錯誤警告
   tooglePassword　：　切換密碼顯示或隱藏
   emailPattern : 帳號格式正規範
   passwordPattern : 密碼格式正規範
*/
const authStore = useAuthStore();
const route = useRouter();
const account = ref(null);
const password = ref(null);

let errorAccount = ref();
let errorPassword = ref();
const tooglePassword = ref(true);
const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const passwordPattern = /^[A-Z][A-Za-z0-9]{7}$/;

/*
  驗證帳號格式
*/
const validateAccount = () => {
  if (!account.value) {
    errorAccount.value = '名稱不能為空!';
  } else if (!emailPattern.test(account.value)) {
    errorAccount.value = '帳號格式不對!';
  } else {
    errorAccount.value = '';
  }
};

/*
  驗證密碼格式
*/
const validatePassword = () => {
  if (!password.value) {
    errorPassword.value = '名稱不能為空!';
  } else if (!passwordPattern.test(password.value)) {
    errorPassword.value = '總共 8 個字 , 只能輸入英文跟數字 , 第一個字要大寫';
  } else {
    errorPassword.value = '';
  }
};

/*
  測試用帳號
*/
const testUser = () => {
  account.value = 'andy@gmail.com';
  password.value = 'Andy1111';
};

/*
  呼叫登入使用者 API
*/
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
      route.push('mall');
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

    <!-- 帳號跟密碼欄位 -->
    <div class="card grid grid-cols-1 gap-4 gap-y-10">
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-user"></i>
        </InputGroupAddon>
        <InputText v-model="account" placeholder="帳號" @input="validateAccount" />
      </InputGroup>
      <span v-if="errorAccount" class="text-red-700 font-semibold text-lg">{{ errorAccount }}</span>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-unlock"></i>
        </InputGroupAddon>
        <InputText
          :type="tooglePassword ? 'password' : 'text'"
          v-model="password"
          placeholder="密碼"
          @input="validatePassword"
        />
        <InputGroupAddon class="cursor-pointer" @click="tooglePassword = !tooglePassword">
          <i :class="['pi', tooglePassword ? 'pi-eye' : 'pi-eye-slash']"></i>
        </InputGroupAddon>
      </InputGroup>
      <span v-if="errorPassword" class="text-red-700 font-semibold text-lg">{{
        errorPassword
      }}</span>
    </div>
    <!-- 按鈕區 -->
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
