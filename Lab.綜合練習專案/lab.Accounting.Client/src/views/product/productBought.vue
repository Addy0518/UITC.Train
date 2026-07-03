<script setup>
import { userBuyProduct } from '@/api//orderService';
import { getUserCoupon } from '@/api/couponService';
import { saveCvsReceiver } from '@/api/logisticsService';
import { couponTypeEnum } from '@/common/enum';
import defaultImgurl from '@/img/預設圖片.jpg';
import { computed } from 'vue';
import { useRoute } from 'vue-router';

/*
   變數名稱代表意義
   router : 改變路由
   route : 路由資訊
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   authStore : pinia
   name : 收件人姓名
   phone : 收件人電話
   address : 收件人地址
   items : 存在 pinia 的購物車選擇的商品
   allCoupons : 用戶的所有優惠卷
   coupon : 選擇的優惠卷
   logisticsTemp : 物流暫存單
   shippingType : 物流方式 ( 宅配或超商 )
   cvsSessionKey : 超商的物流單編號
   homeSessionKey : 宅配的物流單編號
*/
const router = useRouter();
const route = useRoute();
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const name = ref(authStore.userName ?? '');
const phone = ref(authStore.userPhone ?? '');
const address = ref(authStore.userAddress ?? '');
const orderStore = useOrderStore();
const items = orderStore.selectedItems;
const allCoupons = ref();
const coupon = ref(null);
const logisticsTemp = ref(null);
const shippingType = ref('Home');
const cvsSessionKey = ref(route.query.sessionKey || null);
const homeSessionKey = ref();
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化
*/
onMounted(() => {
  getMyCoupon();

  if (cvsSessionKey.value) {
    getLogistics();
    shippingType.value = 'CVS';
  } else if (shippingType.value === 'Home' && !homeSessionKey.value) {
    homeSessionKey.value = 'GN' + crypto.randomUUID().replace(/-/g, '').substring(0, 11);
  }
});

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  // authstore 有的情況就不驗證
  name: authStore.userName ? {} : { required, maxLength: maxLength(50) },
  phone: authStore.userPhone ? {} : { required, vaildCellPhone, maxLength: maxLength(20) },
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
   取得門市資訊 ( 正式區才能選門市 , 這裡測試直接給一個固定的門市 )
