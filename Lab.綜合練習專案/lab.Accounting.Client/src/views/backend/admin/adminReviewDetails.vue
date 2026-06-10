<script setup>
import { getProductsReview, approveOrRejectProductsReview } from '@/api/admin/reviewService';
import { reviewStatusEnum } from '@/common/enum';
import { useRoute } from 'vue-router';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路由
   review : 審查資訊
   notPassReson : 駁回原因
*/
const route = useRoute();
const router = useRouter();
const review = ref();
const notPassReson = ref();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時
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
   查看審查表
*/
const getReview = async (id) => {
  try {
    showLoading();
    const res = await getProductsReview(id);
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

const approveReview = async (status) => {
  if (status === reviewStatusEnum.Reject.value) {
    const isFormCorrect = await v$.value.$validate();
    if (!isFormCorrect) return;
  }

  try {
    const request = {
      productsReviewId: review.value.productsReviewId,
      reviewStatus: status,
      notPassReson: status === reviewStatusEnum.Reject.value ? notPassReson.value : null,
    };

    const res = await approveOrRejectProductsReview(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功!');
      router.push({ name: 'admin-review' });
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="review">
    <!--#region  商品資訊-->
    <div class="bg-white rounded-lg border border-gray-100 p-5 mb-3">
      <p class="text-sm text-gray-400 flex items-center gap-1 mb-4">
        <i class="pi pi-box text-xs" />商品資訊
      </p>
      <div class="grid grid-cols-3 gap-4 mb-4">
        <div>
          <p class="text-xs text-gray-400 mb-1">商品名稱</p>
          <p class="text-sm">{{ review.productsName }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">商品價格</p>
          <p class="text-sm text-orange-500">$ {{ review.productsPrice }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">商品庫存</p>
          <p class="text-sm">{{ review.productsStock }} 件</p>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    
    <!--#region  審核人員資訊-->
    <div class="bg-white rounded-lg border border-gray-100 p-5 mb-3">
      <p class="text-sm text-gray-400 flex items-center gap-1 mb-4">
        <i class="pi pi-user text-xs" />審核人員資訊
      </p>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <p class="text-xs text-gray-400 mb-1">賣家</p>
          <p class="text-sm">{{ review.sellerName ?? '—' }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">審核人員</p>
          <p class="text-sm">{{ review.adminName ?? '—' }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">申請時間</p>
          <p class="text-sm text-gray-500">{{ formatDateTimeString(review.createTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">審核時間</p>
          <p class="text-sm text-gray-500">
            {{ review.reviewTime ? formatDateTimeString(review.reviewTime) : '—' }}
          </p>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    <!--#region  駁回原因（有駁回才顯示）-->
    <div
      v-if="review.reviewStatus === 2"
      class="bg-red-50 rounded-lg border border-red-100 p-5 mb-3"
    >
      <p class="text-sm text-red-400 flex items-center gap-1 mb-2">
        <i class="pi pi-times-circle text-xs" />駁回原因
      </p>
      <p class="text-sm text-red-700">{{ review.notPassReson }}</p>
    </div>
      <!-- #endregion -->

   <!--#region  審核操作（待審核才顯示）-->
    <div v-if="review.reviewStatus === 0" class="bg-white rounded-lg border border-gray-100 p-5">
      <p class="text-sm text-gray-400 flex items-center gap-1 mb-4">
        <i class="pi pi-check-circle text-xs" />審核操作
      </p>
      <div class="mb-4">
        <p class="text-xs text-gray-400 mb-1">駁回原因（駁回時必填）</p>
        <textarea
          v-model="notPassReson"
          class="w-full border border-gray-200 rounded-lg p-2 text-sm resize-y min-h-20 outline-none focus:border-gray-400"
          placeholder="請輸入駁回原因..."
        />
        <InValidErrorMessage :errorDto="v$.notPassReson.$errors" vaildChiName="駁回原因" />
      </div>
      <div class="flex justify-end gap-2">
        <button
          @click="approveReview(reviewStatusEnum.Reject.value)"
          class="px-4 py-2 border border-red-200 text-red-500 rounded-lg text-sm cursor-pointer hover:bg-red-50"
        >
          駁回
        </button>
        <button
          @click="approveReview(reviewStatusEnum.Approved.value)"
          class="px-4 py-2 bg-orange-500 hover:bg-orange-600 text-white rounded-lg text-sm cursor-pointer"
        >
          通過
        </button>
      </div>
        <!-- #endregion -->
    </div>

  </div>
</template>
