<script setup>
import { getFatherCategories, getOneSonCategory } from '@/api/categoryService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
  變數名稱代表意義
  route : 獲取路由資訊
  router : 改變路徑
  allProductsRaw : 原始資料 ( 用來篩選類別之後能從原始資料重抓 )
  totalCount : 商品數量
  allCategories : 所有子類別
  baseUrl : 環境變數裡的圖片基底位址
  selectedCategory : 選擇的類別區塊
  breadCrumCategories : 麵包屑的類別
*/
const route = useRoute();
const router = useRouter();
const allProducts = ref([]);
const baseUrl = import.meta.env.VITE_IMG_URL;
const selectedCategory = ref(null);
const totalCount = ref();
const allCategories = ref();
const breadCrumCategories = ref([]);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化加載所有商品 / 類別 / 麵包屑
*/
onMounted(() => {
  if (route.params.id) {
    loadproducts(route.params.id);
    loadCategory(route.params.id);
    loadBreadCrumb(route.params.id);
  }
});

/*
   監聽路由變化 , 隨時加載類別跟商品
*/
watch(
  () => route.params.id,
  (newId) => {
    loadproducts(newId);
    loadCategory(newId);
    loadBreadCrumb(newId);
  },
);

/*
   初始化時加載商品 , 並取出唯一的類別值放類別區 , 跟去除重複名稱的商品 ( 因為一個商品會有多個類別 , 所以這裡去重複 )
*/
const loadproducts = async (parentId, page = 0) => {
  try {
    showLoading();
    const res = await getAllProduct({ productCategoryId: parentId, pageIndex: page, pageSize: 12 });
    const { data } = res;

    if (data.codeStatus === 2000) {
      allProducts.value = data.returnData.products;
      totalCount.value = data.returnData.totalCount;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   換頁
*/
const pageChange = (event) => {
  loadproducts(route.params.id, event.page);
};

/*
  根據所有商品類別取出不重複的各個子類別
*/
const loadCategory = async (id) => {
  try {
    const res = await getOneSonCategory(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  點擊類別區分商品
*/
const selectCategory = async (categoryId) => {
  if (selectedCategory.value === categoryId) {
    // 再點一次取消，回到當前路由的全部商品
    selectedCategory.value = null;
    loadproducts(route.params.id);
  } else {
    selectedCategory.value = categoryId;
    loadproducts(categoryId); // 打 API 拿該類別商品
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
  拿到麵包屑類別
*/
const loadBreadCrumb = async (id) => {
  try {
    const res = await getFatherCategories(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      breadCrumCategories.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  麵包屑 , 回主頁
*/
const home = ref({
  icon: 'pi pi-home',
  command: () => router.push({ name: 'mall' }),
});

/*
  麵包屑 , 動態讀取所有父類別
*/
const breadCrumbItem = computed(() => {
  if (!allProducts.value) return [];

  return breadCrumCategories.value.map((category, index) => ({
    label: category.productCategoryName,
    command: () =>
      router.push({
        name: 'mall-category',
        params: { id: category.productCategoryId },
        query:
          // 不是第一層才要帶 parentId
          index > 0 ? { parentId: breadCrumCategories.value[index - 1].productCategoryId } : {},
      }),
  }));
});
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="flex flex-col items-center">
      <!-- #region  麵包屑-->
      <div class="card flex justify-start">
        <Breadcrumb :home="home" :model="breadCrumbItem" />
      </div>
      <!-- #endregion -->
      <div class="mt-20 w-300 rounded-lg shadow-sm">
        <!-- #region  類別 / 商品-->
        <div class="flex gap-4">
          <!-- #region  類別-->
          <div class="w-50 shrink-0">
            <span class="text-2xl m-5 font-bold">分類</span>
            <div class="flex flex-col mt-5 gap-2">
              <div v-for="category in allCategories" :key="category.productCategoryId">
                <span
                  @click="selectCategory(category.productCategoryId)"
                  class="cursor-pointer m-5"
                  :class="
                    selectedCategory === category.productCategoryId
                      ? 'text-orange-500 font-medium'
                      : 'text-gray-700'
                  "
                  >{{ category.productCategoryName }}</span
                >
              </div>
            </div>
          </div>
          <!-- #endregion -->
          <!-- #region  商品-->
          <div class="flex-1">
            <span class="text-2xl m-5">商品</span>
            <div class="grid grid-cols-3 mt-5 gap-4">
              <div v-for="product in allProducts" :key="product.productsId">
                <div
                  class="hover:shadow-xl hover:bg-gray-50 flex flex-col items-center rounded-lg p-3"
                >
                  <RouterLink
                    :to="{ name: 'product-detail', params: { id: product.productsId } }"
                    class="flex flex-col items-center cursor-pointer w-full"
                  >
                    <img
                      :src="getProductsImg(product)"
                      alt="Logo"
                      class="w-full max-w-40 max-h-40 mt-4 object-cover"
                    />
                    <span class="mt-3">{{ product.productsName }}</span>
                    <span class="mt-3">{{ product.productsPrice }}</span>
                    <span class="mt-3 ms-2 me-2 text-sm text-gray-500">{{
                      product.productCategoryName
                    }}</span>
                  </RouterLink>
                </div>
              </div>
            </div>
          </div>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->
        <!-- #region  頁碼按鈕-->
        <Paginator
          :template="{
            '640px': 'PrevPageLink CurrentPageReport NextPageLink',
            '960px': 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink',
            '1300px': 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
            default:
              'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageDropdown JumpToPageInput',
          }"
          :rows="12"
          :totalRecords="totalCount"
          @page="pageChange"
        >
        </Paginator>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
