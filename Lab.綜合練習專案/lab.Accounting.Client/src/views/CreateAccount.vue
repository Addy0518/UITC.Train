<script setup>
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';

const route = useRouter();
const account = ref();
const password = ref();
const name = ref();
const phone = ref();

const userRegister=async()=>{
  try {
    const res = await fetch(`https://localhost:7124/api/User/Register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        userAccount: account.value,
        userPassword: password.value,
        userName: name.value,
        userPhone: phone.value,
      }),
    });

    if (res.ok) {
      const data = await res.json();
      route.push('/login');
      console.log('註冊成功:', data);
    } else {
      console.error('註冊失敗:', res.statusText);
      alert('註冊失敗，請檢查輸入資訊');
    }
  } catch (error) {
    console.error('連線失敗:', error);
    alert('連線失敗，請稍後再試');
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
