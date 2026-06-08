<script setup>
import { userBuyProduct } from '@/api//orderService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   router : 改變路由
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   authStore : pinia
   boughtQuantity : 購買數量
   address : 地址
   items : 存在 pinia 的購物車選擇的商品
*/
const router = useRouter();
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const address = ref();
const orderStore = useOrderStore();
const items = orderStore.selectedItems;

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
  // authstore 有的情況就不驗證
  address: authStore.userAddress ? {} : { required, maxLength: maxLength(200) },
}));

/*
   加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
   autoDirty : 一碰到欄位就開始驗證
   lazy : 元件載入時不會馬上驗證 , 等使用者開始互動才會
   scope : 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
*/
const v$ = useVuelidate(rules, { address }, { $autoDirty: true, $lazy: true, $scope: false });

/*
  讀取商品的第一張圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductImg = (item) => {
  const imgs = item.productsImgs;
  if (imgs && imgs.length > 0 && imgs[0].productsImg) {
    return `${baseUrl}/ProductsImg/${imgs[0].productsImg}`;
  }
  return defaultImgurl;
};

/*
  計算總金額
*/
const totalPrice = computed(() => {
  return (items ?? []).reduce((sum, item) => sum + item.productsPrice * item.boughtQuantity, 0);
});

/*
  使用者購買
*/
const userBuy = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  const bought = {
    products: items.map((item) => ({
      productsId: item.productsId,
      boughtQuantity: item.boughtQuantity,
    })),
    shippingAddress: authStore.userAddress ?? address.value,
    boughtTime: new Date().toLocaleDateString('en-CA'),
  };

  try {
    showLoading();
    const res = await userBuyProduct(bought);

    const { data } = res;
    if (data.codeStatus === 2000) {
      try {
        const ecpayData = data.returnData.formData;
        const actionUrl = data.returnData.actionUrl;

        // 建立一個隱藏的 Form
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = actionUrl;

        // 將所有綠界參數塞入 input 中
        for (const key in ecpayData) {
          const input = document.createElement('input');
          input.type = 'hidden';
          input.name = key;
          input.value = ecpayData[key];
          form.appendChild(input);
        }

        // 把表單加到 body 並送出 (這就會觸發頁面跳轉)
        document.body.appendChild(form);
        form.submit();
      } catch (error) {
        console.error('購買失敗 :', error.response);
      }
    }
    if (data.codeStatus === 4000) {
      showToastError(getError400Message(data.error400));
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full items-center mt-20" v-if="items">
    <!-- #region  正在購買的商品資訊 -->
    <div class="w-300 rounded-lg shadow-sm border border-gray-200 p-6 mb-4">
      <h2 class="text-base font-bold mb-4 text-gray-700">正在購買</h2>
      <div
        v-for="item in items"
        :key="item.productsId"
        class="w-full rounded-lg shadow-sm border border-gray-200 p-6 mb-4"
      >
        <div class="flex flex-row gap-5 items-center">
          <img
            :src="getProductImg(item)"
            alt="商品圖片"
            class="w-24 h-24 object-cover rounded-md border border-gray-100"
          />
          <div class="flex flex-col gap-2">
            <span class="font-semibold text-base">{{ item.productsName }}</span>
            <span class="text-gray-500 text-sm">單價：NT$ {{ item.productsPrice }}</span>
          </div>
        </div>

        <!-- 小計 -->
        <div class="flex justify-between items-center border-t border-gray-100 pt-4">
          <span class="font-bold text-lg">
            小計 NT$ {{ (item.productsPrice * item.boughtQuantity).toLocaleString() }}
          </span>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    <div class="w-300 rounded-lg shadow-sm border border-gray-200 p-6 flex flex-col gap-5">
      <!-- #region  購買資訊填寫-->
      <h2 class="text-base font-bold text-gray-700">填寫購買資訊</h2>

      <div class="flex flex-col gap-1">
        <label class="text-sm mb-3">收件地址</label>
        <div v-if="authStore.userAddress">
          <InputText
            v-model="authStore.userAddress"
            class="text-sm rounded-md p-3 text-gray-700"
            readonly
          >
          </InputText>
        </div>
        <div v-else>
          <InputGroup>
            <InputText v-model="address" placeholder="收件地址" :invalid="v$.address.$error" />
          </InputGroup>
          <InValidErrorMessage :errorDto="v$.address.$errors" vaildChiName="收件地址" />
        </div>
      </div>
      <span>總金額 : {{ totalPrice.toLocaleString() }}</span>
      <!-- #endregion -->
      <!-- #region  購買按鈕-->
      <div class="flex gap-3">
        <button
          class="flex-1 border border-gray-300 text-gray-600 p-3 rounded-2xl cursor-pointer"
          @click="router.back()"
        >
          返回
        </button>
        <button
          class="flex-1 bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
          @click="userBuy"
        >
          前往付款
        </button>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
