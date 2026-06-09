<script setup>
import { getDashboard } from '@/api/dashBoardService';

/*
   變數名稱代表意義
   isFiltering : 是否為第一次加載
*/
const isFiltering = ref(false);

const dashboardData = ref({
  totalRevenue: 0,
  monthlyRevenue: 0,
  weekSales: [],
  lowStockProducts: [],
  topSellingProducts: [],
});
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
  getSellerData(true);
});

/*
   查看賣家所有數據
*/
const getSellerData = async (isFirstload = false) => {
  try {
    // 判斷是不是第一次加載
    if (isFirstload) {
      showLoading();
    } else {
      isFiltering.value = true;
    }

    const res = await getDashboard();
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功 !');
      dashboardData.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
    isFiltering.value = false;
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6 bg-gray-50/50 min-h-screen">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 m-0">數據中心</h1>
        <p class="text-xs text-gray-400 mt-1">即時掌握店鋪營運狀態與商品銷量</p>
      </div>
      <button
        @click="getSellerData(false)"
        :disabled="isFiltering"
        class="flex items-center gap-2 px-4 py-2 border border-gray-200 rounded-lg bg-white text-sm text-gray-600 hover:bg-gray-50 cursor-pointer transition-all active:scale-95 disabled:opacity-50"
      >
        <i :class="['pi pi-refresh text-xs', { 'pi-spin': isFiltering }]" />
        重新整理
      </button>
    </div>
    <template v-if="isFiltering">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <Skeleton width="100%" height="96px" class="rounded-xl" />
        <Skeleton width="100%" height="96px" class="rounded-xl" />
      </div>
      <Skeleton width="100%" height="240px" class="rounded-xl mb-6" />
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Skeleton width="100%" height="320px" class="rounded-xl" />
        <Skeleton width="100%" height="320px" class="rounded-xl" />
      </div>
    </template>

    <template v-else>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div
          class="bg-white p-6 rounded-xl border border-gray-100 shadow-sm flex items-center justify-between"
        >
          <div class="space-y-2">
            <span class="text-sm font-medium text-gray-400 block">累積總銷售額</span>
            <span class="text-3xl font-bold text-gray-900 block"
              >$ {{ dashboardData.totalRevenue?.toLocaleString() }}</span
            >
          </div>
          <div
            class="w-12 h-12 rounded-lg bg-orange-50 flex items-center justify-center text-orange-500"
          >
            <i class="pi pi-wallet text-xl" />
          </div>
        </div>

        <div
          class="bg-white p-6 rounded-xl border border-gray-100 shadow-sm flex items-center justify-between"
        >
          <div class="space-y-2">
            <span class="text-sm font-medium text-gray-400 block">本月份銷售額</span>
            <span class="text-3xl font-bold text-orange-500 block"
              >$ {{ dashboardData.monthlyRevenue?.toLocaleString() }}</span
            >
          </div>
          <div
            class="w-12 h-12 rounded-lg bg-blue-50 flex items-center justify-center text-blue-500"
          >
            <i class="pi pi-chart-bar text-xl" />
          </div>
        </div>
      </div>
      <div class="bg-white p-6 rounded-xl border border-gray-100 shadow-sm mb-6">
        <div class="flex items-center justify-between mb-4">
          <span class="text-base font-bold text-gray-800">近七天個別銷售額</span>
          <span class="text-xs text-gray-400">僅顯示有營收交易之日期</span>
        </div>

        <div class="grid grid-cols-2 sm:grid-cols-4 md:grid-cols-7 gap-4">
          <div
            v-for="sales in dashboardData.weekSales"
            :key="sales.orderDate"
            class="bg-gray-50/70 p-4 rounded-xl border border-gray-100 text-center flex flex-col justify-between gap-2"
          >
            <span
              class="text-xs font-semibold text-gray-500 bg-gray-200/60 px-2 py-0.5 rounded-full inline-block mx-auto"
            >
              {{ formatDateString(sales.orderDate) }}
            </span>
            <div>
              <span class="text-xs text-gray-400 block mb-0.5">當日營收</span>
              <span class="text-sm font-bold text-gray-800"
                >$ {{ sales.dailyRevenue?.toLocaleString() }}</span
              >
            </div>
          </div>
          <div
            v-if="!dashboardData.weekSales || dashboardData.weekSales.length === 0"
            class="col-span-full py-6 text-center text-gray-400 text-sm"
          >
            <i class="pi pi-calendar-times mr-2" />暫無近七日營收數據
          </div>
        </div>
      </div>
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          class="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden flex flex-col"
        >
          <div class="p-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
            <span class="text-base font-bold text-gray-800 flex items-center gap-2">
              <i class="pi pi-crown text-amber-500" /> 銷量最好前五商品
            </span>
          </div>
          <div class="divide-y divide-gray-100 flex-1">
            <div
              v-if="
                !dashboardData.topSellingProducts || dashboardData.topSellingProducts.length === 0
              "
              class="py-12 text-center text-gray-400 text-sm"
            >
              暫無銷售數據
            </div>
            <div
              v-for="(item, index) in dashboardData.topSellingProducts"
              :key="item.productsId"
              class="flex items-center justify-between p-4 hover:bg-gray-50/50 transition-colors"
            >
              <div class="flex items-center gap-3 min-w-0">
                <span
                  :class="[
                    'w-5 h-5 flex items-center justify-center rounded-full text-[10px] font-bold shrink-0',
                    index === 0
                      ? 'bg-amber-500 text-white'
                      : index === 1
                        ? 'bg-gray-400 text-white'
                        : index === 2
                          ? 'bg-amber-700 text-white'
                          : 'bg-gray-100 text-gray-500',
                  ]"
                >
                  {{ index + 1 }}
                </span>
                <div class="flex flex-col min-w-0">
                  <span class="text-sm font-medium text-gray-700 truncate">{{
                    item.productsName
                  }}</span>
                  <span class="text-xs text-gray-400"
                    >單價: ${{ item.productsPrice }} | 庫存: {{ item.productsStock }}</span
                  >
                </div>
              </div>
              <div class="text-right shrink-0 ml-4">
                <span class="text-sm font-bold text-gray-900 block">{{ item.totalSales }} 件</span>
                <span class="text-xs text-gray-400 block">累積銷量</span>
              </div>
            </div>
          </div>
        </div>

        <div
          class="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden flex flex-col"
        >
          <div class="p-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
            <span class="text-base font-bold text-gray-800 flex items-center gap-2">
              <i class="pi pi-exclamation-triangle text-red-500" /> 庫存告急警報
            </span>
            <span class="text-xs bg-red-50 text-red-600 px-2 py-0.5 rounded-full font-medium"
              >庫存 &lt; 5</span
            >
          </div>
          <div class="divide-y divide-gray-100 flex-1">
            <div
              v-if="!dashboardData.lowStockProducts || dashboardData.lowStockProducts.length === 0"
              class="py-12 text-center text-sm flex flex-col items-center justify-center gap-2 text-gray-400"
            >
              <i class="pi pi-check-circle text-green-500 text-xl" />
              <span class="text-green-600 font-medium">目前庫存十分充足！</span>
            </div>
            <div
              v-for="product in dashboardData.lowStockProducts"
              :key="product.productsId"
              class="flex items-center justify-between p-4 hover:bg-gray-50/50 transition-colors"
            >
              <div class="flex flex-col gap-1 min-w-0">
                <span class="text-sm font-medium text-gray-700 truncate">{{
                  product.productsName
                }}</span>
                <span class="text-xs text-gray-400"
                  >商品 ID: #{{ product.productsId }} | 售價: ${{ product.productsPrice }}</span
                >
              </div>
              <div class="text-right shrink-0 ml-4">
                <span
                  :class="
                    product.productsStock === 0
                      ? 'text-red-500 bg-red-50 border border-red-100'
                      : 'text-orange-500 bg-orange-50 border border-orange-100'
                  "
                  class="text-xs font-bold px-2.5 py-1 rounded-lg inline-block"
                >
                  剩餘 {{ product.productsStock }} 件
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
