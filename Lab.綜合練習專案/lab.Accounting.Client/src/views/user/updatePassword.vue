<script setup>
import { updatePassword } from '@/api/userService';

/*
   變數名稱代表意義
   oldPassword : 舊密碼
   newPassword : 新密碼
*/
const oldPassword = ref();
const newPassword = ref();
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
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(
  rules,
  { oldPassword, newPassword },
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
  <div class="container">
    <div class="flex flex-col w-full">
      <!--#region 新舊密碼輸入欄位 -->
      <div class="flex justify-end p-20">
        <InputGroup>
          <InputText v-model="oldPassword" placeholder="舊密碼" :invalid="v$.oldPassword.$error" />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.oldPassword.$errors" vaildChiName="舊密碼" />
        <InputGroup>
          <InputText v-model="newPassword" placeholder="新密碼" :invalid="v$.newPassword.$error" />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.newPassword.$errors" vaildChiName="新密碼" />
      </div>
      <!-- #endregion -->
      <!--#region 按鈕區 -->
      <div class="flex justify-end mt-5">
        <button
          @click="updateMyPassword"
          class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
        >
          修改密碼
        </button>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
