<script setup>
import { followStore, unfollowStore, isFollowingStore } from '@/api/storeService';

/*
   變數名稱代表意義
   isFollowing : 目前是否已追蹤這個賣場
*/
const props = defineProps({
  storeId: { type: Number, required: true },
});

const isFollowing = ref(false);
const showToastSuccess = inject('showToastSuccess');
const showToastError = inject('showToastError');

onMounted(async () => {
  await checkFollowStatus();
});

const checkFollowStatus = async () => {
  try {
    console.log('stroeId', props.storeId);
    const res = await isFollowingStore(props.storeId);
    const { data } = res;
    if (data.codeStatus === 2000) {
      isFollowing.value = data.returnData;
    }
  } catch (err) {
    console.log(err);
  }
};

const toggleFollow = async () => {
  try {
    if (isFollowing.value) {
      const res = await unfollowStore(props.storeId);
      if (res.data.codeStatus === 2000) {
        isFollowing.value = false;
        showToastSuccess('已取消追蹤');
      }
    } else {
      const res = await followStore(props.storeId);
      if (res.data.codeStatus === 2000) {
        isFollowing.value = true;
        showToastSuccess('已追蹤此賣場');
      }
      if (res.data.codeStatus === 4000) {
        showToastError('無法追蹤自己的賣場');
      }
    }
  } catch (err) {
    console.log(err);
    showToastError('操作失敗，請稍後再試');
  }
};
</script>

<template>
  <button
    @click="toggleFollow"
    class="px-3 py-1 text-xs rounded-card cursor-pointer flex items-center gap-1 transition-colors"
    :class="
      isFollowing
        ? 'bg-brand-500 border border-brand-500 text-white hover:opacity-90'
        : 'border border-border-soft text-ink-500 hover:bg-surface-muted'
    "
  >
    <i :class="isFollowing ? 'pi pi-heart-fill' : 'pi pi-heart'" class="text-xs"></i>
    {{ isFollowing ? '已追蹤' : '追蹤' }}
  </button>
</template>
