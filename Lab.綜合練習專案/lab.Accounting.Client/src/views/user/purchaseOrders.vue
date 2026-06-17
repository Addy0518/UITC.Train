<script setup>
import { getUserOrder, userRetryBuyProduct } from '@/api//orderService';
import { getOrderRate } from '@/api/productsService';
import { shippingEnum } from '@/common/enum';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   allOrders : 所有訂單
   baseUrl : 環境變數裡的圖片基底位址
   tableNow : 顯示目前頁面狀態
   router : 控制路由
   route : 抓取路由參數
   selectProducts : 選擇的商品
   ratedOrders : 所有評論紀錄,用來判斷是否評論過
*/
const allOrders = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const tableNow = ref(shippingEnum.PendingShipment.value);
const router = useRouter();
const route = useRoute();
const selectProducts = ref([]);
const ratedOrders = ref(new Set());

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時拿到全部訂單 , 跟交易回來時觸發的動作
*/
onMounted(() => {
  getUserAllOrder();

  // 交易成功跳轉回來接下來的動作
  if (route.query.status === 'success' && route.query.orderNo) {
    showToastSuccess('付款成功!');
    tableNow.value = shippingEnum.PendingShipment.value;
  }
  // 失敗則回到代付款
  if (route.query.status === 'fail') {
    showToastError('付款失敗，請重新嘗試!');
    tableNow.value = shippingEnum.PendingPayment.value;
  }
});

/*
   顯示目前頁面狀態
*/
const filtTable = computed(() => {
  if (!allOrders.value) return [];
  return allOrders.value.filter((order) => order.shippingStatus == tableNow.value);
});

/*
  查看訂單評價來比對是否評價過
*/
const existOrderRate = async (id) => {
  try {
    showLoading();
    const res = await getOrderRate(id);
    const { data } = res;

    if (data.codeStatus === 2000 && data.returnData) {
      ratedOrders.value.add(id);
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  查看全部訂單
*/
const getUserAllOrder = async () => {
  try {
    showLoading();
    const res = await getUserOrder();
    const { data } = res;
    if (data.codeStatus === 2000) {
      allOrders.value = data.returnData;
      for (let order of allOrders.value) {
        await existOrderRate(order.orderId);
      }
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
  if (product.productsImg && product.productsImg.length > 0) {
    return `${baseUrl}/ProductsImg/${product.productsImg}`;
  }
  return defaultImgurl;
};

/*
  重新付款
*/
const retryPayment = async () => {
  try {
    showLoading();
    const res = await userRetryBuyProduct(selectProducts.value);
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
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <!--#region Tab -->
        <div class="flex border-b border-gray-200">
          <button
            v-for="tab in shippingEnum"
            :key="tab.value"
            @click="tableNow = tab.value"
            class="flex-1 py-3 text-center text-sm transition-colors cursor-pointer"
            :class="
              tableNow === tab.value
                ? 'border-b-2 border-orange-500 text-orange-500 font-medium'
                : 'text-gray-500 hover:text-gray-700'
            "
          >
            {{ tab.description }}
          </button>
        </div>
        <!-- #endregion -->

        <!--#region 訂單列表 / 沒有訂單時顯示 -->
        <div
          v-if="filtTable.length === 0"
          class="flex justify-center items-center h-40 text-gray-400"
        >
          目前沒有訂單
        </div>
        <!-- #endregion -->

        <!--#region 有訂單時顯示 -->
        <div v-for="order in filtTable">
          <div
            class="hover:shadow-xl hover:bg-gray-50 h-80 flex flex-row ps-10 cursor-pointer items-center"
            @click="router.push({ name: 'purchase-orders-details', params: { id: order.orderId } })"
          >
            <div v-if="tableNow === shippingEnum.PendingPayment.value">
              <input
                type="checkbox"
                v-model="selectProducts"
                :value="order.orderId"
                @click.stop
                class="w-5 h-5 me-2"
              />
            </div>
            <img :src="getProductsImg(order)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
            <span class="mt-3 ms-5 me-5">商品名稱 : {{ order.productsName }}</span>
            <span class="mt-3 ms-5 me-5">購買價格 : {{ order.unitPrice }}</span>
            <span class="mt-3 ms-5 me-5">購買數量 : {{ order.boughtQuantity }}</span>
            <span class="mt-3 ms-5 me-5">訂單金額 : ${{ order.accountAmount }}</span>
            <div v-if="tableNow === shippingEnum.Arrived.value">
              <!-- 用 click.stop 防止冒泡 -->
              <div v-if="ratedOrders.has(order.orderId)">
                <span class="text-sm font-medium px-5 py-2 text-red-500">已評價</span>
              </div>
              <div v-else>
                <button
                  class="bg-black text-white text-sm font-medium px-5 py-2 rounded-lg cursor-pointer"
                  @click.stop="
                    router.push({ name: 'purchaseOrderRate', params: { id: order.orderId } })
                  "
                >
                  去評價
                </button>
              </div>
            </div>
          </div>
        </div>
        <!-- #endregion -->
        <!--#region 按鈕區 -->
        <div v-if="selectProducts.length > 0" class="flex justify-end mt-3">
          <button
            class="bg-black text-white text-sm font-medium px-5 py-2 rounded-lg cursor-pointer"
            @click="retryPayment"
          >
            重新付款
          </button>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
