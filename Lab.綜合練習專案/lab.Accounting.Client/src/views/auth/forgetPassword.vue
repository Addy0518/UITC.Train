<script setup>
import { forgetUpdatePassword, sendVerfiyCode } from '@/api/userService';

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   變數名稱代表意義
   router : 路由跳轉用
   step : 目前在第幾步 ( 1 = 輸入帳號 + 寄送驗證碼 , 2 = 輸入驗證碼 + 新密碼 )
   account : 帳號
   code : 驗證碼
   newPassword : 新密碼
   tooglePassword　：　切換密碼顯示或隱藏

*/
const router = useRouter();
const step = ref(1);
const account = ref(null);
const code = ref(null);
const newPassword = ref(null);
const tooglePassword = ref(true);

/*
   先驗證帳號
*/
const rules1 = computed(() => ({
  account: { required, maxLength: maxLength(200), vaildEmail },
}));

/*
   再驗證密碼跟驗證碼
*/
const rules2 = computed(() => ({
  code: { required },
  newPassword: { required, vaildLoginPassword },
}));

/*
   加入套件驗證設定
*/
const v1$ = useVuelidate(rules1, { account }, { $autoDirty: true, $lazy: true, $scope: false });

/*
   加入套件驗證設定
*/
const v2$ = useVuelidate(
  rules2,
  { code, newPassword },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
  輸入帳號後寄送驗證碼
*/
const sendCode = async () => {
  const isFormCorrect = await v1$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const request = {
      UserAccount: account.value,
    };
    const res = await sendVerfiyCode(request);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('驗證碼已寄送至您的信箱,請注意查收 !');
      step.value = 2;
    }
    if (data.codeStatus === 4000) {
      showToastError('錯誤', getError400Message(data.error400));
    }
    if (data.codeStatus === 4001) {
      showToastError('錯誤', data.message);
    }
  } catch (error) {
    console.error('寄送驗證碼錯誤 ', error.response);
  } finally {
    hideLoading();
  }
};

/*
  更新密碼
*/
const updatePassword = async () => {
  const isFormCorrect = await v2$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const request = {
      userAccount: account.value,
      code: code.value,
      newUserPassword: newPassword.value,
    };
    const res = await forgetUpdatePassword(request);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('密碼重設成功,請重新登入 !');
      router.push({ name: 'login' });
    }
    if (data.codeStatus === 4000) {
      showToastError('錯誤', getError400Message(data.error400));
    }
    if (data.codeStatus === 4001) {
      showToastError('錯誤', data.message);
    }
  } catch (error) {
    console.error('重設密碼錯誤 ', error.response);
  } finally {
    hideLoading();
  }
};
</script>

```vue
<template>
  <div class="bg-page-bg-soft py-10 px-4">
    <div class="max-w-md mx-auto">
      <div class="bg-page-bg border border-border-soft rounded-card p-6">
        <!-- #region 標題 -->
        <div class="text-center mb-8">
          <h1 class="text-2xl font-bold text-ink-900 m-0">忘記密碼</h1>
          <p class="text-sm text-ink-500 mt-2 mb-0">
            {{ step === 1 ? '請輸入您的帳號' : '請輸入驗證碼與新密碼' }}
          </p>
        </div>
        <!-- #endregion -->

        <!-- #region 第一步：輸入帳號 -->
        <div v-if="step === 1" class="flex flex-col gap-4">
          <div>
            <label class="block text-sm font-medium text-ink-900 mb-2"> 帳號 </label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-user"></i>
              </InputGroupAddon>
              <InputText v-model="account" placeholder="請輸入帳號" :invalid="v1$.account.$error" />
            </InputGroup>
            <InValidErrorMessage :errorDto="v1$.account.$errors" vaildChiName="帳號" />
          </div>

          <button
            @click="sendCode"
            class="w-full py-3 rounded-card bg-brand-500 text-white font-medium cursor-pointer transition-opacity hover:opacity-90"
          >
            寄送驗證碼
          </button>
        </div>
        <!-- #endregion -->

        <!-- #region 第二步：輸入驗證碼 + 新密碼 -->
        <div v-else class="flex flex-col gap-4">
          <div>
            <label class="block text-sm font-medium text-ink-900 mb-2"> 驗證碼 </label>
            <InputGroup>
              <InputGroupAddon>
                <i class="pi pi-key"></i>
              </InputGroupAddon>
              <InputText v-model="code" placeholder="請輸入驗證碼" :invalid="v2$.code.$error" />
            </InputGroup>
            <InValidErrorMessage :errorDto="v2$.code.$errors" vaildChiName="驗證碼" />
          </div>

          <div>
            <label class="block text-sm font-medium text-ink-900 mb-2"> 新密碼 </label>
            <InputGroup>
              <InputGroupAddon class="cursor-pointer" @click="tooglePassword = !tooglePassword">
                <i :class="['pi', tooglePassword ? 'pi-eye-slash' : 'pi-eye']" />
              </InputGroupAddon>
              <InputText
                :type="tooglePassword ? 'password' : 'text'"
                v-model="newPassword"
                placeholder="請輸入新密碼"
                :invalid="v2$.newPassword.$error"
              />
            </InputGroup>
            <InValidErrorMessage :errorDto="v2$.newPassword.$errors" vaildChiName="新密碼" />
          </div>

          <button
            @click="updatePassword"
            class="w-full py-3 rounded-card bg-brand-500 text-white font-medium cursor-pointer transition-opacity hover:opacity-90"
          >
            重設密碼
          </button>

          <!-- 給使用者一個回上一步、重新申請驗證碼的退路 -->
          <button @click="step = 1" class="text-sm text-ink-500 underline cursor-pointer">
            返回上一步
          </button>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
```
