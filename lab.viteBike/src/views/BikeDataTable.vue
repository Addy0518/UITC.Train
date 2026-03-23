<script setup>
import { computed, onMounted, ref, watch } from 'vue';

// 拿到的原始資料
const bikeData = ref([]);
// 當前頁數
const currpage = ref(1);
// 每頁幾筆
const perpage = ref(20);

// 判斷排序往上還是往下
const sortType = ref('desc');

// 切換排序
const toggleSort = () => {
  sortType.value === 'asc' ? (sortType.value = 'desc') : (sortType.value = 'asc');
};

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
  console.log('總項目數', bikeData.value.length);
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

// 找出總頁數 , 判斷總長度好做限制
// 注意是依照搜尋完的數量去算 (searchData) , 而不是原始資料 (bikeData) , 不然頁碼不會跟著搜尋結果變
const totalPage = computed(() => {
  const count = searchData.value.length;
  return count > 0 ? Math.ceil(count / perpage.value) : 1;
});

// 經過每頁 10 筆篩選過的資料
const filterData = computed(() => {
  if (!searchData.value) return [];

  // 因為要依照這個欄位去排序 , 所以要先解構本來的資料才拿的到欄位
  const items = [...searchData.value];
  items.sort((a, b) => {
    // 要轉數字不然本來是 json
    const valA = Number(a.available_rent_bikes);
    const valB = Number(b.available_rent_bikes);
    if (sortType.value === 'asc') {
      return valA - valB;
    } else if (sortType.value === 'desc') {
      return valB - valA;
    }
  });

  // 設定開始和結束的範圍
  const start = (currpage.value - 1) * perpage.value;
  const end = start + perpage.value;

  // 用 js 的 slice 切段
  return items.slice(start, end);
});


const pageNumbers = computed(() => {
  // 找出小於現在頁數的5頁跟大於的四頁 ( 總共10 );
  let start = currpage.value - 5;
  let end = currpage.value + 4;

  // 先看有沒有搜尋 , 有搜尋結果到小於 10 之後 , 就依照剛剛設定的搜尋結果分頁
  if (totalPage.value <= 10) {
    start = 1;
    end = totalPage.value;
  } else {
    // 小於 1 就不置中現在頁數 , 維持顯示 1 - 10 頁
    if (start < 1) {
      start = 1;
      end = 10;
    }
    // 跟小於一樣概念 , 直接從最後減九就可以了
    if (end > totalPage.value) {
      end = totalPage.value;
      start = totalPage.value - 9;
    }
  }

  console.log(totalPage.value);
  // 最重要得 , 用迴圈拿到動態頁碼 , 待會 html 根據這個迴圈渲染
  const pages = [];
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
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
            <th
              class="px-4 py-3 font-semibold border-b text-center"
              style="cursor: pointer"
              @click="toggleSort"
            >
              <button>{{ sortType == 'desc' ? '▼' : '▲' }}</button>
              可租借的腳踏車數量
            </th>
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
    <div class="flex gap-2 mt-4 items-center justify-center">
      <!-- disabled :　當第一頁的時候就點不了 -->
      <button
        @click="currpage--"
        :disabled="currpage === 1"
        class="px-3 py-1 border rounded disabled:opacity-50"
      >
        上一頁
      </button>

      <div>
        <!-- 根據剛剛的動態頁數迴圈列出來 , 用動態切換 class 讓點到的頁碼變色 -->
        <button
          v-for="page in pageNumbers"
          :key="page"
          @click="currpage = page"
          class="px-3 py-1 border rounded disabled:opacity-50"
          :class="{ 'bg-blue-500 text-white': page === currpage }"
        >
          {{ page }}
        </button>
      </div>

      <button
        @click="currpage++"
        class="px-3 py-1 border rounded disabled:opacity-50"
        :disabled="currpage === totalPage"
      >
        下一頁
      </button>
    </div>
  </div>
</template>

<style scoped>
.numbercolor {
  color: red;
}
</style>
