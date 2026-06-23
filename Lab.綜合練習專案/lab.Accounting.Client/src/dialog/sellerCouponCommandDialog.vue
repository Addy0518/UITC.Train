<script setup>
import { sellerCreateCoupons, sellerUpdateCoupons } from '@/api/couponService';
import { couponTypeEnum } from '@/common/enum';

const emit = defineEmits(['refresh']);

/*
   變數名稱代表意義
   showDialog : 顯示 Dialog
   isEdit : 編輯 or 新增開關

*/
const showDialog = ref(false);
const isEdit = ref(false);
const isLimit = ref(false);

const initForm = {
  code: '',
  name: '',
  type: couponTypeEnum.百分比折扣 ? couponTypeEnum.百分比折扣.value : couponTypeEnum.固定金額折抵,
  discount: 0,
  minimunSpend: 0,
  startTime: null,
  endTime: null,
  isActive: true,
};

const couponForm = ref({ ...initForm });

/* 注入 Loading 跟 Toast */
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   👉 提供給父元件呼叫的函式 (打開 Dialog)
   如果有傳入 couponData 就是「編輯」，沒傳就是「新增」
*/
const open = (couponData = null) => {
  if (couponData) {
    isEdit.value = true;
    isLimit.value = couponData.totalQuantity != null;
    couponForm.value = {
      couponId: couponData.couponId,
      code: couponData.code,
      name: couponData.name,
      type: couponData.type,
      discount: couponData.discount,
      minimunSpend: couponData.minimunSpend,
      totalQuantity: couponData.totalQuantity,
      startTime: couponData.startTime ? new Date(couponData.startTime) : null,
      endTime: couponData.endTime ? new Date(couponData.endTime) : null,
      isActive: couponData.isActive,
    };
  } else {
    isEdit.value = false;
    isLimit.value = false;
    couponForm.value = { ...initForm };
  }
  showDialog.value = true;
};

/* 將 open 函式暴露出去，讓父元件可以透過 ref 呼叫 */
defineExpose({ open });

/* 確認送出 (新增 / 編輯) */
const submitCoupon = async () => {
  try {
    showLoading();
    const payload = {
      ...couponForm.value,
      startTime: couponForm.value.startTime
        ? new Date(couponForm.value.startTime).toISOString()
        : null,
      endTime: couponForm.value.endTime ? new Date(couponForm.value.endTime).toISOString() : null,
    };

    let res;
    if (isEdit.value) {
      res = await sellerUpdateCoupons(payload);
    } else {
      res = await sellerCreateCoupons(payload);
    }

    const { data } = res;
    if (data.codeStatus === 2000) {
      showToastSuccess(isEdit.value ? '編輯成功 !' : '新增成功 !');
      showDialog.value = false;
      emit('refresh'); // 呼叫父元件刷新列表
    } else {
      showToastError(data.message || '操作失敗');
    }
  } catch (err) {
    console.log(err);
    showToastError('系統發生錯誤');
  } finally {
    hideLoading();
  }
};

/* 切換限制數量 */
const toggleLimit = () => {
  isLimit.value = !isLimit.value;
  if (!isLimit.value) {
    couponForm.value.totalQuantity = null;
  }
};
</script>
<template>
  <Dialog
    v-model:visible="showDialog"
    modal
    :header="isEdit ? '編輯優惠券' : '新增優惠券'"
    :style="{ width: '500px' }"
  >
    <div class="flex flex-col gap-4 py-4">
      <div class="flex flex-col gap-2" v-if="isEdit">
        <label class="text-sm font-medium text-gray-700"
          >優惠碼 <span class="text-red-500">*</span></label
        >
        <InputText
          v-model="couponForm.code"
          placeholder="請輸入優惠碼"
          :disabled="isEdit"
          readonly
        />
      </div>

      <div class="flex flex-col gap-2">
        <label class="text-sm font-medium text-gray-700"
          >優惠券名稱 <span class="text-red-500">*</span></label
        >
        <InputText v-model="couponForm.name" placeholder="請輸入優惠券名稱" />
      </div>

      <div class="grid grid-cols-2 gap-4">
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium text-gray-700">折扣類型</label>
          <select
            v-model="couponForm.type"
            class="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option :value="couponTypeEnum.百分比折扣.value">百分比折扣</option>
            <option :value="couponTypeEnum.固定金額折抵.value">固定金額折抵</option>
          </select>
        </div>

        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium text-gray-700">折扣額度</label>
          <InputNumber v-model="couponForm.discount" placeholder="例如: 9 或 100" />
        </div>
      </div>

      <div class="flex flex-col gap-2">
        <label class="text-sm font-medium text-gray-700">最低消費 (0表示無限制)</label>
        <InputNumber v-model="couponForm.minimunSpend" placeholder="0" />
      </div>
      <div class="grid grid-cols-2 gap-4 items-end">
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium text-gray-700">發行數量</label>
          <button
            type="button"
            class="px-4 py-1.5 rounded-md text-sm border w-fit cursor-pointer transition-colors"
            :class="
              isLimit
                ? 'bg-black text-white border-black'
                : 'bg-white text-gray-600 border-gray-300'
            "
            @click="toggleLimit"
          >
            {{ isLimit ? '限制數量' : '不限制數量' }}
          </button>
        </div>

        <div class="flex flex-col gap-2" v-if="isLimit">
          <label class="text-sm font-medium text-gray-700">總數量</label>
          <InputNumber v-model="couponForm.totalQuantity" placeholder="0" :min="1" />
        </div>
      </div>
      <div class="grid grid-cols-2 gap-4">
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium text-gray-700">開始時間</label>
          <DatePicker
            v-model="couponForm.startTime"
            showTime
            hourFormat="24"
            placeholder="選擇開始時間"
          />
        </div>

        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium text-gray-700">結束時間</label>
          <DatePicker
            v-model="couponForm.endTime"
            showTime
            hourFormat="24"
            placeholder="選擇結束時間"
          />
        </div>
      </div>

      <div class="flex items-center gap-2 mt-2">
        <input
          type="checkbox"
          id="isActiveSwitch"
          v-model="couponForm.isActive"
          class="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 cursor-pointer"
        />
        <label for="isActiveSwitch" class="text-sm font-medium text-gray-700 cursor-pointer"
          >是否啟用</label
        >
      </div>
    </div>

    <template #footer>
      <div class="flex gap-2 justify-end">
        <button
          @click="showDialog = false"
          class="px-4 py-2 text-sm text-gray-600 bg-gray-100 rounded-md hover:bg-gray-200 cursor-pointer transition-colors"
        >
          取消
        </button>
        <button
          @click="submitCoupon"
          class="px-4 py-2 text-sm text-white bg-black rounded-md hover:bg-gray-800 cursor-pointer transition-colors"
        >
          {{ isEdit ? '儲存變更' : '確認新增' }}
        </button>
      </div>
    </template>
  </Dialog>
</template>
