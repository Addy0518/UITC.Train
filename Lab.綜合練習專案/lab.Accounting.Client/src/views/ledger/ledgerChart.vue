<script setup>
import * as echarts from 'echarts';
import { getAllLedger } from '@/api/ledgerService';

/*
  初始化時把資料帶入圖表 , 點選各類別又會再跳到細項圖表
*/
/*
  變數名稱代表意義
  categoryPrice : 類別價格
  itemPrice : 項目價格
*/
const chartData = ref([]);
const chartData2 = ref([]);
const categoryChartRef = ref(null);
const itemChartRef = ref(null);
onMounted(async () => {
  /*
    變數名稱代表意義
    categoryChart : 類別圖表
    itemChart : 各類別細項圖表
    categoryName : 類別名稱
    categoryCost : 類別花費
    chartdata : 類別圖表資料
  */
  const categoryChart = echarts.init(categoryChartRef.value);

  const itemChart = echarts.init(itemChartRef.value);

  const res = await getAllLedger();
  const { data } = res;

  /*
    reduce 把所有同類別的值加總
    acc 代表例如 1+1=2, 2+3=5 , 5+5=10 , 這個值就是 acc
    curr 是下一個數字 ( 例如現在是 1 下一個是 2 , 這個 2 就是 curr )

    acc[curr.categoryName] += curr.itemCost
    代表每一次加總都會加上相同類別的項目 ( curr.categoryName ) , 一直加直到把項目加完

    最後再把他們的 key 跟 value 取出來 , 就是類別名稱跟花費
  */
  const aggregatedData = data.returnData.reduce((acc, curr) => {
    if (!acc[curr.categoryName]) {
      acc[curr.categoryName] = 0;
    }
    acc[curr.categoryName] += curr.itemCost;

    return acc;
  }, {});

  chartData.value = Object.entries(aggregatedData).map(([name, value]) => ({
    name,
    value,
  }));

  let option = {
    title: {
      text: '各類別金額統計',
      left: 'center',
      top: 'center',
    },
    series: [
      {
        type: 'pie',
        data: chartData.value,
        radius: ['40%', '70%'],
      },
    ],
  };

  categoryChart.setOption(option);
  categoryChart.resize({
    width: 800,
    height: 500,
  });

  /*
    點擊類別圖表到細項表
  */
  categoryChart.on('click', (params) => {
    /*
      變數名稱代表意義
      filteCategory : 點選到的類別的細項
      itemCost : 各類別細項圖表
      itemName : 類別名稱
      chartdata2 : 細項圖表資料
    */
    const filteCategory = data.returnData.filter((item) => item.categoryName === params.name);

    chartData2.value = filteCategory.map((item) => ({
      name: item.itemName,
      value: item.itemCost,
    }));

    let option2 = {
      title: {
        text: `${params.name}各項目金額統計`,
        left: 'center',
        top: 'center',
      },

      series: [
        {
          type: 'pie',
          data: chartData2.value,
          radius: ['40%', '70%'],
        },
      ],
    };

    itemChart.setOption(option2);
    itemChart.resize({
      width: 800,
      height: 500,
    });
  });
});
</script>

<template>
  <div class="w-screen p-5">
    <!-- 使用 flex 並設為項目置頂對齊 -->
    <div class="flex items-start gap-10">
      <!-- 左側第一組：類別金額統計 -->
      <div class="w-1/2 border-r border-gray-100">
        <h3 class="text-lg font-bold mb-4 text-center">類別統計</h3>
        <div class="flex flex-col items-center">
          <!-- 圖表 -->
          <div ref="categoryChartRef" style="width: 100%; height: 350px"></div>
          <!-- 數據清單 -->
          <div class="mt-40 w-100 bg-gray-50 p-5 rounded-lg shadow-sm">
            <div class="flex justify-between mb-2 border-b border-gray-200 pb-1">
              <span class="text-gray-600"> 類別</span>
              <span class="text-gray-600"> 價格</span>
            </div>
            <div
              v-for="item in chartData"
              :key="item.name"
              class="flex justify-between mb-2 border-b border-gray-200 pb-1"
            >
              <span class="text-gray-600"> {{ item.name }}</span>
              <span class="font-bold text-blue-600">${{ item.value }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 右側第二組：細項金額統計 -->
      <div class="w-1/2">
        <h3 class="text-lg font-bold mb-4 text-center">細項統計</h3>
        <div class="flex flex-col items-center">
          <!-- 圖表 -->
          <div ref="itemChartRef" style="width: 100%; height: 350px"></div>
          <!-- 數據清單 -->
          <div class="mt-40 w-100 bg-gray-50 p-5 rounded-lg shadow-sm">
            <div class="flex justify-between mb-2 border-b border-gray-200 pb-1">
              <span class="text-gray-600"> 項目</span>
              <span class="text-gray-600"> 價格</span>
            </div>
            <div
              v-for="item in chartData2"
              :key="item.name"
              class="flex justify-between mb-2 border-b border-gray-200 pb-1"
            >
              <span class="text-gray-600">{{ item.name }}</span>
              <span class="font-bold text-blue-600">${{ item.value }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
