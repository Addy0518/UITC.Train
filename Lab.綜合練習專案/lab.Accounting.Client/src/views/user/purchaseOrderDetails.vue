<script setup>
import { getUserOneOrder } from '@/api/orderService';
import { logisticsEnum } from '@/common/enum';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   order : 訂單
   baseUrl : 環境變數裡的圖片基底位址
   route : 獲取路由資訊
   isCvs : 是否為超商取貨
   isHomeDelivery : 是否為宅配
*/

const route = useRoute();
const order = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const isCvs = computed(() => order.value?.logisticsType === 'CVS');
const isHomeDelivery = computed(() => order.value?.logisticsType === 'Home');

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時拿到訂單
*/
onMounted(() => {
  if (route.params.id) {
    getOneOrder(route.params.id);
  }
});

/*
   依照物流狀態改變文字跟背景顏色：
   文字從 shippingEnum 取，顏色在 badgeStyleMap 管
*/
const badgeStyleMap = {
  [shippingEnum.PendingPayment.value]: { bg: '#fcebeb', color: '#a32d2d' },
  [shippingEnum.PendingShipment.value]: { bg: '#faeeda', color: '#854f0b' },
  [shippingEnum.InTransit.value]: { bg: '#fff4ed', color: '#c9543f' },
  [shippingEnum.Arrived.value]: { bg: '#e6f1fb', color: '#185fa5' },
  [shippingEnum.Completed.value]: { bg: '#e1f5ee', color: '#0f6e56' },
  [shippingEnum.Cancelled.value]: { bg: '#f1efe8', color: '#888780' },
};

/*
   status : 物流狀態文字
   style : 文字跟背景顏色
*/
const shippingBadge = computed(() => {
  const status = order.value?.shippingStatus;
  const style = badgeStyleMap[status] ?? { bg: '#f1efe8', color: '#888780' };
  const found = Object.values(shippingEnum).find((e) => e.value === status);
  return { label: found?.description ?? '未知', ...style };
});

