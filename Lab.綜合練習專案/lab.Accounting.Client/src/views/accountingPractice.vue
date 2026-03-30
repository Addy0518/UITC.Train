<script setup>
import { ref, onMounted, compile, computed, watch } from 'vue';

// 總花費
const spend = computed(() => {
  let total = 0;
  for (let i = 0; i < products.value.length; i++) {
    if (!products.value[i].isDelete) {
      total += products.value[i].itemCost || 0;
    }
  }
  return total;
});
// 刪除 (判斷 isDelete 欄位決定是否真的刪除)
const deleteChange = async (id) => {
  if (!id) return;
  const res = await fetch(`https://localhost:7124/api/Ledger/DeleteLedger/${id}`, {
    method: 'Delete',
  });

  if (res.ok) {
    await ItemData();
  } else {
    alert('刪除失敗!');
  }
};
// 復原軟刪除狀態 (用更新 api)
const reserve = async (item) => {
  if (!item) return;
  const res = await fetch(`https://localhost:7124/api/Ledger/UpdateLedger`, {
    method: 'Put',
    headers: {
      'Content-Type': 'application/json', // 告訴後端這是 JSON
    },
    body: JSON.stringify({
      categoryname: item.categoryName || '',
      itemId: item.itemId,
      itemName: item.itemName,
      itemCost: item.itemCost,
      ItemIllustrate: item.ItemIllustrate || '',
      categoryId: item.categoryId ? String(item.categoryId) : null,
      isDelete: false,
      itemUpdateDate: new Date().toLocaleDateString('en-CA'),
    }),
  });

  if (res.ok) {
    await ItemData();
  } else {
    alert('復原失敗!');
    const errorData = await res.json();
    console.log('400 錯誤詳情:', errorData);
  }
};

//所設立的類別
const category = ref([]);

// 監聽 datepicker 選擇的日期並呼叫 api
const date = ref();
const selectedValue = ref(null);

// 一次監聽兩個 日期跟類別 , 一起塞進 url
watch([date, selectedValue], ([newDate, newVal]) => {
  console.log(newVal);
  const ids = newVal ? Object.keys(newVal).map(Number) : null;
  ItemData(newDate ?? null, ids);
});

// 所有項目
const products = ref([]);
// 初始時抓取所有資料
const ItemData = async (selectdate = null, cateId = null) => {
  try {
    let url = `https://localhost:7124/api/Ledger/GetAllLedger`;

    if (cateId && cateId.length > 0) {
      url += `?` + cateId.map((id) => `categoryId=${id}`).join(`&`);
    }
    if (selectdate) {
      // 如果用 toString 的話怕格式會不一樣 , 而用 ISO 再把 t 後面的時間去掉也不行 , 因為時區傳患的關係 , 所以用 英文格式 en-CA 轉成 1990-01-01 的格式

      const datestring = selectdate.toLocaleDateString('en-CA');
      url += (url.includes('?') ? '&' : '?') + `date=${datestring}`;
    }
    const res = await fetch(url);
    const data = await res.json();

    if (data.codeStatus === 2000) {
      products.value = data.returnData;
      // 這裡用 Set 來去除重複的類別名稱 , 因為 Set 集合內沒有索引 , 所以它會自動選重複的
      // 但是 Set 沒有索引所以她 key 會從0開始 , 而不是照著 categoryid , 所以先不用
      // const categorydata=[...new Set(data.returnData.map(item=>item.categoryName))]

      // 這個則是指對純數值有效 , 也沒法用
      // category.value=data.returnData.filter((item,index)=>data.returnData.indexOf(item)===index).map(item=>({key:String(item.categoryId),label:item.categoryName}))

      //最後選擇 set + filter的方法 , 保留索引並去重
      if (!selectdate && !cateId) {
        const seen = new Set();
        category.value = data.returnData
          .filter((item) => {
            if (seen.has(item.categoryName)) return false;
            seen.add(item.categoryName);
            return true;
          })
          .map((item) => ({
            key: String(item.categoryId),
            label: item.categoryName,
          }));
      }
    } else {
      console.error('API 回傳錯誤:', data.message);
    }
  } catch (error) {
    console.error('連線失敗:', error);
  }
};

onMounted(() => {
  ItemData();
});
</script>

<template>
  <!-- 主區域 -->
  <div class="w-full mx-auto max-w-screen-2xl">
    <div class="container mx-auto text-xl mt-10 mb-auto">
      <!-- 金額 -->
      <div class="text-center text-5xl font-bold">
        <p class="pb-10">總消費</p>
        <p>$ : {{ spend }}</p>
      </div>
      <div class="flex justify-center items-center gap-10 mt-10">
        <!-- 選擇顯示項目 -->
        <div class="card">
          <TreeSelect
            v-model="selectedValue"
            :options="category"
            selectionMode="multiple"
            display="chip"
            placeholder="選擇類別"
            class="md:w-80"
          />
        </div>
        <div>
          <DatePicker
            v-model="date"
            placeholder="選擇日期"
            dateFormat="yy-mm-dd"
            class="md:w-80 h-11.5"
            :placeholder="font - size"
          />
        </div>
        <div class="w-10">
          <RouterLink :to="{ name: 'add-ledger' }">
            <img src="/src/img/add.png" alt=""
          /></RouterLink>
        </div>
      </div>
      <!-- 顯示所有帳目 -->
      <div style="margin-top: 50px">
        <div class="card max-w-6xl mx-auto px-10">
          <DataTable :value="products" scrollable scrollHeight="430px" size="large">
            <Column field="itemId" header="編號"></Column>
            <Column field="itemName" header="項目名稱"></Column>
            <Column field="categoryName" header="類別"></Column>
            <Column field="itemCost" header="花費"
              ><template #body="slotProps">
                {{ slotProps.data.itemCost ? slotProps.data.itemCost : 0 }}
              </template></Column
            >
            <Column
              field="
itemCreateDate
"
              header="建立日期"
              ><template #body="slotProps">
                {{
                  slotProps.data.itemCreateDate ? slotProps.data.itemCreateDate.split('T')[0] : ''
                }}
              </template></Column
            >
            <Column header="狀態"
              ><template #body="slotProps">
                <span :class="slotProps.data.isDelete ? 'text-red-500' : 'text-green-500'">
                  {{ slotProps.data.isDelete ? '已刪除' : '正常' }}
                </span>
              </template></Column
            >
            <Column
              ><template #body="slotProps">
                <div class="flex justify-start gap-3 ml-10">
                  <RouterLink :to="{ name: 'edit-ledger', params: { id: slotProps.data.itemId } }">
                    <button class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold">
                      編輯
                    </button>
                  </RouterLink>
                  <button
                    @click="reserve(slotProps.data)"
                    class="text-white p-3 rounded-2xl cursor-pointer bg-amber-500 font-bold"
                    v-if="slotProps.data.isDelete"
                  >
                    復原
                  </button>
                  <button
                    @click="deleteChange(slotProps.data.itemId)"
                    class="text-white p-3 rounded-2xl cursor-pointer bg-red-500 font-bold"
                  >
                    {{ slotProps.data.isDelete ? '確定刪除' : '刪除' }}
                  </button>
                </div>
              </template></Column
            >
          </DataTable>
        </div>
      </div>
    </div>
  </div>
</template>
