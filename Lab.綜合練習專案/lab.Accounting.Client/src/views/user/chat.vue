<script setup>
import { GetMessageHistory, getChatUserList } from '@/api/chatService';
import { startConnection, getConnection, stopConnection } from '@/common/signalrConnection';

const props = defineProps({
  targetUserId: Number,
});

const router = useRouter();
const authStore = useAuthStore();
const myUserId = authStore.userId;

// 左側列表
const chatUserList = ref([]);

// 右側對話
const messages = ref([]);
const inputContent = ref('');
const messageList = ref(null);
const currentTargetId = ref(props.targetUserId ?? null);

onMounted(async () => {
  // 載入聊天對象列表
  await loadChatUserList();

  // 建立 SignalR 連線
  await startConnection();
  const connection = getConnection();

  // 監聽收到訊息
  connection.on('ReceiveMessage', (senderId, content, sendTime) => {
    if (Number(senderId) === Number(currentTargetId.value)) {
      messages.value.push({ senderId: Number(senderId), content, sendTime });
      scrollToBottom();
    }
  });
  // 如果路由有帶 targetUserId，直接載入對話
  if (currentTargetId.value) {
    await loadMessages(currentTargetId.value);
  }
});

onUnmounted(() => {
  stopConnection();
});

// 載入聊天對象列表
const loadChatUserList = async () => {
  try {
    const res = await getChatUserList();
    if (res.data.codeStatus === 2000) {
      chatUserList.value = res.data.returnData;
    }
  } catch (err) {
    console.log(err);
  }
};

// 點擊左側某個聊天對象
const selectUser = async (targetId) => {
  currentTargetId.value = targetId;
  router.replace({ name: 'chat', params: { targetUserId: targetId } });
  await loadMessages(targetId);
};

// 載入歷史訊息
const loadMessages = async (targetId) => {
  try {
    const res = await GetMessageHistory(targetId);
    if (res.data.codeStatus === 2000) {
      messages.value = res.data.returnData;
      scrollToBottom();
    }
  } catch (err) {
    console.log(err);
  }
};

// 傳送訊息
const sendMessage = async () => {
  if (!inputContent.value.trim() || !currentTargetId.value) return;

  const connection = getConnection();
  await connection.invoke('SendMessage', myUserId, currentTargetId.value, inputContent.value);

  messages.value.push({
    senderId: myUserId,
    content: inputContent.value,
    sendTime: new Date().toLocaleTimeString(),
  });

  inputContent.value = '';
  scrollToBottom();
};

// 自動捲到最下面
const scrollToBottom = () => {
  nextTick(() => {
    if (messageList.value) {
      messageList.value.scrollTop = messageList.value.scrollHeight;
    }
  });
};
</script>

<template>
  <div class="flex w-full h-[calc(100vh-80px)]">
    <!-- 左側：聊天對象列表 -->
    <div class="w-64 border-r border-border-soft bg-page-bg overflow-y-auto shrink-0">
      <div class="px-4 py-3 border-b border-border-soft">
        <span class="text-sm font-medium text-ink-900">訊息</span>
      </div>

      <div v-if="chatUserList.length === 0" class="py-8 text-center text-xs text-ink-500">
        還沒有任何對話
      </div>

      <div
        v-for="userId in chatUserList"
        :key="userId"
        @click="selectUser(userId)"
        class="px-4 py-3 cursor-pointer hover:bg-surface-muted border-b border-border-soft flex items-center gap-3"
        :class="currentTargetId === userId ? 'bg-surface-muted' : ''"
      >
        <div class="w-9 h-9 rounded-full bg-brand-100 flex items-center justify-center shrink-0">
          <i class="pi pi-user text-brand-500 text-sm"></i>
        </div>
        <span class="text-sm text-ink-900">用戶 {{ userId }}</span>
      </div>
    </div>

    <!-- 右側：對話內容 -->
    <div class="flex flex-col flex-1 overflow-hidden">
      <!-- 沒選對象時的提示 -->
      <div
        v-if="!currentTargetId"
        class="flex-1 flex items-center justify-center text-ink-500 text-sm"
      >
        請選擇一個聊天對象
      </div>

      <template v-else>
        <!-- 訊息列表 -->
        <div class="flex-1 overflow-y-auto px-6 py-4 flex flex-col gap-3" ref="messageList">
          <div
            v-for="(msg, index) in messages"
            :key="index"
            class="flex"
            :class="msg.senderId === myUserId ? 'justify-end' : 'justify-start'"
          >
            <div
              class="max-w-xs px-4 py-2 rounded-2xl text-sm"
              :class="
                msg.senderId === myUserId
                  ? 'bg-brand-500 text-white rounded-br-sm'
                  : 'bg-surface-muted text-ink-900 rounded-bl-sm'
              "
            >
              <p class="m-0">{{ msg.content }}</p>
              <small class="text-[10px] opacity-60 block text-right mt-1">{{ msg.sendTime }}</small>
            </div>
          </div>
        </div>

        <!-- 輸入框 -->
        <div class="border-t border-border-soft px-4 py-3 flex gap-3 items-center bg-page-bg">
          <input
            v-model="inputContent"
            @keyup.enter="sendMessage"
            placeholder="輸入訊息..."
            class="flex-1 border border-border-soft rounded-full px-4 py-2 text-sm outline-none focus:border-brand-500"
          />
          <button
            @click="sendMessage"
            class="bg-brand-500 text-white px-4 py-2 rounded-full text-sm hover:opacity-90 cursor-pointer"
          >
            傳送
          </button>
        </div>
      </template>
    </div>
  </div>
</template>
