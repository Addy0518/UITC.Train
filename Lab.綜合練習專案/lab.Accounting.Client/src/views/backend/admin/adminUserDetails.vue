<script setup>
import { getUserDetails, deleteUser, updateUserDeleteStatus } from '@/api/admin/userService';

/*
   變數名稱代表意義
   route : 獲取路由資訊
   router : 改變路由
   userInfo : 用戶資訊
   goDelete : 停用用戶
   deleteReason : 停用原因
*/
const route = useRoute();
const router = useRouter();
const userInfo = ref();
const deleteReason = ref();
const goDelete = ref(false);
/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   初始化時
*/
onMounted(() => {
  getUser(route.params.id);
});

/*
   加入已經寫好的驗證規則
*/
const rules = computed(() => ({
  deleteReason: { required, maxLength: maxLength(500) },
}));

/*
   加入套件驗證設定
*/
const v$ = useVuelidate(rules, { deleteReason }, { $autoDirty: true, $lazy: true, $scope: false });

/*
   查看用戶
*/
const getUser = async (id) => {
  try {
    showLoading();
    const res = await getUserDetails(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      userInfo.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   停用用戶
*/
const deleteOneUser = async (id, reason) => {
  if (goDelete) {
    const isFormCorrect = await v$.value.$validate();
    if (!isFormCorrect) return;
  }

  try {
    showLoading();
    const res = await deleteUser(id, reason);
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功!');
      router.push({ name: 'admin-alluser' });
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};

/*
   復原用戶停用狀態
*/
const updateUserStatus = async (id) => {
  try {
    showLoading();
    const res = await updateUserDeleteStatus(id);
    const { data } = res;

    if (data.codeStatus === 2000) {
      showToastSuccess('成功!');
      router.push({ name: 'admin-alluser' });
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="flex flex-col w-full p-6" v-if="userInfo">
    <!--#region  用戶資訊 -->
    <div class="bg-page-bg rounded-card border border-border-soft p-5 mb-3">
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-user text-xs" />用戶資訊
      </p>
      <div class="grid grid-cols-3 gap-4 mb-4">
        <div>
          <p class="text-xs text-ink-500 mb-1">名稱</p>
          <p class="text-sm text-ink-900">{{ userInfo.userName }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">帳號</p>
          <p class="text-sm text-brand-price">{{ userInfo.userAccount }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">地址</p>
          <p class="text-sm text-ink-900">{{ userInfo.userAddress }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">電話</p>
          <p class="text-sm text-ink-900">{{ userInfo.userPhone }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">生日</p>
          <p class="text-sm text-ink-900">{{ userInfo.userBirthDate }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">性別</p>
          <p class="text-sm">
            <span class="px-2 py-0.5 rounded-full text-xs bg-surface-muted text-ink-500">
              {{ userInfo.userGender === 0 ? '女性' : userInfo.userGender === 1 ? '男性' : '其他' }}
            </span>
          </p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">註冊時間</p>
          <p class="text-sm text-ink-900">{{ formatDateTimeString(userInfo.createTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">最後更新時間</p>
          <p class="text-sm text-ink-900">{{ formatDateTimeString(userInfo.updateTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">狀態</p>
          <p class="text-sm">
            <span
              class="px-2 py-0.5 rounded-full text-xs"
              :class="
                userInfo.isDelete === 0
                  ? 'bg-status-success/10 text-status-success'
                  : 'bg-surface-muted text-status-neutral'
              "
            >
              {{ userInfo.isDelete === 0 ? '正常' : '停用' }}
            </span>
          </p>
        </div>
        <div>
          <p class="text-xs text-ink-500 mb-1">角色</p>
          <p class="text-sm">
            <span class="px-2 py-0.5 rounded-full text-xs bg-surface-muted text-ink-500">
              {{
                userInfo.userRole === 'Seller'
                  ? '賣家'
                  : userInfo.userRole === 'Admin'
                    ? '管理員'
                    : '一般用戶'
              }}
            </span>
          </p>
        </div>
      </div>
    </div>
    <!-- #endregion -->
    <!--#region  停用或恢復帳號 -->
    <button
      v-if="userInfo.isDelete === 0"
      @click="goDelete = !goDelete"
      class="px-4 py-2 border border-action-danger/30 text-action-danger rounded-card text-sm cursor-pointer hover:bg-action-danger-50 w-fit"
    >
      停用帳號
    </button>
    <button
      v-if="userInfo.isDelete === 1"
      @click="updateUserStatus(userInfo.userId)"
      class="px-4 py-2 border border-status-success/30 text-status-success rounded-card text-sm cursor-pointer hover:bg-status-success/10 w-fit"
    >
      恢復帳號
    </button>
    <div
      v-if="userInfo.isDelete === 1"
      class="bg-action-danger-50 rounded-card border border-action-danger/20 p-5 mb-3 mt-3"
    >
      <p class="text-sm text-action-danger flex items-center gap-1 mb-2">
        <i class="pi pi-user text-xs" />停用管理員
      </p>
      <p class="text-sm text-ink-900">{{ userInfo.deleteAdminId }}</p>
      <p class="text-sm text-action-danger flex items-center gap-1 mb-2 mt-3">
        <i class="pi pi-times-circle text-xs" />停用原因
      </p>
      <p class="text-sm text-ink-900">{{ userInfo.deleteReason }}</p>
    </div>

    <!-- 停用操作 -->
    <div v-if="goDelete" class="bg-page-bg rounded-card border border-border-soft p-5 mt-3">
      <p class="text-sm text-ink-500 flex items-center gap-1 mb-4">
        <i class="pi pi-check-circle text-xs" />停用操作
      </p>
      <div class="mb-4">
        <p class="text-xs text-ink-500 mb-1">停用原因（停用時必填）</p>
        <textarea
          v-model="deleteReason"
          class="w-full border border-border-soft rounded-card p-2 text-sm resize-y min-h-20 outline-none focus:border-ink-300 text-ink-900"
          placeholder="請輸入停用原因..."
        />
        <InValidErrorMessage :errorDto="v$.deleteReason.$errors" vaildChiName="停用原因" />
      </div>
      <div class="flex justify-end gap-2">
        <button
          @click="deleteOneUser(userInfo.userId, deleteReason)"
          class="px-4 py-2 border border-action-danger/30 text-action-danger rounded-card text-sm cursor-pointer hover:bg-action-danger-50"
        >
          停用
        </button>
      </div>
    </div>
    <!-- #endregion -->
  </div>
</template>
