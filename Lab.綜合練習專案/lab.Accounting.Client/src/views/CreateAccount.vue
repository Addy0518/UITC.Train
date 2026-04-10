<script setup>
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { registerApi } from '@/api/account-api';
import Swal from 'sweetalert2';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   account : 帳號
   password : 密碼
   name : 名稱
   phone : 電話
   errorName : 名稱錯誤警告
   errorPhone : 電話錯誤警告
   errorAccount : 帳號錯誤警告
   errorPassword :　密碼錯誤警告
   tooglePassword　：　切換密碼顯示或隱藏
   emailPattern : 帳號格式正規範
   passwordPattern : 密碼格式正規範
   phonePattern : 電話格式正規範
*/
const route = useRouter();
const account = ref();
const password = ref();
const name = ref();
const phone = ref();

let errorName = ref();
let errorPhone = ref();
let errorAccount = ref();
let errorPassword = ref();

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const passwordPattern = /^[A-Z][A-Za-z0-9]{7}$/;
const phonePattern = /^[0-9]$/;

const tooglePassword = ref(true);

/*
  驗證名稱格式
*/
const validateUserName = () => {
  if (!name.value) {
    errorName.value = '名稱不能為空!';
  } else {
    errorName.value = '';
  }
};

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
  驗證電話格式
*/
const validatePhone = () => {
  if (!phonePattern.test(phone.value)) {
    errorPhone.value = '只能輸入數字!';
  } else {
    errorPhone.value = '';
  }
};

/*
  呼叫註冊使用者 API
*/
const userRegister = async () => {
  try {
    const userRegisterData = {
      userAccount: account.value,
      userPassword: password.value,
      userName: name.value,
      userPhone: phone.value,
    };
    const res = await registerApi(userRegisterData);
    const { data } = res;
    if (data.codeStatus === 2000) {
      Swal.fire({
        icon: 'success',
        title: '註冊成功!',
      });
      route.push('/login');
    } else if (data.codeStatus === 4000) {
      const errorMsg = data.error400.UserAccount;
      Swal.fire({
        icon: 'error',
        title: errorMsg,
      });
    } else {
      Swal.fire({
        icon: 'question',
        title: `意外成功:${data.codeStatus}`,
      });
    }
  } catch (error) {
    console.error('使用者註冊錯誤 ', error.response);
  }
};
</script>

<template>
  <div class="container mx-auto p-10">
    <p class="text-center mb-10 text-3xl font-bold">註冊帳號</p>

    <!-- 欄位區 -->
    <div class="card grid grid-cols-1 gap-4 gap-y-5">
      <!-- 帳號跟密碼 -->
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
      <!-- 名稱跟電話 -->
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-id-card"></i>
        </InputGroupAddon>
        <InputText v-model="name" placeholder="姓名" @input="validateUserName" />
      </InputGroup>
      <span v-if="errorName" class="text-red-700 font-semibold text-lg">{{ errorName }}</span>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-phone"></i>
        </InputGroupAddon>
        <InputText v-model="phone" placeholder="電話" @input="validatePhone" />
      </InputGroup>
      <span v-if="errorPhone" class="text-red-700 font-semibold text-lg">{{ errorPhone }}</span>
    </div>

    <div class="justify-end flex mt-5">
      <button
        @click="userRegister"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        註冊
      </button>
    </div>
  </div>
</template>
