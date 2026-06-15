<script setup>
import { getUserCoupon } from '@/api/admin/couponService';
import { onMounted } from 'vue';

/*
   變數名稱代表意義
   allCoupons : 用戶的所有優惠卷
*/
const allCoupons = ref();

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化
*/
onMounted(() => {
  getMyCoupon();
});

/*
   加入已經寫好的驗證規則
*/
// const rules = computed(() => ({
//   oldPassword: { vaildLoginPassword, required },
//   newPassword: { vaildLoginPassword, required },
// }));

/*
   加入套件驗證設定
*/
// const v$ = useVuelidate(
//   rules,
//   { oldPassword, newPassword },
//   { $autoDirty: true, $lazy: true, $scope: false },
// );

/*
   查看用戶優惠卷
*/
const getMyCoupon = async () => {
  // const isFormCorrect = await v$.value.$validate();
  // if (!isFormCorrect) return;
  try {
    showLoading();

    const res = await getUserCoupon();

    const { data } = res;

    if (data.codeStatus === 2000) {
      allCoupons.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

const getCouponTypeName = (type) => {
  return Object.values(couponTypeEnum).find((t) => t.value === type)?.description ?? '未知類型';
};
</script>

<template>
  <div class="flex flex-col w-full p-6">
    <p class="text-2xl font-bold m-0 mb-4">我的優惠券</p>

    <!-- 空狀態 -->
    <div
      v-if="!allCoupons || allCoupons.length === 0"
      class="flex flex-col items-center justify-center py-16 text-gray-400"
    >
      <i class="pi pi-ticket text-4xl mb-3" />
      <span class="text-sm">目前沒有優惠券</span>
    </div>

    <!-- 優惠券列表 -->
    <div
      v-for="coupon in allCoupons"
      :key="coupon.couponId"
      class="bg-white rounded-lg border border-gray-100 p-5 mb-3 flex items-stretch gap-0"
    >
      <!-- 左側優惠碼 -->
      <div
        class="flex flex-col items-center justify-center pr-5 min-w-28 border-r border-dashed border-gray-200"
      >
        <p class="text-xs text-gray-400 mb-2">優惠碼</p>
        <div
          class="bg-gray-50 border border-dashed border-gray-300 rounded-lg px-3 py-1.5 font-mono text-sm tracking-widest"
        >
          {{ coupon.code }}
        </div>
      </div>

      <!-- 右側資訊 -->
      <div class="flex-1 pl-5 grid grid-cols-3 gap-3">
        <div>
          <p class="text-xs text-gray-400 mb-1">名稱</p>
          <p class="text-sm">{{ coupon.name }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">類型</p>
          <span class="px-2 py-0.5 rounded-full text-xs bg-blue-50 text-blue-700">
            {{ getCouponTypeName(coupon.type) }}
          </span>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">狀態</p>
          <span
            class="px-2 py-0.5 rounded-full text-xs"
            :class="coupon.isActive ? 'bg-green-50 text-green-700' : 'bg-gray-100 text-gray-500'"
          >
            {{ coupon.isActive ? '啟用中' : '未啟用' }}
          </span>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">折扣</p>
          <p class="text-sm text-orange-500 font-medium">
            {{ coupon.type === 1 ? `${coupon.discount} 折` : `$ ${coupon.discount} 元 ` }}
          </p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">最低消費</p>
          <p class="text-sm">
            {{ coupon.minimunSpend > 0 ? `$ ${coupon.minimunSpend}` : '無限制' }}
          </p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">有效期限</p>
          <p class="text-xs text-gray-600">
            {{ formatDateTimeString(coupon.startTime) }} ～
            {{ formatDateTimeString(coupon.endTime) }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
