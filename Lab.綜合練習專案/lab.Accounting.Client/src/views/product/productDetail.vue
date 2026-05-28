<script setup>
import { getProduct } from '@/api/productsService';
import { addProductsInShoppingCar } from '@/api/shoppingcarService';
import { getStore } from '@/api/storeService';
import { getOneUser } from '@/api/userService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路徑
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   productAllRate : 商品的所有評價
   sellerAllRate : 賣家的所有評價
   sellerAVGRate : 賣家評分
   boughtQuantity : 購買數量
   displayBasic : 大圖展開開關
   activeIndex : 當下選擇開啟的大圖
   seller : 賣家
   store : 賣場
*/
const route = useRoute();
const router = useRouter();
const product = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const productAllRate = ref(null);
const sellerAllRate = ref(null);
const sellerAVGRate = ref();
const boughtQuantity = ref(1);
const displayBasic = ref(false);
const activeIndex = ref();
const seller = ref();
const store = ref({});
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   查看商品細節資訊
*/
const getProductDetail = async (id) => {
  try {
    showLoading();
    const res = await getProduct(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      product.value = data.returnData;

      productAllRate.value = data.returnData.productsAllRates;
      await getSellerInfo(product.value.userId);
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   初始化時從 url 拿取 商品 ID
*/
onMounted(async () => {
  await getProductDetail(route.params.id);
  await getStoreInfo(product.value.userId);
});

/*
   拿到賣家資訊
*/
const getSellerInfo = async (id) => {
  try {
    showLoading();
    const res = await getOneUser(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      seller.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   拿到賣場資訊
*/
const getStoreInfo = async (id) => {
  try {
    showLoading();
    const res = await getStore(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      store.value = data.returnData;
      sellerAllRate.value = data.returnData.allProductsRateCount;
      sellerAVGRate.value = data.returnData.countAVGAllProductRate;
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
const getProductsImg = (img) => {
  if (img && img.productsImg) {
    return `${baseUrl}/ProductsImg/${img.productsImg}`;
  }
  return defaultImgurl;
};

/*
  商品圖片大圖跟縮排
*/
const productsImages = computed(
  () =>
    product.value?.productsImgs?.map((img) => ({
      // 大圖
      itemImageSrc: getProductsImg(img),
      // 小圖
      thumbnailImageSrc: getProductsImg(img),
      alt: '商品圖片',
    })) ?? [],
);

/*
  不同螢幕下的商品圖片列顯示數量設定
*/
const responsiveOptions = [
  { breakpoint: '1300px', numVisible: 4 }, // 螢幕寬度 < 1300px 時，縮圖顯示 4 個
  { breakpoint: '575px', numVisible: 3 }, // 以此類推
  { breakpoint: '768px', numVisible: 2 },
  { breakpoint: '560px', numVisible: 1 },
];

/*
  開啟商品大圖
*/
const openGalleria = (index) => {
  activeIndex.value = index;
  displayBasic.value = true;
};

/*
   載入評論區頭貼
*/
const userImg = (user) => {
  if (user) {
    return `${baseUrl}/UserHeadShot/${user.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
};

/*
   載入賣家頭貼
*/
const sellerImg = (user) => {
  if (user) {
    return `${baseUrl}/UserHeadShot/${user.userHeadshot}`;
  } else {
    return defaultImgurl;
  }
};

/*
  商品加入購物車
*/
const addProductsInCar = async (productId, boughtquantity) => {
  try {
    var res = await addProductsInShoppingCar(productId, boughtquantity);
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('加入成功!');
    }
    if (data.codeStatus === 4000) {
      showToastError(getError400Message(data.error400));
    }
  } catch (err) {
    console.log(err);
  } finally {
  }
};

/*
  直接購買
*/
const boughtProduct = async (id, boughtquantity) => {
  await addProductsInCar(id, boughtquantity);
  router.push({ name: 'shopping-car' });
};

/*
  麵包屑
*/
const home = ref({
  icon: 'pi pi-home',
  command: () => router.push({ name: 'mall' }),
});
const breadCrumbItem = computed(() => {
  if (!product.value) return [];
  return [
    {
      label: product.value.parentCategoryName,
      command: () =>
        router.push({ name: 'mall-category', params: { id: product.value.productParentId } }),
    },
    {
      label: product.value.productCategoryName,
      command: () =>
        // 父類別也要帶過去才能顯示所有類別
        router.push({
          name: 'mall-category',
          params: { id: product.value.productCategoryId },
          query: { parentId: product.value.productParentId },
        }),
    },
  ];
});
</script>

<template>
  <div class="flex flex-col w-full gap-4" v-if="product">
    <div class="min-h-screen py-6 px-4 bg-gray-50">
      <div class="max-w-5xl mx-auto flex flex-col gap-3">
        <!--#region 商品圖片 / 簡介區 -->
        <div class="bg-white rounded-lg flex gap-8">
          <!-- #region 圖片 -->
          <div class="flex flex-col gap-2">
            <div>
              <!-- #region 頁面圖片 -->
              <Galleria
                v-model:activeIndex="activeIndex"
                :value="productsImages"
                :responsiveOptions="responsiveOptions"
                :numVisible="5"
                containerStyle="max-width: 640px"
                :circular="true"
                :showItemNavigators="true"
                :showThumbnailNavigators="true"
              >
                <template #item="slotProps">
                  <img
                    :src="slotProps.item.itemImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-120 h-120 object-cover rounded-lg"
                    @click="openGalleria(productsImages.indexOf(slotProps.item))"
                  />
                </template>
                <template #thumbnail="slotProps">
                  <img
                    :src="slotProps.item.thumbnailImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-16 h-16 object-cover rounded-lg"
                    @click="openGalleria(productsImages.indexOf(slotProps.item))"
                  />
                </template>
              </Galleria>
              <!-- #endregion -->
              <!-- #region 全螢幕圖片 -->
              <Galleria
                v-model:visible="displayBasic"
                v-model:activeIndex="activeIndex"
                :value="productsImages"
                :responsiveOptions="responsiveOptions"
                :numVisible="5"
                containerStyle="max-width: 640px"
                :fullScreen="true"
                :circular="true"
                :showItemNavigators="true"
                :showThumbnailNavigators="true"
                :pt="{
                  mask: { onClick: () => (displayBasic = false) },
                }"
              >
                <template #item="slotProps">
                  <img
                    :src="slotProps.item.itemImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-120 h-120 object-cover rounded-lg"
                  />
                </template>
                <template #thumbnail="slotProps">
                  <img
                    :src="slotProps.item.thumbnailImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-16 h-16 object-cover rounded-lg"
                  />
                </template>
              </Galleria>
              <!-- #endregion -->
            </div>
          </div>
          <!-- #endregion -->

          <!-- #region 商品簡介 -->
          <div class="p-5 flex flex-col gap-4">
            <!-- #region 商品名稱 -->
            <p class="text-lg font-medium text-gray-900 leading-snug m-0">
              {{ product.productsName }}
            </p>
            <!-- #endregion -->

            <!-- #region 評分列 -->
            <div class="flex items-center gap-2 pb-4 border-b border-gray-100">
              <span class="text-sm font-medium border-b border-gray-900">{{
                product.productsAVGRate
              }}</span>
              <Rating :modelValue="product.productsAVGRate" :stars="5" :readonly="true" />
              <div class="w-px h-3.5 bg-gray-200"></div>
              <span class="text-xs text-gray-400">{{ productAllRate?.length ?? 0 }} 則評價</span>
            </div>
            <!-- #endregion -->

            <!-- #region 價格 -->
            <div class="bg-orange-50 rounded-md px-4 py-3">
              <span class="text-3xl font-medium text-orange-600"
                >$ {{ product.productsPrice }}</span
              >
            </div>
            <!-- #endregion -->

            <!-- #region 分類 / 庫存-->
            <div class="flex flex-col gap-2 text-sm">
              <div class="flex items-center gap-3">
                <span class="text-gray-400 min-w-15">分類</span>
                <div class="flex gap-2 flex-wrap">
                  <span
                    class="bg-gray-100 text-gray-500 px-3 py-0.5 rounded-full border border-gray-200 text-xs"
                    >{{ product.productCategoryName }}</span
                  >
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span class="text-gray-400 min-w-15">庫存</span>
                <span class="text-gray-800">{{ product.productsStock }} 件</span>
              </div>
            </div>
            <!-- #endregion -->

            <!-- #region 購買數量 -->
            <div class="flex items-center gap-4">
              <span class="text-sm text-gray-400 min-w-15">購買數量</span>
              <div class="flex items-center">
                <button
                  class="w-9 h-9 border border-gray-300 rounded-l flex items-center justify-center text-gray-600 hover:bg-gray-100 cursor-pointer text-lg"
                  @click="boughtQuantity = Math.max(1, boughtQuantity - 1)"
                >
                  −
                </button>
                <span
                  class="w-12 h-9 border-y border-gray-300 flex items-center justify-center text-sm select-none"
                >
                  {{ boughtQuantity }}
                </span>
                <button
                  class="w-9 h-9 border border-gray-300 rounded-r flex items-center justify-center text-gray-600 hover:bg-gray-100 cursor-pointer text-lg"
                  @click="boughtQuantity = Math.min(product.productsStock, boughtQuantity + 1)"
                >
                  +
                </button>
              </div>
              <span
                class="text-xs px-3 py-1 rounded-full"
                :class="
                  product.productsStock > 0
                    ? 'bg-green-50 text-green-700'
                    : 'bg-red-50 text-red-500'
                "
              >
                {{ product.productsStock > 0 ? '尚有庫存' : '已售罄' }}
              </span>
            </div>
            <!-- #endregion -->

            <!-- #region 操作按鈕 -->
            <div class="flex gap-3 mt-2">
              <button
                class="flex-1 py-2.5 border border-orange-500 bg-orange-50 text-orange-600 rounded-lg text-sm font-medium hover:bg-orange-100 cursor-pointer flex items-center justify-center gap-1.5"
                @click="addProductsInCar(product.productsId, boughtQuantity)"
              >
                <i class="pi pi-shopping-cart text-sm"></i>加入購物車
              </button>
              <button
                class="flex-1 py-2.5 bg-orange-500 text-white rounded-lg text-sm font-medium hover:bg-orange-600 cursor-pointer"
                @click="boughtProduct(product.productsId, boughtQuantity)"
              >
                立即購買
              </button>
            </div>
            <!-- #endregion -->
          </div>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->

        <!--#region 賣場資訊 -->
        <div class="bg-white rounded-lg p-6 flex gap-8 items-center" v-if="product.userId">
          <!--#region  頭像 + 名稱 + 按鈕 -->
          <div class="flex gap-5 items-center min-w-52">
            <img
              :src="sellerImg(seller)"
              alt="賣家頭像"
              class="w-18 h-18 rounded-full object-cover border border-gray-100"
            />
            <div class="flex flex-col gap-1.5">
              <p class="m-0 text-base font-medium text-gray-900">{{ store.storeName }}</p>
              <div class="flex items-center gap-1.5">
                <span class="w-2 h-2 rounded-full bg-green-500 inline-block"></span>
                <span class="text-xs text-gray-400">在線上</span>
              </div>
              <div class="flex gap-2 mt-1">
                <button
                  class="px-3 py-1 border border-gray-300 text-gray-500 text-xs rounded cursor-pointer hover:bg-gray-50 flex items-center gap-1"
                >
                  <i class="pi pi-comment text-xs"></i>聊聊
                </button>
                <button
                  class="px-3 py-1 border border-orange-500 text-orange-500 text-xs rounded cursor-pointer hover:bg-orange-50 flex items-center gap-1"
                  @click="router.push({ name: 'seller-store', params: { id: seller.userId } })"
                >
                  <i class="pi pi-plus text-xs"></i>前往賣場
                </button>
              </div>
            </div>
          </div>
          <!-- #endregion -->

          <!-- 分隔線 -->
          <div class="w-px h-20 bg-gray-100"></div>

          <!--#region  統計資訊 -->
          <div class="grid grid-cols-3 gap-x-10 gap-y-2.5 flex-1 text-sm">
            <div class="flex items-center gap-2">
              <span class="text-gray-400">商品</span>
              <span class="text-orange-500 font-medium">{{ store.allProductsCount }}</span>
            </div>

            <div class="flex items-center gap-2">
              <span class="text-gray-400">加入時間</span>
              <span class="text-gray-700 font-medium">{{ formatDateOnly(store.createTime) }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-gray-400">評價</span>
              <span class="text-orange-500 font-medium">{{ sellerAllRate }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-gray-400">賣場評分</span
              ><span class="text-orange-500 font-medium">{{ sellerAVGRate }}</span>
              <Rating :modelValue="sellerAVGRate" :stars="5" :readonly="true" />
            </div>
            <div class="flex items-center gap-2">
              <span class="text-gray-400">公司名稱</span>
              <span class="text-gray-700 font-medium">{{ store.storeCompanyName }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-gray-400">公司統編</span>
              <span class="text-gray-700 font-medium">{{ store.storeUnifiedNumber }}</span>
            </div>
          </div>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->
        <!--#region 商品描述 -->
        <div class="bg-white rounded-lg p-6" v-if="product.productsDescription">
          <h3 class="text-sm font-medium text-gray-700 mb-4 pb-2 border-b border-gray-100">
            商品描述
          </h3>
          <div class="card flex justify-start">
            <Breadcrumb :home="home" :model="breadCrumbItem" />
          </div>
          <div v-html="product.productsDescription" class="leading-relaxed text-gray-700" />
        </div>
        <!-- #endregion -->
        <!--#region 評論區 -->
        <span class="text-xs text-gray-400">{{ productAllRate?.length ?? 0 }} 則評價</span>
        <div
          v-for="rate in productAllRate"
          class="hover:shadow-xl bg-white h-20 hover:bg-gray-50 flex flex-row ps-10 items-center"
        >
          <img :src="userImg(rate)" alt="頭貼" class="w-10 h-10 rounded-full object-cover me-5" />
          <span class="mt-3 me-5">評價者名稱 : {{ rate.userName }}</span>
          <span class="mt-3 me-5">評論 : {{ rate.comment }}</span>
          <span class="mt-3 me-5">評價時間 : {{ formatDateTimeString(rate.createTime) }}</span>
          <span class="mt-3 me-5">評分 : {{ rate.rating }}</span>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
