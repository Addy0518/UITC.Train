<script setup>
import { computed, onMounted, inject, ref } from 'vue';
import { userHeadShot, getUser, updateUser, updatePassword } from '@/api/userService';
import { useAuthStore } from '@/stores/auth';
import { genderEnum, getEnumDescription } from '../../common/enum';
import { formatDateOnly } from '@/common/formats';

import { vaildLoginPassword, required } from '@/validator/validators';
import { useVuelidate } from '@vuelidate/core';
import InValidErrorMessage from '@/common/InValidErrorMessage.vue';
import { codeStatusEnum } from '@/common/enum';
import { getError400Message } from '@/common/method';

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

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  oldPassword: { vaildLoginPassword, required },
  newPassword: { vaildLoginPassword, required },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
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
      <!-- 送出按鈕 -->
      <div class="flex justify-end mt-5">
        <button
          @click="updateMyPassword"
          class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
        >
          修改密碼
        </button>
      </div>
    </div>
  </div>
</template>
