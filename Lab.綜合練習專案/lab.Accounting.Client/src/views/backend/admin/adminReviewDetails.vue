<script setup>
import {
  getProductsReview,
  approveOrRejectProductsReview,
  getReviewAllImg,
} from '@/api/admin/reviewService';
import { getProduct } from '@/api/productsService';
import { reviewStatusEnum } from '@/common/enum';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路由
   review : 審查資訊
   notPassReson : 駁回原因
   oldProduct : 舊的商品資料
   baseUrl : 環境變數裡的圖片基底位址
   reviewImgs : 審核表圖片 ( 新增商品時先預覽用的 )
*/
const route = useRoute();
const router = useRouter();
const review = ref();
const notPassReson = ref();
const oldProduct = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;
const reviewImgs = ref([]);
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
      if (data.returnData.productsId) {
        const productRes = await getProduct(data.returnData.productsId);
        if (productRes.data.codeStatus === 2000) {
          oldProduct.value = productRes.data.returnData;  
        }
      } else {
        const imgRes = await getReviewAllImg(data.returnData.productsReviewId);
        if (imgRes.data.codeStatus === 2000) {
          reviewImgs.value = imgRes.data.returnData;
        }
      }
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
      productsReviewId: review.value.productsReviewId,
      reviewStatus: status,
      notPassReson: status === reviewStatusEnum.Reject.value ? notPassReson.value : null,
    };

    const res = await approveOrRejectProductsReview(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功!');
      router.push({ name: 'admin-allreview' });
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (img) => {
  if (img && img.productsImg) {
    return `${baseUrl}/ProductsImg/${img.productsImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="review">
    <!--#region  標題列-->
    <div class="flex items-center gap-3 mb-4">
      <button
        @click="router.push({ name: 'admin-allreview' })"
        class="p-2 rounded-card hover:bg-surface-muted cursor-pointer text-ink-500"
      >
        <i class="pi pi-arrow-left text-sm" />
      </button>
      <p class="text-2xl font-bold m-0 text-ink-900">審查詳細</p>
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
      <span class="text-sm text-ink-500 ml-auto"># {{ review.productsReviewId }}</span>
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
    <!--#region  商品變更比對 ( 有 productsId , 代表更新商品 ,  無 productsId , 代表新增商品 )-->
    <div class="bg-page-bg rounded-card border border-border-soft p-5 mb-3">
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-box text-xs" />{{ review.productsId ? '商品變更比對' : '商品新增' }}
      </p>

      <!-- 比對標頭（更新才顯示） -->
      <div
        v-if="review.productsId"
        class="grid grid-cols-[120px_1fr_1fr] mb-2 pb-2 border-b border-border-soft"
      >
        <span></span>
        <span class="text-xs text-ink-500 font-medium">舊商品</span>
        <span class="text-xs text-ink-500 font-medium">申請變更</span>
      </div>

      <!-- 商品名稱 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">商品名稱</span>
        <span v-if="review.productsId">
          <span
            v-if="oldProduct?.productsName !== review.productsName"
            class="px-1.5 py-0.5 rounded text-xs bg-action-danger-50 text-action-danger"
          >
            {{ oldProduct?.productsName }}
          </span>
          <span v-else class="text-sm text-ink-500">{{ oldProduct?.productsName }}</span>
        </span>
        <span>
          <span
            v-if="review.productsId && oldProduct?.productsName !== review.productsName"
            class="px-1.5 py-0.5 rounded text-xs bg-status-success/10 text-status-success"
          >
            {{ review.productsName }}
          </span>
          <span v-else class="text-sm text-ink-500">{{ review.productsName }}</span>
        </span>
      </div>

      <!-- 商品價格 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">商品價格</span>
        <span v-if="review.productsId">
          <span
            v-if="oldProduct?.productsPrice !== review.productsPrice"
            class="px-1.5 py-0.5 rounded text-xs bg-action-danger-50 text-action-danger"
          >
            $ {{ oldProduct?.productsPrice }}
          </span>
          <span v-else class="text-sm text-ink-500">$ {{ oldProduct?.productsPrice }}</span>
        </span>
        <span>
          <span
            v-if="review.productsId && oldProduct?.productsPrice !== review.productsPrice"
            class="px-1.5 py-0.5 rounded text-xs bg-status-success/10 text-status-success"
          >
            $ {{ review.productsPrice }}
          </span>
          <span v-else class="text-sm text-ink-500">$ {{ review.productsPrice }}</span>
        </span>
      </div>

      <!-- 庫存 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">庫存</span>
        <span v-if="review.productsId">
          <span
            v-if="oldProduct?.productsStock !== review.productsStock"
            class="px-1.5 py-0.5 rounded text-xs bg-action-danger-50 text-action-danger"
          >
            {{ oldProduct?.productsStock }} 件
          </span>
          <span v-else class="text-sm text-ink-500">{{ oldProduct?.productsStock }} 件</span>
        </span>
        <span>
          <span
            v-if="review.productsId && oldProduct?.productsStock !== review.productsStock"
            class="px-1.5 py-0.5 rounded text-xs bg-status-success/10 text-status-success"
          >
            {{ review.productsStock }} 件
          </span>
          <span v-else class="text-sm text-ink-500">{{ review.productsStock }} 件</span>
        </span>
      </div>

      <!-- 類別 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">類別</span>
        <span v-if="review.productsId">
          <span
            v-if="oldProduct?.productCategoryId !== review.productCategoryId"
            class="px-1.5 py-0.5 rounded text-xs bg-action-danger-50 text-action-danger"
          >
            {{ oldProduct?.parentCategoryName ? `${oldProduct.parentCategoryName} › ` : ''
            }}{{ oldProduct?.productCategoryName }}
          </span>
          <span v-else class="text-sm text-ink-500">
            {{ oldProduct?.parentCategoryName ? `${oldProduct.parentCategoryName} › ` : ''
            }}{{ oldProduct?.productCategoryName }}
          </span>
        </span>
        <span>
          <span
            v-if="review.productsId && oldProduct?.productCategoryId !== review.productCategoryId"
            class="px-1.5 py-0.5 rounded text-xs bg-status-success/10 text-status-success"
          >
            {{ review.productCategoryName }}
          </span>
          <span v-else class="text-sm text-ink-500">{{ review.productCategoryName }}</span>
        </span>
      </div>

      <!-- 折扣 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">折扣</span>
        <span v-if="review.productsId">
          <span
            v-if="oldProduct?.discount !== review.discount"
            class="px-1.5 py-0.5 rounded text-xs bg-action-danger-50 text-action-danger"
          >
            {{ oldProduct?.discount ? `${oldProduct.discount} 折` : '無折扣' }}
          </span>
          <span v-else class="text-sm text-ink-500">{{
            oldProduct?.discount ? `${oldProduct.discount} 折` : '無折扣'
          }}</span>
        </span>
        <span>
          <span
            v-if="review.productsId && oldProduct?.discount !== review.discount"
            class="px-1.5 py-0.5 rounded text-xs bg-status-success/10 text-status-success"
          >
            {{ review.discount ? `${review.discount} 折` : '無折扣' }}
          </span>
          <span v-else class="text-sm text-ink-500">{{
            review.discount ? `${review.discount} 折` : '無折扣'
          }}</span>
        </span>
      </div>

      <!-- 折扣期間 -->
      <div class="grid grid-cols-[120px_1fr_1fr] py-2 border-b border-border-soft items-start">
        <span class="text-xs text-ink-500">折扣期間</span>
        <span v-if="review.productsId" class="text-xs text-ink-500">
          {{ oldProduct?.discountStart ? formatDateTimeString(oldProduct.discountStart) : '—' }}
          {{ oldProduct?.discountEnd ? ` ～ ${formatDateTimeString(oldProduct.discountEnd)}` : '' }}
        </span>
        <span class="text-xs text-ink-500">
          {{ review.discountStart ? formatDateTimeString(review.discountStart) : '—' }}
          {{ review.discountEnd ? ` ～ ${formatDateTimeString(review.discountEnd)}` : '' }}
        </span>
      </div>

      <!-- 商品描述 -->
      <div class="mt-4 pt-4 border-t border-border-soft">
        <div :class="review.productsId ? 'grid grid-cols-2 gap-4' : ''">
          <div v-if="review.productsId">
            <p class="text-xs text-ink-500 mb-2">商品描述（舊）</p>
            <div
              class="bg-surface-muted rounded-card p-3 text-sm prose max-w-none max-h-100 overflow-y-auto [&_img]:max-w-2xl [&_img]:max-h-80 text-ink-900"
              v-html="oldProduct?.productsDescription ?? '—'"
            />
          </div>
          <div>
            <p class="text-xs text-ink-500 mb-2">
              {{ review.productsId ? '商品描述（申請變更）' : '商品描述' }}
            </p>
            <div
              class="bg-surface-muted rounded-card p-3 text-sm prose max-w-none max-h-100 overflow-y-auto [&_img]:max-w-2xl [&_img]:max-h-80 text-ink-900"
              v-html="review.productsDescription ?? '—'"
            />
          </div>
        </div>
      </div>

      <!-- 商品圖片 -->
      <div class="mt-4 pt-4 border-t border-border-soft">
        <p class="text-xs text-ink-500 mb-2">商品圖片（現有）</p>
        <div class="flex flex-wrap gap-2">
          <template v-if="review.productsId">
            <img
              v-for="img in oldProduct?.productsImgs"
              :key="img.productsImgId"
              :src="getProductsImg(img)"
              class="w-50 h-50 object-cover rounded-card border border-border-soft"
            />
            <span v-if="!oldProduct?.productsImgs?.length" class="text-sm text-ink-500"
              >無圖片</span
            >
          </template>
          <template v-else>
            <img
              v-for="img in reviewImgs"
              :key="img.productsImgId"
              :src="getProductsImg(img)"
              class="w-50 h-50 object-cover rounded-card border border-border-soft"
            />
            <span v-if="!reviewImgs.length" class="text-sm text-ink-500">無圖片</span>
          </template>
        </div>
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
