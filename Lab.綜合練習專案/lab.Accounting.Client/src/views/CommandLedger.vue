<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { getAllLedger, getLedger, createLedger, updateLedger } from '@/api/account-api';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   itemName : 項目名稱
   itemIllustrate : 項目說明
   itemCost : 項目花費
   itemDate　：　項目建立日期
   selectedCategory : 項目類別名稱
   category : 所有項目類別 ( 不重複 )
   visible : 控制彈窗
   isAdd : 路由判斷新增或刪除
*/
const route = useRoute();
const itemName = ref(null);
const itemIllustrate = ref(null);
const itemCost = ref(null);
const itemDate = ref(null);
const selectedCategory = ref(null);
const category = ref([]);
const visible = ref(true);
const isAdd = computed(() => route.name === 'add-ledger');

onMounted(() => {
  categoryData();
  if (route.params.id) {
    updateData(route.params.id);
  }
});

/*
  呼叫查看所有類別帳本 API
*/
const categoryData = async () => {
  try {
    const res = await getAllLedger();
    /*
       這裡用 Set 來去除重複的類別名稱
    */
    const categorydata = [...new Set(res.data.returnData.map((item) => item.categoryName))];

    category.value = categorydata.map((name, index) => ({
      key: index.toString(),
      name: name,
    }));
  } catch (error) {
    console.error('連線失敗:', error);
  }
};

/*
  呼叫查看單一帳本 API ( 讓編輯帳本時前端能看到原本資料 )
*/
const updateData = async (productId) => {
  try {
    if (!productId) return;
    const res = await getLedger(productId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      const item = data.returnData;

      itemName.value = item.itemName;
      itemCost.value = item.itemCost;
      itemIllustrate.value = item.itemIllustrate;

      selectedCategory.value = item.categoryName;
    }
    if (data.codeStatus === 4001) {
      alert(data.message);
    }
  } catch (error) {
    console.error('編輯錯誤 ', error.response);
  }
};

/*
  呼叫新增帳本 API
*/
const addoreditledger = async (id = null) => {
  try {
    /*
       我後端已經寫了轉換類別的方法了 , 這裡就丟類別名稱回去就好
       但是因為儲存類別有兩個方法 , 一個是選現有的就會是物件 , 一個用輸入的就會是變數 , 所以要看他的型別來判斷
    */
    const categoryname =
      typeof selectedCategory.value === 'object'
        ? selectedCategory.value?.name
        : selectedCategory.value;
    if (isAdd.value) {
      const createdata = {
        categoryname: categoryname,
        ItemName: itemName.value,
        ItemCreateDate: itemDate.value
          ? new Date(itemDate.value).toLocaleDateString('en-CA')
          : null,
        ItemCost: itemCost.value || 0,
        ItemIllustrate: itemIllustrate.value,
      };

      const res = await createLedger(createdata);
      const { data } = res;
      if (data.returnData > 0 && data.codeStatus === 2000) {
        visible.value = false;
      }
      if (data.codeStatus === 4001) {
        alert(data.message);
      }
    } else if (!isAdd.value) {
      const updatedata = {
        categoryname: categoryname || '',
        ItemId: parseInt(id),
        ItemName: itemName.value,
        ItemUpdateDate: new Date(itemDate.value).toLocaleDateString('en-CA'),
        ItemCost: itemCost.value,
        ItemIllustrate: itemIllustrate.value,
      };
      const res = await updateLedger(updatedata);
      const { data } = res;
      if (data.returnData > 0 && data.codeStatus === 2000) {
        visible.value = false;
      }
      if (data.codeStatus === 4001) {
        alert(data.message);
      }
    }
  } catch (err) {
    console.error('資料操作錯誤 ', error.response);
  }
};
</script>

<template>
  <Dialog
    v-model:visible="visible"
    header="帳本項目"
    :style="{ width: '40rem' }"
    @hide="$router.push({ name: 'accounting-practice' })"
  >
    <div class="container">
      <p class="text-center mb-10 text-3xl font-bold">{{ isAdd ? '新增' : '編輯' }}帳本項目</p>

      <!-- 項目欄位 -->
      <!-- 編輯的話就多顯示一個帳本編號 -->
      <div class="card grid grid-cols-1 gap-4 gap-y-10 max-h-[370px] overflow-y-auto">
        <div class="text-xl font-bold" v-if="isAdd === false">
          <p>編號 : {{ $route.params.id }}</p>
        </div>
        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-user"></i>
          </InputGroupAddon>
          <InputText v-model="itemName" placeholder="項目名稱" />
        </InputGroup>

        <InputGroup>
          <InputGroupAddon>$</InputGroupAddon>
          <InputNumber v-model="itemCost" placeholder="花費" />
          <InputGroupAddon>.00</InputGroupAddon>
        </InputGroup>
        <InputGroup v-if="isAdd">
          <InputGroupAddon>
            <i class="pi pi-calendar"></i>
          </InputGroupAddon>
          <DatePicker v-model="itemDate" placeholder="日期" dateFormat="yy-mm-dd" />
        </InputGroup>

        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-align-justify"></i>
          </InputGroupAddon>
          <Select
            v-model="selectedCategory"
            :options="category"
            editable
            optionLabel="name"
            placeholder="類別"
          />
        </InputGroup>

        <InputGroup>
          <InputGroupAddon><i class="pi pi-book"></i></InputGroupAddon>
          <Textarea v-model="itemIllustrate" placeholder="補充說明" class="w-full" />
        </InputGroup>
      </div>
      <!-- 按鈕區 -->
      <div class="justify-end flex mt-5">
        <button
          @click="addoreditledger($route.params.id)"
          class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
        >
          儲存
        </button>
      </div>
    </div>
  </Dialog>
</template>
