/*
   定義 pinia 的訂單設定
*/
export const useOrderStore = defineStore('order', {
  state: () => ({
    // 訂單
    selectedItems: [],
  }),
  persist: true,
});
