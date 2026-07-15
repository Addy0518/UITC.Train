<script setup>
import { getDashboard } from '@/api/dashBoardService';

/*
   變數名稱代表意義
   isFiltering          : 是否正在重新整理（非首次加載用）
   weekSalesChartData   : 近七天銷售額圖表數據
   weekSalesChartOptions: 近七天銷售額圖表選項
   topSellingChartData  : 銷量前五的商品圖表數據
   topSellingChartOptions: 銷量前五的商品圖表選項
   rateChartData        : 評分分布圖表數據
   rateChartOptions     : 評分分布圖表選項
*/
const isFiltering = ref(false);
const weekSalesChartData = ref();
const weekSalesChartOptions = ref();
const topSellingChartData = ref();
const topSellingChartOptions = ref();
const rateChartData = ref();
const rateChartOptions = ref();

/*
   數據中心的所有資料
*/
const dashboardData = ref({
  totalRevenue: 0,
  monthlyRevenue: 0,
  weekSales: [],
  lowStockProducts: [],
  topSellingProducts: [],
  rateDistribution: [],
});

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
  getSellerData(true);
  weekSalesChartOptions.value = setChartOptions();
  const { textColorSecondary, surfaceBorder } = getThemeColors();
  topSellingChartOptions.value = setBarChartOptions(textColorSecondary, surfaceBorder, '件');
  rateChartOptions.value = setBarChartOptions(textColorSecondary, surfaceBorder, '個');
});

/*
   商品名稱截斷，超過 max 字加 …
*/
const truncate = (str, max = 7) => (str.length > max ? str.slice(0, max) + '…' : str);

/*
   查看賣家所有數據
*/
const getSellerData = async (isFirstload = false) => {
  try {
    if (isFirstload) {
      showLoading();
    } else {
      isFiltering.value = true;
    }

    const res = await getDashboard();
    const { data } = res;

    if (data.codeStatus === 2000) {
      if (!isFirstload) showToastSuccess('資料已更新');
      dashboardData.value = data.returnData;
      weekSalesChartData.value = updateWeekSalesChartData();
      topSellingChartData.value = updateTopSellingChartData();
      rateChartData.value = updateRateChartData();
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
    isFiltering.value = false;
  }
};

/*
   取得全局 PrimeVue 主題顏色
*/
const getThemeColors = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  return {
    textColor: documentStyle.getPropertyValue('--p-text-color'),
    textColorSecondary: documentStyle.getPropertyValue('--p-text-muted-color'),
    surfaceBorder: documentStyle.getPropertyValue('--p-content-border-color'),
  };
};

/*
   近七天銷售額折線圖數據
*/
const updateWeekSalesChartData = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  const labels = dashboardData.value.weekSales.map((item) => formatDateString(item.orderDate));
  const revenues = dashboardData.value.weekSales.map((item) => item.dailyRevenue);
  return {
    labels,
    datasets: [
      {
        label: '當日營收',
        data: revenues,
        fill: true,
        borderColor: documentStyle.getPropertyValue('--p-orange-500') || '#ff6b35',
        backgroundColor: 'rgba(255, 107, 53, 0.08)',
        tension: 0.4,
      },
    ],
  };
};

/*
   近七天銷售額折線圖設定
*/
const setChartOptions = () => {
  const { textColorSecondary, surfaceBorder } = getThemeColors();
  return {
    maintainAspectRatio: false,
    aspectRatio: 0.8,
    plugins: {
      legend: { display: false },
      tooltip: { mode: 'index', intersect: false },
    },
    scales: {
      x: {
        ticks: { color: textColorSecondary, font: { size: 13 } },
        grid: { display: false },
      },
      y: {
        beginAtZero: true,
        ticks: { color: textColorSecondary, font: { size: 13 } },
        grid: { color: surfaceBorder },
      },
    },
  };
};

/*
   銷量最好前五商品長條圖數據
*/
const updateTopSellingChartData = () => {
  const products = dashboardData.value.topSellingProducts || [];
  return {
    labels: products.map((item) => truncate(item.productsName)),
    datasets: [
      {
        label: '累積銷量',
        data: products.map((item) => item.totalSales),
        backgroundColor: [
          'rgba(255, 107, 53, 0.15)',
          'rgba(6, 182, 212, 0.15)',
          'rgba(139, 92, 246, 0.15)',
          'rgba(234, 179, 8, 0.15)',
          'rgba(107, 114, 128, 0.15)',
        ],
        borderColor: [
          '#ff6b35',
          'rgb(6, 182, 212)',
          'rgb(139, 92, 246)',
          'rgb(234, 179, 8)',
          'rgb(107, 114, 128)',
        ],
        borderWidth: 1,
        barThickness: 32,
      },
    ],
  };
};

