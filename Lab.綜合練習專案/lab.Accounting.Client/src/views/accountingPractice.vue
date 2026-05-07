<script setup>
import { ref, onMounted, computed, watch, inject } from 'vue';
import { getAllLedger, deleteLedger } from '@/api/account-api';
import { useRoute, useRouter } from 'vue-router';
/*
   變數名稱代表意義
   ledger : 所有帳本項目
   category : 帳本類別
   date : datePicker 選擇的日期
   selectedValue : treeSelect 選擇的類別
   router : 改變路由
*/
const ledger = ref([]);
const category = ref([]);
const date = ref();
const selectedValue = ref(null);
const router = useRouter();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(() => {
  ItemData();
});

/*
   總花費變動偵測
*/
const spend = computed(() => {
  let total = 0;
  for (let i = 0; i < ledger.value.length; i++) {
    if (!ledger.value[i].isDelete) {
      total += ledger.value[i].itemCost || 0;
    }
  }
  return total;
});

/*
   一次監聽兩個 日期跟類別 , 一起塞進 url
*/
watch([date, selectedValue], ([newDate, newVal]) => {
  const ids = newVal ? Object.keys(newVal).map(Number) : null;
  ItemData(newDate ?? null, ids);
});

/*
   偵測所有帳本資料並篩選出軟刪除狀態的帳本
*/
const visible = computed(() => {
  return ledger.value.filter((m) => !m.isDelete);
});

/*
   呼叫刪除帳本 API
*/
const deleteChange = async (id) => {
  try {
    showLoading();
    if (!id) return;

    const res = await deleteLedger(id);

    if (res.data.codeStatus === 2000) {
      showToastSuccess('成功丟到回收桶!');
      await ItemData();
    }
  } catch (error) {
    console.error('帳本刪除錯誤 ', error.response);
  } finally {
    hideLoading();
  }
};

/*
   呼叫查看全部帳本 API
*/
const ItemData = async (selectdate = null, cateId = null) => {
  try {
    showLoading();
    let querystring = '';
    if (cateId && cateId.length > 0) {
      querystring += `?` + cateId.map((id) => `categoryId=${id}`).join(`&`);
    }
    if (selectdate) {
      /*
         如果用 toString 的話怕格式會不一樣 , 而用 ISO 再把 t 後面的時間去掉也不行 , 因為時區傳患的關係 , 所以用 英文格式 en-CA 轉成 1990-01-01 的格式
      */
      const datestring = selectdate.toLocaleDateString('en-CA');
      querystring += (querystring.includes('?') ? '&' : '?') + `date=${datestring}`;
    }

    const res = await getAllLedger(querystring);

    const { data } = res;
    if (data.codeStatus === 2000) {
      ledger.value = data.returnData;
      /*
         這裡用 Set 來去除重複的類別名稱 , 因為 Set 集合內沒有索引 , 所以它會自動選重複的
         但是 Set 沒有索引所以她 key 會從0開始 , 而不是照著 categoryid , 所以先不用
         const categorydata=[...new Set(data.returnData.map(item=>item.categoryName))]
      */
      /*
         這個則是指對純數值有效 , 也沒法用
         category.value=data.returnData.filter((item,index)=>data.returnData.indexOf(item)===index).map(item=>({key:String(item.categoryId),label:item.categoryName}))
      */
      /*
         最後選擇 set + filter的方法 , 保留索引並去重
      */
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
    }
  } catch (error) {
    console.error('搜尋資料錯誤 ', error.response);
  } finally {
    hideLoading();
  }
};
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
            showClear
          />
        </div>
        <div>
          <DatePicker
            v-model="date"
            placeholder="選擇日期"
            dateFormat="yy-mm-dd"
            class="md:w-80 h-11.5"
            :placeholder="font - size"
            showClear
          />
        </div>
        <div>
          <button
            class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
            @click="router.push({ name: 'recycling-ledger' })"
          >
            前往回收桶
          </button>
        </div>
      </div>
      <!-- 顯示所有帳目 -->
      <div style="margin-top: 50px">
        <div class="card max-w-6xl mx-auto px-10">
          <DataTable :value="visible" scrollable scrollHeight="430px" size="large">
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

            <Column
              ><template #body="slotProps">
                <div class="flex justify-start gap-3 ml-10">
                  <RouterLink :to="{ name: 'edit-ledger', params: { id: slotProps.data.itemId } }">
                    <button class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold">
                      編輯
                    </button>
                  </RouterLink>

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
