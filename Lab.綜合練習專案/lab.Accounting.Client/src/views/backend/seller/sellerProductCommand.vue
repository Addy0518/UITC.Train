<script setup>
import {
  productsImgUpload,
  productsImgDelete,
  createProducts,
  getProduct,
  updateProducts,
  productsDescriptionImgUpload,
} from '@/api/productsService';
import {
  getOneFatherCategory,
  getOneSonCategory,
  getFatherCategories,
} from '@/api/categoryService';
// Tiptap 的 Editor 和內容元件
import { useEditor, EditorContent } from '@tiptap/vue-3';
// StarterKit 跟 Image 是 Tiptap 的工具包 , 讓他支援編輯 , 圖片新增等功能
import StarterKit from '@tiptap/starter-kit';
import ImageResize from 'tiptap-extension-resize-image';
import Swal from 'sweetalert2';

/*
   變數名稱代表意義
   imgs : 商品圖片
   route : 獲取路由資訊
   router : 改變路由
   productName : 商品名稱
   productPrice : 商品價格
   productStock : 商品庫存
   productDescription : 商品描述
   productDiscount : 商品折扣趴數
   productDiscountStart : 折扣開始時間
   productDiscountEnd : 折扣結束時間
   productsId : 商品 ID
   isAdd : 路由判斷新增或刪除
   baseUrl : 環境變數裡的圖片基底位址
   categoryLevels : 每層的選項清單
   selectedLevels : 每層選到的值
   parentCategory : 頂層的類別清單
*/
let imgs = ref([]);

const route = useRoute();
const router = useRouter();
const productName = ref();
const productPrice = ref();
const productStock = ref();
const productDescription = ref();
const productDiscount = ref();
const productDiscountStart = ref();
const productDiscountEnd = ref();
const productsId = ref();
const categoryLevels = ref([]);
const selectedLevels = ref([]);
const parentCategory = ref([]);
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
  extensions: [
    StarterKit,
    ImageResize.configure({
      // 這裡可以設定預設置入時的圖片寬度，比如預設是 300px 或 50%
      HTMLAttributes: {
        style: 'max-width: 100%; width: 300px; height: auto;',
      },
    }),
  ],

  // 每次編輯器內容有變動時觸發
  // 把編輯器當下內容轉成 HTML 字串，同步存進 productDescription ref
  // 這樣按儲存時 productDescription.value 就是最新的 HTML
  onUpdate: ({ editor }) => {
    productDescription.value = editor.getHTML();
  },
});

/*
   載入類別跟判斷是不是更新商品
*/
onMounted(async () => {
  await getCategories();
  if (route.params.id) {
    updateData(route.params.id);
  }
});

/*
  取得最終選到的 CategoryId（最後一個有選的）
*/
const finalCategoryId = computed(() => {
  const last = selectedLevels.value.at(-1);
  return last?.productCategoryId ?? null;
});

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  productName: { required, maxLength: maxLength(50) },
  productPrice: { required },
  productStock: { required },
  finalCategoryId: { required },
  productDescription: { required },
}));

/*
   加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
   autoDirty : 一碰到欄位就開始驗證
   lazy : 元件載入時不會馬上驗證 , 等使用者開始互動才會
   scope : 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
*/
const v$ = useVuelidate(
  rules,
  { productName, productPrice, productStock, finalCategoryId, productDescription },
  { $autoDirty: true, $lazy: true, $scope: false },
);

/*
  查看最頂層類別
*/
const getCategories = async () => {
  showLoading();
  const res = await getOneFatherCategory();
  const { data } = res;
  if (data.codeStatus === 2000) {
    // parentCategory 是備用 , 是給更新時 updateData 拿重建下拉清單時用的
    parentCategory.value = data.returnData;
    categoryLevels.value = [data.returnData];
    selectedLevels.value = [];
  }
  hideLoading();
};

