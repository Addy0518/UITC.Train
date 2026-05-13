<script setup>
import { getProduct } from '@/api/productsService';
import { userBuyProduct } from '@/api//orderService';
import defaultImgurl from '@/img/oguri-cap-chibi.png';
/*
   變數名稱代表意義
   route : 獲取路由資訊
   product : 商品資訊
   baseUrl : 環境變數裡的圖片基底位址
   comment : 評論
   shippingAddress :
   createTime : 現在時間
*/
const route = useRoute();
const product = ref(null);
const baseUrl = import.meta.env.VITE_IMG_URL;
const boughtQuantity = ref();
const authStore = useAuthStore();
const comment = ref();

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
    var res = await getProduct(id);
    const { data } = res;
    if (data.codeStatus === 2000) {
      product.value = data.returnData;
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
  讀取商品圖片 , 判斷是否有圖片沒有就回傳預設
*/
const getProductsImg = (img) => {
  if (img && img.productsImg) {
    return `${baseUrl}/ProductsImg/${img.productsImg}`;
  }
  return defaultImgurl;
};

/*
  使用者購買跟評分
*/
const userBuy = async () => {
  const bought = {
    productsId: Number(route.params.id),
    boughtQuantity: boughtQuantity.value,
    comment: comment.value,
    shippingAddress: authStore.userAddress,
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

/*
  去除後端傳回類別重複
*/
const productscategory = (categories) => {
  if (!categories) return [];

  return [...new Set(categories.split(','))];
};
</script>

<template>
  <div class="flex flex-col w-full" v-if="product">
    <div class="border-gray-200h-full flex flex-col items-center">
      <div class="mt-40 w-300 rounded-lg shadow-sm">
        <div class="justify-between flex flex-col">
          <div v-for="img in product.productsImgs">
            <img :src="getProductsImg(img)" alt="Logo" class="w-full max-w-40 max-h-40 mt-4" />
          </div>
          <span class="mt-3">商品名稱 : {{ product.productsName }}</span>
          <span class="mt-3">商品價格 : {{ product.productsPrice }}</span>
          <span class="mt-3">商品評分 : {{ product.productsRate }}</span>
          <span class="mt-3">商品庫存 : {{ product.productsStock }}</span>
          <span class="mt-3">商品擁有者 ID : {{ product.userId }}</span>
          <InputGroup>
            <InputNumber v-model="boughtQuantity" placeholder="購買數量" />
          </InputGroup>

          <InputGroup>
            <InputText v-model="comment" placeholder="評論" />
          </InputGroup>
          <div class="flex gap-2 mt-3">
            <span
              v-for="cat in productscategory(product.productCategoryName)"
              :key="cat"
              class="bg-gray-100 px-2 py-1 rounded text-sm"
            >
              {{ cat }}
            </span>
            <button
              class="bg-black text-white p-3 rounded-2xl cursor-pointer font-bold"
              @click="userBuy()"
            >
              購買
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
