<script setup>
import { ref, onMounted } from 'vue';

// 花費
const spend = ref(12000);
//所設立的類別
const nodes = ref([
  {
    key: '0',
    label: 'Documents',
    data: 'Documents Folder',
    icon: 'pi pi-fw pi-inbox',
    // children: [
    //   {
    //     key: '0-0',
    //     label: 'Work',
    //     data: 'Work Folder',
    //     icon: 'pi pi-fw pi-cog',
    //     children: [
    //       {
    //         key: '0-0-0',
    //         label: 'Expenses.doc',
    //         icon: 'pi pi-fw pi-file',
    //         data: 'Expenses Document',
    //       },
    //       { key: '0-0-1', label: 'Resume.doc', icon: 'pi pi-fw pi-file', data: 'Resume Document' },
    //     ],
    //   },
    //   {
    //     key: '0-1',
    //     label: 'Home',
    //     data: 'Home Folder',
    //     icon: 'pi pi-fw pi-home',
    //     children: [
    //       {
    //         key: '0-1-0',
    //         label: 'Invoices.txt',
    //         icon: 'pi pi-fw pi-file',
    //         data: 'Invoices for this month',
    //       },
    //     ],
    //   },
    // ],
  },
  {
    key: '0',
    label: 'Documents',
    data: 'Documents Folder',
    icon: 'pi pi-fw pi-inbox',
  },
]);
// 日期選擇
const date = ref();
// 她選了哪些類別
const selectedValue = ref(null);
// 所有項目
const products = ref([
  { code: 'P001', name: '無線滑鼠', category: '配件', cost: 15000 },
  { code: 'P002', name: '機械鍵盤', category: '配件', cost: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', cost: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', cost: 3 },
  { code: 'P001', name: '無線滑鼠', category: '配件', cost: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', cost: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', cost: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', cost: 3 },
  { code: 'P001', name: '無線滑鼠', category: '配件', cost: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', cost: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', cost: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', cost: 3 },
]);

onMounted(() => {});
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
            :options="nodes"
            selectionMode="multiple"
            display="chip"
            :maxSelectedLabels="3"
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
            <Column field="code" header="編號"></Column>
            <Column field="name" header="項目名稱"></Column>
            <Column field="category" header="類別"></Column>
            <Column field="cost" header="花費"></Column>
            <Column
              ><template #body="slotProps">
                <div class="flex justify-start gap-3 ml-10">
                  <RouterLink :to="{ name: 'edit-ledger', params: { id: slotProps.data.code } }">
                    <button class="bg-black text-white p-4 rounded-2xl cursor-pointer font-bold">
                      編輯
                    </button>
                  </RouterLink>
                  <button
                    @click="confirm1($event)"
                    class="text-white p-4 rounded-2xl cursor-pointer bg-red-500 font-bold"
                  >
                    刪除
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
