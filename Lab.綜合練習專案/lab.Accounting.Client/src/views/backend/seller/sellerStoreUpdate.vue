<script setup>
import { getStore, storeUpdateToCompany } from '@/api/storeService';

/*
   變數名稱代表意義
   storeId            : 賣場 ID
   storeUnifiedNumber : 統一編號
   storeCompanyName   : 公司名稱
   documentFile : 選擇的文件檔案
   documentPreviewUrl : 預覽網址 ( 圖片格式才有 , PDF 不會有預覽圖 )
   documentFileName : 顯示用的檔名
*/

const router = useRouter();
const authStore = useAuthStore();
const storeId = ref(null);
const storeUnifiedNumber = ref('');
const storeCompanyName = ref('');
const documentFile = ref(null);
const documentPreviewUrl = ref(null);
const documentFileName = ref('');

const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(async () => {
  await getStoreId();
});

const rules = computed(() => ({
  storeUnifiedNumber: { required, vaildUnifiedNumber },
  storeCompanyName: { required, maxLength: maxLength(100) },
}));

const v$ = useVuelidate(
  rules,
  { storeUnifiedNumber, storeCompanyName },
  { $autoDirty: true, $lazy: true, $scope: false },
);

const getStoreId = async () => {
  try {
    showLoading();
    const res = await getStore(authStore.userId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      storeId.value = data.returnData.storeId;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

const submitUpgrade = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();

    const fd = new FormData();
    fd.append('StoreId', storeId.value);
    fd.append('StoreCompanyName', storeCompanyName.value);
    fd.append('StoreUnifiedNumber', storeUnifiedNumber.value);

    if (documentFile.value) {
      fd.append('Document', documentFile.value);
    }

    const res = await storeUpdateToCompany(fd);

    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('已送出審核申請');
      router.push({ name: 'seller-store-edit' });
    } else {
      showToastError(data.message || '申請失敗請稍後再試');
    }
  } catch (err) {
    console.log(err);
    showToastError('申請失敗，請稍後再試');
  } finally {
    hideLoading();
  }
};

const uploadDocument = (event) => {
  const file = event.target.files[0];
  if (!file) return;

  documentFile.value = file;
  documentFileName.value = file.name;

  // 只有圖片格式才產生預覽圖，PDF 顯示檔名即可
  documentPreviewUrl.value = file.type.startsWith('image/') ? URL.createObjectURL(file) : null;

  event.target.value = '';
};

const removeDocument = () => {
  documentFile.value = null;
  documentPreviewUrl.value = null;
  documentFileName.value = '';
};
</script>

<template>
  <div class="container mx-auto">
    <div class="flex flex-col">
      <div class="p-8">
        <p class="text-2xl font-bold mb-1 text-ink-900">升級為企業賣場</p>
        <p class="text-xs text-ink-500 mb-6">送出後將由平台審核，通過後即可獲得企業賣場標章</p>

        <div class="bg-page-bg rounded-card border border-border-soft p-8 max-w-2xl mx-auto">
          <div
            class="bg-brand-50 border border-brand-tag rounded-card px-4 py-3 mb-6 flex gap-2 items-start"
          >
            <i class="pi pi-info-circle text-brand-price mt-0.5"></i>
            <p class="text-xs text-brand-price m-0 leading-relaxed">
              此為選填功能，僅企業或行號賣家需要申請。申請送出後將進入人工審核，審核期間您仍可使用一般賣場功能。
            </p>
          </div>

          <div class="flex flex-col gap-4">
            <div>
              <label class="text-sm text-ink-500 block mb-2">公司名稱</label>
              <InputText
                v-model="storeCompanyName"
                placeholder="輸入公司登記名稱"
                :invalid="v$.storeCompanyName.$error"
                class="w-full"
              />
              <InValidErrorMessage
                :errorDto="v$.storeCompanyName.$errors"
                vaildChiName="公司名稱"
              />
            </div>

            <div>
              <label class="text-sm text-ink-500 block mb-2">統一編號</label>
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
            <div>
              <label class="text-sm text-ink-500 block mb-2">營業登記證明文件</label>

              <div
                v-if="!documentFile"
                class="border border-dashed border-ink-300 rounded-card p-6 text-center"
              >
                <label class="cursor-pointer flex flex-col items-center gap-2">
                  <i class="pi pi-upload text-ink-300 text-xl"></i>
                  <span class="text-sm text-ink-500">上傳營業登記證或公司登記證影本</span>
                  <span class="text-xs text-ink-300">支援 JPG、PNG、PDF</span>
                  <input
                    type="file"
                    accept="image/*,.pdf"
                    @change="uploadDocument"
                    class="hidden"
                  />
                </label>
              </div>

              <div
                v-else
                class="relative border border-border-soft rounded-card p-4 flex items-center gap-3"
              >
                <img
                  v-if="documentPreviewUrl"
                  :src="documentPreviewUrl"
                  class="w-16 h-16 object-cover rounded-card border border-border-soft shrink-0"
                />
                <i v-else class="pi pi-file text-2xl text-ink-500 shrink-0"></i>
                <span class="text-sm text-ink-900 flex-1 truncate">{{ documentFileName }}</span>
                <button
                  @click="removeDocument"
                  class="bg-action-danger text-white rounded-full w-5 h-5 flex items-center justify-center text-xs cursor-pointer shrink-0"
                >
                  ✕
                </button>
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-3 mt-7 pt-5 border-t border-border-soft">
            <button
              @click="router.back()"
              class="bg-transparent border border-border-soft text-ink-900 hover:bg-page-bg-soft px-6 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              取消
            </button>
            <button
              @click="submitUpgrade"
              class="bg-brand-500 hover:opacity-90 text-white px-8 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
            >
              送出審核申請
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
