<script setup>
import {
  getAllNotifications,
  getNotification,
  updateAllNotificationReadStatus,
} from '@/api/notificationService';

/*
   變數名稱代表意義
   notifications : 所有通知
   totalCount    : 通知總筆數
   currentPage   : 目前頁碼
   isFiltering   : 換頁/重整中（非首次加載）
*/
const router = useRouter();
const notifications = ref([]);
const totalCount = ref(0);
const currentPage = ref(0);
const isFiltering = ref(false);
const PAGE_SIZE = 10;

const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');

/*
   初始化
*/
onMounted(() => {
  loadNotifications(true);
});

/*
   載入通知列表
*/
const loadNotifications = async (isFirstLoad = false) => {
  try {
    if (isFirstLoad) {
      showLoading();
    } else {
      isFiltering.value = true;
    }
    const res = await getAllNotifications({
      pageIndex: currentPage.value,
      pageSize: PAGE_SIZE,
    });
    const { data } = res;

    if (data.codeStatus === 2000) {
      notifications.value = data.returnData.notifications;
      totalCount.value = data.returnData.totalCount;
    } else if (data.codeStatus === 4001) {
      notifications.value = [];
      totalCount.value = 0;
    }
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
    isFiltering.value = false;
  }
};

/*
   換頁
*/
const pageChange = (event) => {
  currentPage.value = event.page;
  loadNotifications();
};

/*
   通知類型對應 badge
*/
const notifBadge = (type) => {
  if ([5, 6].includes(type)) return { label: '訂單', style: 'background:#fff4ed; color:#ff6b35;' };
  if ([1, 2, 8].includes(type))
    return { label: '審核', style: 'background:#f1efe8; color:#888780;' };
  if ([3, 4].includes(type)) return { label: '商店', style: 'background:#f1efe8; color:#888780;' };
  if ([7].includes(type)) return { label: '評價', style: 'background:#fff4ed; color:#c9543f;' };
  if ([9].includes(type)) return { label: '物流', style: 'background:#fff4ed; color:#ff6b35;' };
  if ([10].includes(type)) return { label: '商品', style: 'background:#f0fdf4; color:#16a34a;' };
  return { label: '通知', style: 'background:#f1efe8; color:#888780;' };
};

/*
   點擊通知：標記已讀 + 跳轉
*/
const readNotification = async (notif) => {
  try {
    await getNotification(notif.notificationId);
    notif.isRead = true;

    if (notif.notificationType === 5 && notif.relatedId) {
      router.push({ name: 'purchase-orders-details', params: { id: notif.relatedId } });
    } else if ([6, 9].includes(notif.notificationType) && notif.relatedId) {
      router.push({ name: 'seller-orders-details', params: { id: notif.relatedId } });
    } else if ([1, 2, 8, 10].includes(notif.notificationType) && notif.relatedId) {
      router.push({ name: 'product-detail', params: { id: notif.relatedId } });
    } else if ([3, 4].includes(notif.notificationType)) {
      router.push({ name: 'seller-store-edit' });
    }
  } catch (err) {
    console.log(err);
  }
};

/*
   全部標為已讀
*/
const markAllRead = async () => {
  try {
    await updateAllNotificationReadStatus();
    notifications.value.forEach((n) => (n.isRead = true));
  } catch (err) {
    console.log(err);
  }
};

/*
   是否有任何未讀
*/
const hasUnread = computed(() => notifications.value.some((n) => !n.isRead));
</script>

<template>
  <!--#region 通知中心頁面 -->
  <div class="container mx-auto">
    <div class="flex flex-col p-8">
      <!--#region 標題列 -->
      <div class="flex items-center justify-between mb-5">
        <p class="text-2xl font-bold text-ink-900 m-0">通知中心</p>
        <button
          v-if="hasUnread"
          @click="markAllRead"
          class="text-sm text-brand-500 cursor-pointer hover:opacity-80"
        >
          全部標為已讀
        </button>
      </div>
      <!-- #endregion -->

      <!--#region 通知列表卡片 -->
      <div
        class="w-full max-w-2xl mx-auto bg-page-bg border border-border-soft rounded-card overflow-hidden"
      >
        <!--#region 骨架加載 -->
        <template v-if="isFiltering">
          <div
            v-for="n in PAGE_SIZE"
            :key="n"
            class="px-5 py-4 border-b border-border-soft flex gap-3 items-start"
          >
            <Skeleton width="8px" height="8px" borderRadius="50%" class="mt-1.5 shrink-0" />
            <div class="flex-1 flex flex-col gap-2">
              <Skeleton width="60px" height="18px" borderRadius="20px" />
              <Skeleton width="40%" height="14px" />
              <Skeleton width="80%" height="12px" />
              <Skeleton width="25%" height="10px" />
            </div>
          </div>
        </template>
        <!-- #endregion -->

        <!--#region 查無通知 -->
        <div
          v-else-if="notifications.length === 0"
          class="flex flex-col items-center justify-center py-20 text-ink-500"
        >
          <i class="pi pi-bell-slash text-4xl mb-3" />
          <span class="text-sm">目前沒有任何通知</span>
        </div>
        <!-- #endregion -->

        <!--#region 通知列表 -->
        <template v-else>
          <div
            v-for="notif in notifications"
            :key="notif.notificationId"
            @click="readNotification(notif)"
            class="px-5 py-4 border-b border-border-soft last:border-b-0 flex gap-3 items-start cursor-pointer transition-colors"
            :class="notif.isRead ? 'hover:bg-surface-muted' : 'bg-[#FFF4ED] hover:bg-[#FFE8DA]'"
          >
            <div
              class="w-2 h-2 rounded-full mt-2 shrink-0"
              :class="notif.isRead ? 'bg-transparent' : 'bg-brand-500'"
            ></div>
            <div class="flex-1 min-w-0">
              <span
                class="inline-block text-[11px] px-2 py-0.5 rounded-full mb-1"
                :style="notifBadge(notif.notificationType).style"
              >
                {{ notifBadge(notif.notificationType).label }}
              </span>
              <p
                class="text-sm font-medium m-0 mb-1"
                :class="notif.isRead ? 'text-ink-500' : 'text-ink-900'"
              >
                {{ notif.title }}
              </p>
              <p class="text-sm text-ink-500 m-0 leading-relaxed">{{ notif.content }}</p>
              <p class="text-xs text-ink-300 m-0 mt-1.5">
                {{ formatDateTimeString(notif.createTime) }}
              </p>
            </div>
            <i
              v-if="notif.relatedId || [3, 4].includes(notif.notificationType)"
              class="pi pi-chevron-right text-ink-300 text-xs mt-2 shrink-0"
            ></i>
          </div>
        </template>
        <!-- #endregion -->

        <!--#region 分頁 -->
        <div
          v-if="totalCount > PAGE_SIZE"
          class="flex items-center justify-center gap-4 py-4 border-t border-border-soft"
        >
          <span class="text-sm text-ink-500">共 {{ totalCount }} 則通知</span>
          <Paginator
            :template="{
              '640px': 'PrevPageLink CurrentPageReport NextPageLink',
              default: 'FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink',
            }"
            :rows="PAGE_SIZE"
            :totalRecords="totalCount"
            @page="pageChange"
          />
        </div>
        <!-- #endregion -->
      </div>
      <!-- #endregion -->
    </div>
  </div>
  <!-- #endregion -->
</template>
