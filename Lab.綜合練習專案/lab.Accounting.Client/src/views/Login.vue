<script setup>
import { ref, computed, inject } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { loginApi } from '@/api/account-api';
import { required, vaildEmail, vaildLoginPassword } from '@/validator/validators';
import { useVuelidate } from '@vuelidate/core';
import InValidErrorMessage from '../common/InValidErrorMessage.vue';

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   變數名稱代表意義
   authStore : pinia 注入
   route : 獲取路由資訊
   account : 帳號
   password : 密碼
   tooglePassword　：　切換密碼顯示或隱藏

*/
const authStore = useAuthStore();
const route = useRouter();
const account = ref(null);
const password = ref(null);
const tooglePassword = ref(true);

/*
  驗證帳號格式
*/
// 加入已經寫好的驗證規則
const rules = computed(() => ({
  account: { required, vaildEmail },
  password: { required, vaildLoginPassword },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(
  rules,
  { account, password },
  { $autoDirty: true, $lazy: true, $scope: false },
);

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
  // 要儲存前先驗證
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const userlogin = { userAccount: account.value, userPassword: password.value };
    const res = await loginApi(userlogin);
    const { data } = res;
    if (data.codeStatus === 2000) {
      authStore.setAuth(data.returnData);
      showToastSuccess('登入成功 !');
      route.push('mall');
    }
    if (data.codeStatus === 4001) {
      showToastError('錯誤', data.message);
    }
  } catch (error) {
    console.error('使用者登入錯誤 ', error.response);
  } finally {
    hideLoading();
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
        <InputText v-model="account" placeholder="帳號" :invalid="v$.account.$error" />
      </InputGroup>
      <!-- 自訂的顯示錯誤訊息元件 -->
      <InValidErrorMessage :errorDto="v$.account.$errors" vaildChiName="帳號" />
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-unlock"></i>
        </InputGroupAddon>
        <InputText
          :type="tooglePassword ? 'password' : 'text'"
          v-model="password"
          placeholder="密碼"
          :invalid="v$.password.$error"
        />
        <InputGroupAddon class="cursor-pointer" @click="tooglePassword = !tooglePassword">
          <i :class="['pi', tooglePassword ? 'pi-eye' : 'pi-eye-slash']"></i>
        </InputGroupAddon>
      </InputGroup>
      <InValidErrorMessage :errorDto="v$.password.$errors" vaildChiName="密碼" />
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
