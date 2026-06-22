<script setup>
import { getUserCoupon, getCanReceiveCoupon, createUserCoupon } from '@/api/couponService';

/*
   變數名稱代表意義
   myCoupons : 用戶的所有優惠卷
   mainTab : 大頁面 ( 優惠卷 / 錢包 / 點數 .. )
   couponTab : 優惠卷頁面 ( 可領取 / 已領取 )
   availableCoupons : 可領取但尚未領取的優惠卷
   receiveCoupon : 選取的優惠卷
*/
const myCoupons = ref();
const mainTab = ref('coupon');
const couponTab = ref('receive');
const availableCoupons = ref();
const receiveCouponId = ref(null);

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
  getReceiveCoupon();
});

/*
   查看用戶優惠卷
*/
const getMyCoupon = async () => {
  try {
    showLoading();

    const res = await getUserCoupon();
    const { data } = res;

    if (data.codeStatus === 2000) {
      myCoupons.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   查看可領取優惠卷
*/
const getReceiveCoupon = async () => {
  try {
    showLoading();

    const res = await getCanReceiveCoupon();
    const { data } = res;

    if (data.codeStatus === 2000) {
      availableCoupons.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   領取優惠卷
*/
const receiveCoupon = async (coupon) => {
  if (receiveCouponId.value) return;

  try {
    showLoading();
    receiveCouponId.value = coupon.couponId;
    const request = {
      couponId: receiveCouponId.value,
    };
    const res = await createUserCoupon(request);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('領取成功 !');
      availableCoupons.value = availableCoupons.value.filter((c) => c.couponId !== coupon.couponId);
    }

    getMyCoupon();
  } catch (err) {
    console.log(err);
  } finally {
    receiveCouponId.value = null;
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6">
    <p class="text-2xl font-bold m-0 mb-4">我的錢包</p>

    <!-- #region  主分頁-->
    <div class="flex gap-2 border-b border-gray-200 mb-5">
      <button
        class="border-0 px-1 py-2.5 text-sm cursor-pointer bg-transparent"
        :class="mainTab === 'coupon' ? 'font-bold border-b-2 border-black' : 'text-gray-400'"
        @click="mainTab = 'coupon'"
      >
        優惠券
      </button>
      <button
        class="border-0 px-1 py-2.5 text-sm cursor-not-allowed bg-transparent text-gray-400 flex items-center gap-1.5"
        disabled
      >
        點數
        <span class="text-xs bg-gray-100 text-gray-400 px-1.5 py-0.5 rounded-full">敬請期待</span>
      </button>
      <button
        class="border-0 px-1 py-2.5 text-sm cursor-not-allowed bg-transparent text-gray-400 flex items-center gap-1.5"
        disabled
      >
        儲值
        <span class="text-xs bg-gray-100 text-gray-400 px-1.5 py-0.5 rounded-full">敬請期待</span>
      </button>
    </div>
    <!-- #endregion -->

    <template v-if="mainTab === 'coupon'">
      <!-- #region  優惠卷次分頁-->
      <div class="flex gap-2 mb-4">
        <button
          class="text-xs px-3.5 py-1.5 rounded-md cursor-pointer"
          :class="
            couponTab === 'receive'
              ? 'bg-black text-white'
              : 'border border-gray-300 text-gray-500 bg-transparent'
          "
          @click="couponTab = 'receive'"
        >
          可領取 {{ availableCoupons?.length ?? 0 }}
        </button>
        <button
          class="text-xs px-3.5 py-1.5 rounded-md cursor-pointer"
          :class="
            couponTab === 'received'
              ? 'bg-black text-white'
              : 'border border-gray-300 text-gray-500 bg-transparent'
          "
          @click="couponTab = 'received'"
        >
          已領取 {{ myCoupons?.length ?? 0 }}
        </button>
      </div>
      <!-- #endregion -->

      <!-- #region  可領取列表-->
      <template v-if="couponTab === 'receive'">
        <div
          v-if="!availableCoupons || availableCoupons.length === 0"
          class="flex flex-col items-center justify-center py-16 text-gray-400"
        >
          <i class="pi pi-ticket text-4xl mb-3" />
          <span class="text-sm">目前沒有可領取的優惠券</span>
        </div>

        <div
          v-for="coupon in availableCoupons"
          :key="coupon.couponId"
          class="bg-white rounded-lg border border-gray-100 p-5 mb-3 flex items-stretch gap-0"
        >
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

          <div class="flex-1 pl-5 flex items-center justify-between gap-3">
            <div>
              <p class="text-sm m-0">{{ coupon.name }}</p>
              <p class="text-xs text-gray-400 m-0 mt-1">
                {{ coupon.minimunSpend > 0 ? `滿 $${coupon.minimunSpend} 可用` : '無門檻' }}
                ·
                {{ formatDateTimeString(coupon.endTime) }} 到期
              </p>
            </div>
            <div class="flex items-center gap-3">
              <p class="text-sm font-medium text-orange-500 m-0">
                {{
                  coupon.type === couponTypeEnum.百分比折扣.value
                    ? `${coupon.discount} 折`
                    : `$${coupon.discount} 元`
                }}
              </p>
              <button
                class="bg-black text-white text-xs px-4 py-1.5 rounded-md cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                :disabled="receiveCouponId === coupon.couponId"
                @click="receiveCoupon(coupon)"
              >
                領取
              </button>
            </div>
          </div>
        </div>
      </template>
      <!-- #endregion -->

      <!-- #region  已領取列表-->
      <template v-else>
        <div
          v-if="!myCoupons || myCoupons.length === 0"
          class="flex flex-col items-center justify-center py-16 text-gray-400"
        >
          <i class="pi pi-ticket text-4xl mb-3" />
          <span class="text-sm">目前沒有優惠券</span>
        </div>

        <div
          v-for="coupon in myCoupons"
          :key="coupon.couponId"
          class="relative flex bg-white rounded-lg border border-gray-100 overflow-hidden mb-3"
          :class="{ 'bg-gray-50': coupon.usedTime }"
       
        >
          <!-- 左側狀態色條 -->
          <div class="w-1.5" :class="coupon.usedTime ? 'bg-gray-300' : 'bg-orange-400'"></div>

          <!-- 折扣金額區塊 -->
          <div class="w-28 flex flex-col items-center justify-center py-4 relative">
            <p
              class="text-2xl font-medium m-0"
              :class="coupon.usedTime ? 'text-gray-400' : 'text-orange-500'"
            >
              {{
                coupon.type === couponTypeEnum.百分比折扣.value
                  ? `${coupon.discount}折`
                  : `$${coupon.discount}`
              }}
            </p>
            <p class="text-xs text-gray-400 mt-1">
              {{ getEnumDescription(coupon.type) }}
            </p>
          </div>

          <!-- 車票打孔虛線 -->
          <div class="relative my-3" style="border-left: 1px dashed #e5e7eb">
            <div
              class="absolute -top-1.5 -left-1.5 w-3.5 h-3.5 rounded-full bg-gray-50 border border-gray-100"
            ></div>
            <div
              class="absolute -bottom-1.5 -left-1.5 w-3.5 h-3.5 rounded-full bg-gray-50 border border-gray-100"
            ></div>
          </div>

          <!-- 右側資訊 -->
          <div class="flex-1 px-5 py-4 flex flex-col justify-center gap-1.5">
            <div class="flex items-center justify-between">
              <p class="text-sm font-medium m-0" :class="{ 'text-gray-400': coupon.usedTime }">
                {{ coupon.name }}
              </p>
              <span
                class="px-2 py-0.5 rounded-full text-xs"
                :class="
                  coupon.usedTime
                    ? 'bg-white text-gray-400 border border-gray-200'
                    : coupon.isActive
                      ? 'bg-green-50 text-green-700'
                      : 'bg-gray-100 text-gray-500'
                "
              >
                {{ coupon.usedTime ? '已使用' : coupon.isActive ? '啟用中' : '未啟用' }}
              </span>
            </div>
            <p class="text-xs text-gray-400 m-0">
              {{ coupon.minimunSpend > 0 ? `滿 $${coupon.minimunSpend} 可用` : '無門檻' }}
              ·
              {{ formatDateTimeString(coupon.startTime) }} ～
              {{ formatDateTimeString(coupon.endTime) }}
            </p>
            <p class="font-mono text-xs text-gray-400 m-0 mt-0.5">{{ coupon.code }}</p>
          </div>

          <!-- 已使用浮水印 -->
          <div
            v-if="coupon.usedTime"
            class="absolute inset-0 flex items-center justify-center pointer-events-none"
          >
            <span
              class="text-2xl font-medium text-gray-300 opacity-60 tracking-widest"
              style="transform: rotate(-18deg)"
            >
              已使用
            </span>
          </div>
        </div>
      </template>
      <!-- #endregion -->
    </template>
  </div>
</template>
