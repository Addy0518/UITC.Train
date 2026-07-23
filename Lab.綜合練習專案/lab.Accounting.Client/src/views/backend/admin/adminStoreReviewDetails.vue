<script setup>
import { getStoreReview, approveOrRejectStoreReview } from '@/api/admin/reviewService';
import { reviewStatusEnum } from '@/common/enum';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路由
   review : 賣場審查資訊
   notPassReson : 駁回原因
   baseUrl : 環境變數裡的圖片基底位址
*/
const route = useRoute();
const router = useRouter();
const review = ref();
const notPassReson = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;

const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化
*/
onMounted(() => {
  getReview(route.params.id);
});

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  notPassReson: { required, maxLength: maxLength(500) },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(rules, { notPassReson }, { $autoDirty: true, $lazy: true, $scope: false });

/*
   查看賣場審查表
*/
const getReview = async (id) => {
  try {
    showLoading();
    const res = await getStoreReview(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      review.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  通過或駁回審查
*/
const approveReview = async (status) => {
  if (status === reviewStatusEnum.Reject.value) {
    const isFormCorrect = await v$.value.$validate();
    if (!isFormCorrect) return;
  }

  try {
    const request = {
      storeCompanyReviewId: review.value.storeCompanyReviewId,
      reviewStatus: status,
      notPassReson: status === reviewStatusEnum.Reject.value ? notPassReson.value : null,
    };

    const res = await approveOrRejectStoreReview(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功!');
      router.push({ name: 'admin-all-store-review' });
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  讀取登記文件 , 判斷是否有文件沒有就回傳預設
*/
const getProductsImg = (img) => {
  if (img) {
    return `${baseUrl}/StoreUpdateDocument/${img}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="review">
    <!--#region  標題列-->
    <div class="flex items-center gap-3 mb-4">
      <button
        @click="router.push({ name: 'admin-store-allreview' })"
        class="p-2 rounded-card hover:bg-surface-muted cursor-pointer text-ink-500"
      >
        <i class="pi pi-arrow-left text-sm" />
      </button>
      <p class="text-2xl font-bold m-0 text-ink-900">企業賣場審查詳細</p>
      <span
        class="px-2 py-0.5 rounded-full text-xs"
        :class="{
          'bg-status-warning/10 text-status-warning': review.reviewStatus === 0,
          'bg-status-success/10 text-status-success': review.reviewStatus === 1,
          'bg-action-danger-50 text-action-danger': review.reviewStatus === 2,
        }"
      >
        {{ review.reviewStatus === 0 ? '待審核' : review.reviewStatus === 1 ? '已通過' : '已駁回' }}
      </span>
      <span class="text-sm text-ink-500 ml-auto"># {{ review.storeCompanyReviewId }}</span>
    </div>
    <!-- #endregion -->

    <!--#region  審核人員資訊-->
    <div class="bg-page-bg rounded-card border border-border-soft p-5 mb-3">
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-user text-xs" />審核人員資訊
      </p>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <p class="text-xs text-ink-500 mb-1">賣家</p>
          <p class="text-sm text-ink-900">{{ review.sellerName ?? '—' }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">審核人員</p>
          <p class="text-sm text-ink-900">{{ review.adminName ?? '—' }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">申請時間</p>
          <p class="text-sm text-ink-500">{{ formatDateTimeString(review.createTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">審核時間</p>
          <p class="text-sm text-ink-500">
            {{ review.reviewTime ? formatDateTimeString(review.reviewTime) : '—' }}
          </p>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    <!--#region  公司資訊-->
    <div class="bg-page-bg rounded-card border border-border-soft p-5 mb-3">
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-building text-xs" />申請公司資訊
      </p>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <p class="text-xs text-ink-500 mb-1">公司名稱</p>
          <p class="text-sm text-ink-900">{{ review.storeCompanyName }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">統一編號</p>
          <p class="text-sm text-ink-900">{{ review.storeUnifiedNumber }}</p>
        </div>
      </div>

      <!-- 營業登記證明文件 -->
      <div class="mt-4 pt-4 border-t border-border-soft">
        <p class="text-xs text-ink-500 mb-2">營業登記證明文件</p>
        <div v-if="review.documentPath">
          <img
            :src="getProductsImg(review.documentPath)"
            class="w-50 h-50 object-cover rounded-card border border-border-soft"
          />
        </div>
        <span v-else class="text-sm text-ink-500">未上傳文件</span>
      </div>
    </div>
    <!-- #endregion -->

    <!--#region  駁回原因（有駁回才顯示）-->
    <div
      v-if="review.reviewStatus === 2"
      class="bg-action-danger-50 rounded-card border border-action-danger/20 p-5 mb-3"
    >
      <p class="text-sm text-action-danger flex items-center gap-1 mb-2">
        <i class="pi pi-times-circle text-xs" />駁回原因
      </p>
      <p class="text-sm text-ink-900">{{ review.notPassReson }}</p>
    </div>
    <!-- #endregion -->

    <!--#region  審核操作（待審核才顯示）-->
    <div
      v-if="review.reviewStatus === 0"
      class="bg-page-bg rounded-card border border-border-soft p-5"
    >
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-check-circle text-xs" />審核操作
      </p>
      <div class="mb-4">
        <p class="text-xs text-ink-500 mb-1">駁回原因（駁回時必填）</p>
        <textarea
          v-model="notPassReson"
          class="w-full border border-border-soft rounded-card p-2 text-sm resize-y min-h-20 outline-none focus:border-ink-300 text-ink-900"
          placeholder="請輸入駁回原因..."
        />
        <InValidErrorMessage :errorDto="v$.notPassReson.$errors" vaildChiName="駁回原因" />
      </div>
      <div class="flex justify-end gap-2">
        <button
          @click="approveReview(reviewStatusEnum.Reject.value)"
          class="px-4 py-2 border border-action-danger/30 text-action-danger rounded-card text-sm cursor-pointer hover:bg-action-danger-50"
        >
          駁回
        </button>
        <button
          @click="approveReview(reviewStatusEnum.Approved.value)"
          class="px-4 py-2 bg-brand-500 hover:opacity-90 text-white rounded-card text-sm cursor-pointer"
        >
          通過
        </button>
      </div>
    </div>
    <!-- #endregion -->
  </div>
</template>
