<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
// 引入 pinia 的 useAuthStore 來管理登入狀態
const authStore = useAuthStore();
const route = useRouter();
const account = ref(null);
const password = ref(null);




const userLogin=async()=>{
  try {
    const res = await fetch(`https://localhost:7124/api/User/Login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        userAccount: account.value,
        userPassword: password.value,
      }),
    });

    if (res.ok) {
      const data = await res.json();
      // 把後端回傳的 token 和 userId 存到 pinia 的 authStore 裡面
      authStore.setAuth(data.returnData);
      route.push('/accounting-practice');
      console.log('登入成功:', data);
    } else {
      console.error('登入失敗:', res.statusText);
      alert('登入失敗，請檢查帳號和密碼');
    }
  } catch (error) {
    console.error('連線失敗:', error);
    alert('連線失敗，請稍後再試');
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
        @click="userLogin"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        登入
      </button>
    </div>
  </div>
</template>
