<script setup>
import { getDashboard } from '@/api/dashBoardService';

/*
   變數名稱代表意義
   isFiltering : 是否為第一次加載
   weekSalesChartData : 近七天銷售額圖表數據
   weekSalesChartOptions : 近七天銷售額圖表選項
   topSellingChartData : 銷量前五的商品圖表數據
   topSellingChartOptions :銷量前五的商品圖表選項
   rateChartData : 評分分布圖表數據
   rateChartOptions : 評分分布圖表選項
*/
const isFiltering = ref(false);
const weekSalesChartData = ref();
const weekSalesChartOptions = ref();
const topSellingChartData = ref();
const topSellingChartOptions = ref();
const rateChartData = ref();
const rateChartOptions = ref();

/*
   數據表的所有資料
*/
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
  weekSalesChartOptions.value = setChartOptions();
  const { textColorSecondary, surfaceBorder } = getThemeColors();
  topSellingChartOptions.value = setBarChartOptions(textColorSecondary, surfaceBorder, '件');
  rateChartOptions.value = setBarChartOptions(textColorSecondary, surfaceBorder, '個');
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
      if (!isFirstload) showToastSuccess('成功 !');
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
   更新近七天銷售額圖表數據內容
*/
const updateWeekSalesChartData = () => {
  const documentStyle = getComputedStyle(document.documentElement);

  const labels = dashboardData.value.weekSales.map((item) => formatDateString(item.orderDate));
  const revenues = dashboardData.value.weekSales.map((item) => item.dailyRevenue);

  return {
    labels: labels,
    datasets: [
      {
        label: '當日營收',
        data: revenues,
        fill: true, // 改為 true 並配合色調會更好看
        borderColor: documentStyle.getPropertyValue('--p-orange-500') || '#f97316',
        backgroundColor: 'rgba(249, 115, 22, 0.1)', // 加上淡淡的背景色
        tension: 0.4,
      },
    ],
  };
};

/*
   編輯近七天銷售額圖表的設定
*/
const setChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  const textColor = documentStyle.getPropertyValue('--p-text-color');
  const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
  const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');

  return {
    // 是否維持原本的長寬比
    maintainAspectRatio: false,
    // 長寬比 ( 寬度 / 高度 )
    aspectRatio: 0.8,
    plugins: {
      legend: {
        display: false, // 隱藏圖例
      },
      // 當滑鼠移到圖表上時，跳出來的小黑框
      tooltip: {
        mode: 'index',
        intersect: false,
      },
    },
    scales: {
      x: {
        ticks: {
          color: textColorSecondary,
          font: {
            size: 15, // X 軸日期字體大小
          },
        },
        grid: { color: surfaceBorder, display: false }, // 隱藏 X 軸網格較清爽
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: textColorSecondary,
          font: {
            size: 15, // Y 軸金額字體大小
          },
        },
        grid: { color: surfaceBorder },
      },
    },
  };
};

// 【新改動】獲取全局 PrimeVue 主題顏色的私有小工具
const getThemeColors = () => {
  const documentStyle = getComputedStyle(document.documentElement);
  return {
    textColor: documentStyle.getPropertyValue('--p-text-color'),
    textColorSecondary: documentStyle.getPropertyValue('--p-text-muted-color'),
    surfaceBorder: documentStyle.getPropertyValue('--p-content-border-color'),
  };
};

/*
   銷量最好前五商品的長條圖數據
*/
const updateTopSellingChartData = () => {
  const products = dashboardData.value.topSellingProducts || [];
  return {
    labels: products.map((item) => item.productsName), // X 軸：商品名稱
    datasets: [
      {
        label: '累積銷量',
        data: products.map((item) => item.totalSales), // Y 軸：銷量件數
        backgroundColor: [
          'rgba(249, 115, 22, 0.2)', // 橘
          'rgba(6, 182, 212, 0.2)', // 藍綠
          'rgba(139, 92, 246, 0.2)', // 紫
          'rgba(234, 179, 8, 0.2)', // 黃
          'rgba(107, 114, 128, 0.2)', // 灰
        ],
        borderColor: [
          'rgb(249, 115, 22)',
          'rgb(6, 182, 212)',
          'rgb(139, 92, 246)',
          'rgb(234, 179, 8)',
          'rgb(107, 114, 128)',
        ],
        borderWidth: 1,
        barThickness: 32, // 調整柱子粗細
      },
    ],
  };
};

