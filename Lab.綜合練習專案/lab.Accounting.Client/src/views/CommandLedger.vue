<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
// 路由參數
const route = useRoute();
// 使用者 id (暫定)
const userId = ref(1);
// 項目名稱
const itemName = ref(null);
// 項目說明
const itemIllustrate = ref(null);
// 項目花費
const itemCost = ref(null);
// 項目類別名稱
const selectedCategory = ref(null);
// 所有項目類別物件
const category = ref([]);
// 項目建立日期
const itemDate = ref(null);

// 初始時抓取所有資料 < 這裡只需要類別
const categoryData = async () => {
  try {
    let url = `https://localhost:7124/api/Ledger/GetAllLedger`;

    const res = await fetch(url);
    const data = await res.json();

    if (data.codeStatus === 2000) {
      // 這裡用 Set 來去除重複的類別名稱 , 因為 Set 集合內沒有索引 , 所以它會自動選重複的
      const categorydata = [...new Set(data.returnData.map((item) => item.categoryName))];

      category.value = categorydata.map((name, index) => ({
        key: index.toString(),
        name: name,
      }));
    } else {
      console.error('API 回傳錯誤:', data.message);
    }
  } catch (error) {
    console.error('連線失敗:', error);
  }
};

// 初始化抓取單筆資料 , 用來編輯
const updateData = async (id) => {
  if (!id) return;
  let url = `https://localhost:7124/api/Ledger/GetLedger?ledgerId=${id}`;

  const res = await fetch(url);
  const data = await res.json();

  if (data.codeStatus === 2000) {
    const item = data.returnData;

    itemName.value = item.itemName;
    itemCost.value = item.itemCost;
    itemIllustrate.value = item.itemIllustrate;

    selectedCategory.value = item.categoryName;
  } else {
    console.error('API 回傳錯誤:', data.message);
  }
};

onMounted(() => {
  // 初始化先抓類別資料 , 在抓個別資料
  categoryData();
  if (route.params.id) {
    updateData(route.params.id);
  }
});

// 新增帳本
const addoreditledger = async (id = null) => {
  if (!itemName.value || !itemCost.value || !selectedCategory.value) {
    alert('項目名稱,花費,類別為必填');
    return;
  }

  if (!itemCost.value || itemCost.value < 0) {
    alert('請輸入正確金額,不能為負數');
    return;
  }
  try {
    // 我後端已經寫了轉換類別的方法了 , 這裡就丟類別名稱回去就好
    // 但是因為儲存類別有兩個方法 , 一個是選現有的就會是物件 , 一個用輸入的就會是變數 , 所以要看他的型別來判斷
    const categoryname =
      typeof selectedCategory.value === 'object'
        ? selectedCategory.value.name
        : selectedCategory.value;
    if (isAdd.value) {
      const addres = await fetch(`https://localhost:7124/api/Ledger/CreateLedger`, {
        method: 'Post',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          categoryname: categoryname,
          ItemName: itemName.value,
          UserId: userId.value,
          // 轉日期格式
          ItemCreateDate: itemDate.value
            ? new Date(itemDate.value).toLocaleDateString('en-CA')
            : null,
          ItemCost: itemCost.value,
          ItemIllustrate: itemIllustrate.value,
        }),
      });
      if (addres.ok) {
        // 成功就關閉彈窗
        visible.value = false;
      }
    } else if (!isAdd.value) {
      const updateres = await fetch(`https://localhost:7124/api/Ledger/UpdateLedger`, {
        method: 'Put',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          categoryname: categoryname || '',
          ItemId: parseInt(id),
          ItemName: itemName.value,
          UserId: userId.value,
          // 轉日期格式
          ItemUpdateDate: new Date(itemDate.value).toLocaleDateString('en-CA'),
          ItemCost: itemCost.value,
          ItemIllustrate: itemIllustrate.value,
        }),
      });
      if (updateres.ok) {
        // 成功就關閉彈窗
        visible.value = false;
      }
    }
  } catch (err) {
    console.log(err);
  }
};

// 控制彈窗
const visible = ref(true);
// 路由判斷新增或刪除
const isAdd = computed(() => route.name === 'add-ledger');
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
