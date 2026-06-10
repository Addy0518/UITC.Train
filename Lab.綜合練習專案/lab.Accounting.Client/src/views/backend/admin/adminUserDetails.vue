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
    <div class="bg-white rounded-lg border border-gray-100 p-5 mb-3">
      <p class="text-sm text-gray-400 flex items-center gap-1 mb-4">
        <i class="pi pi-user text-xs" />用戶資訊
      </p>
      <div class="grid grid-cols-3 gap-4 mb-4">
        <div>
          <p class="text-xs text-gray-400 mb-1">名稱</p>
          <p class="text-sm">{{ userInfo.userName }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">帳號</p>
          <p class="text-sm text-orange-500">{{ userInfo.userAccount }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">地址</p>
          <p class="text-sm">{{ userInfo.userAddress }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">電話</p>
          <p class="text-sm">{{ userInfo.userPhone }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">生日</p>
          <p class="text-sm">{{ userInfo.userBirthDate }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">性別</p>
          <p
            class="text-sm"
            :class="{
              'bg-pink-100 text-pink-700': userInfo.userGender === 0,
              'bg-blue-100 text-blue-700': userInfo.userGender === 1,
              'bg-gray-100 text-gray-700': userInfo.userGender === 2,
            }"
          >
            {{ userInfo.userGender === 0 ? '女性' : userInfo.userGender === 1 ? '男性' : '其他' }}
          </p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">註冊時間</p>
          <p class="text-sm">{{ formatDateTimeString(userInfo.createTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">最後更新時間</p>
          <p class="text-sm">{{ formatDateTimeString(userInfo.updateTime) }}</p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">狀態</p>
          <p
            class="text-sm"
            :class="{
              'bg-yellow-100 text-green-700': userInfo.isDelete === 0,
              'bg-green-100 text-red-700': userInfo.isDelete === 1,
            }"
          >
            {{ userInfo.isDelete === 0 ? '正常' : '停用' }}
          </p>
        </div>
        <div>
          <p class="text-xs text-gray-400 mb-1">角色</p>
          <p
            class="text-sm"
            :class="{
              'bg-orange-100 text-orange-700': userInfo.userRole === 'Seller',
              'bg-purple-100 text-purple-700': userInfo.userRole === 'Admin',
              'bg-gray-100 text-gray-700': !userInfo.userRole,
            }"
          >
            {{
              userInfo.userRole === 'Seller'
                ? '賣家'
                : userInfo.userRole === 'Admin'
                  ? '管理員'
                  : '一般用戶'
            }}
          </p>
        </div>
      </div>
    </div>
  <!-- #endregion -->
    <!--#region  停用或恢復帳號 -->
    <button
      v-if="userInfo.isDelete === 0"
      @click="goDelete = !goDelete"
      class="px-4 py-2 border border-red-200 text-red-500 rounded-lg text-sm cursor-pointer hover:bg-red-50"
    >
      停用帳號
    </button>
    <button
      v-if="userInfo.isDelete === 1"
      @click="updateUserStatus(userInfo.userId)"
      class="px-4 py-2 border border-green-200 text-green-500 rounded-lg text-sm cursor-pointer hover:bg-green-50"
    >
      恢復帳號
    </button>
    <div v-if="userInfo.isDelete === 1" class="bg-red-50 rounded-lg border border-red-100 p-5 mb-3">
      <p class="text-sm text-red-400 flex items-center gap-1 mb-2">
        <i class="pi pi-user text-xs" />停用管理員
      </p>
      <p class="text-sm text-red-700">{{ userInfo.deleteAdminId }}</p>
      <p class="text-sm text-red-400 flex items-center gap-1 mb-2">
        <i class="pi pi-times-circle text-xs" />停用原因
      </p>
      <p class="text-sm text-red-700">{{ userInfo.deleteReason }}</p>
    </div>

    <!-- 停用操作 -->
    <div v-if="goDelete" class="bg-white rounded-lg border border-gray-100 p-5">
      <p class="text-sm text-gray-400 flex items-center gap-1 mb-4">
        <i class="pi pi-check-circle text-xs" />停用操作
      </p>
      <div class="mb-4">
        <p class="text-xs text-gray-400 mb-1">停用原因（停用時必填）</p>
        <textarea
          v-model="deleteReason"
          class="w-full border border-gray-200 rounded-lg p-2 text-sm resize-y min-h-20 outline-none focus:border-gray-400"
          placeholder="請輸入停用原因..."
        />
        <InValidErrorMessage :errorDto="v$.deleteReason.$errors" vaildChiName="停用原因" />
      </div>
      <div class="flex justify-end gap-2">
        <button
          @click="deleteOneUser(userInfo.userId, deleteReason)"
          class="px-4 py-2 border border-red-200 text-red-500 rounded-lg text-sm cursor-pointer hover:bg-red-50"
        >
          停用
        </button>
      </div>
    </div>
     <!-- #endregion -->
  </div>
</template>
