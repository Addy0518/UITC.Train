<script setup>
import { ref, watch, computed, inject } from 'vue';
import { useRouter } from 'vue-router';
import { registerApi } from '@/api/account-api';
import Swal from 'sweetalert2';
import {
  required,
  maxLength,
  vaildEmail,
  vaildLoginPassword,
  vaildCellPhone,
} from '@/validator/validators';
import { useVuelidate } from '@vuelidate/core';
import InValidErrorMessage from '../common/InValidErrorMessage.vue';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   account : 帳號
   password : 密碼
   name : 名稱
   phone : 電話
   tooglePassword　：　切換密碼顯示或隱藏
*/
const route = useRouter();
const account = ref();
const password = ref();
const name = ref();
const phone = ref();
const tooglePassword = ref(true);

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  account: { required, maxLength: maxLength(200), vaildEmail },
  password: { required, vaildLoginPassword },
  name: { required, maxLength: maxLength(50) },
  phone: { required, vaildCellPhone },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(
  rules,
  { account, password, name, phone },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
  呼叫註冊使用者 API
*/
const userRegister = async () => {
  // 要儲存前先驗證
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    showLoading();
    const userRegisterData = {
      userAccount: account.value,
      userPassword: password.value,
      userName: name.value,
      userPhone: phone.value,
    };

    const res = await registerApi(userRegisterData);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('註冊成功!');
      route.push('/login');
    } else if (data.codeStatus === 4000) {
      const errorMsg = data.error400.UserAccount;
      showToastError('錯誤', errorMsg);
    }
  } catch (error) {
    console.error('使用者註冊錯誤 ', error.response);
  } finally {
    hideLoading();
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
        <InputText v-model="account" placeholder="帳號" :invalid="v$.account.$error" />
      </InputGroup>
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
      <!-- 名稱跟電話 -->
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-id-card"></i>
        </InputGroupAddon>
        <InputText v-model="name" placeholder="姓名" :invalid="v$.name.$error" />
      </InputGroup>
      <InValidErrorMessage :errorDto="v$.name.$errors" vaildChiName="名稱" />

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-phone"></i>
        </InputGroupAddon>
        <InputText v-model="phone" placeholder="電話" :invalid="v$.phone.$error" />
      </InputGroup>
      <InValidErrorMessage :errorDto="v$.phone.$errors" vaildChiName="電話" />
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
