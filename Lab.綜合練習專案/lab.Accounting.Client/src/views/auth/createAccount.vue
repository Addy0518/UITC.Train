<script setup>
import { registerApi } from '@/api/userService';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   account : 帳號
   password : 密碼
   name : 名稱
   phone : 電話
   address : 地址
   tooglePassword　：　切換密碼顯示或隱藏
*/
const route = useRouter();
const account = ref();
const password = ref();
const name = ref();
const phone = ref();
const address = ref();
const tooglePassword = ref(true);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  account: { required, maxLength: maxLength(200), vaildEmail },
  password: { required, vaildLoginPassword },
  name: { required, maxLength: maxLength(50) },
  phone: { vaildCellPhone },
  address: {},
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(
  rules,
  { account, password, name, phone, address },
  { $autoDirty: true, $lazy: true, $scope: false },
);

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
      userAddress: address.value,
    };

    const res = await registerApi(userRegisterData);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('註冊成功!');
      route.push('/login');
    } else if (data.codeStatus === 4000) {
      showToastError('錯誤', getError400Message(data.error400));
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

    <!-- #region  註冊欄位-->
    <div class="card grid grid-cols-1 gap-4 gap-y-5">
      <!-- #region  帳號 / 密碼 -->
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
      <!-- #endregion -->
      <!-- #region  名稱 / 電話-->
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

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-home"></i>
        </InputGroupAddon>
        <InputText v-model="address" placeholder="地址" :invalid="v$.address.$error" />
      </InputGroup>
      <InValidErrorMessage :errorDto="v$.address.$errors" vaildChiName="地址" />
      <!-- #endregion -->
    </div>
    <!-- #endregion -->
    <!-- #region  儲存按鈕-->
    <div class="justify-end flex mt-5">
      <button
        @click="userRegister"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        註冊
      </button>
    </div>
    <!-- #endregion -->
  </div>
</template>
