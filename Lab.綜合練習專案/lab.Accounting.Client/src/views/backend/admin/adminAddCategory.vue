<script setup>
import { addCategory, getAllCategories } from '@/api/admin/categoryService';
import { useRoute } from 'vue-router';

/*
   變數名稱代表意義
   router : 改變路由
   allCategories : 所有商品類別
   selectParent : 選擇的父類別
   categoryName : 類別名稱
   imgs : 圖片
*/
const router = useRouter();
const allCategories = ref([]);
const selectParent = ref();
const categoryName = ref('');
const imgs = ref(null);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   載入類別跟判斷是不是更新商品
*/
onMounted(() => {
  getCategoriesAll();
});

/*
   查看所有類別
*/
const getCategoriesAll = async () => {
  try {
    showLoading();
    const request = {
      pageIndex: 0,
      pageSize: 9999,
    };
    const res = await getAllCategories(request);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
    } else if (data.codeStatus === 4001) {
      allCategories.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  categoryName: { required, maxLength: maxLength(100) },
}));

/*
   加入套件驗證設定 , 包含剛剛自定的規則 ( rules ) , 要驗證的資料 ( form )
   autoDirty : 一碰到欄位就開始驗證
   lazy : 元件載入時不會馬上驗證 , 等使用者開始互動才會
   scope : 隔離驗證範圍 , 設定 false 代表這個驗證只驗證這裡的 , 不驗證父元件
*/
const v$ = useVuelidate(rules, { categoryName }, { $autoDirty: true, $lazy: true, $scope: false });

/*
    新增類別
*/
const createCategory = async () => {
  if (!selectParent.value && !imgs.value) {
    showToastError('頂層類別必須上傳圖片');
    return;
  }

  const isFormCorrect = await v$.value.$validate();
  if (!isFormCorrect) return;
  try {
    showLoading();
    const fd = new FormData();
    fd.append('ProductCategoryName', categoryName.value);
    if (selectParent.value) fd.append('ProductParentId', selectParent.value);
    if (imgs.value?.file) fd.append('ProductCategoryImgFile', imgs.value.file);
    const res = await addCategory(fd);
    const { data } = res;

    if (data.codeStatus === 2000) {
      allCategories.value = data.returnData;
      showToastSuccess('成功 ! ');
      router.push({ name: 'admin-allCategories' });
    } else if (data.codeStatus === 4001) {
      allCategories.value = [];
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   切換為父層類別時
*/
const changeParent = () => {
  imgs.value = null;
};

/*
   上傳商品圖片並在前端顯示
*/
const uploadFile = async (event) => {
  const file = event.target.files[0];

  const previewUrl = URL.createObjectURL(file);

  imgs.value = {
    file,
    url: previewUrl,
  };

  event.target.value = '';
};

/*
   移除圖片
*/
const removeImage = () => {
  imgs.value = null;
};
</script>

<template>
  <div class="flex flex-col w-full p-6">
    <div class="bg-white rounded-lg border border-gray-100 overflow-hidden">
      <!-- #region  標題 / 類別圖片-->
      <div class="px-6 py-4 border-b border-gray-100">
        <p class="text-2xl font-bold m-0">新增類別</p>
      </div>

      <div class="p-6 flex flex-col gap-6">
        <div v-if="!selectParent">
          <div>
            <p class="text-sm text-gray-400 mb-2">類別圖片</p>
            <div class="flex flex-wrap gap-2.5">
              <!-- 有圖片就顯示 -->
              <div v-if="imgs" class="relative w-40 h-40">
                <img
                  :src="imgs.url"
                  class="w-full h-full object-cover rounded-lg border border-gray-100"
                />
                <button
                  @click="removeImage"
                  class="absolute -top-1.5 -right-1.5 bg-red-500 text-white rounded-full w-4 h-4 flex items-center justify-center text-xs cursor-pointer"
                >
                  ✕
                </button>
              </div>

              <!-- 沒圖片才顯示上傳 -->
              <label
                v-else
                class="w-40 h-40 border border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center cursor-pointer hover:bg-gray-50 gap-1"
              >
                <i class="pi pi-plus text-gray-400 text-sm"></i>
                <span class="text-xs text-gray-400">上傳圖片</span>
                <input type="file" @change="uploadFile" accept="image/*" class="hidden" />
              </label>
            </div>
          </div>
        </div>
        <!-- #endregion -->
        <!-- #region  表單欄位-->
        <div class="grid grid-cols-2 gap-4">
          <!-- 類別名稱 -->
          <div class="col-span-2">
            <label class="text-sm text-gray-400 block mb-1.5">類別名稱</label>
            <InputText
              v-model="categoryName"
              placeholder="輸入類別名稱"
              :invalid="v$.categoryName.$error"
              class="w-full"
            />
            <InValidErrorMessage :errorDto="v$.categoryName.$errors" vaildChiName="類別名稱" />
          </div>

          <!-- 父類別 -->
          <div class="col-span-2">
            <label class="text-sm text-gray-400 block mb-1.5">父類別（不選代表頂層類別）</label>
            <Select
              v-model="selectParent"
              @change="changeParent"
              :options="[
                { productCategoryName: '無（頂層類別）', productCategoryId: null },
                ...allCategories,
              ]"
              optionLabel="productCategoryName"
              optionValue="productCategoryId"
              placeholder="選擇父類別"
              class="w-full"
            />
          </div>
        </div>
        <!-- #endregion -->

        <!-- #region  儲存按鍵-->
        <div class="flex justify-end pt-4 border-t border-gray-100">
          <button
            @click="createCategory"
            class="bg-orange-500 hover:bg-orange-600 text-white px-8 py-2.5 rounded-lg text-xl font-medium cursor-pointer transition-colors"
          >
            儲存
          </button>
        </div>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
