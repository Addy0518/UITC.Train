<script setup>
import * as echarts from 'echarts';
import { ref, onMounted, compile, computed, watch } from 'vue';
import { getAllLedger } from '@/api/account-api';
const categoryChartRef = ref(null);
const itemChartRef = ref(null);
/*
  初始化時把資料帶入圖表 , 點選各類別又會再跳到細項圖表
*/
onMounted(async () => {
  /*
  參數名稱代表意義
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

  const chartData = Object.entries(aggregatedData).map(([name, value]) => ({
    name,
    value,
  }));
  console.log('aggregatedData', aggregatedData);

  let option = {
    title: {
      text: '各類別金額統計',
      left: 'center',
      top: 'center',
    },
    series: [
      {
        type: 'pie',
        data: chartData,
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
     參數名稱代表意義
     filteCategory : 點選到的類別的細項
     itemCost : 各類別細項圖表
     itemName : 類別名稱
     chartdata2 : 細項圖表資料
    */

    const filteCategory = data.returnData.filter((item) => item.categoryName === params.name);

    const chartData2 = filteCategory.map((item) => ({
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
          data: chartData2,
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
  <div class="w-full">
    <div class="container text-xl mt-10 flex">
      <div ref="categoryChartRef"></div>
      <div ref="itemChartRef"></div>
    </div>
  </div>
</template>