/*
   評分分布長條圖數據
*/
const updateRateChartData = () => {
  const rates = dashboardData.value.rateDistribution || [];
  return {
    labels: rates.map((item) => item.rateDistribution),
    datasets: [
      {
        label: '評分數量',
        data: rates.map((item) => item.rateCount),
        backgroundColor: [
          'rgba(234, 179, 8, 0.15)',
          'rgba(6, 182, 212, 0.15)',
          'rgba(139, 92, 246, 0.15)',
          'rgba(255, 107, 53, 0.15)',
          'rgba(107, 114, 128, 0.15)',
        ],
        borderColor: [
          'rgb(234, 179, 8)',
          'rgb(6, 182, 212)',
          'rgb(139, 92, 246)',
          '#ff6b35',
          'rgb(107, 114, 128)',
        ],
        borderWidth: 1,
        barThickness: 32,
      },
    ],
  };
};

/*
   共用長條圖 Option 生成器
   maxRotation: 0 強制 X 軸標籤不旋轉，搭配 truncate 避免標籤過長
*/
const setBarChartOptions = (textColorSecondary, surfaceBorder, unit = '') => {
  return {
    maintainAspectRatio: false,
    aspectRatio: 0.8,
    plugins: {
      legend: { display: false },
      tooltip: {
        titleFont: { size: 14, weight: 'bold' },
        bodyFont: { size: 13 },
        callbacks: {
          label: (context) => ` ${context.dataset.label}: ${context.raw} ${unit}`,
        },
      },
    },
    scales: {
      x: {
        ticks: {
          color: textColorSecondary,
          font: { size: 12 },
          maxRotation: 0,
          minRotation: 0,
        },
        grid: { display: false },
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: textColorSecondary,
          font: { size: 12 },
          precision: 0,
        },
        grid: { color: surfaceBorder },
      },
    },
  };
};
</script>