*/
const goToCvsMap = async () => {
  try {
    showLoading();
    if (!cvsSessionKey.value) {
      cvsSessionKey.value = 'GN' + crypto.randomUUID().replace(/-/g, '').substring(0, 11);
    }
    const res = await getCvsMapUrl({
      sessionKey: cvsSessionKey.value,
      logisticsSubType: 'UNIMARTC2C',
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      // 建立一個隱藏的 form 表單
      const form = document.createElement('form');
      form.method = 'POST';
      form.action = res.data.returnData.ActionUrl;

      // 將後端給的參數全部轉換為 input 塞進去
      Object.keys(data.returnData).forEach((key) => {
        if (key !== 'ActionUrl') {
          const input = document.createElement('input');
          input.type = 'hidden';
          input.name = key;
          input.value = res.data.returnData[key];
          form.appendChild(input);
        }
      });

      document.body.appendChild(form);
      form.submit();
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   存收件人資訊到物流暫存單
*/
const InsertHomeLogisticsTemp = async () => {
  try {
    showLoading();
    if (!homeSessionKey.value) {
      homeSessionKey.value = 'GN' + crypto.randomUUID().replace(/-/g, '').substring(0, 11);
    }
    const request = {
      sessionKey: homeSessionKey.value,
      logisticsType: 'Home',
      logisticsSubType: 'TCAT',
      receiverName: name.value,
      receiverPhone: phone.value,
      receiverAddress: address.value,
    };
    const res = await saveHomeLogisticsTemp(request);
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   取得物流暫存單 ( 門市資訊 )
*/
const getLogistics = async () => {
  try {
    showLoading();

    const res = await getLogisticsTemp(route.query.sessionKey);
    const { data } = res;

    if (data.codeStatus === 2000) {
      logisticsTemp.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  計算總金額
*/
const totalPrice = computed(() => {
  return (items ?? []).reduce((sum, item) => sum + item.productsPrice * item.boughtQuantity, 0);
});

/*
  判斷優惠卷是否可用
*/
const isCouponCanUse = (coupon) => {
  if (coupon.usedTime) return false;
  if (!coupon.isActive) return false;
  if (new Date(coupon.StartTime) > new Date() || new Date(coupon.EndTime) < new Date())
    return false;
  if (coupon.minimunSpend > 0 && totalPrice.value < coupon.minimunSpend) return false;
  return true;
};

/*
  前端及時判斷優惠價格 ( 僅供前端瀏覽 , 實際以後端為主 )
*/
const discountAmount = computed(() => {
  const selected = allCoupons.value?.find((c) => c.couponId === coupon.value);
  if (!selected) return 0;
  if (selected.type === couponTypeEnum.百分比折扣.value) {
    return Math.round(totalPrice.value * (1 - selected.discount / 100));
  } else {
    // Math min 取出兩個數中的最小值 , 這樣當折扣大於金額時就直接回傳總金額 , 就會變成免費而不是負數 , 變成我們還要給他錢
    return Math.min(selected.discount, totalPrice.value);
  }
});

/*
  最後計算扣除優惠的價格
*/
const finalPrice = computed(() => totalPrice.value - discountAmount.value);

/*
  點選的優惠卷
*/
const selectCoupon = (c) => {
  if (!isCouponCanUse(c)) return;
  // 再點一次可以取消
  coupon.value = coupon.value === c.couponId ? null : c.couponId;
};

/*
   查看用戶優惠卷
*/
const getMyCoupon = async () => {
  try {
    showLoading();

    const res = await getUserCoupon();
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCoupons.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
  使用者購買
*/
const userBuy = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  if (shippingType.value === 'CVS' && !cvsSessionKey.value) {
    showToastError('請先選擇取貨門市');
    return;
  }
  // 存收件人資料到暫存表
  if (shippingType.value == 'Home') {
    await InsertHomeLogisticsTemp();
  } else if (shippingType.value == 'CVS') {
    const request = {
      sessionKey: cvsSessionKey.value,
      receiverName: name.value,
      receiverPhone: phone.value,
    };
    await saveCvsReceiver(request);
  }
  const bought = {
    products: items.map((item) => ({
      productsId: item.productsId,
      boughtQuantity: item.boughtQuantity,
    })),
    sessionKey: shippingType.value === 'CVS' ? cvsSessionKey.value : homeSessionKey.value,
    couponId: coupon.value,
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
    <div class="w-300 rounded-card border border-border-soft p-6 mb-4 bg-page-bg">
      <h2 class="text-base font-bold mb-4 text-ink-900">正在購買</h2>
      <div
        v-for="item in items"
        :key="item.productsId"
        class="w-full rounded-card border border-border-soft p-6 mb-4"
      >
        <div class="flex flex-row gap-5 items-center">
          <img
            :src="getProductImg(item)"
            alt="商品圖片"
            class="w-24 h-24 object-cover rounded-card border border-border-soft"
          />
          <div class="flex flex-col gap-2">
            <span class="font-semibold text-base text-ink-900">{{ item.productsName }}</span>
            <span class="text-ink-500 text-sm">單價：NT$ {{ item.productsPrice }}</span>
          </div>
        </div>

        <!-- 小計 -->
        <div class="flex justify-between items-center border-t border-border-soft pt-4">
          <span class="font-bold text-lg text-brand-price">
            小計 NT$ {{ (item.productsPrice * item.boughtQuantity).toLocaleString() }}
          </span>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    <!-- #region  優惠券選擇 -->
    <div class="w-300 rounded-card border border-border-soft p-6 mb-4 bg-page-bg">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-base font-bold text-ink-900">選擇優惠券</h2>
        <span class="text-xs text-ink-500">{{ allCoupons?.length ?? 0 }} 張可用</span>
      </div>

      <div
        v-if="!allCoupons || allCoupons.length === 0"
        class="text-sm text-ink-500 py-4 text-center"
      >
        目前沒有可用的優惠券
      </div>

      <div class="flex flex-col gap-2">
        <!-- 不使用優惠券 -->
        <div
          class="flex items-center gap-3 p-3 border rounded-card cursor-pointer"
          :class="coupon === null ? 'border-selection bg-selection-50' : 'border-border-soft'"
          @click="coupon = null"
        >
          <div
            class="w-4.5 h-4.5 rounded-full border"
            :class="coupon === null ? 'border-selection bg-selection' : 'border-ink-300'"
          ></div>
          <span class="text-sm text-ink-500">不使用優惠券</span>
        </div>

        <div
          v-for="c in allCoupons"
          :key="c.couponId"
          class="flex items-center gap-3 p-3 border rounded-card"
          :class="[
            isCouponCanUse(c) ? 'cursor-pointer' : 'cursor-not-allowed opacity-45',
            coupon === c.couponId && isCouponCanUse(c)
              ? 'border-selection bg-selection-50'
              : 'border-border-soft',
          ]"
          @click="selectCoupon(c)"
        >
          <div
            class="w-4.5 h-4.5 rounded-full border"
            :class="
              coupon === c.couponId && isCouponCanUse(c)
                ? 'border-selection bg-selection'
                : 'border-ink-300'
            "
          ></div>

          <div
            class="bg-surface-muted border border-dashed border-ink-300 rounded-card px-2.5 py-1 font-mono text-xs text-ink-500"
          >
            {{ c.code }}
          </div>
          <div class="flex-1">
            <p class="text-sm m-0 text-ink-900">{{ c.name }}</p>
            <p class="text-xs text-ink-500 m-0 mt-0.5">
              {{ c.minimunSpend > 0 ? `滿 $${c.minimunSpend} 可用` : '無門檻' }}
              ·
              {{
                c.usedTime
                  ? '已使用'
                  : totalPrice < c.minimunSpend
                    ? '未達門檻'
                    : `${formatDateTimeString(c.endTime)} 到期`
              }}
            </p>
          </div>
          <p class="text-sm font-medium text-brand-price m-0">
            {{
              c.type === couponTypeEnum.百分比折扣.value ? `${c.discount} 折` : `$${c.discount} 元`
            }}
          </p>
        </div>
      </div>
    </div>
    <!-- #endregion -->

    <div class="w-300 rounded-card border border-border-soft p-6 flex flex-col gap-5 bg-page-bg">
      <!-- #region  購買資訊填寫-->
      <h2 class="text-base font-bold text-ink-900">填寫購買資訊</h2>
      <div class="flex flex-col gap-1">
        <label class="text-sm mb-3 text-ink-900">配送方式</label>
        <div class="flex flex-col gap-1">
          <label class="text-sm mb-3 text-ink-900">收件人姓名</label>

          <InputGroup>
            <InputText v-model="name" placeholder="收件人姓名" :invalid="v$.name.$error" />
          </InputGroup>
          <InValidErrorMessage :errorDto="v$.name.$errors" vaildChiName="收件人姓名" />
        </div>
        <div class="flex flex-col gap-1">
          <label class="text-sm mb-3 text-ink-900">收件人電話</label>

          <InputGroup>
            <InputText v-model="phone" placeholder="收件人電話" :invalid="v$.phone.$error" />
          </InputGroup>
          <InValidErrorMessage :errorDto="v$.phone.$errors" vaildChiName="收件人電話" />
        </div>
        <div class="flex gap-4 mb-4 mt-10">
          <button
            @click="shippingType = 'Home'"
            :class="[
              shippingType === 'Home' ? 'border-selection bg-selection-50' : 'border-border-soft',
            ]"
            class="flex-1 border p-3 rounded-card"
          >
            宅配到府
          </button>
          <button
            @click="shippingType = 'CVS'"
            :class="[
              shippingType === 'CVS' ? 'border-selection bg-selection-50' : 'border-border-soft',
            ]"
            class="flex-1 border p-3 rounded-card"
          >
            超商取貨
          </button>
        </div>

        <div v-if="shippingType === 'CVS'" class="flex flex-col gap-2">
          <div
            @click="goToCvsMap"
            class="cursor-pointer border border-dashed border-selection text-selection p-4 rounded-card text-center hover:bg-selection-50 transition-colors"
          >
            + 點擊選擇超商門市
          </div>

          <div
            v-if="logisticsTemp"
            class="border border-border-soft rounded-card p-4 mt-2 flex flex-col gap-1 text-sm"
          >
            <span class="text-ink-900 font-bold mb-1">門市資料</span>
            <span class="text-ink-500">門市代號：{{ logisticsTemp.storeCode }}</span>
            <span class="text-ink-500">門市名稱：{{ logisticsTemp.storeName }}</span>
            <span class="text-ink-500">門市地址：{{ logisticsTemp.storeAddress }}</span>
          </div>
        </div>

        <div v-else>
          <div class="flex flex-col gap-1">
            <label class="text-sm mb-3 text-ink-900">收件地址</label>

            <InputGroup>
              <InputText v-model="address" placeholder="收件地址" :invalid="v$.address.$error" />
            </InputGroup>
            <InValidErrorMessage :errorDto="v$.address.$errors" vaildChiName="收件地址" />
          </div>
        </div>
      </div>

      <span class="text-ink-900 font-medium"
        >總金額 :
        <span class="text-brand-price font-bold text-lg">{{
          finalPrice.toLocaleString()
        }}</span></span
      >
      <!-- #endregion -->
      <!-- #region  購買按鈕-->
      <div class="flex gap-3">
        <button
          class="flex-1 border border-border-soft text-ink-500 p-3 rounded-card cursor-pointer hover:bg-surface-muted transition-colors"
          @click="router.back()"
        >
          返回
        </button>
        <button
          class="flex-1 bg-brand-500 text-white p-3 rounded-card cursor-pointer font-bold"
          @click="userBuy"
        >
          前往付款
        </button>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
