<script setup>
import { computed, onMounted, ref, watch } from 'vue';

// 拿到的原始資料
const bikeData = ref([]);
// 當前頁數
const currpage = ref(1);
// 每頁幾筆
const perpage = ref(10);

// 搜尋
const search = ref('');
// 用 watch 監聽 search 有沒有變化 , 有變化就返回第一頁 , 不然搜尋完頁數還會停在本來的地方
watch(search, () => {
  currpage.value = 1;
});

// 在初始時就先載入資料
onMounted(async () => {
  const response = await fetch(
    'https://tcgbusfs.blob.core.windows.net/dotapp/youbike/v2/youbike_immediate.json',
  );

  bikeData.value = await response.json();
  console.log(bikeData.value);
});
// 根據地址搜尋(篩選)
const searchData = computed(() => {
  if (!search.value) {
    return bikeData.value;
  }

  return bikeData.value.filter((item) => {
    return item.ar.includes(search.value);
  });
});

// 經過每頁 10 筆篩選過的資料
const filterData = computed(() => {
  if (!searchData.value) return [];

  // 設定開始和結束的範圍
  const start = (currpage.value - 1) * perpage.value;
  const end = start + perpage.value;
  // 用 js 的 slice 切段
  return searchData.value.slice(start, end);
});
</script>

<template>
  <div class="p-6 bg-gray-50 min-h-screen">
    <div class="flex gap-4 mb-6 justify-end">
      <input
        type="text"
        placeholder="輸入站點地址"
        class="border border-gray-300 rounded-lg px-4 py-2 w-64 focus:ring-2 focus:ring-blue-500 outline-none"
        v-model="search"
      />
      <button class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition">
        查詢
      </button>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-blue-600 text-white">
            <th class="px-4 py-3 font-semibold border-b">站點編號</th>
            <th class="px-4 py-3 font-semibold border-b">站點名稱</th>
            <th class="px-4 py-3 font-semibold border-b">站點所在區域</th>
            <th class="px-4 py-3 font-semibold border-b text-center">站點地址</th>
            <th class="px-4 py-3 font-semibold border-b text-center">總車位數量</th>
            <th class="px-4 py-3 font-semibold border-b text-center">可租借的腳踏車數量</th>
            <th class="px-4 py-3 font-semibold border-b text-center">站點緯度</th>
            <th class="px-4 py-3 font-semibold border-b text-center">站點經度</th>
            <th class="px-4 py-3 font-semibold border-b text-center">可歸還的腳踏車數量</th>
          </tr>
        </thead>
        <tbody class="text-gray-700" v-for="bike in filterData" :key="bike.sno">
          <tr class="border-b hover:bg-blue-50 transition">
            <td class="px-4 py-4 text-black">{{ bike.sno }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.sna }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.sarea }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.ar }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.Quantity }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.available_rent_bikes }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.latitude }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.longitude }}</td>
            <td class="px-4 py-4 font-medium">{{ bike.available_return_bikes }}</td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="flex gap-2 mt-4 items-center">
      <!-- disabled :　當第一頁的時候就點不了 -->
      <button
        @click="currpage--"
        :disabled="currpage === 1"
        class="px-3 py-1 border rounded disabled:opacity-50"
      >
        上一頁
      </button>

      <span>第 {{ currpage }} 頁</span>

      <button @click="currpage++" class="px-3 py-1 border rounded disabled:opacity-50">
        下一頁
      </button>
    </div>
  </div>
</template>
