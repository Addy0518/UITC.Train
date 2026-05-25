<script setup>
import { getAllProduct } from '@/api/productsService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
import advertise1 from '@/img/廣告1.jpg';
import advertise2 from '@/img/廣告2.jpg';
import advertise3 from '@/img/廣告3.jpg';

/*
  變數名稱代表意義
  route : 獲取路由資訊
  router :　改變路由
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  baseUrl : 環境變數裡的圖片基底位址
  selectedCategory : 選擇的類別區塊
  advertiseImg : 廣告圖片
*/
const route = useRoute();
const router = useRouter();
const allProducts = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const selectedCategory = ref(null);
const advertiseImg = ref([
  { itemImageSrc: advertise1 },
  { itemImageSrc: advertise2 },
  { itemImageSrc: advertise3 },
]);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化加載所有商品
*/
onMounted(() => {
  if (route.query.forbidden) {
    showToastError('你沒有訪問權限');
  }
  loadproducts();
});

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async () => {
  try {
    showLoading();

    const res = await getAllProduct();
    const { data } = res;
    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData.products;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  根據所有商品類別取出不重複的各個父類別
*/
const allCategories = computed(() => {
  if (!allProducts.value) return [];
  const seen = new Set();
  // 創建新的 Set 來記錄類別 id 跟名稱
  return allProducts.value
    .filter((p) => {
      if (seen.has(p.parentCategoryName)) return false;
      seen.add(p.parentCategoryName);
      return true;
    })
    .map((p) => ({ id: p.productParentId, name: p.parentCategoryName }));
});

/*
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImgs && product.productsImgs.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImgs[0].productsImg}`;
  }
  return defaultImgurl;
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-20 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <!-- #region 頁面圖片 -->

          <div class="card">
            <Galleria
              :value="advertiseImg"
              :showThumbnails="false"
              :showIndicators="true"
              :showItemNavigators="true"
              :showIndicatorsOnItem="true"
              indicatorsPosition="bottom"
            >
              <template #item="slotProps">
                <img
                  :src="slotProps.item.itemImageSrc"
                  style="width: 100%; height: 400px; display: block; object-fit: contain"
                />
              </template>
            </Galleria>
          </div>

          <!-- #endregion -->
          <span class="text-2xl m-5">分類</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="categoryname in allCategories">
              <div
                @click="router.push({ name: 'mall-category', params: { id: categoryname.id } })"
                class="hover:shadow-xl hover:bg-gray-50 h-80 cursor-pointer flex flex-col items-center"
              >
                <img :src="defaultImgurl" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
                <span class="mt-15">{{ categoryname.name }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-10 w-300 rounded-lg shadow-sm">
        <div class="justify-between">
          <span class="text-2xl m-5">商品</span>
          <div class="grid grid-cols-4 mt-5">
            <div v-for="product in allProducts">
              <div class="hover:shadow-xl hover:bg-gray-50 h-100 flex flex-col items-center">
                <RouterLink
                  :to="{ name: 'product-detail', params: { id: product.productsId } }"
                  class="flex flex-col items-center cursor-pointer"
                >
                  <img
                    :src="getProductsImg(product)"
                    alt="Logo"
                    class="w-full max-w-40 max-h-40 mt-4"
                  />
                  <span class="mt-3">{{ product.productsName }}</span>
                  <span class="mt-3">{{ product.productsPrice }}</span>

                  <span class="mt-3 ms-2 me-2 text-sm text-gray-500">
                    {{ product.productCategoryName }}
                  </span>
                </RouterLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <Paginator
      :template="{
        '640px': 'PrevPageLink CurrentPageReport NextPageLink',
        '960px': 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink',
        '1300px': 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
        default:
          'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageDropdown JumpToPageInput',
      }"
      :rows="10"
      :totalRecords="120"
    >
    </Paginator>
  </div>
</template>
