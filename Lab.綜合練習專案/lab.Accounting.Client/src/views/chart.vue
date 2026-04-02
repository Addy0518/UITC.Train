<script setup>
import * as echarts from 'echarts';
import { ref, onMounted, compile, computed, watch } from 'vue';
const chartRef = ref(null);




onMounted(() => {



  // 初始化圖表
  const myChart = echarts.init(chartRef.value);

  // 設定參數
  const option = {
    title: { text: 'ECharts 範例' },
    xAxis: { data: ['A', 'B', 'C'] },
    yAxis: {},
    series: [{ type: 'bar', data: [5, 20, 36] }],
  };

  myChart.setOption(option);
});
</script>

<template>
  <div ref="chartRef" style="width: 600px; height: 400px"></div>
</template>
