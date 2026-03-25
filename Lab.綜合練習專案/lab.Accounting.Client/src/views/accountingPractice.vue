<template>
  <div style="margin-top: 200px">
    <div class="card">
      <DataTable
        :value="products"
        scrollable
        scrollHeight="400px"
        table-style="padding-left:150px;padding-right:150px"
        size="large"
      >
        <Column field="code" header="Code"></Column>
        <Column field="name" header="Name"></Column>
        <Column field="category" header="Category"></Column>
        <Column field="quantity" header="Quantity"></Column>
      </DataTable>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';

onMounted(() => {});

const products = ref([
  { code: 'P001', name: '無線滑鼠', category: '配件', quantity: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', quantity: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', quantity: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', quantity: 3 },
  { code: 'P005', name: 'USB-C 集線器', category: '配件', quantity: 20 },
  { code: 'P001', name: '無線滑鼠', category: '配件', quantity: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', quantity: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', quantity: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', quantity: 3 },
  { code: 'P005', name: 'USB-C 集線器', category: '配件', quantity: 20 },
  { code: 'P001', name: '無線滑鼠', category: '配件', quantity: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', quantity: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', quantity: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', quantity: 3 },
  { code: 'P005', name: 'USB-C 集線器', category: '配件', quantity: 20 },
  { code: 'P001', name: '無線滑鼠', category: '配件', quantity: 15 },
  { code: 'P002', name: '機械鍵盤', category: '配件', quantity: 8 },
  { code: 'P003', name: '27吋顯示器', category: '螢幕', quantity: 5 },
  { code: 'P004', name: '人體工學椅', category: '家具', quantity: 3 },
  { code: 'P005', name: 'USB-C 集線器', category: '配件', quantity: 20 },
]);
</script>
