<script setup>
import {
  productsImgUpload,
  productsImgDelete,
  createProducts,
  getProduct,
  updateProducts,
  getCategory,
  productsDescriptionImgUpload,
} from '@/api/productsService';
// Tiptap 的 Editor 和內容元件
import { useEditor, EditorContent } from '@tiptap/vue-3';
// StarterKit 跟 Image 是 Tiptap 的工具包 , 讓他支援編輯 , 圖片新增等功能
import StarterKit from '@tiptap/starter-kit';
import Image from '@tiptap/extension-image';
import Swal from 'sweetalert2';
import { getError400Message } from '@/common/method';

/*
   變數名稱代表意義
   imgs : 商品圖片
   route : 獲取路由資訊
   router : 改變路由
   productCategoryName : 商品類型名稱
   productName : 商品名稱
   productPrice : 商品價格
   productStock : 商品庫存
   productDescription : 商品描述
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
const productDescription = ref();
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

/*
   把 Tiptap 的擴充套件載入
*/
const editor = useEditor({
  // StarterKit：包含粗體、斜體、清單、段落等基本功能
  // Image：讓編輯器支援插入圖片
  extensions: [StarterKit, Image],

  // 每次編輯器內容有變動時觸發
  // 把編輯器當下內容轉成 HTML 字串，同步存進 productDescription ref
  // 這樣按儲存時 productDescription.value 就是最新的 HTML
  onUpdate: ({ editor }) => {
    productDescription.value = editor.getHTML();
  },
});

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
      productDescription.value = item.productsDescription ?? '';
      nextTick(() => {
        editor.value?.commands.setContent(item.productsDescription ?? '');
      });

      if (item.productParentId) {
        // 找父類別物件中的類別名稱
        selectParent.value = parentCategory.value.find(
          (p) => p.productCategoryId === item.productParentId,
        );

        // 載入子類別清單
        const catRes = await getCategory(item.productParentId);
        const { data: catData } = catRes;
        if (catData.codeStatus === 2000) {
          childCategory.value = catData.returnData;
          // 找子類別物件中的類別名稱
          selectChild.value = childCategory.value.find(
            (c) => c.productCategoryId === item.productCategoryId,
          );
        }
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
        productsDescription: productDescription.value,
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
      if (data.codeStatus === 4000) {
        showToastError(getError400Message(data.error400));
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
        productsDescription: productDescription.value,
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

/*
   上傳商品描述圖片
*/
const uploadDescriptionImage = async (e) => {
  const file = e.target.files[0];
  if (!file) return;

  const formData = new FormData();
  formData.append('productsDescriptionImgsFiles', file);

  const res = await productsDescriptionImgUpload(formData);
  const { data } = res;
  if (data.codeStatus === 2000) {
    const url = `${baseUrl}/ProductsDescriptionImg/${data.returnData}`;
    editor.value.chain().focus().setImage({ src: url }).run();
  }
  e.target.value = '';
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

    <!-- 商品描述整體區塊 -->
    <div class="mt-4">
      <label class="text-sm text-gray-400 block mb-2">商品描述</label>

      <!-- 工具列 -->
      <div class="flex gap-1 p-2 border border-b-0 rounded-t-lg bg-gray-50">
        <!-- 粗體按鈕 -->
        <!-- editor.chain().focus() : 確保操作後游標回到編輯器 -->
        <!-- toggleBold() : 切換粗體狀態（有就關，沒有就開） -->
        <!-- isActive('bold') : 目前游標在粗體文字上時加深背景，給使用者視覺回饋 -->
        <button
          type="button"
          @click="editor.chain().focus().toggleBold().run()"
          :class="editor?.isActive('bold') ? 'bg-gray-200' : ''"
          class="px-2 py-1 rounded text-sm font-bold hover:bg-gray-200"
        >
          B
        </button>

        <!-- 斜體按鈕，邏輯同粗體 -->
        <button
          type="button"
          @click="editor.chain().focus().toggleItalic().run()"
          :class="editor?.isActive('italic') ? 'bg-gray-200' : ''"
          class="px-2 py-1 rounded text-sm italic hover:bg-gray-200"
        >
          I
        </button>

        <!-- 項目清單按鈕 -->
        <button
          type="button"
          @click="editor.chain().focus().toggleBulletList().run()"
          class="px-2 py-1 rounded text-sm hover:bg-gray-200"
        >
          • 清單
        </button>

        <!-- 圖片上傳 -->
        <!-- 用 label 包住隱藏的 input，點 label 就等於點 input，視覺上比較好看 -->
        <label class="px-2 py-1 rounded text-sm hover:bg-gray-200 cursor-pointer">
          🖼 插入圖片
          <input type="file" accept="image/*" class="hidden" @change="uploadDescriptionImage" />
        </label>
      </div>

      <!-- 編輯區 -->
      <EditorContent :editor="editor" class="border rounded-b-lg p-3 min-h-40 prose max-w-none" />
    </div>
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
