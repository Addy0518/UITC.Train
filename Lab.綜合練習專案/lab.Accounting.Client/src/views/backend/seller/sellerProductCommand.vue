<script setup>
import {
  productsImgUpload,
  productsImgDelete,
  createProducts,
  getProduct,
  updateProducts,
  getCategory,
} from '@/api/productsService';
import Swal from 'sweetalert2';


/*
   變數名稱代表意義
   imgs : 商品圖片
   route : 獲取路由資訊
   router : 改變路由
   productCategoryName : 商品類型名稱
   productName : 商品名稱
   productPrice : 商品價格
   productStock : 商品庫存
   productsId : 商品 ID
   isAdd : 路由判斷新增或刪除
   baseUrl : 環境變數裡的圖片基底位址
*/
let imgs = ref([]);

const route = useRoute();
const router = useRouter();
const productCategoryName = ref([]);
const parentCategory = ref([]);
const selectParent = ref();
const selectChild = ref();
const childCategory = ref([]);
const productName = ref();
const productPrice = ref();
const productStock = ref();
const productsId = ref();
const isAdd = computed(() => route.name === 'add-product');
const baseUrl = import.meta.env.VITE_IMG_URL;

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(() => {
  getCategories();
  if (route.params.id) {
    updateData(route.params.id);
  }
});

// 加入已經寫好的驗證規則
const rules = computed(() => ({
  productName: { required, maxLength: maxLength(50) },
  productPrice: { required },
  productStock: { required },
  selectChild: { required },
}));

