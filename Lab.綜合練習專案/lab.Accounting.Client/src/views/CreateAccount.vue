<script setup>
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { registerApi } from '@/api/account-api';
const route = useRouter();
const account = ref();
const password = ref();
const name = ref();
const phone = ref();

const userRegister = async () => {
  const userRegisterData = {
    userAccount: account.value,
    userPassword: password.value,
    userName: name.value,
    userPhone: phone.value,
  };

  try {
    const res = await registerApi(userRegisterData);
    const { data } = res;
    if (data.codeStatus === 2000) {
      alert('註冊成功!');
      route.push('/login');
    }
    // 帳號重複註冊
    else if (data.codeStatus === 4000) {
      const errorMsg = data.error400.UserAccount;
      alert(errorMsg);
    } else {
      alert(`意外成功:${data.codeStatus}`);
    }
  } catch (error) {
    console.error('使用者註冊錯誤 ', error.response);
  }
};
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
        <InputText v-model="name" placeholder="姓名" />
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
        @click="userRegister"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        註冊
      </button>
    </div>
  </div>
</template>