/*
  偵測父類別並改變子類別
*/
const changeCategory = async (levelIndex) => {
  // levelIndex 是目前選到的類別階層
  // slice 把目前選到的類別階層都清掉 , 只保留前一個類別階層 ( 比如選擇了衣服就把衣服後面的類別階層都清掉只留衣服跟前面的男士服裝 )
  // 目的是怕使用者選到一半又回去重選 , 要及時清掉舊的選擇
  categoryLevels.value = categoryLevels.value.slice(0, levelIndex + 1);
  selectedLevels.value = selectedLevels.value.slice(0, levelIndex + 1);

  // 拿到當前選擇的類別
  const selected = selectedLevels.value[levelIndex];
  if (!selected) return;

  const res = await getOneSonCategory(selected.productCategoryId);
  const { data } = res;

  // 還有下一層的話就繼續加
  if (data.codeStatus === 2000 && data.returnData.length > 0) {
    categoryLevels.value.push(data.returnData);
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
      productDiscount.value = item.discount;
      productDiscountStart.value = item.discountStart ? new Date(item.discountStart) : null;
      productDiscountEnd.value = item.discountEnd ? new Date(item.discountEnd) : null;
      nextTick(() => {
        editor.value?.commands.setContent(item.productsDescription ?? '');
      });

      if (item.productsImgs) {
        imgs.value = item.productsImgs.map((img) => ({
          productsImgId: img.productsImgId,
          url: `${baseUrl}/ProductsImg/${img.productsImg}`,
          file: null,
        }));
      }

      // 用 GetFatherCategories 拿到麵包屑路徑，依序重建每層下拉
      const catRes = await getFatherCategories(item.productCategoryId);
      const { data: catData } = catRes;

      if (catData.codeStatus === 2000) {
        const ancestors = catData.returnData;
        // ancestors 的內容範例：
        // [
        //   { productCategoryId: 27, productCategoryName: '男士衣服' },  ← index 0
        //   { productCategoryId: 28, productCategoryName: '長褲' },      ← index 1
        // ]

        // 第一層固定是頂層選項（男士衣服、女士衣服、娛樂...）
        // parentCategory 是 getCategories 時存好的備份
        categoryLevels.value = [parentCategory.value];

        // selectedLevels 現在：
        // []  ← 還沒有任何選擇
        selectedLevels.value = [];

        for (let i = 0; i < ancestors.length; i++) {
          // 把這層的父類別塞進 selectedLevels，代表「這層選了誰」
          // i=0 → selectedLevels = [男士衣服]
          // i=1 → selectedLevels = [男士衣服, 長褲]
          selectedLevels.value.push(ancestors[i]);

          // 如果不是最後一層，就要載入下一層的選項清單
          if (i < ancestors.length - 1) {
            // 用 getOneSonCategory 一層一層下去找子類別
            const res = await getOneSonCategory(ancestors[i].productCategoryId);
            const { data } = res;
            if (data.codeStatus === 2000) {
              // 把子類別選項塞進 categoryLevels，變成新的一層下拉選單
              categoryLevels.value.push(data.returnData);
            }
          }
        }
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
        ProductCategoryId: finalCategoryId.value,
        productsName: productName.value,
        productsPrice: productPrice.value,
        productsStock: productStock.value,
        productsDescription: productDescription.value,
        discount: productDiscount.value,
        discountStart: formatUTC8Date(productDiscountStart.value),
        discountEnd: formatUTC8Date(productDiscountEnd.value),
      };
      const res = await createProducts(createData);
      const { data } = res;
      if (data.codeStatus === 2000) {
        for (const img of imgs.value) {
          if (img.file) {
            const fd = new FormData();
            fd.append('productsImgsFiles', img.file);
            fd.append('reviewId', data.returnData);
            await productsImgUpload(fd);
          }
        }

        showToastSuccess('送出成功 ! 等待審核通過');
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
        ProductCategoryId: finalCategoryId.value,
        productsName: productName.value,
        productsPrice: productPrice.value,
        productsStock: productStock.value,
        productsDescription: productDescription.value,
        discount: productDiscount.value,
        discountStart: formatUTC8Date(productDiscountStart.value),
        discountEnd: formatUTC8Date(productDiscountEnd.value),
      };
      const res = await updateProducts(updateData);
      const { data } = res;
      if (data.codeStatus === 2000) {
        for (const img of imgs.value) {
          if (img.file) {
            const fd = new FormData();
            fd.append('productsImgsFiles', img.file);
            fd.append('reviewId', data.returnData);
            await productsImgUpload(fd);
          }
        }
        showToastSuccess('送出成功 ! 等待審核通過');
        router.back();
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
  <div class="flex flex-col w-full p-6">
    <div class="bg-page-bg rounded-card border border-border-soft overflow-hidden">
      <!-- #region  標題 / 商品圖片-->
      <div class="px-6 py-4 border-b border-border-soft">
        <p class="text-2xl font-bold m-0 text-ink-900">
          {{ route.params.id ? '編輯商品' : '新增商品' }}
        </p>
      </div>

      <div class="p-6 flex flex-col gap-6">
        <div>
          <p class="text-sm text-ink-500 mb-2">商品圖片</p>
          <div class="flex flex-wrap gap-2.5">
            <div v-for="(img, index) in imgs" :key="index" class="relative w-80 h-80">
              <img
                :src="img.url"
                class="w-full h-full object-cover rounded-card border border-border-soft"
              />
              <button
                @click="removeImage(index)"
                class="absolute -top-1.5 -right-1.5 bg-action-danger text-white rounded-full w-4 h-4 flex items-center justify-center text-xs cursor-pointer"
              >
                ✕
              </button>
            </div>
            <label
              class="w-80 h-80 border border-dashed border-ink-300 rounded-card flex flex-col items-center justify-center cursor-pointer hover:bg-surface-muted gap-1"
            >
              <i class="pi pi-plus text-ink-500 text-sm"></i>
              <span class="text-xs text-ink-500">上傳照片</span>
              <input type="file" @change="uploadFile" accept="image/*" class="hidden" multiple />
            </label>
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  表單欄位-->
        <div class="grid grid-cols-2 gap-4">
          <!-- 商品名稱 -->
          <div class="col-span-2">
            <label class="text-sm text-ink-500 block mb-1.5">商品名稱</label>
            <InputText
              v-model="productName"
              placeholder="輸入商品名稱"
              :invalid="v$.productName.$error"
              class="w-full"
            />
            <InValidErrorMessage :errorDto="v$.productName.$errors" vaildChiName="商品名稱" />
          </div>

          <!-- 類別（動態多層） -->
          <div v-for="(options, index) in categoryLevels" :key="index">
            <label class="text-sm text-ink-500 block mb-1.5">
              {{ index === 0 ? '類別' : `子類別 ${index}` }}
            </label>
            <Select
              v-model="selectedLevels[index]"
              :options="options"
              optionLabel="productCategoryName"
              placeholder="請選擇"
              @change="changeCategory(index)"
              class="w-full"
            />
          </div>
          <InValidErrorMessage :errorDto="v$.finalCategoryId.$errors" vaildChiName="類別" />

          <!-- 價格 -->
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">商品價格</label>
            <InputGroup>
              <InputNumber
                v-model="productPrice"
                placeholder="0"
                :invalid="v$.productPrice.$error"
                class="w-full"
              />
              <InputGroupAddon>.00</InputGroupAddon>
            </InputGroup>
            <InValidErrorMessage :errorDto="v$.productPrice.$errors" vaildChiName="商品價格" />
          </div>

          <!-- 庫存 -->
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">商品庫存</label>
            <InputNumber
              v-model="productStock"
              placeholder="0"
              :invalid="v$.productStock.$error"
              class="w-full"
            />
            <InValidErrorMessage :errorDto="v$.productStock.$errors" vaildChiName="商品庫存" />
          </div>

          <!-- 折扣 -->
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">商品折扣</label>
            <InputNumber
              v-model="productDiscount"
              placeholder="例如輸入 80 代表 8 折"
              class="w-full"
              :min="1"
              :max="99"
            />
          </div>

          <!-- 折扣開始時間 -->
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">折扣開始時間</label>
            <DatePicker
              v-model="productDiscountStart"
              class="w-full"
              :maxDate="productDiscountEnd"
              showTime
            />
          </div>
          <!-- 折扣結束時間 -->
          <div>
            <label class="text-sm text-ink-500 block mb-1.5">折扣結束時間</label>

            <DatePicker
              v-model="productDiscountEnd"
              class="w-full"
              :minDate="productDiscountStart"
              showTime
            />
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  商品描述 -->
        <div>
          <label class="text-sm text-ink-500 block mb-1.5">商品描述</label>
          <div class="border border-border-soft rounded-card overflow-hidden">
            <div class="flex gap-1 p-2 bg-surface-muted border-b border-border-soft">
              <button
                type="button"
                @click="editor.chain().focus().toggleBold().run()"
                :class="editor?.isActive('bold') ? 'bg-border-soft' : ''"
                class="px-2 py-1 rounded-card text-sm font-bold hover:bg-border-soft text-ink-900"
              >
                B
              </button>
              <button
                type="button"
                @click="editor.chain().focus().toggleItalic().run()"
                :class="editor?.isActive('italic') ? 'bg-border-soft' : ''"
                class="px-2 py-1 rounded-card text-sm italic hover:bg-border-soft text-ink-900"
              >
                I
              </button>
              <button
                type="button"
                @click="editor.chain().focus().toggleBulletList().run()"
                class="px-2 py-1 rounded-card text-sm hover:bg-border-soft text-ink-900"
              >
                • 清單
              </button>
              <label
                class="px-2 py-1 rounded-card text-sm hover:bg-border-soft cursor-pointer text-ink-900"
              >
                🖼 插入圖片
                <input
                  type="file"
                  accept="image/*"
                  class="hidden"
                  @change="uploadDescriptionImage"
                />
              </label>
            </div>
            <EditorContent
              :editor="editor"
              class="p-3 min-h-32 prose max-w-none max-h-100 overflow-y-auto [&_img]:max-w-3xl [&_img]:h-auto"
              :invalid="v$.productDescription.$error"
            />
            <InValidErrorMessage
              :errorDto="v$.productDescription.$errors"
              vaildChiName="商品描述"
            />
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  儲存按鍵-->
        <div class="flex justify-end pt-4 border-t border-border-soft">
          <button
            @click="createOrUpdateProduct()"
            class="bg-brand-500 hover:opacity-90 text-white px-8 py-2.5 rounded-card text-xl font-medium cursor-pointer transition-colors"
          >
            儲存
          </button>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
<style>
/* 讓編輯器裡的圖片點擊時會出現控制外框與手勢 */
.tiptap .prose img {
  display: inline-block;
  float: none;
}
.tiptap img.ProseMirror-selectednode {
  outline: 3px solid #b8473d; /* 點擊時的外框顏色，改成規範的品牌主色 */
}
</style>
