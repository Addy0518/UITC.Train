<script setup>
import { loginApi } from '@/api/userService';

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
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  account: { required, maxLength: maxLength(200), vaildEmail },
  password: { required, vaildLoginPassword },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(
  rules,
  { account, password },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
  測試用賣家帳號
*/
const testUser = () => {
  account.value = 'andy@gmail.com';
  password.value = 'Andy1111';
};
/*
  測試用買家帳號
*/
const test2User = () => {
  account.value = 'anggininder@gmail.com';
  password.value = 'Andy1111';
};

/*
  測試用管理員帳號
*/
const test3User = () => {
  account.value = 'aaa@gmail.com';
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
      route.push({ name: 'mall' });
    }
    if (data.codeStatus === 4000) {
      showToastError('錯誤', getError400Message(data.error400));
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

```vue
<template>
  <div class="bg-page-bg-soft py-10 px-4">
    <div class="max-w-md mx-auto">
      <!-- #region 登入卡片 -->
      <div class="bg-page-bg border border-border-soft rounded-card p-6">
        <!-- #region 標題 -->
        <div class="text-center mb-8">
          <h1 class="text-2xl font-bold text-ink-900 m-0">登入帳號</h1>

          <p class="text-sm text-ink-500 mt-2 mb-0">歡迎回來</p>
        </div>
        <!-- #endregion -->

        <!-- #region 登入欄位 -->
        <div class="flex flex-col gap-4">
          <!-- 帳號 -->
          <div>
            <label class="block text-sm font-medium text-ink-900 mb-2"> 帳號 </label>

            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-user"></i>
              </InputGroupAddon>

              <InputText v-model="account" placeholder="請輸入帳號" :invalid="v$.account.$error" />
            </InputGroup>

            <InValidErrorMessage :errorDto="v$.account.$errors" vaildChiName="帳號" />
          </div>

          <!-- 密碼 -->
          <div>
            <label class="block text-sm font-medium text-ink-900 mb-2"> 密碼 </label>

            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-lock"></i>
              </InputGroupAddon>

              <InputText
                :type="tooglePassword ? 'password' : 'text'"
                v-model="password"
                placeholder="請輸入密碼"
                :invalid="v$.password.$error"
              />

              <InputGroupAddon class="cursor-pointer" @click="tooglePassword = !tooglePassword">
                <i :class="['pi', tooglePassword ? 'pi-eye-slash' : 'pi-eye']" />
              </InputGroupAddon>
            </InputGroup>

            <InValidErrorMessage :errorDto="v$.password.$errors" vaildChiName="密碼" />
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region 忘記密碼 -->
        <div class="mt-8 flex justify-end">
          <RouterLink
            class="text-sm font-medium text-ink-900 mb-3"
            :to="{ name: 'forget-password' }"
            >忘記密碼 ?</RouterLink
          >
        </div>
        <!-- #endregion -->
        <!-- #region 測試帳號 -->
        <div class="mt-8">
          <p class="text-sm font-medium text-ink-900 mb-3">測試帳號</p>

          <div class="grid grid-cols-1 sm:grid-cols-3 gap-2">
            <button
              @click="testUser"
              class="py-2 px-3 text-sm border border-border-soft rounded-card bg-page-bg text-ink-900 cursor-pointer hover:bg-surface-muted"
            >
              賣家帳號
            </button>

            <button
              @click="test2User"
              class="py-2 px-3 text-sm border border-border-soft rounded-card bg-page-bg text-ink-900 cursor-pointer hover:bg-surface-muted"
            >
              買家帳號
            </button>

            <button
              @click="test3User"
              class="py-2 px-3 text-sm border border-border-soft rounded-card bg-page-bg text-ink-900 cursor-pointer hover:bg-surface-muted"
            >
              管理員帳號
            </button>
          </div>
        </div>
        <!-- #endregion -->

        <!-- #region 登入按鈕 -->
        <div class="mt-8">
          <button
            @click="userLogin"
            class="w-full py-3 rounded-card bg-brand-500 text-white font-medium cursor-pointer transition-opacity hover:opacity-90"
          >
            登入
          </button>
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
```
