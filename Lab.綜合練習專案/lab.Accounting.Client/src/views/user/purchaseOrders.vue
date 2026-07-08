<script setup>
import { getUserOrder, userRetryBuyProduct } from '@/api//orderService';
import { getOrderRate } from '@/api/productsService';
import { shippingEnum } from '@/common/enum';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   allOrders : 所有訂單
   baseUrl : 環境變數裡的圖片基底位址
   currentGroup : 顯示目前頁面狀態
   router : 控制路由
   route : 抓取路由參數
   selectProducts : 選擇的商品
   ratedOrders : 所有評論紀錄,用來判斷是否評論過
*/
const allOrders = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
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
    currentGroup.value = tabGroups.find((t) => t.label === '待出貨');
  }
  // 失敗則回到代付款
  if (route.query.status === 'fail') {
    showToastError('付款失敗，請重新嘗試!');
    currentGroup.value = tabGroups.find((t) => t.label === '待付款');
  }
});

/*
   頁面分類
*/
const tabGroups = [
  { label: '待付款', statuses: [shippingEnum.PendingPayment.value] },
  { label: '待出貨', statuses: [shippingEnum.PendingShipment.value] },
  { label: '待收貨', statuses: [shippingEnum.InTransit.value, shippingEnum.Arrived.value] },
  { label: '已完成', statuses: [shippingEnum.Completed.value] },
  { label: '已取消', statuses: [shippingEnum.Cancelled.value] },
];

/*
   顯示目前頁面狀態
*/
const currentGroup = ref(tabGroups[0]);
const filtTable = computed(() => {
  if (!allOrders.value) return [];
  return allOrders.value.filter((order) =>
    currentGroup.value.statuses.includes(order.shippingStatus),
  );
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
    <div class="flex flex-col items-center">
      <div class="mt-8 w-300 rounded-card border border-border-soft bg-page-bg overflow-hidden">
        <!--#region Tab -->
        <div class="flex border-b border-border-soft">
          <button
            v-for="tab in tabGroups"
            :key="tab.value"
            @click="currentGroup = tab"
            class="flex-1 py-3 text-center text-sm transition-colors cursor-pointer"
            :class="
              currentGroup.label === tab.label
                ? 'border-b-2 border-brand-500 text-brand-500 font-medium'
                : 'text-ink-500 hover:text-ink-900'
            "
          >
            {{ tab.label }}
          </button>
        </div>
        <!-- #endregion -->

        <!--#region 訂單列表 / 沒有訂單時顯示 -->
        <div
          v-if="filtTable.length === 0"
          class="flex justify-center items-center h-40 text-ink-500"
        >
          目前沒有訂單
        </div>
        <!-- #endregion -->

        <!--#region 有訂單時顯示 -->
        <div
          v-for="order in filtTable"
          :key="order.orderId"
          class="border-b border-border-soft hover:bg-surface-muted transition-colors flex flex-row items-center px-6 py-4 cursor-pointer gap-5"
          @click="router.push({ name: 'purchase-orders-details', params: { id: order.orderId } })"
        >
          <div v-if="currentGroup.label === '待付款'" @click.stop>
            <input
              type="checkbox"
              v-model="selectProducts"
              :value="order.orderId"
              class="w-5 h-5"
            />
          </div>
          <img
            :src="getProductsImg(order)"
            alt="商品圖片"
            class="w-16 h-16 object-cover rounded-card border border-border-soft"
          />
          <div class="flex-1 grid grid-cols-3 gap-3 items-center">
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">商品名稱</p>
              <p class="text-sm text-ink-900 m-0 truncate">{{ order.productsName }}</p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">購買數量</p>
              <p class="text-sm text-ink-900 m-0">
                {{ order.boughtQuantity }} 件 (單價 ${{ order.unitPrice }})
              </p>
            </div>
            <div>
              <p class="text-xs text-ink-500 m-0 mb-1">訂單金額</p>
              <p class="text-sm font-medium text-brand-price m-0">$ {{ order.accountAmount }}</p>
            </div>
          </div>
          <div class="flex flex-col items-end gap-2">
            <div
              v-if="order.shippingStatus === shippingEnum.InTransit.value"
              class="text-xs font-medium px-2.5 py-1 rounded-full bg-orange-100 text-orange-700"
            >
              運送中
            </div>

            <div
              v-else-if="order.shippingStatus === shippingEnum.Arrived.value"
              class="text-xs font-medium px-2.5 py-1 rounded-full bg-green-100 text-green-700"
            >
              已抵達門市
            </div>

            <div @click.stop>
              <div v-if="currentGroup.label === '已完成'">
                <span v-if="ratedOrders.has(order.orderId)" class="text-sm text-ink-400"
                  >已評價</span
                >
                <button
                  v-else
                  class="bg-brand-500 hover:opacity-90 text-white text-sm font-medium px-4 py-1.5 rounded-lg shadow-sm transition-all"
                  @click="router.push({ name: 'purchaseOrderRate', params: { id: order.orderId } })"
                >
                  去評價
                </button>
              </div>

              <div v-else-if="currentGroup.label === '已取消'">
                <span class="text-xs font-medium px-3 py-1 bg-gray-100 text-gray-500 rounded-full">
                  已取消
                </span>
              </div>
            </div>
          </div>
        </div>
        <!-- #endregion -->
        <!--#region 按鈕區 -->
        <div v-if="selectProducts.length > 0" class="flex justify-end px-6 py-4">
          <button
            class="bg-brand-500 hover:opacity-90 text-white text-sm font-medium px-5 py-2 rounded-card cursor-pointer"
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
