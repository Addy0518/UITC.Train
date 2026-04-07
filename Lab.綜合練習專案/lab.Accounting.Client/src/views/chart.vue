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

  const categoryName = [...new Set(data.returnData.map((item) => item.categoryName))];

  const categoryCost = [...new Set(data.returnData.map((item) => item.itemCost))];

  const chartdata = [
    {
      name: categoryName,
      type: 'bar',
      data: categoryCost,
    },
  ];

  let option = {
    title: {
      text: '類別總金額統計',
      textStyle: {
        fontSize: 36,
        color: '#333',
      },
    },

    xAxis: {
      data: categoryName,
      axisLabel: {
        textStyle: {
          fontSize: 25,
        },
      },
    },

    yAxis: {
      type: 'value',
      axisLabel: {
        textStyle: {
          fontSize: 20,
        },
      },
    },
    series: chartdata,
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
    const filteCategory = [
      ...new Set(data.returnData.filter((item) => item.categoryName === params.name)),
    ];

    const itemCost = [...new Set(filteCategory.map((x) => x.itemCost))];

    const itemName = [...new Set(filteCategory.map((x) => x.itemName))];

    const chartdata2 = [
      {
        name: itemName,
        type: 'bar',
        data: itemCost,
      },
    ];

    let option2 = {
      title: {
        text: `${params.name}各項目金額統計`,
        textStyle: {
          fontSize: 36,
          color: '#333',
        },
      },

      xAxis: {
        data: itemName,
        axisLabel: {
          textStyle: {
            fontSize: 25,
          },
        },
      },

      yAxis: {
        type: 'value',
        axisLabel: {
          textStyle: {
            fontSize: 20,
          },
        },
      },
      series: chartdata2,
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
    <div class="container mx-auto text-xl mt-10 flex">
      <div ref="categoryChartRef"></div>
      <div ref="itemChartRef"></div>
    </div>
  </div>
</template>
