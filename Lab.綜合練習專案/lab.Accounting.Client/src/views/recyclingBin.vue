<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import Swal from 'sweetalert2';
import {
  deleteAllSoftDeleteLedger,
  updateLedger,
  getAllLedger,
  deleteLedger,
} from '@/api/account-api';
// 所有項目
const products = ref([]);
// 是否有資料顯示
let isItem = ref(true);
//所設立的類別
const category = ref([]);
// 監聽 datepicker 選擇的日期並呼叫 api
const date = ref();
const selectedValue = ref(null);

// 刪除 (判斷 isDelete 欄位決定是否真的刪除)
const deleteChange = async (id) => {
  try {
    if (!id) return;

    const res = await deleteLedger(id);

    if (res.data.codeStatus === 2000) {
      await ItemData();
      if (products.value.length === 0) {
        isItem.value = false;
      }
    }
  } catch (error) {
    console.error('帳本刪除錯誤 ', error.response);
  }
};

// 復原軟刪除狀態 (用更新 api)
const reserve = async (item) => {
  try {
    console.log('item', item);
    if (!item) return;
    const updateData = {
      categoryname: item.categoryName || '',
      itemId: item.itemId,
      itemName: item.itemName,
      itemCost: item.itemCost,
      ItemIllustrate: item.ItemIllustrate || '',
      categoryId: item.categoryId ? String(item.categoryId) : null,
      isDelete: false,
      itemUpdateDate: new Date().toLocaleDateString('en-CA'),
    };

    const res = await updateLedger(updateData);
    const { data } = res;
    if (data.codeStatus === 2000) {
      await ItemData();
      if (products.value.length === 0) {
        isItem.value = false;
      }
    }
  } catch (error) {
    console.error('帳本復原錯誤 ', error.response);
  }
};

// 初始時抓取所有資料
const ItemData = async (selectdate = null, cateId = null, isDelete = true) => {
  try {
    let querystring = '';
    if (cateId && cateId.length > 0) {
      querystring += `?` + cateId.map((id) => `categoryId=${id}`).join(`&`);
    }
    if (selectdate) {
      // 如果用 toString 的話怕格式會不一樣 , 而用 ISO 再把 t 後面的時間去掉也不行 , 因為時區傳患的關係 , 所以用 英文格式 en-CA 轉成 1990-01-01 的格式

      const datestring = selectdate.toLocaleDateString('en-CA');
      querystring += (querystring.includes('?') ? '&' : '?') + `date=${datestring}`;
    }

    if (isDelete) {
      querystring += (querystring.includes('?') ? '&' : '?') + `isDelete=${isDelete}`;
    }

    const res = await getAllLedger(querystring);

    const { data } = res;
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
    }
  } catch (error) {
    console.error('搜尋資料錯誤 ', error.response);
  }
};
// 刪除 (刪除所有軟刪除狀態項目)
const deleteAll = async () => {
  try {
    const res = await deleteAllSoftDeleteLedger();
    const { data } = res;
    if (data.codeStatus === 2000) {
      await ItemData();
    }
  } catch (error) {
    console.error('帳本刪除錯誤 ', error.response);
  }
};

// 一次監聽兩個 日期跟類別 , 一起塞進 url
watch([date, selectedValue], ([newDate, newVal]) => {
  const ids = newVal ? Object.keys(newVal).map(Number) : null;
  ItemData(newDate ?? null, ids);
});

onMounted(async () => {
  await ItemData();

  if (products.value.length === 0) {
    isItem.value = false;
  }
});
</script>

<template>
  <!-- 主區域 -->

  <div v-if="isItem" class="w-full mx-auto max-w-screen-2xl">
    <div class="container mx-auto text-xl mt-10 mb-auto">
      <div class="justify-items-end pe-50 pt-10">
        <div class="justify-items-end">
          <Button class="bg-red-500" @click="deleteAll">刪除所有已刪除狀態項目</Button>
        </div>
      </div>

      <!-- 金額 -->

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

            <Column
              ><template #body="slotProps">
                <div class="flex justify-start gap-3 ml-10">
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
                    確定刪除
                  </button>
                </div>
              </template></Column
            >
          </DataTable>
        </div>
      </div>
    </div>
  </div>

  <div
    v-else
    class="w-full mx-auto max-w-screen-2xl bg-[url(../img/查無資料.png)] bg-no-repeat bg-size-[auto_350px] bg-center"
  ></div>
</template>
