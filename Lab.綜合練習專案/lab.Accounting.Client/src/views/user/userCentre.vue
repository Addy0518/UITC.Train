<script setup>
import { userHeadShot } from '@/api/userService';
import defaultImgurl from '@/img/預設圖片.jpg';
/*
   變數名稱代表意義
   router : 改變路由
   baseUrl : 基底位址
   authStore : localstorage
   menuItems : 清單列表
*/
const router = useRouter();
const baseUrl = import.meta.env.VITE_IMG_URL;
const authStore = useAuthStore();
const menuItems = ref([
  {
    label: '用戶資料',
    icon: 'pi pi-user',
    command: () => router.push('/user-centre/profile'),
    items: [
      { label: '個人檔案', command: () => router.push('/user-centre/profile') },
      { label: '更改密碼', command: () => router.push('/user-centre/update-password') },
    ],
  },
  {
    label: '購買清單',
    icon: 'pi pi-list',
    command: () => router.push('/user-centre/purchase-orders'),
  },
  {
    label: '我的錢包',
    icon: 'pi pi-wallet',
    command: () => router.push('/user-centre/wallet'),
  },
  {
    label: '更多功能',
    icon: 'pi pi-info-circle',
    command: () => router.push('/user-centre/function'),
  },
  {
    label: '服務支援',
    icon: 'pi pi-question-circle',
    command: () => router.push('/user-centre/help'),
    items: [
      { label: '常見問題', command: () => router.push('/user-centre/faq') },
      { label: '服務條款', command: () => router.push('/user-centre/terms-privacy') },
    ],
  },
]);

/*
   注入 Loading 跟 Toast
*/
const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

/*
   載入頭貼
*/
const imgUrl = computed(() => {
  const headshot = authStore.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
  return `${baseUrl}/UserHeadShot/${headshot}`;
});

/*
   上傳檔案 ( 大頭照 ) 並在前端顯示
*/
const uploadFile = async (event) => {
  try {
    showLoading();
    const file = event.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('userFile', file);
    const res = await userHeadShot(formData);

    const { data } = res;

    if (data.codeStatus === 2000) {
      imgUrl.value = `${baseUrl}/UserHeadShot/${data.returnData.userHeadshot}`;
      authStore.userHeadshot = data.returnData.userHeadshot;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
};
</script>

<template>
  <div class="container mx-auto">
    <div class="flex gap-6 p-6">
      <!-- #region  側邊攔 -->
      <aside class="w-60 p-5 bg-surface-dark rounded-card">
        <div class="flex justify-end mb-10 mt-10">
          <!-- #region  頭貼 -->
          <label class="relative cursor-pointer group">
            <img
              :src="imgUrl"
              alt="User Avatar"
              class="w-50 h-50 rounded-full object-cover border-2 border-ink-on-dark-faint group-hover:opacity-75 transition-opacity"
            />
            <div
              class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity rounded-full"
            >
              <span class="bg-black/50 text-white text-xs px-2 py-1 rounded-card">更換照片</span>
            </div>
            <input type="file" @change="uploadFile" accept="image/*" class="hidden" />
          </label>
          <!-- #endregion -->
        </div>
        <PanelMenu :model="menuItems" />
      </aside>
      <!-- #endregion -->

      <!-- #region  內容區 -->
      <main class="flex-1">
        <RouterView />
      </main>
      <!-- #endregion -->
    </div>
  </div>
</template>
