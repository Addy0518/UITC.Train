<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
// 路由參數
const route = useRoute();
const text1 = ref(null);
const detail = ref(null);
const number = ref(null);
const date = ref(null);
const selectedCategory = ref();
const category = ref([
  { name: 'New York', code: 'NY' },
  { name: 'Rome', code: 'RM' },
  { name: 'London', code: 'LDN' },
  { name: 'Istanbul', code: 'IST' },
  { name: 'Paris', code: 'PRS' },
]);

const isAdd = computed(() => route.name === 'add-ledger');
</script>

<template>
  <div class="container mx-auto p-10">
    <p class="text-center mb-10 text-3xl font-bold">{{ isAdd ? '新增' : '編輯' }}帳本項目</p>

    <!-- 項目欄位 -->
    <div class="card grid grid-cols-1 gap-4 gap-y-10 max-h-[370px] overflow-y-auto">
      <div class="text-xl font-bold" v-if="$route.params.id">
        <p>編號 : {{ $route.params.id }}</p>
      </div>
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-user"></i>
        </InputGroupAddon>
        <InputText v-model="text1" placeholder="項目名稱" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>$</InputGroupAddon>
        <InputNumber v-model="number" placeholder="花費" />
        <InputGroupAddon>.00</InputGroupAddon>
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-calendar"></i>
        </InputGroupAddon>
        <DatePicker v-model="date" placeholder="日期" dateFormat="yy-mm-dd" />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-align-justify"></i>
        </InputGroupAddon>
        <Select
          v-model="selectedCategory"
          :options="category"
          optionLabel="name"
          placeholder="類別"
        />
      </InputGroup>

      <InputGroup>
        <InputGroupAddon><i class="pi pi-book"></i></InputGroupAddon>
        <Textarea v-model="detail" placeholder="補充說明" class="w-full" />
      </InputGroup>
    </div>
    <div class="justify-end flex mt-5">
      <button
        @click="confirm1($event)"
        label="Save"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        儲存
      </button>
    </div>
  </div>
</template>
