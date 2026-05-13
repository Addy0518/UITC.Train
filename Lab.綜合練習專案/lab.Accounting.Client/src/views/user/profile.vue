<script setup>
import { computed, onMounted, inject, ref } from 'vue';
import { userHeadShot, getUser, updateUser, updatePassword } from '@/api/userService';
import { useAuthStore } from '@/stores/auth';
import { genderEnum, getEnumDescription } from '../../common/enum';
import { formatDateOnly } from '@/common/formats';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import { required, maxLength, vaildCellPhone, vaildEmail } from '@/validator/validators';
import { useVuelidate } from '@vuelidate/core';
import InValidErrorMessage from '@/common/InValidErrorMessage.vue';

/*
   變數名稱代表意義
   imgUrl : 大頭照圖片路徑
   baseUrl : 基底位址
   authStore : localstorage
   userInfo : 用戶資料
*/
let imgUrl = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const userInfo = ref();

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  userAccount: { vaildEmail },
  userName: { required, maxLength: maxLength(50) },
  userPhone: { vaildCellPhone },
  userAddress: { maxLength: maxLength(200) },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(
  rules,
  computed(() => userInfo.value ?? {}),
  { $autoDirty: true, $lazy: true, $scope: false },
);

onMounted(() => {
  getMyUser();
  if (authStore.userHeadshot) {
    imgUrl.value = `${baseUrl}/UserHeadShot/${authStore.userHeadshot}`;
  } else {
    imgUrl.value = defaultImgurl;
  }
});

/*
   上傳檔案 ( 大頭照 ) 並在前端顯示
*/
const uploadFile = async (event) => {
  try {
    showLoading();
    const file = event.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('userFile', file);
    const res = await userHeadShot(formData);

    const { data } = res;

    if (data.codeStatus === 2000) {
      imgUrl.value = `${baseUrl}/UserHeadShot/${data.returnData.userHeadshot}`;
      authStore.userHeadshot = data.returnData.userHeadshot;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   載入用戶資訊
*/
const getMyUser = async () => {
  try {
    showLoading();
    const res = await getUser();

    const { data } = res;

    if (data.codeStatus === 2000) {
      console.log('rawBirthDate:', data.returnData.userBirthDate);
      userInfo.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   更新用戶資訊
*/
const updateMyUser = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    showLoading();

    const request = {
      ...userInfo.value,
      // 生日轉為 DateOnly 
      userBirthDate: userInfo.value.userBirthDate
        ? formatDateOnly(userInfo.value.userBirthDate)
        : null,
    };

    const res = await updateUser(request);

    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('更新成功 !');
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
        <label class="relative cursor-pointer group">
          <!-- 顯示照片 -->
          <img
            :src="imgUrl"
            alt="User Avatar"
            class="w-50 h-50 rounded-full object-cover border-2 border-gray-200 group-hover:opacity-75 transition-opacity"
          />

          <!-- 提示文字 -->
          <div
            class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity rounded-full"
          >
            <span class="bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded"
              >更換照片</span
            >
          </div>

          <!-- 隱藏的檔案輸入框 -->
          <input type="file" @change="uploadFile" accept="image/*" class="hidden" />
        </label>
      </div>

      <div class="mt-40 w-300 rounded-lg shadow-sm" v-if="userInfo">
        <!-- 帳號 -->
        <InputGroup>
          <InputText
            v-model="userInfo.userAccount"
            placeholder="使用者帳號"
            :invalid="v$.userAccount.$error"
          />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.userAccount.$errors" vaildChiName="使用者帳號" />

        <!-- 姓名 -->
        <InputGroup>
          <InputText v-model="userInfo.userName" placeholder="姓名" :invalid="v$.userName.$error" />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.userName.$errors" vaildChiName="姓名" />

        <!-- 電話 -->
        <InputGroup>
          <InputText
            v-model="userInfo.userPhone"
            placeholder="電話"
            :invalid="v$.userPhone.$error"
          />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.userPhone.$errors" vaildChiName="電話" />

        <!-- 地址 -->
        <InputGroup>
          <InputText
            v-model="userInfo.userAddress"
            placeholder="地址"
            :invalid="v$.userAddress.$error"
          />
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.userAddress.$errors" vaildChiName="地址" />

        <!-- 生日 -->

        <DatePicker v-model="userInfo.userBirthDate" placeholder="生日" dateFormat="yy-mm-dd" />

        <!-- 性別 -->
        <div class="flex gap-5 ms-5 mt-3">
          <label
            v-for="g in genderEnum"
            :key="g.value"
            class="flex items-center gap-2 cursor-pointer"
          >
            <input type="radio" :value="g.value" v-model="userInfo.userGender" />
            {{ g.description }}
          </label>
        </div>

        <!-- 送出按鈕 -->
        <div class="flex justify-end mt-5">
          <button
            @click="updateMyUser"
            class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
          >
            更新資料
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