/*
   評分分布的長條圖數據
*/
const updateRateChartData = () => {
  const rates = dashboardData.value.rateDistribution || [];
  return {
    labels: rates.map((item) => item.rateDistribution), // X 軸：評分名稱 (如 5顆星)
    datasets: [
      {
        label: '評分數量',
        data: rates.map((item) => item.rateCount), // Y 軸：數量
        backgroundColor: [
          'rgba(234, 179, 8, 0.2)', // 第一名預設給亮眼的黃色
          'rgba(6, 182, 212, 0.2)',
          'rgba(139, 92, 246, 0.2)',
          'rgba(249, 115, 22, 0.2)',
          'rgba(107, 114, 128, 0.2)',
        ],
        borderColor: [
          'rgb(234, 179, 8)',
          'rgb(6, 182, 212)',
          'rgb(139, 92, 246)',
          'rgb(249, 115, 22)',
          'rgb(107, 114, 128)',
        ],
        borderWidth: 1,
        barThickness: 32,
      },
    ],
  };
};

/*
    統一個共用的長條圖 Option 生成器，包含字體控制
*/
const setBarChartOptions = (textColorSecondary, surfaceBorder, unit = '') => {
  return {
    maintainAspectRatio: false,
    aspectRatio: 0.8,
    plugins: {
      legend: {
        display: false, // 長條圖通常不需要顯示上方 dataset 標籤
      },
      tooltip: {
        titleFont: { size: 14, weight: 'bold' },
        bodyFont: { size: 13 },
        callbacks: {
          label: function (context) {
            return ` ${context.dataset.label}: ${context.raw} ${unit}`; // 滑鼠移上去時顯示單位 (件/個)
          },
        },
      },
    },
    scales: {
      x: {
        ticks: {
          color: textColorSecondary,
          font: { size: 11 }, // X軸商品名通常較長，字體設 11px 防擠壓
          maxRotation: 15, // 稍微傾斜防止長文字重疊
          minRotation: 0,
        },
        grid: { display: false }, // 隱藏直網格線更清爽
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: textColorSecondary,
          font: { size: 12 },
          precision: 0, // 強制只顯示整數
        },
        grid: { color: surfaceBorder },
      },
    },
  };
};
</script>

<template>
  <div class="flex flex-col w-full p-6 bg-gray-50/50 min-h-screen">
    <!--#region  標題列-->
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
    <!-- #endregion -->
    <!--#region  加載骨架 -->
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
    <!-- #endregion -->
    <!--#region  所有數據 -->
    <template v-else>
      <!--#region  銷售額區 -->
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
      <!-- #endregion -->
      <!--#region  近七天銷售額圖表 -->
      <div class="bg-white p-6 rounded-xl border border-gray-100 shadow-sm mb-6">
        <div class="flex items-center justify-between mb-4">
          <span class="text-base font-bold text-gray-800">近七天個別銷售額</span>
          <span class="text-xs text-gray-400">最近一週的每日營收波動</span>
        </div>

        <div class="h-100 w-full">
          <Chart
            v-if="dashboardData.weekSales?.length > 0"
            type="line"
            :data="weekSalesChartData"
            :options="weekSalesChartOptions"
            class="h-full w-full"
          />
          <div v-else class="h-full flex flex-col items-center justify-center text-gray-400">
            <i class="pi pi-calendar-times text-4xl mb-2" />
            <span>暫無近七日營收數據</span>
          </div>
        </div>
      </div>
      <!-- #endregion -->

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!--#region  銷量區 -->
        <div
          class="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden flex flex-col"
        >
          <div class="p-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
            <span class="text-base font-bold text-gray-800 flex items-center gap-2">
              <i class="pi pi-crown text-amber-500" /> 銷量最好前五商品
            </span>
          </div>
          <div class="divide-y divide-gray-100 flex-1">
            <div class="h-100 w-full flex-1 flex items-center justify-center">
              <Chart
                v-if="dashboardData.topSellingProducts?.length > 0"
                type="bar"
                :data="topSellingChartData"
                :options="topSellingChartOptions"
                class="h-full w-full"
              />
              <div v-else class="text-gray-400 text-sm py-12">暫無銷售數據</div>
            </div>
          </div>
        </div>
        <!-- #endregion -->
        <!--#region  庫存區 -->
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
        <!-- #endregion -->
      </div>
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!--#region  評分區 -->
        <div
          class="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden flex flex-col"
        >
          <div class="p-5 border-b border-gray-100 flex items-center justify-between bg-gray-50/50">
            <span class="text-base font-bold text-gray-800 flex items-center gap-2">
              <i class="pi pi-star text-yellow-500" /> 評分分布
            </span>
          </div>
          <div class="divide-y divide-gray-100 flex-1">
            <div class="h-100 w-full flex-1 flex items-center justify-center">
              <Chart
                v-if="dashboardData.rateDistribution?.length > 0"
                type="bar"
                :data="rateChartData"
                :options="rateChartOptions"
                class="h-full w-full"
              />
              <div v-else class="text-gray-400 text-sm py-12">暫無評分</div>
            </div>
          </div>
        </div>
        <!-- #endregion -->
      </div>
    </template>
    <!-- #endregion -->
  </div>
</template>