<template>
  <!--#region 整體容器 -->
  <div class="flex flex-col w-full p-6 bg-page-bg-soft min-h-screen">
    <!--#region 標題列 -->
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-xl font-bold text-ink-900 m-0">數據中心</h1>
        <p class="text-xs text-ink-500 mt-1 m-0">即時掌握店鋪營運狀態與商品銷量</p>
      </div>
      <button
        @click="getSellerData(false)"
        :disabled="isFiltering"
        class="flex items-center gap-2 px-4 py-2 border border-border-soft rounded-card bg-page-bg text-sm text-ink-500 hover:bg-surface-muted cursor-pointer transition-all active:scale-95 disabled:opacity-50"
      >
        <i :class="['pi pi-refresh text-xs', { 'pi-spin': isFiltering }]" />
        重新整理
      </button>
    </div>
    <!-- #endregion -->

    <!--#region 加載骨架 -->
    <template v-if="isFiltering">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <Skeleton width="100%" height="96px" class="rounded-card" />
        <Skeleton width="100%" height="96px" class="rounded-card" />
      </div>
      <Skeleton width="100%" height="240px" class="rounded-card mb-6" />
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Skeleton width="100%" height="320px" class="rounded-card" />
        <Skeleton width="100%" height="320px" class="rounded-card" />
      </div>
    </template>
    <!-- #endregion -->

    <!--#region 所有數據 -->
    <template v-else>
      <!--#region 銷售額卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div
          class="bg-page-bg p-6 rounded-card border border-border-soft flex items-center justify-between"
        >
          <div>
            <span class="text-sm text-ink-500 block mb-2">累積總銷售額</span>
            <span class="text-3xl font-bold text-ink-900 block">
              $ {{ dashboardData.totalRevenue?.toLocaleString() }}
            </span>
          </div>
          <div class="w-12 h-12 rounded-card bg-brand-50 flex items-center justify-center">
            <i class="pi pi-wallet text-xl text-brand-500" />
          </div>
        </div>

        <div
          class="bg-page-bg p-6 rounded-card border border-border-soft flex items-center justify-between"
        >
          <div>
            <span class="text-sm text-ink-500 block mb-2">本月份銷售額</span>
            <span class="text-3xl font-bold text-brand-price block">
              $ {{ dashboardData.monthlyRevenue?.toLocaleString() }}
            </span>
          </div>
          <div class="w-12 h-12 rounded-card bg-brand-50 flex items-center justify-center">
            <i class="pi pi-chart-bar text-xl text-brand-500" />
          </div>
        </div>
      </div>
      <!-- #endregion -->

      <!--#region 近七天銷售額折線圖 -->
      <div class="bg-page-bg p-6 rounded-card border border-border-soft mb-6">
        <div class="flex items-center justify-between mb-4">
          <span class="text-base font-bold text-ink-900">近七天個別銷售額</span>
          <span class="text-xs text-ink-500">最近一週的每日營收波動</span>
        </div>
        <div class="h-64 w-full">
          <Chart
            v-if="dashboardData.weekSales?.length > 0"
            type="line"
            :data="weekSalesChartData"
            :options="weekSalesChartOptions"
            class="h-full w-full"
          />
          <div v-else class="h-full flex flex-col items-center justify-center gap-2 text-ink-500">
            <i class="pi pi-calendar-times text-4xl" />
            <span class="text-sm">暫無近七日營收數據</span>
          </div>
        </div>
      </div>
      <!-- #endregion -->

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!--#region 銷量最好前五商品長條圖 -->
        <div
          class="bg-page-bg rounded-card border border-border-soft overflow-hidden flex flex-col"
        >
          <div
            class="px-5 py-4 border-b border-border-soft bg-surface-muted flex items-center gap-2"
          >
            <i class="pi pi-crown text-brand-500 text-sm" />
            <span class="text-sm font-bold text-ink-900">銷量最好前五商品</span>
          </div>
          <div class="p-4 flex-1 flex items-center justify-center h-64">
            <Chart
              v-if="dashboardData.topSellingProducts?.length > 0"
              type="bar"
              :data="topSellingChartData"
              :options="topSellingChartOptions"
              class="h-full w-full"
            />
            <div v-else class="text-ink-500 text-sm">暫無銷售數據</div>
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 庫存告急警報 -->
        <div
          class="bg-page-bg rounded-card border border-border-soft overflow-hidden flex flex-col"
        >
          <div
            class="px-5 py-4 border-b border-border-soft bg-surface-muted flex items-center justify-between"
          >
            <div class="flex items-center gap-2">
              <i class="pi pi-exclamation-triangle text-action-danger text-sm" />
              <span class="text-sm font-bold text-ink-900">庫存告急警報</span>
            </div>
            <span
              class="text-xs bg-action-danger-50 text-action-danger px-2 py-0.5 rounded-full font-medium"
            >
              庫存 &lt; 5
            </span>
          </div>
          <div class="flex-1 divide-y divide-border-soft">
            <div
              v-if="!dashboardData.lowStockProducts || dashboardData.lowStockProducts.length === 0"
              class="h-full py-12 flex flex-col items-center justify-center gap-2"
            >
              <i class="pi pi-check-circle text-status-success text-2xl" />
              <span class="text-sm text-status-success font-medium">目前庫存十分充足！</span>
            </div>
            <div
              v-for="product in dashboardData.lowStockProducts"
              :key="product.productsId"
              class="flex items-center justify-between p-4 hover:bg-surface-muted transition-colors"
            >
              <div class="flex flex-col gap-1 min-w-0">
                <span class="text-sm font-medium text-ink-900 truncate">{{
                  product.productsName
                }}</span>
                <span class="text-xs text-ink-500">
                  商品 ID: #{{ product.productsId }} · 售價: ${{ product.productsPrice }}
                </span>
              </div>
              <span
                class="shrink-0 ml-4 text-xs font-bold px-2.5 py-1 rounded-card"
                :class="
                  product.productsStock === 0
                    ? 'text-action-danger bg-action-danger-50'
                    : 'text-status-warning bg-status-warning/10'
                "
              >
                剩餘 {{ product.productsStock }} 件
              </span>
            </div>
          </div>
        </div>
        <!-- #endregion -->
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!--#region 評分分布長條圖 -->
        <div
          class="bg-page-bg rounded-card border border-border-soft overflow-hidden flex flex-col"
        >
          <div
            class="px-5 py-4 border-b border-border-soft bg-surface-muted flex items-center gap-2"
          >
            <i class="pi pi-star text-brand-500 text-sm" />
            <span class="text-sm font-bold text-ink-900">評分分布</span>
          </div>
          <div class="p-4 flex-1 flex items-center justify-center h-64">
            <Chart
              v-if="dashboardData.rateDistribution?.length > 0"
              type="bar"
              :data="rateChartData"
              :options="rateChartOptions"
              class="h-full w-full"
            />
            <div v-else class="text-ink-500 text-sm">暫無評分數據</div>
          </div>
        </div>
        <!-- #endregion -->
      </div>
    </template>
    <!-- #endregion -->
  </div>
  <!-- #endregion -->
</template>
