<script setup>
import { getAllProduct } from '@/api/productsService';
import { getStore, updateStore } from '@/api/storeService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   store          : 賣場資訊
   allproduct     : 賣場所有商品
   baseUrl        : 環境變數裡的圖片基底位址
   authStore      : pinia，取賣家自己的 userId 跟頭貼
   storeName      : 編輯中的賣場名稱
   storeCompanyName : 編輯中的公司名稱
   selectedCategory : 目前選取的商品類別篩選
*/
const router = useRouter();
const authStore = useAuthStore();
const baseUrl = import.meta.env.VITE_IMG_URL;

const store = ref(null);
const allproduct = ref(null);
const storeName = ref('');
const storeCompanyName = ref('');
const selectedCategory = ref(null);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   驗證規則
*/
const rules = computed(() => ({
  storeName: { required, maxLength: maxLength(100) },
  storeCompanyName: { maxLength: maxLength(100) },
}));

const v$ = useVuelidate(
  rules,
  { storeName, storeCompanyName },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
   初始化
*/
onMounted(() => {
  getStoreInfo();
  getSellerProduct();
});

/*
   載入賣場資訊
*/
const getStoreInfo = async () => {
  try {
    showLoading();
    const res = await getStore(authStore.userId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      store.value = data.returnData;
      storeName.value = data.returnData.storeName;
      storeCompanyName.value = data.returnData.storeCompanyName ?? '';
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   載入賣家頭貼
*/
const sellerImg = computed(() => {
  const headshot = authStore.userHeadshot;
  if (!headshot) return defaultImgurl;
  if (headshot.includes('googleusercontent.com')) return headshot;
  return `${baseUrl}/UserHeadShot/${headshot}`;
});

/*
   載入賣場所有商品
*/
const getSellerProduct = async () => {
  try {
    showLoading();
    const res = await getAllProduct({
      pageIndex: 0,
      pageSize: 100,
      sellerId: authStore.userId,
    });
    const { data } = res;
    if (data.codeStatus === 2000) {
      allproduct.value = data.returnData.products;
    } else if (data.codeStatus === 4001) {
      allproduct.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   讀取商品圖片，沒有就回傳預設
*/
const getProductsImg = (product) => {
  if (product.productsImgs && product.productsImgs.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImgs[0].productsImg}`;
  }
  return defaultImgurl;
};

/*
   從所有商品取出不重複的類別清單
*/
const allCategories = computed(() => {
  if (!allproduct.value) return [];
  return [...new Set(allproduct.value.map((x) => x.productCategoryName))];
});

/*
   依選取類別篩選商品
*/
const filterProducts = computed(() => {
  if (!allproduct.value) return [];
  if (!selectedCategory.value) return allproduct.value;
  return allproduct.value.filter((p) => p.productCategoryName === selectedCategory.value);
});

/*
   儲存賣場資訊
*/
const saveStore = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  try {
    showLoading();
    const res = await updateStore({
      storeId: store.value.storeId,
      storeName: storeName.value,
      storeUnifiedNumber: store.value.storeUnifiedNumber,
      storeCompanyName: storeCompanyName.value || null,
    });
    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('賣場資訊已更新');
      await getStoreInfo();
    } else {
      showToastError(data.message || '更新失敗，請稍後再試');
    }
  } catch (err) {
    console.log(err);
    showToastError('更新失敗，請稍後再試');
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <!--#region 整體容器 -->
  <div class="flex flex-col w-full p-6 gap-4" v-if="store">
    <!--#region 賣場頭貼 + 數據總覽卡片 -->
    <div class="bg-page-bg border border-border-soft rounded-card p-6">
      <div class="flex items-center gap-4 mb-5">
        <img
          :src="sellerImg"
          alt="賣家頭貼"
          class="w-16 h-16 rounded-full object-cover border border-border-soft shrink-0"
        />
        <div>
          <p class="text-base font-bold text-ink-900 m-0 mb-1">{{ store.storeName }}</p>
          <div class="flex items-center gap-2">
            <span class="text-sm font-medium text-brand-price">{{
              store.countAVGAllProductRate
            }}</span>
            <Rating
              :modelValue="store.countAVGAllProductRate"
              :stars="5"
              :readonly="true"
              :pt="{
                onIcon: { class: 'text-brand-price' },
                offIcon: { class: 'text-slate-300' },
              }"
            />
            <span class="text-xs text-ink-500">{{ store.allProductsRateCount }} 則評價</span>
          </div>
          <p class="text-xs text-ink-500 m-0 mt-1">
            加入時間：{{ formatDateOnly(store.createTime) }}
          </p>
        </div>
      </div>

      <!--#region 統計數字 -->
      <div class="grid grid-cols-3 gap-3">
        <div class="bg-page-bg-soft rounded-card p-3 border border-border-soft">
          <p class="text-xs text-ink-500 m-0 mb-1">上架商品</p>
          <p class="text-lg font-bold text-brand-price m-0">{{ store.allProductsCount }}</p>
        </div>
        <div class="bg-page-bg-soft rounded-card p-3 border border-border-soft">
          <p class="text-xs text-ink-500 m-0 mb-1">累計評價</p>
          <p class="text-lg font-bold text-ink-900 m-0">{{ store.allProductsRateCount }}</p>
        </div>
        <div class="bg-page-bg-soft rounded-card p-3 border border-border-soft">
          <p class="text-xs text-ink-500 m-0 mb-1">平均評分</p>
          <p class="text-lg font-bold text-brand-price m-0">{{ store.countAVGAllProductRate }}</p>
        </div>
      </div>
      <!-- #endregion -->
    </div>
    <!-- #endregion -->

    <!--#region 編輯賣場資訊卡片 -->
    <div class="bg-page-bg border border-border-soft rounded-card p-6">
      <p class="text-base font-bold text-ink-900 mb-4 m-0">編輯賣場資訊</p>

      <div class="flex flex-col gap-4">
        <!--#region 賣場名稱 -->
        <div class="flex items-start gap-4">
          <label class="text-sm text-ink-500 w-24 text-right shrink-0 pt-1.5"> 賣場名稱 </label>
          <div class="flex-1">
            <InputText
              v-model="storeName"
              placeholder="輸入賣場顯示名稱"
              :invalid="v$.storeName.$error"
              class="w-full"
            />
            <InValidErrorMessage :errorDto="v$.storeName.$errors" vaildChiName="賣場名稱" />
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 統一編號（唯讀） -->
        <div class="flex items-start gap-4">
          <label class="text-sm text-ink-500 w-24 text-right shrink-0 pt-1.5">統一編號</label>
          <div class="flex-1">
            <div class="flex items-center gap-2">
              <InputText :modelValue="store.storeUnifiedNumber" disabled class="w-full" />
              <span
                class="shrink-0 flex items-center gap-1 text-xs text-ink-300 border border-border-soft rounded-card px-2 py-1"
              >
                <i class="pi pi-lock text-xs"></i>
                不可修改
              </span>
            </div>
            <p class="text-xs text-ink-300 mt-1 m-0">如需修改請聯絡客服</p>
          </div>
        </div>
        <!-- #endregion -->

        <!--#region 公司名稱 -->
        <div class="flex items-start gap-4">
          <label class="text-sm text-ink-500 w-24 text-right shrink-0 pt-1.5">公司名稱</label>
          <div class="flex-1">
            <InputText
              v-model="storeCompanyName"
              placeholder="輸入公司名稱（選填）"
              :invalid="v$.storeCompanyName.$error"
              class="w-full"
            />
            <InValidErrorMessage :errorDto="v$.storeCompanyName.$errors" vaildChiName="公司名稱" />
          </div>
        </div>
        <!-- #endregion -->
      </div>

      <!--#region 按鈕區 -->
      <div class="flex justify-end gap-3 mt-6 pt-4 border-t border-border-soft">
        <button
          @click="
            storeName = store.storeName;
            storeCompanyName = store.storeCompanyName ?? '';
          "
          class="bg-transparent border border-[#D3D1C7] text-ink-900 hover:bg-page-bg-soft px-6 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
        >
          取消
        </button>
        <button
          @click="saveStore"
          class="bg-brand-500 hover:opacity-90 text-white px-8 py-2 rounded-card cursor-pointer text-sm font-medium transition-colors"
        >
          儲存變更
        </button>
      </div>
      <!-- #endregion -->
    </div>
    <!-- #endregion -->

    <!--#region 商品類別篩選 -->
    <div class="bg-page-bg border border-border-soft rounded-card p-4" v-if="allproduct">
      <div class="flex flex-wrap gap-2">
        <button
          @click="selectedCategory = null"
          class="px-4 py-2 text-sm rounded-full border cursor-pointer transition-all"
          :class="
            !selectedCategory
              ? 'border-selection bg-selection-50 text-ink-900'
              : 'border-border-soft bg-page-bg text-ink-500 hover:bg-surface-muted'
          "
        >
          全部
        </button>
        <button
          v-for="cat in allCategories"
          :key="cat"
          @click="selectedCategory = selectedCategory === cat ? null : cat"
          class="px-4 py-2 text-sm rounded-full border cursor-pointer transition-all"
          :class="
            selectedCategory === cat
              ? 'border-selection bg-selection-50 text-ink-900'
              : 'border-border-soft bg-page-bg text-ink-500 hover:bg-surface-muted'
          "
        >
          {{ cat }}
        </button>
      </div>
    </div>
    <!-- #endregion -->

    <!--#region 商品列表標題 -->
    <div class="flex items-center justify-between" v-if="allproduct">
      <h2 class="text-base font-bold text-ink-900 m-0">我的商品</h2>
      <span class="text-sm text-ink-500">共 {{ filterProducts.length }} 件</span>
    </div>
    <!-- #endregion -->

    <!--#region 商品列表 -->
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-[14px]" v-if="allproduct">
      <!--#region 商品卡片 -->

      <div
        v-for="product in filterProducts"
        :key="product.productsId"
        @click="
          router.push({
            name: 'product-detail',
            params: { id: product.productsId },
          })
        "
        class="bg-page-bg border border-border-soft rounded-card overflow-hidden cursor-pointer transition-colors hover:border-brand-500"
      >
        <!-- 商品圖片 -->
        <div class="aspect-square bg-surface-muted overflow-hidden">
          <img :src="getProductsImg(product)" class="w-full h-full object-cover" />
        </div>

        <!-- 商品資訊 -->
        <div class="p-4">
          <p class="text-sm font-bold text-ink-900 mb-2 line-clamp-2 min-h-[40px]">
            {{ product.productsName }}
          </p>

          <p class="text-base font-bold text-brand-price mb-2">NT$ {{ product.productsPrice }}</p>

          <p class="text-xs text-ink-500">
            {{ product.productCategoryName }}
          </p>
        </div>
      </div>

      <!-- #endregion -->

      <!--#region 新增商品卡片 -->
      <div
        @click="router.push({ name: 'add-product' })"
        class="border border-dashed border-ink-300 rounded-card flex flex-col items-center justify-center cursor-pointer hover:bg-surface-muted transition-colors gap-2 min-h-[180px]"
      >
        <i class="pi pi-plus text-ink-300 text-xl"></i>
        <span class="text-sm text-ink-500">新增商品</span>
      </div>
      <!-- #endregion -->
    </div>
    <!-- #endregion -->
  </div>
  <!-- #endregion -->
</template>
