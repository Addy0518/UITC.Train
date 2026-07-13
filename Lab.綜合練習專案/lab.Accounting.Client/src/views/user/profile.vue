<script setup>
import { getMyUser, updateUser } from '@/api/userService';
import ZipCodeSelector from '@/common/ZipCodeSelector.vue';
/*
   變數名稱代表意義
   userInfo : 用戶資料
   zipInfo : 縣市/鄉鎮市區/郵遞區號
*/
const userInfo = ref();
const zipInfo = ref();
const detailAddress = ref('');
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  userAccount: { vaildEmail, required },
  userName: { required, maxLength: maxLength(50) },
  userPhone: { vaildCellPhone },
  detailAddress: { maxLength: maxLength(200) },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(
  rules,
  computed(() => ({ ...userInfo.value, detailAddress: detailAddress.value })),
  { $autoDirty: true, $lazy: true, $scope: false },
);

onMounted(() => {
  getMineUser();
});

/*
   載入用戶資訊
*/
const getMineUser = async () => {
  try {
    showLoading();
    const res = await getMyUser();

    const { data } = res;

    if (data.codeStatus === 2000) {
      userInfo.value = data.returnData;
      if (data.returnData.userZipCode) {
        zipInfo.value = reverseLookupZipCode(data.returnData.userZipCode);

        // 把完整地址開頭的「縣市 + 鄉鎮市區」截掉，剩下的塞進 detailAddress
        const prefix = `${zipInfo.value.city}${zipInfo.value.district}`;
        const fullAddress = data.returnData.userAddress ?? '';
        detailAddress.value = fullAddress.startsWith(prefix)
          ? fullAddress.slice(prefix.length)
          : fullAddress;
      } else {
        // 沒有郵遞區號的舊資料 , 沒辦法拆 , 整串放進詳細地址就好
        detailAddress.value = data.returnData.userAddress ?? '';
      }
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

    const fullAddress = detailAddress.value
      ? `${zipInfo.value.city}${zipInfo.value.district}${detailAddress.value}`
      : userInfo.value.userAddress;

    const request = {
      ...userInfo.value,
      userAddress: fullAddress,
      userZipCode: zipInfo.value?.zipCode ?? userInfo.value.userZipCode,
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
  <div class="container mx-auto">
    <div class="flex flex-col">
      <!-- #region  表單區 -->
      <div class="p-8" v-if="userInfo">
        <p class="text-2xl font-bold mb-5 text-ink-900">我的個人資料</p>

        <div class="bg-page-bg rounded-card border border-border-soft p-6 max-w-2xl mx-auto">
          <div class="flex flex-col gap-4">
            <!-- #region  帳號 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0">帳號</label>
              <div class="flex-1">
                <InputText
                  v-model="userInfo.userAccount"
                  placeholder="使用者帳號"
                  :invalid="v$.userAccount.$error"
                  class="w-full"
                />
                <InValidErrorMessage :errorDto="v$.userAccount.$errors" vaildChiName="使用者帳號" />
              </div>
            </div>
            <!-- #endregion -->
            <!-- #region  姓名 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0">姓名</label>
              <div class="flex-1">
                <InputText
                  v-model="userInfo.userName"
                  placeholder="姓名"
                  :invalid="v$.userName.$error"
                  class="w-full"
                />
                <InValidErrorMessage :errorDto="v$.userName.$errors" vaildChiName="姓名" />
              </div>
            </div>
            <!-- #endregion -->
            <!-- #region  電話 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0">電話</label>
              <div class="flex-1">
                <InputText
                  v-model="userInfo.userPhone"
                  placeholder="電話"
                  :invalid="v$.userPhone.$error"
                  class="w-full"
                />
                <InValidErrorMessage :errorDto="v$.userPhone.$errors" vaildChiName="電話" />
              </div>
            </div>
            <!-- #endregion -->
            <!-- #region  地址 -->
            <div class="flex items-start gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0 pt-2">地址</label>
              <div class="flex-1 flex flex-col gap-2">
                <ZipCodeSelector v-model="zipInfo" />

                <InputGroup>
                  <InputGroupAddon>
                    <i class="pi pi-home"></i>
                  </InputGroupAddon>

                  <InputText
                    v-model="detailAddress"
                    placeholder="請輸入詳細地址 ( 路名、門牌、樓層 )"
                    :invalid="v$.detailAddress.$error"
                  />
                </InputGroup>

                <InValidErrorMessage :errorDto="v$.detailAddress.$errors" vaildChiName="地址" />
              </div>
            </div>
            <!-- #endregion -->
            <!-- #region  生日 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0">生日</label>
              <div class="flex-1">
                <DatePicker
                  v-model="userInfo.userBirthDate"
                  placeholder="生日"
                  dateFormat="yy-mm-dd"
                  class="w-full"
                />
              </div>
            </div>
            <!-- #endregion -->
            <!-- #region  性別 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-20 text-right shrink-0">性別</label>
              <div class="flex gap-5">
                <label
                  v-for="g in genderEnum"
                  :key="g.value"
                  class="flex items-center gap-2 cursor-pointer text-sm text-ink-900"
                >
                  <input type="radio" :value="g.value" v-model="userInfo.userGender" />
                  {{ g.description }}
                </label>
              </div>
            </div>
            <!-- #endregion -->
          </div>
          <!-- #region  送出按鈕 -->
          <div class="flex justify-end mt-6 pt-4 border-t border-border-soft">
            <button
              @click="updateMyUser"
              class="bg-brand-500 hover:opacity-90 text-white px-8 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              儲存
            </button>
          </div>
          <!-- #endregion -->
        </div>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