// 加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
// autoDirty => 一碰到欄位就開始驗證
// lazy => 元件載入時不會馬上驗證 , 等使用者開始互動才會
// scope => 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
const v$ = useVuelidate(
  rules,
  { productName, productPrice, productStock, selectChild },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
  依據階層查看類別
*/
const getCategories = async () => {
  showLoading();
  const res = await getCategory();
  const { data } = res;
  if (data.codeStatus === 2000) {
    parentCategory.value = data.returnData;
  }
  hideLoading();
};

/*
  偵測父類別並改變子類別
*/
const changeCategory = async () => {
  childCategory.value = [];
  selectChild.value = null;

  if (selectParent.value) {
    const res = await getCategory(selectParent.value.productCategoryId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      childCategory.value = data.returnData;
    }
  }
};

/*
  查看單一商品 API ( 讓編輯帳本時前端能看到原本資料 )
*/
const updateData = async (productId) => {
  try {
    if (!productId) return;
    showLoading();
    const res = await getProduct(productId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      const item = data.returnData;
      productsId.value = item.productsId;
      productName.value = item.productsName;
      productPrice.value = item.productsPrice;
      productStock.value = item.productsStock;

      if (item.productCategoryName) {
        productCategoryName.value = item.productCategoryName.split(',');
      }

      if (item.productsImgs) {
        imgs.value = item.productsImgs.map((img) => ({
          productsImgId: img.productsImgId,
          url: `${baseUrl}/ProductsImg/${img.productsImg}`,
          file: null,
        }));
      }
    }
    if (data.codeStatus === 4001) {
      showToastError('錯誤', data.message);
    }
  } catch (error) {
    console.error('編輯錯誤 ', error.response);
  } finally {
    hideLoading();
  }
};

/*
   新增或更新商品
*/
const createOrUpdateProduct = async () => {
  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;

  if (isAdd.value) {
    showLoading();
    try {
      const createData = {
        ProductCategoryId: selectChild.value.productCategoryId,
        productsName: productName.value,
        productsPrice: productPrice.value,
        productsStock: productStock.value,
      };
      const res = await createProducts(createData);
      const { data } = res;
      if (data.codeStatus === 2000) {
        for (const img of imgs.value) {
          if (img.file) {
            const fd = new FormData();
            fd.append('productsImgsFiles', img.file);
            fd.append('productId', data.returnData);
            await productsImgUpload(fd);
          }
        }

        showToastSuccess('上架商品成功!');
        router.push({ name: 'mall' });
      }
    } catch (err) {
      console.log(err);
    } finally {
      hideLoading();
    }
  } else if (!isAdd.value) {
    showLoading();
    try {
      const updateData = {
        productsId: productsId.value,
        ProductCategoryId: selectChild.value.productCategoryId,

        productsName: productName.value,
        productsPrice: productPrice.value,
        productsStock: productStock.value,
      };
      const res = await updateProducts(updateData);
      const { data } = res;
      if (data.codeStatus === 2000) {
        for (const img of imgs.value) {
          if (img.file) {
            const fd = new FormData();
            fd.append('productsImgsFiles', img.file);
            fd.append('productId', productsId.value);
            fd.append('productsImgId', img.productsImgId);
            await productsImgUpload(fd);
          }
        }
        showToastSuccess('更新商品成功!');

        router.push({ name: 'mall' });
      }
    } catch (err) {
      console.log(err);
    } finally {
      hideLoading();
    }
  }
};

/*
   上傳商品圖片並在前端顯示
*/
const uploadFile = async (event) => {
  const files = Array.from(event.target.files);
  if (files.length === 0) return;

  for (const file of files) {
    const previewUrl = URL.createObjectURL(file);

    imgs.value.push({
      file: file,
      url: previewUrl,
    });
  }

  event.target.value = '';
};

/*
   移除圖片
*/
const removeImage = async (index) => {
  const targetImg = imgs.value[index];

  if (targetImg.productsImgId) {
    try {
      const result = await Swal.fire({
        title: '確定要刪除這張圖片嗎？',
        text: '刪除後將無法復原！',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '確定刪除',
        cancelButtonText: '取消',
      });

      if (result.isConfirmed) {
        const res = await productsImgDelete(targetImg.productsImgId);
        const { data } = res;

        if (data.codeStatus === 2000) {
          imgs.value.splice(index, 1);
        }
      }
    } catch (err) {
      console.error('資料操作錯誤 ', err);
    }
  } else {
    imgs.value.splice(index, 1);
  }
};
</script>

<template>
  <div>
    <div class="flex flex-wrap gap-4 p-5">
      <!-- 顯示已上傳的圖片預覽 -->
      <div v-for="(img, index) in imgs" :key="index" class="relative w-100 h-100">
        <img :src="img.url" class="w-full h-full object-cover rounded-lg shadow" />
        <!-- 刪除按鈕 -->
        <button
          @click="removeImage(index)"
          class="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs cursor-pointer"
        >
          ✕
        </button>
      </div>

      <!-- 上傳按鈕 (永遠在最後面) -->
      <label
        class="w-100 h-100 border-2 border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center cursor-pointer hover:bg-gray-50 transition"
      >
        <i class="pi pi-plus text-gray-400"></i>
        <span class="text-xs text-gray-400 mt-1">上傳照片</span>
        <input type="file" @change="uploadFile" accept="image/*" class="hidden" multiple />
      </label>
    </div>

    <InputGroup>
      <InputText v-model="productName" placeholder="商品名稱" :invalid="v$.productName.$error" />
    </InputGroup>
    <InValidErrorMessage :errorDto="v$.productName.$errors" vaildChiName="商品名稱" />

    <InputGroup>
      <Select
        v-model="selectParent"
        :options="parentCategory"
        optionLabel="productCategoryName"
        placeholder="類別"
        @change="changeCategory()"
      />
    </InputGroup>
    <InputGroup v-if="childCategory.length > 0">
      <Select
        v-model="selectChild"
        :options="childCategory"
        optionLabel="productCategoryName"
        placeholder="子類別"
      />
    </InputGroup>
    <InValidErrorMessage :errorDto="v$.selectChild.$errors" vaildChiName="子類別" />
    <InputGroup>
      <InputNumber
        v-model="productPrice"
        placeholder="商品價格"
        :invalid="v$.productPrice.$error"
      />
      <InputGroupAddon>.00</InputGroupAddon>
    </InputGroup>
    <InValidErrorMessage :errorDto="v$.productPrice.$errors" vaildChiName="商品價格" />
    <InputGroup>
      <InputNumber
        v-model="productStock"
        placeholder="商品庫存"
        :invalid="v$.productStock.$error"
      />
    </InputGroup>
    <InValidErrorMessage :errorDto="v$.productStock.$errors" vaildChiName="商品庫存" />
    <!-- 按鈕區 -->
    <div class="justify-end flex mt-5">
      <button
        @click="createOrUpdateProduct()"
        class="bg-black text-white p-4 rounded-2xl px-5 cursor-pointer"
      >
        儲存
      </button>
    </div>
  </div>
</template>
