<script setup>
import zipCodeData from '@/assets/zipCodeData.json';

/*
    變數名稱代表意義
    props : 定義接收的父元件傳來的物件參數 => 接收 { 城市 , 地區 , 郵遞區號 }
    emit : 定義事件 => 當內部資料改變時 ( 剛剛定義的城市地區 ) , 傳回父元件更新 v-model
    route : 獲取路由資訊
    account : 帳號
*/
const props = defineProps({
  modelValue: { type: Object, default: () => ({ city: '', district: '', zipCode: '' }) },
});
const emit = defineEmits(['update:modelValue']);
const selectedCity = ref(props.modelValue.city);
const selectedDistrict = ref(props.modelValue.district);


/*
   將 zipCodeData（原始資料來源）的值（縣市名稱）轉換為 PrimeVue Select 組件所需的格式 { label, value }
*/
const cityOptions = computed(() =>
  Object.keys(zipCodeData).map((city) => ({ label: city, value: city })),
);

/*
   將 zipCodeData（原始資料來源）的值（縣市名稱）轉換為 PrimeVue Select 組件所需的格式 { label, value }
*/

const districtOptions = computed(() => {
  if (!selectedCity.value) return [];
  return Object.keys(zipCodeData[selectedCity.value]).map((district) => ({
    label: district,
    value: district,
  }));
});

/*
   換縣市時，清空已選的鄉鎮市區（避免殘留上一個縣市的區）
*/
watch(selectedCity, () => {
  selectedDistrict.value = '';
});

/*
   選完鄉鎮市區後，自動帶出郵遞區號並往外 emit
*/
watch(selectedDistrict, (district) => {
  const zipCode = district ? zipCodeData[selectedCity.value][district] : '';
  emit('update:modelValue', {
    city: selectedCity.value,
    district,
    zipCode,
  });
});
</script>

<template>
  <div class="flex gap-3">
    <Select
      v-model="selectedCity"
      :options="cityOptions"
      option-label="label"
      option-value="value"
      placeholder="請選擇縣市"
      class="flex-1"
    />
    <Select
      v-model="selectedDistrict"
      :options="districtOptions"
      option-label="label"
      option-value="value"
      placeholder="請選擇鄉鎮市區"
      :disabled="!selectedCity"
      class="flex-1"
    />
  </div>
</template>
