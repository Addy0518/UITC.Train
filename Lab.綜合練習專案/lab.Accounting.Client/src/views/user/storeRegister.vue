<script setup>
import { register } from '@/api/storeService';

/*
   變數名稱代表意義
   storeName          : 賣場名稱
   storeUnifiedNumber : 統一編號
   storeCompanyName   : 公司名稱
*/
const router = useRouter();
const storeName = ref('');
const storeUnifiedNumber = ref('');
const storeCompanyName = ref('');

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   驗證規則
*/
const rules = computed(() => ({
  storeName: { required, maxLength: maxLength(100) },
  storeUnifiedNumber: { required, vaildUnifiedNumber },
  storeCompanyName: { maxLength: maxLength(100) },
}));

const v$ = useVuelidate(
  rules,
  { storeName, storeUnifiedNumber, storeCompanyName },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
   送出賣家申請
*/
const submitRegister = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const res = await register({
      storeName: storeName.value,
      storeUnifiedNumber: storeUnifiedNumber.value,
      storeCompanyName: storeCompanyName.value || null,
    });

    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('已成功申請成為賣家！');
      // TODO: 導去編輯賣場頁面，待路由建立後補上 name
      // router.push({ name: 'edit-store' });
    } else {
      showToastError(data.message || '申請失敗，請稍後再試');
    }
  } catch (err) {
    console.log(err);
    showToastError('申請失敗，請稍後再試');
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="container mx-auto">
    <div class="flex flex-col">
      <!--#region 表單區 -->
      <div class="p-8">
        <p class="text-2xl font-bold mb-5 text-ink-900">申請成為賣家</p>

        <div class="bg-page-bg rounded-card border border-border-soft p-6 max-w-2xl mx-auto">
          <div class="flex flex-col gap-4">
            <!--#region 賣場名稱 -->
            <div class="flex gap-4 items-center">
              <label class="text-sm text-ink-500 w-24 text-right shrink-0"> 賣場名稱 </label>
              <div class="flex-1">
                <InputText
                  v-model="storeName"
                  placeholder="輸入賣場顯示名稱"
                  :invalid="v$.storeName.$error"
                  class="w-full"
                />
                <p class="text-xs text-ink-300 mt-1 m-0">
                  最多 100 字，這將顯示在買家看到的所有訂單頁面上
                </p>
                <InValidErrorMessage :errorDto="v$.storeName.$errors" vaildChiName="賣場名稱" />
              </div>
            </div>
            <!-- #endregion -->

            <!--#region 統一編號 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-24 text-right shrink-0"> 統一編號 </label>
              <div class="flex-1">
                <InputText
                  v-model="storeUnifiedNumber"
                  placeholder="輸入 8 碼統一編號"
                  :invalid="v$.storeUnifiedNumber.$error"
                  class="w-full"
                  :maxlength="8"
                />
                <p class="text-xs text-ink-300 mt-1 m-0">請輸入合法的台灣公司或商號統一編號</p>
                <InValidErrorMessage
                  :errorDto="v$.storeUnifiedNumber.$errors"
                  vaildChiName="統一編號"
                />
              </div>
            </div>
            <!-- #endregion -->

            <!--#region 公司名稱 -->
            <div class="flex items-center gap-4">
              <label class="text-sm text-ink-500 w-24 text-right shrink-0">公司名稱</label>
              <div class="flex-1">
                <InputText
                  v-model="storeCompanyName"
                  placeholder="輸入公司名稱（選填）"
                  :invalid="v$.storeCompanyName.$error"
                  class="w-full"
                />
                <p class="text-xs text-ink-300 mt-1 m-0">選填，若有公司登記名稱可填寫</p>
                <InValidErrorMessage
                  :errorDto="v$.storeCompanyName.$errors"
                  vaildChiName="公司名稱"
                />
              </div>
            </div>
            <!-- #endregion -->
          </div>

          <!--#region 注意事項 -->
          <div
            class="mt-5 p-4 bg-page-bg-soft rounded-card border border-border-soft flex items-start gap-2"
          >
            <i class="pi pi-info-circle text-ink-300 mt-0.5 shrink-0"></i>
            <p class="text-xs text-ink-500 m-0 leading-relaxed">
              送出後帳號將升級為賣家，即可進入賣家中心管理商品與訂單。請確認填寫的統一編號正確，送出後如需修改請聯絡客服。
            </p>
          </div>
          <!-- #endregion -->

          <!--#region 按鈕區 -->
          <div class="flex justify-end gap-3 mt-6 pt-4 border-t border-border-soft">
            <button
              @click="router.back()"
              class="bg-transparent border border-[#D3D1C7] text-ink-900 hover:bg-page-bg-soft px-6 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              取消
            </button>
            <button
              @click="submitRegister"
              class="bg-brand-500 hover:opacity-90 text-white px-8 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              送出申請
            </button>
          </div>
          <!-- #endregion -->
        </div>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
