<script setup>
import { getFatherCategories } from '@/api/categoryService';
import { getProduct, sellerReplyComment } from '@/api/productsService';
import { addProductsInShoppingCar } from '@/api/shoppingcarService';
import { getStore } from '@/api/storeService';
import { getOneUser } from '@/api/userService';
import defaultImgurl from '@/img/預設圖片.jpg';

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
   breadCrumCategories : 麵包屑的類別
   replyComment : 賣家回復的評論
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
const breadCrumCategories = ref([]);
const replyComment = ref();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  replyComment: { required, maxLength: maxLength(3000) },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(rules, { replyComment }, { $autoDirty: true, $lazy: true, $scope: false });

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

      const catRes = await getFatherCategories(product.value.productCategoryId);
      const { data: catData } = catRes;
      if (catData.codeStatus === 2000) {
        breadCrumCategories.value = catData.returnData;
      }
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
   賣家回覆評論
*/
const sellerReply = async (orderId) => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const res = await sellerReplyComment({ orderId: orderId, reply: replyComment.value });
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功回覆 !');
      replyComment.value = '';
      await getProductDetail(route.params.id);
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
  const headshot = user.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
  return `${baseUrl}/UserHeadShot/${headshot}`;
};

/*
   載入賣家頭貼
*/
const sellerImg = (user) => {
  if (!user || !user.userHeadshot) {
    return defaultImgurl;
  }
  if (user.userHeadshot.includes('googleusercontent.com')) {
    return user.userHeadshot;
  }
  return `${baseUrl}/UserHeadShot/${user.userHeadshot}`;
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
  if (!product.value) return [];

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
  <div class="flex flex-col w-full gap-4" v-if="product">
    <div class="min-h-screen py-6 px-4 bg-page-bg-soft">
      <div class="max-w-5xl mx-auto flex flex-col gap-3">
        <!--#region 商品圖片 / 簡介區 -->
        <div class="bg-page-bg rounded-card border border-border-soft flex gap-8">
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
                    class="w-120 h-120 object-cover rounded-card"
                    @click="openGalleria(productsImages.indexOf(slotProps.item))"
                  />
                </template>
                <template #thumbnail="slotProps">
                  <img
                    :src="slotProps.item.thumbnailImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-16 h-16 object-cover rounded-card"
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
                  mask: {
                    onClick: (event) => {
                      if (event.target === event.currentTarget) {
                        displayBasic = false;
                      }
                    },
                  },
                }"
              >
                <template #item="slotProps">
                  <img
                    :src="slotProps.item.itemImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-120 h-120 object-cover rounded-card"
                  />
                </template>
                <template #thumbnail="slotProps">
                  <img
                    :src="slotProps.item.thumbnailImageSrc"
                    :alt="slotProps.item.alt"
                    class="w-16 h-16 object-cover rounded-card"
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
            <p class="text-lg font-medium text-ink-900 leading-snug m-0">
              {{ product.productsName }}
            </p>
            <!-- #endregion -->

            <!-- #region 評分列 -->
            <div class="flex items-center gap-2 pb-4 border-b border-border-soft">
              <span class="text-sm font-medium border-b border-ink-900 text-ink-900">{{
                product.productsAVGRate
              }}</span>
              <Rating
                :modelValue="product.productsAVGRate"
                :stars="5"
                :readonly="true"
                :pt="{
                  onIcon: { class: 'text-brand-price' } /* 已點亮星星的顏色 */,
                  offIcon: { class: 'text-slate-300' } /* 未點亮星星的顏色 */,
                }"
              />
              <div class="w-px h-3.5 bg-border-soft"></div>
              <span class="text-xs text-ink-500">{{ productAllRate?.length ?? 0 }} 則評價</span>
            </div>
            <!-- #endregion -->

            <!-- #region 價格 -->
            <div class="bg-brand-50 rounded-card px-4 py-3">
              <div v-if="product.isDiscount" class="flex items-center gap-2">
                <span class="text-3xl font-medium text-brand-price"
                  >$ {{ product.finalPrice }}</span
                >
                <span class="text-sm font-medium line-through text-ink-300">
                  $ {{ product.productsPrice }}
                </span>
              </div>
              <div v-else>
                <span class="text-3xl font-medium text-brand-price"
                  >$ {{ product.productsPrice }}</span
                >
              </div>
            </div>
            <!-- #endregion -->

            <!-- #region 分類 / 庫存-->
            <div class="flex flex-col gap-2 text-sm">
              <div class="flex items-center gap-3">
                <span class="text-ink-500 min-w-15">分類</span>
                <div class="flex gap-2 flex-wrap">
                  <span
                    class="bg-surface-muted text-ink-500 px-3 py-0.5 rounded-full border border-border-soft text-xs"
                    >{{ product.productCategoryName }}</span
                  >
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span class="text-ink-500 min-w-15">庫存</span>
                <span class="text-ink-900">{{ product.productsStock }} 件</span>
              </div>
            </div>
            <!-- #endregion -->

            <!-- #region 購買數量 -->
            <div class="flex items-center gap-4">
              <span class="text-sm text-ink-500 min-w-15">購買數量</span>
              <div class="flex items-center">
                <button
                  class="w-9 h-9 border border-border-soft rounded-l-card flex items-center justify-center text-ink-500 hover:bg-surface-muted cursor-pointer text-lg"
                  @click="boughtQuantity = Math.max(1, boughtQuantity - 1)"
                >
                  −
                </button>
                <span
                  class="w-12 h-9 border-y border-border-soft flex items-center justify-center text-sm select-none text-ink-900"
                >
                  {{ boughtQuantity }}
                </span>
                <button
                  class="w-9 h-9 border border-border-soft rounded-r-card flex items-center justify-center text-ink-500 hover:bg-surface-muted cursor-pointer text-lg"
                  @click="boughtQuantity = Math.min(product.productsStock, boughtQuantity + 1)"
                >
                  +
                </button>
              </div>
              <span
                class="text-xs px-3 py-1 rounded-full"
                :class="
                  product.productsStock > 0
                    ? 'bg-status-success/10 text-status-success'
                    : 'bg-surface-muted text-status-neutral'
                "
              >
                {{ product.productsStock > 0 ? '尚有庫存' : '已售罄' }}
              </span>
            </div>
            <!-- #endregion -->

            <!-- #region 操作按鈕 -->
            <div class="flex gap-3 mt-2">
              <button
                class="flex-1 py-2.5 border border-brand-500 bg-brand-50 text-brand-500 rounded-card text-sm font-medium hover:bg-brand-100 cursor-pointer flex items-center justify-center gap-1.5"
                @click="addProductsInCar(product.productsId, boughtQuantity)"
              >
                <i class="pi pi-shopping-cart text-sm"></i>加入購物車
              </button>
              <button
                class="flex-1 py-2.5 bg-brand-500 text-white rounded-card text-sm font-medium hover:opacity-90 cursor-pointer"
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
        <div
          class="bg-page-bg rounded-card border border-border-soft p-6 flex gap-8 items-center"
          v-if="product.userId"
        >
          <!--#region  頭像 + 名稱 + 按鈕 -->
          <div class="flex gap-5 items-center min-w-52">
            <img
              :src="sellerImg(seller)"
              alt="賣家頭像"
              class="w-18 h-18 rounded-full object-cover border border-border-soft"
            />
            <div class="flex flex-col gap-1.5">
              <p class="m-0 text-base font-medium text-ink-900">{{ store.storeName }}</p>
              <div class="flex items-center gap-1.5">
                <span class="w-2 h-2 rounded-full bg-status-success inline-block"></span>
                <span class="text-xs text-ink-500">在線上</span>
              </div>
              <div class="flex gap-2 mt-1">
                <button
                  class="px-3 py-1 border border-border-soft text-ink-500 text-xs rounded-card cursor-pointer hover:bg-surface-muted flex items-center gap-1"
                >
                  <i class="pi pi-comment text-xs"></i>聊聊
                </button>
                <button
                  class="px-3 py-1 border border-brand-500 text-brand-500 text-xs rounded-card cursor-pointer hover:bg-brand-50 flex items-center gap-1"
                  @click="router.push({ name: 'seller-store', params: { id: seller.userId } })"
                >
                  <i class="pi pi-arrow-circle-right text-xs"></i>前往賣場
                </button>
              </div>
            </div>
          </div>
          <!-- #endregion -->

          <!-- 分隔線 -->
          <div class="w-px h-20 bg-border-soft"></div>

          <!--#region  統計資訊 -->
          <div class="grid grid-cols-3 gap-x-10 gap-y-2.5 flex-1 text-sm">
            <div class="flex items-center gap-2">
              <span class="text-ink-500">商品</span>
              <span class="text-brand-price font-medium">{{ store.allProductsCount }}</span>
            </div>

            <div class="flex items-center gap-2">
              <span class="text-ink-500">加入時間</span>
              <span class="text-ink-900 font-medium">{{ formatDateOnly(store.createTime) }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-ink-500">評價</span>
              <span class="text-brand-price font-medium">{{ sellerAllRate }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-ink-500">賣場評分</span
              ><span class="text-brand-price font-medium">{{ sellerAVGRate }}</span>
              <Rating
                :modelValue="sellerAVGRate"
                :stars="5"
                :readonly="true"
                :pt="{
                  onIcon: { class: 'text-brand-price' } /* 已點亮星星的顏色 */,
                  offIcon: { class: 'text-slate-300' } /* 未點亮星星的顏色 */,
                }"
              />
            </div>
            <div class="flex items-center gap-2">
              <span class="text-ink-500">公司名稱</span>
              <span class="text-ink-900 font-medium">{{ store.storeCompanyName }}</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="text-ink-500">公司統編</span>
              <span class="text-ink-900 font-medium">{{ store.storeUnifiedNumber }}</span>
            </div>
          </div>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->
        <!--#region 麵包屑 -->
        <div class="card flex justify-start">
          <Breadcrumb :home="home" :model="breadCrumbItem" />
        </div>
        <!-- #endregion -->
        <!--#region 商品描述 -->
        <div
          class="bg-page-bg rounded-card border border-border-soft p-6"
          v-if="product.productsDescription"
        >
          <h3 class="text-sm font-medium text-ink-900 mb-4 pb-2 border-b border-border-soft">
            商品描述
          </h3>

          <div v-html="product.productsDescription" class="leading-relaxed text-ink-500" />
        </div>
        <!-- #endregion -->
        <!--#region 評論區 -->
        <span class="text-xs text-ink-500">{{ productAllRate?.length ?? 0 }} 則評價</span>
        <div
          v-for="rate in productAllRate"
          :key="rate.productsRateId"
          class="bg-page-bg border border-border-soft rounded-card hover:border-ink-300 transition-colors p-5 flex flex-col gap-3"
        >
          <!-- 買家評論 -->
          <div class="flex items-start gap-4">
            <img
              :src="userImg(rate)"
              alt="頭貼"
              class="w-10 h-10 rounded-full object-cover shrink-0"
            />
            <div class="flex-1 flex flex-col gap-1">
              <div class="flex items-center gap-3">
                <span class="text-sm font-medium text-ink-900">{{ rate.userName }}</span>
                <Rating
                  :modelValue="rate.rating"
                  :stars="5"
                  :readonly="true"
                  :pt="{
                    onIcon: { class: 'text-brand-price' },
                    offIcon: { class: 'text-slate-300' },
                  }"
                />
              </div>
              <p class="text-sm text-ink-500 m-0">{{ rate.comment }}</p>
              <span class="text-xs text-ink-300">{{ formatDateTimeString(rate.createTime) }}</span>
            </div>
          </div>

          <!-- 賣家回覆 ( 有回覆才顯示 ) -->
          <div
            v-if="rate.sellerReply"
            class="ms-14 bg-brand-50 rounded-card px-4 py-3 flex flex-col gap-1"
          >
            <div class="flex items-center gap-2">
              <i class="pi pi-reply text-brand-500 text-xs"></i>
              <span class="text-xs font-medium text-brand-500">賣家回覆</span>
            </div>
            <p class="text-sm text-ink-900 m-0">{{ rate.sellerReply }}</p>
            <span class="text-xs text-ink-300">{{
              formatDateTimeString(rate.sellerReplyTime)
            }}</span>
          </div>
          <div
            v-else-if="authStore.userId === rate.sellerUserId"
            class="ms-14 bg-brand-50 rounded-card px-4 py-3 flex flex-col gap-1"
          >
            <div class="flex items-center gap-2">
              <i class="pi pi-reply text-brand-500 text-xs"></i>
              <span class="text-xs font-medium text-brand-500">回覆買家</span>
            </div>
            <InputGroup>
              <InputText
                v-model="replyComment"
                placeholder="回覆評論 ..."
                :invalid="v$.replyComment.$error"
              >
              </InputText>
            </InputGroup>

            <InValidErrorMessage :errorDto="v$.replyComment.$errors" vaildChiName="賣家回覆" />
          </div>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
