<script setup>
import { updatePassword } from '@/api/userService';

/*
   變數名稱代表意義
   oldPassword : 舊密碼
   newPassword : 新密碼
   confirmPassword : 確認密碼
   tooglePassword　：　切換密碼顯示或隱藏
*/
const oldPassword = ref();
const newPassword = ref();
const confirmPassword = ref();
const tooglePassword1 = ref(true);
const tooglePassword2 = ref(true);
const tooglePassword3 = ref(true);
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
  oldPassword: { vaildLoginPassword, required },
  newPassword: { vaildLoginPassword, required },
  confirmPassword: {
    required,
    sameAsPassword: sameAsPassword(newPassword),
  },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(
  rules,
  { oldPassword, newPassword, confirmPassword },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
   更新密碼
*/
const updateMyPassword = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    showLoading();

    const request = {
      OldUserPassword: oldPassword.value,
      NewUserPassword: newPassword.value,
    };

    const res = await updatePassword(request);

    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('更新成功 !');
    }
    if (data.codeStatus === codeStatusEnum.NotFound) {
      showToastError(getError400Message(data.error400));
    }
    if (data.codeStatus === codeStatusEnum.RequestError) {
      showToastError(getError400Message(data.error400));
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="container mx-auto">
    <div class="flex flex-col w-full p-8">
      <p class="text-2xl font-bold m-0 mb-4 text-ink-900">修改密碼</p>

      <!--#region 密碼輸入欄位 -->
      <div class="bg-page-bg rounded-card border border-border-soft w-full p-6 max-w-2xl mx-auto">
        <div class="flex flex-col gap-4">
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">目前密碼</label>
            <InputGroup>
              <InputText
                v-model="oldPassword"
                :type="tooglePassword1 ? 'password' : 'text'"
                placeholder="請輸入目前密碼"
                :invalid="v$.oldPassword.$error"
                class="w-full" /><InputGroupAddon
                class="cursor-pointer"
                @click="tooglePassword1 = !tooglePassword1"
              >
                <i :class="['pi', tooglePassword1 ? 'pi-eye-slash' : 'pi-eye']" /> </InputGroupAddon
            ></InputGroup>

            <InValidErrorMessage :errorDto="v$.oldPassword.$errors" vaildChiName="舊密碼" />
          </div>

          <div>
            <label class="text-sm text-ink-500 block mb-1.5">新密碼</label>

            <InputGroup
              ><InputText
                v-model="newPassword"
                :type="tooglePassword2 ? 'password' : 'text'"
                placeholder="請輸入新密碼"
                :invalid="v$.newPassword.$error"
                class="w-full" /><InputGroupAddon
                class="cursor-pointer"
                @click="tooglePassword2 = !tooglePassword2"
              >
                <i :class="['pi', tooglePassword2 ? 'pi-eye-slash' : 'pi-eye']" /> </InputGroupAddon
            ></InputGroup>
            <InValidErrorMessage :errorDto="v$.newPassword.$errors" vaildChiName="新密碼" />
          </div>

          <div>
            <label class="text-sm text-ink-500 block mb-1.5">確認新密碼</label>
            <InputGroup>
              <InputText
                v-model="confirmPassword"
                :type="tooglePassword3 ? 'password' : 'text'"
                placeholder="請再次輸入新密碼"
                :invalid="v$.confirmPassword.$error"
                class="w-full" /><InputGroupAddon
                class="cursor-pointer"
                @click="tooglePassword3 = !tooglePassword3"
              >
                <i :class="['pi', tooglePassword3 ? 'pi-eye-slash' : 'pi-eye']" /> </InputGroupAddon
            ></InputGroup>

            <InValidErrorMessage :errorDto="v$.confirmPassword.$errors" vaildChiName="確認新密碼" />
          </div>
        </div>

        <!--#region 按鈕區 -->
        <div class="flex justify-end mt-6 pt-4 border-t border-border-soft">
          <button
            @click="updateMyPassword"
            class="bg-brand-500 hover:opacity-90 text-white px-8 py-2.5 rounded-card cursor-pointer text-sm font-medium transition-colors"
          >
            修改密碼
          </button>
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
