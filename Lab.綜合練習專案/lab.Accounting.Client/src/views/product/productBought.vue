<script setup>
import { getProduct } from '@/api/productsService';
import { userBuyProduct } from '@/api//orderService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   route : 獲取路由資訊
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   authStore : pinia
   boughtQuantity : 購買數量
   address : 地址
*/
const route = useRoute();
const product = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const boughtQuantity = ref();
const address = ref();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  address: { required, maxLength: maxLength(200) },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(rules, { address }, { $autoDirty: true, $lazy: true, $scope: false });

/*
   查看商品細節資訊
*/
const getProductDetail = async (id) => {
  try {
    showLoading();
    var res = await getProduct(id);
    const { data } = res;
    if (data.codeStatus === 2000) {
      product.value = data.returnData;
    } else {
      router.push({ name: 'products' });
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
onMounted(() => {
  getProductDetail(route.params.id);
});

/*
  讀取商品的第一張圖片 , 判斷是否有圖片沒有就回傳預設
*/
const productImg = computed(() => {
  const imgs = product.value?.productsImgs;
  if (imgs && imgs.length > 0 && imgs[0].productsImg) {
    return `${baseUrl}/ProductsImg/${imgs[0].productsImg}`;
  }
  return defaultImgurl;
});

/*
  使用者購買跟評分
*/
const userBuy = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  const bought = {
    productsId: Number(route.params.id),
    boughtQuantity: boughtQuantity.value,
    shippingAddress: authStore.userAddress ?? address.value,
    createTime: new Date().toLocaleDateString('en-CA'),
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
      showToastError('庫存不足!');
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full items-center mt-20" v-if="product">
    <!-- 正在購買的商品資訊 -->
    <div class="w-150 rounded-lg shadow-sm border border-gray-200 p-6 mb-4">
      <h2 class="text-base font-bold mb-4 text-gray-700">正在購買</h2>
      <div class="flex flex-row gap-5 items-center">
        <img
          :src="productImg"
          alt="商品圖片"
          class="w-24 h-24 object-cover rounded-md border border-gray-100"
        />
        <div class="flex flex-col gap-2">
          <span class="font-semibold text-base">{{ product.productsName }}</span>
          <span class="text-gray-500 text-sm">單價：NT$ {{ product.productsPrice }}</span>
          <span class="text-gray-500 text-sm">剩餘庫存：{{ product.productsStock }}</span>
        </div>
      </div>
    </div>

    <!-- 購買資訊填寫 -->
    <div class="w-150 rounded-lg shadow-sm border border-gray-200 p-6 flex flex-col gap-5">
      <h2 class="text-base font-bold text-gray-700">填寫購買資訊</h2>

      <!-- 收件地址（從 authStore 拿） -->
      <div class="flex flex-col gap-1">
        <label class="text-sm text-gray-500 mb-3">收件地址</label>
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

      <!-- 購買數量 -->
      <div class="flex flex-col gap-1">
        <label class="text-sm text-gray-500">購買數量</label>
        <InputGroup>
          <InputNumber
            v-model="boughtQuantity"
            placeholder="請輸入數量"
            :min="1"
            :max="product.productsStock"
          />
        </InputGroup>
      </div>

      <!-- 小計 -->
      <div class="flex justify-between items-center border-t border-gray-100 pt-4">
        <span class="text-sm text-gray-500">小計</span>
        <span class="font-bold text-lg">
          NT$ {{ (product.productsPrice * (boughtQuantity ?? 0)).toLocaleString() }}
        </span>
      </div>

      <!-- 按鈕 -->
      <div class="flex gap-3">
        <button
          class="flex-1 border border-gray-300 text-gray-600 p-3 rounded-2xl cursor-pointer"
          @click="router.back()"
        >
          返回
        </button>
        <button
          class="flex-1 bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
          @click="userBuy()"
        >
          前往付款
        </button>
      </div>
    </div>
  </div>
</template>
