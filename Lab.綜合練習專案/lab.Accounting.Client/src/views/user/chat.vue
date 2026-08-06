<script setup>
import { GetMessageHistory, getChatUserList } from '@/api/chatService';
import { startConnection, getConnection, stopConnection } from '@/common/signalrConnection';
import defaultImgurl from '@/img/預設圖片.jpg';
const props = defineProps({
  targetUserId: Number,
});

/*
   變數名稱代表意義
   myUserId : 用戶 ID
   chatUserList : 左側用戶列表
   messages : 所有歷史訊息
   inputContent : 傳送的訊息
   messageList : 訊息列表
   currentTargetId : 聊天對象 ID
   baseUrl : 基底位址
*/
const router = useRouter();
const authStore = useAuthStore();
const myUserId = authStore.userId;
const chatUserList = ref([]);
const messages = ref([]);
const inputContent = ref('');
const messageList = ref(null);
const currentTargetId = ref(props.targetUserId ?? null);
const baseUrl = import.meta.env.VITE_IMG_URL;
/*
   初始化
*/
onMounted(async () => {
  await loadChatUserList();

  // 開始建立 Signal 連線
  await startConnection();
  const connection = getConnection();

  // 監聽收到的訊息
  // ReceiveMessage 是後端 Hub 裡的自訂事件 ( SendMessage )
  // 也就是當今天一則訊息發送的順序是
  // 1. A 發送訊息 => 後端 SendMessage 觸發
  // 2. SendMessage 裡儲存訊息到資料庫順便觸發 ReceiveMessage
  // 3. B 接收到訊息 => 前端 connection.on('ReceiveMessage', ...) 觸發
  connection.on('ReceiveMessage', (senderId, content, sendTime) => {
    // 確認傳送的訊息是不是當下的聊天對象
    if (Number(senderId) === Number(currentTargetId.value)) {
      messages.value.push({ senderId: Number(senderId), content, sendTime });
      scrollToBottom();
    }
  });
  // 如果路由有帶 targetUserId，直接載入歷史對話
  if (currentTargetId.value) {
    await loadMessages(currentTargetId.value);
  }
});

/*
   onUnmounted 是初始化的相反 , onMounted 是開啟網頁時觸發 , onUnmounted 則是離開網頁時觸發
   這裡離開網頁時 stopConnection 是因為 Signal 不會自動關閉連線 , 要手動關
*/
onUnmounted(() => {
  stopConnection();
});

/*
   載入聊天對象列表
*/
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

/*
   點擊左側某個聊天對象
*/
const selectUser = async (user) => {
  currentTargetId.value = user.chatPartnerId;

  // 替換路由到這個聊天對象的聊天室
  router.replace({ name: 'chat', params: { targetUserId: user.chatPartnerId } });
  // 一樣載入歷史聊天紀錄
  await loadMessages(user.chatPartnerId);
};

/*
   載入歷史訊息
*/
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

/*
   傳送訊息
*/
const sendMessage = async () => {
  if (!inputContent.value.trim() || !currentTargetId.value) return;

  const connection = getConnection();

  // invoke 是呼叫後端 Hub 的方法 , 後端 Hub 有一隻我自訂的方法是 SendMessage
  // 後面則是對應這個方法要的參數 , 一樣順序要對其
  await connection.invoke('SendMessage', myUserId, currentTargetId.value, inputContent.value);

  // 順便把訊息也堆上前端畫面 , 讓前端畫面同步即時更新
  messages.value.push({
    senderId: myUserId,
    content: inputContent.value,
    sendTime: new Date().toLocaleTimeString(),
  });

  inputContent.value = '';
  scrollToBottom();
};

/*
   自動捲到最下面
*/
const scrollToBottom = () => {
  // nextTick 是等待 Dom 更新完之後再執行 , 這裡是等最新訊息更新完再執行捲動
  nextTick(() => {
    if (messageList.value) {
      messageList.value.scrollTop = messageList.value.scrollHeight;
    }
  });
};

/*
   載入頭貼
*/
const imgUrl = (user) => {
  const headshot = user.userHeadshot;
  if (!headshot) {
    return defaultImgurl;
  }
  if (headshot.includes('googleusercontent.com')) {
    return headshot;
  }
 return `${baseUrl}/UserHeadShot/${headshot}`;
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
        v-for="user in chatUserList"
        :key="user.chatPartnerId"
        @click="selectUser(user)"
        class="px-4 py-3 cursor-pointer hover:bg-surface-muted border-b border-border-soft flex items-center gap-3"
        :class="currentTargetId === user.chatPartnerId ? 'bg-surface-muted' : ''"
      >
        <div class="w-9 h-9 rounded-full bg-brand-100 flex items-center justify-center shrink-0">
          <img  :src="imgUrl"></img>
        </div>
        <span class="text-sm text-ink-900">{{ user.userName }}</span>
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