/*
   拿到訂單
*/
const getOneOrder = async (id) => {
  try {
    showLoading();
    const res = await getUserOneOrder(id);
    const { data } = res;
    if (data.codeStatus === 2000) {
      order.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImg && product.productsImg.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImg}`;
  }
  return defaultImgurl;
};

/*
   物流進度步驟定義 ( 文字顏色 , Logo 等等 ... )
*/
const logisticsSteps = computed(() => {
  if (!order.value) return [];

  const status = order.value.logisticsStatus;

  /*
     取消或異常時在最上面插入一個警示步驟，
     不走正常流程，讓使用者清楚知道出了什麼狀況
  */
  if (status === logisticsEnum.Cancelled.value) {
    return [
      {
        label: logisticsEnum.Cancelled.description,
        icon: 'ti-x',
        time: '',
        note: order.value.logisticsRtnMessage ?? '',
        special: 'cancelled',
      },
    ];
  }
  if (status === logisticsEnum.Exception.value) {
    return [
      {
        label: logisticsEnum.Exception.description,
        icon: 'ti-alert-triangle',
        time: '',
        note: order.value.logisticsRtnMessage ?? '',
        special: 'exception',
      },
    ];
  }
  /*
     正常流程
  */
  return [
    {
      enumValue: logisticsEnum.Created.value,
      label: logisticsEnum.Created.description,
      icon: 'ti-file-check',
      time: order.value.boughtTime ? formatDateTimeString(order.value.boughtTime) : '',
      note: '',
    },
    {
      enumValue: logisticsEnum.PendingShipment.value,
      label: logisticsEnum.PendingShipment.description,
      icon: 'ti-credit-card',
      time: order.value.paidTime ? formatDateTimeString(order.value.paidTime) : '',
      note: formatPaidType(order.value.paidType),
    },
    {
      enumValue: logisticsEnum.Shipped.value,
      label: logisticsEnum.Shipped.description,
      icon: 'ti-truck',
      time: order.value.shippedAt ? formatDateTimeString(order.value.shippedAt) : '',
      note: order.value.logisticsRtnMessage ?? '',
    },
    {
      enumValue: logisticsEnum.InTransit.value,
      label: logisticsEnum.InTransit.description,
      icon: 'ti-navigation',
      time: '',
      note: '',
    },
    {
      enumValue: logisticsEnum.Delivered.value,
      label: logisticsEnum.Delivered.description,
      icon: 'ti-building-store',
      time: '',
      note: order.value.storeName ?? '',
    },
    {
      enumValue: logisticsEnum.PickedUp.value,
      label: logisticsEnum.PickedUp.description,
      icon: 'ti-circle-check',
      time: order.value.pickedUpAt ? formatDateTimeString(order.value.pickedUpAt) : '',
      note: '',
    },
  ];
});

/*
   取消 / 異常快速判斷，template 用
*/
const isSpecialStatus = computed(() => !!logisticsSteps.value[0]?.special);
</script>

<template>
  <div class="flex flex-col w-full p-6">
    <div class="flex flex-col items-center" v-if="order">
      <!--#region 訂單 -->
      <div
        class="w-full max-w-2xl border border-border-soft rounded-card bg-page-bg overflow-hidden"
      >
        <!--#region 訂單標題 -->
        <div class="px-6 py-4 border-b border-border-soft flex items-center justify-between">
          <div>
            <p class="text-xs text-ink-500 m-0">訂單編號</p>
            <p class="text-sm font-medium mt-1 m-0 text-ink-900">{{ order.orderNumber }}</p>
          </div>
          <span
            class="text-xs px-3 py-1 rounded-card"
            :style="{ background: shippingBadge.bg, color: shippingBadge.color }"
          >
            {{ shippingBadge.label }}
          </span>
        </div>
        <!-- #endregion -->

        <!--#region 商品資訊 -->
        <div class="px-6 py-5 border-b border-border-soft flex gap-4 items-center">
          <img
            :src="getProductsImg(order)"
            alt="商品圖片"
            class="w-16 h-16 rounded-card object-cover border border-border-soft shrink-0"
          />
          <div class="flex-1">
            <p class="font-medium text-sm m-0 mb-1 text-ink-900">{{ order.productsName }}</p>
            <p class="text-xs text-ink-500 m-0 mb-2">數量：{{ order.boughtQuantity }} 件</p>
            <p class="text-base font-medium text-brand-price m-0">
              $ {{ order.accountAmount.toLocaleString() }}
            </p>
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 物流進度 -->
        <div class="px-6 py-5 border-b border-border-soft">
          <p class="text-xs font-medium text-ink-500 mb-4 m-0">物流進度</p>

          <!--#region 取消 / 異常特殊狀態 -->
          <div v-if="isSpecialStatus" class="flex items-start gap-3">
            <div
              class="w-7 h-7 rounded-full flex items-center justify-center shrink-0"
              :style="{
                background: logisticsSteps[0].special === 'exception' ? '#fcebeb' : '#f1efe8',
              }"
            >
              <i
                :class="['ti', logisticsSteps[0].icon, 'text-sm']"
                :style="{
                  color: logisticsSteps[0].special === 'exception' ? '#a32d2d' : '#888780',
                }"
                aria-hidden="true"
              ></i>
            </div>
            <div class="flex-1">
              <p
                class="text-sm font-medium m-0 mb-0.5"
                :style="{
                  color: logisticsSteps[0].special === 'exception' ? '#a32d2d' : '#888780',
                }"
              >
                {{ logisticsSteps[0].label }}
              </p>
              <p v-if="logisticsSteps[0].note" class="text-xs text-ink-500 m-0 mt-0.5">
                {{ logisticsSteps[0].note }}
              </p>
            </div>
          </div>
          <!-- #endregion -->

          <!--#region 正常進度步驟 -->
          <template v-else>
            <div
              v-for="step in logisticsSteps"
              :key="step.enumValue"
              class="flex items-start gap-3"
            >
              <!--#region 步驟 icon 與連接線 -->
              <div class="flex flex-col items-center">
                <div
                  class="w-7 h-7 rounded-full flex items-center justify-center shrink-0 transition-all"
                  :class="{
                    'bg-[#e1f5ee]': order.logisticsStatus > step.enumValue,
                    'bg-[#e6f1fb]': order.logisticsStatus === step.enumValue,
                    'bg-page-bg-soft border border-border-soft':
                      order.logisticsStatus < step.enumValue,
                  }"
                  :style="
                    order.logisticsStatus === step.enumValue
                      ? 'animation: orderPulse 1.8s ease-in-out infinite;'
                      : ''
                  "
                >
                  <i
                    :class="['ti', step.icon, 'text-sm']"
                    :style="{
                      color:
                        order.logisticsStatus > step.enumValue
                          ? '#1d9e75'
                          : order.logisticsStatus === step.enumValue
                            ? '#378add'
                            : '#b4aba3',
                    }"
                    aria-hidden="true"
                  ></i>
                </div>
                <!-- 連接線（最後一步不顯示） -->
                <div
                  v-if="step.enumValue < logisticsEnum.PickedUp.value"
                  class="w-0.5 mt-1"
                  style="min-height: 28px; flex: 1"
                  :style="{
                    background: order.logisticsStatus > step.enumValue ? '#1d9e75' : '#f0e6de',
                  }"
                ></div>
              </div>
              <!-- #endregion -->

              <!--#region 步驟文字 -->
              <div
                class="flex-1"
                :class="step.enumValue < logisticsEnum.PickedUp.value ? 'pb-5' : 'pb-0'"
              >
                <p
                  class="text-sm font-medium m-0 mb-0.5"
                  :style="{
                    color:
                      order.logisticsStatus > step.enumValue
                        ? '#1d9e75'
                        : order.logisticsStatus === step.enumValue
                          ? '#378add'
                          : '#b4aba3',
                  }"
                >
                  {{ step.label }}
                  <span
                    v-if="order.logisticsStatus === step.enumValue"
                    class="ml-2 text-xs font-normal px-2 py-0.5 rounded-full"
                    style="background: #e6f1fb; color: #378add; vertical-align: middle"
                  >
                    目前
                  </span>
                </p>
                <p v-if="step.time" class="text-xs text-ink-500 m-0">{{ step.time }}</p>
                <p v-if="step.note" class="text-xs text-ink-300 m-0 mt-0.5">{{ step.note }}</p>
              </div>
              <!-- #endregion -->
            </div>
          </template>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->

        <!--#region 取貨資訊 ( 只有超商取貨才顯示 ) -->
        <div class="px-6 py-5 border-b border-border-soft" v-if="isCvs">
          <p class="text-xs font-medium text-ink-500 mb-3 m-0">取貨資訊</p>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">取貨門市</p>
              <p class="text-sm m-0 text-ink-900">{{ order.storeName }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">物流類型</p>
              <p class="text-sm m-0 text-ink-900">
                {{ formatLogisticsSubType(order.logisticsSubType) }}
              </p>
            </div>
            <div class="col-span-2">
              <p class="text-xs text-ink-500 m-0 mb-1">門市地址</p>
              <p class="text-sm m-0 text-ink-900">{{ order.storeAddress }}</p>
            </div>
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 收件人資訊 -->
        <div class="px-6 py-5 border-b border-border-soft">
          <p class="text-xs font-medium text-ink-500 mb-3 m-0">收件人資訊</p>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">姓名</p>
              <p class="text-sm m-0 text-ink-900">{{ order.receiverName }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">電話</p>
              <p class="text-sm m-0 text-ink-900">{{ order.receiverPhone }}</p>
            </div>
            <div class="col-span-2" v-if="isHomeDelivery">
              <p class="text-xs text-ink-500 m-0 mb-1">收件地址</p>
              <p class="text-sm m-0 text-ink-900">{{ order.receiverAddress }}</p>
            </div>
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 付款資訊 -->
        <div class="px-6 py-5">
          <p class="text-xs font-medium text-ink-500 mb-3 m-0">付款資訊</p>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">付款方式</p>
              <p class="text-sm m-0 text-ink-900">{{ formatPaidType(order.paidType) }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">實付金額</p>
              <p class="text-base font-medium text-brand-price m-0">
                $ {{ order.accountAmount.toLocaleString() }}
              </p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">購買時間</p>
              <p class="text-sm m-0 text-ink-900">{{ formatDateTimeString(order.paidTime) }}</p>
            </div>
          </div>
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
