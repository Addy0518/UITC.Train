<script setup>
import { getAllProductsInShoppingCar, deleteProductsInShoppingCar } from '@/api/shoppingcarService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import { useOrderStore } from '@/stores/order';

/*
  變數名稱代表意義
  allProductsRaw : 初始資料 ( 全部商品 )
  products : 全部商品
  baseUrl : 環境變數裡的圖片基底位址
  router : 控制路由
  boughtQuantity : 購買數量
  selectProducts : 選擇的商品
  orderStore : 訂單 pinia
*/

const products = ref([]);
const allProductsRaw = ref();
const baseUrl = import.meta.env.VITE_IMG_URL;
const router = useRouter();
const selectProducts = ref([]);
const orderStore = useOrderStore();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時加載購物車商品
*/
onMounted(() => {
  loadproducts();
});

/*
   初始化時加載購物車商品
*/
const loadproducts = async () => {
  try {
    showLoading();
    const res = await getAllProductsInShoppingCar();
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProductsRaw.value = data.returnData;

      /*
        在解構的陣列 products 裡面再建立一個陣列 [x.productCategoryName, x] , 為 key 跟 value
        用 map 去除重複的 key 再把陣列轉回 values 陣列
    */
      products.value = [...new Map(allProductsRaw.value.map((x) => [x.productsName, x])).values()];
    } else {
      allProductsRaw.value = [];
      products.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImgs && product.productsImgs.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImgs[0].productsImg}`;
  }
  return defaultImgurl;
};

/*
  移除購物車
*/
const deleteProductsInCar = async (productId) => {
  try {
    showLoading();
    await deleteProductsInShoppingCar(productId);
    await loadproducts();
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  前往訂單
*/
const goToOrder = async () => {
  if (selectProducts.value <= 0) {
    showToastError('請先選擇商品!');
    return;
  }

  orderStore.selectedItems = products.value.filter((p) =>
    selectProducts.value.includes(p.productsId),
  );
  router.push({ name: 'product-bought' });
};

/*
  總金額計算
*/
const totalPrice = computed(() =>
  products.value
    .filter((p) => selectProducts.value.includes(p.productsId))
    .reduce((sum, p) => sum + p.productsPrice * p.boughtQuantity, 0),
);
</script>

<template>
  <div class="flex flex-col w-full mt-20">
    <div class="max-w-5xl mx-auto w-full">
      <div class="border border-gray-200 rounded-xl overflow-hidden">
        <!-- 標題 -->
        <div class="px-5 py-4 border-b border-gray-200 flex items-center gap-2">
          <span class="font-medium">購物車</span>
        </div>

        <!-- 商品列表 -->
        <div
          v-for="product in products"
          :key="product.productsId"
          class="px-5 py-4 border-b border-gray-100 flex items-center gap-4"
        >
          <input
            type="checkbox"
            v-model="selectProducts"
            :value="product.productsId"
            class="w-5 h-5 me-2"
          />

          <img
            :src="getProductsImg(product)"
            class="w-30 h-30 rounded-lg object-cover cursor-pointer bg-gray-100"
            @click="router.push({ name: 'product-detail', params: { id: product.productsId } })"
          />

          <div class="flex-1 min-w-0">
            <p class="text-xl font-medium mb-2">{{ product.productsName }}</p>
            <p class="text-sm text-gray-400 mb-2">
              NT$ {{ product.productsPrice }}　｜　
              <span>{{ product.productCategoryName }}</span>
            </p>
            <div class="flex items-center gap-2">
              <div class="flex items-center border border-gray-200 rounded-lg overflow-hidden">
                <button
                  class="px-2 py-1 text-gray-500 hover:bg-gray-100 cursor-pointer"
                  @click="product.boughtQuantity = Math.max(1, product.boughtQuantity - 1)"
                >
                  −
                </button>
                <span class="px-3 py-1 text-sm border-x border-gray-200">{{
                  product.boughtQuantity
                }}</span>
                <button
                  class="px-2 py-1 text-gray-500 hover:bg-gray-100 cursor-pointer"
                  @click="
                    product.boughtQuantity = Math.min(
                      product.productsStock,
                      product.boughtQuantity + 1,
                    )
                  "
                >
                  +
                </button>
              </div>
              <span class="text-xs text-gray-400">{{
                product.productsStock ? '尚有庫存' : '已售罄'
              }}</span>
            </div>
          </div>

          <div class="flex flex-col items-end gap-2">
            <span class="text-xl font-medium"
              >NT$ {{ (product.productsPrice * product.boughtQuantity).toLocaleString() }}</span
            >
            <button
              class="text-sm text-red-400 flex items-center gap-1 cursor-pointer"
              @click="deleteProductsInCar(product.productsId)"
            >
              移除
            </button>
          </div>
        </div>

        <!-- 底部結算 -->
        <div class="px-5 py-4 flex justify-between items-center">
          <span class="text-sm text-gray-400">已選 {{ selectProducts.length }} 件</span>
          <div class="flex items-center gap-4">
            <span class="text-sm text-gray-500">
              總金額：<span class="text-base font-medium text-gray-900"
                >NT$ {{ totalPrice.toLocaleString() }}</span
              >
            </span>

            <button
              class="bg-black text-white text-sm font-medium px-5 py-2 rounded-lg cursor-pointer"
              @click="goToOrder"
            >
              前往訂單
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
