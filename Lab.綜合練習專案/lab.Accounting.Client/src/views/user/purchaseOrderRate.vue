<script setup>
import defaultImgurl from '@/img/預設圖片.jpg';
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';

/*
   變數名稱代表意義
   router : 改變路由
   route : 拿道路由資訊
   baseUrl : 環境變數裡的圖片基底位址
   authStore : pinia
   rateOrder : 評價的訂單
   rate : 評分
   comment : 評論
*/
const router = useRouter();
const route = useRoute();
const baseUrl = import.meta.env.VITE_IMG_URL;
const rateOrder = ref();
const rate = ref();
const comment = ref();
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
  rate: { required },
  comment: { maxLength: maxLength(300) },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(rules, { rate, comment }, { $autoDirty: true, $lazy: true, $scope: false });

/*
   初始化時拿到訂單
*/
onMounted(() => {
  getOrder(route.params.id);
});

/*
  讀取商品的第一張圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductImg = (item) => {
  if (item.productsImg) {
    return `${baseUrl}/ProductsImg/${item.productsImg}`;
  }
  return defaultImgurl;
};

/*
  查看這筆訂單
*/
const getOrder = async (id) => {
  try {
    showLoading();
    const res = await getUserOneOrder(id);

    const { data } = res;
    if (data.codeStatus === 2000) {
      rateOrder.value = data.returnData;
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

/*
  創建評價
*/
const createRate = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    showLoading();

    const request = {
      orderId: rateOrder.value.orderId,
      productsId: rateOrder.value.productsId,
      rating: rate.value,
      comment: comment.value,
    };
    const res = await createProductRate(request);

    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess('評價成功!');
      router.push({ name: 'purchase-orders' });
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
  <div class="flex flex-col w-full items-center mt-20" v-if="rateOrder">
    <!--#region 正在評價的訂單資訊 -->
    <div class="w-300 rounded-lg shadow-sm border border-gray-200 p-6 mb-4">
      <h2 class="text-base font-bold mb-4 text-gray-700">正在評價</h2>
      <div class="w-full rounded-lg shadow-sm border border-gray-200 p-6 mb-4">
        <div class="flex flex-row gap-5 items-center">
          <!--#region 資訊細項 -->
          <img
            :src="getProductImg(rateOrder)"
            alt="商品圖片"
            class="w-24 h-24 object-cover rounded-md border border-gray-100"
          />
          <div class="flex flex-col gap-2">
            <span class="font-semibold text-base">訂單編號 : {{ rateOrder.orderNumber }}</span>
            <span class="font-semibold text-base">{{ rateOrder.productsName }}</span>
            <span class="text-gray-500 text-sm">購買數量 : {{ rateOrder.boughtQuantity }}</span>
            <span class="text-gray-500 text-sm">訂單金額：NT$ {{ rateOrder.accountPrice }}</span>
          </div>
          <!-- #endregion -->
        </div>
      </div>
    </div>
    <!-- #endregion -->
    <!--#region 購買資訊填寫 -->
    <div class="w-300 rounded-lg shadow-sm border border-gray-200 p-6 flex flex-col gap-5">
      <h2 class="text-base font-bold text-gray-700">填寫評價</h2>

      <div>
        <InputGroup class="mb-5">
          <label class="me-3">評分</label>
          <Rating v-model="rate" :stars="5"></Rating>
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.rate.$errors" vaildChiName="評分" />
        <InputGroup>
          <label class="me-3">評論</label>
          <InputText v-model="comment"></InputText>
        </InputGroup>
        <InValidErrorMessage :errorDto="v$.comment.$errors" vaildChiName="評論" />
      </div>
      <!-- #endregion -->
      <!--#region 按鈕區 -->
      <div class="flex gap-3">
        <button
          class="flex-1 border border-gray-300 text-gray-600 p-3 rounded-2xl cursor-pointer"
          @click="router.back()"
        >
          返回
        </button>
        <button
          class="flex-1 bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
          @click="createRate"
        >
          送出
        </button>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
