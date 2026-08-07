<script setup>
import { GetMessageHistory, getChatUserList, updateReadStatus } from '@/api/chatService';
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
   searchKeyword :　搜尋關鍵字
   chatUser : 暫存的賣家用戶資料
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
const searchKeyword = ref('');
const chatUser = useChatUserStore();
const sellerProfile = chatUser.userProfile;
/*
   初始化
*/
onMounted(async () => {
  await loadChatUserList();

  if (sellerProfile && sellerProfile.chatPartnerId) {
    const exists = chatUserList.value.find((u) => u.chatPartnerId === sellerProfile.chatPartnerId);
    if (!exists) {
      chatUserList.value.unshift(sellerProfile);
    }
    chatUser.userProfile = { chatPartnerId: null, userName: null, userHeadshot: null };
  }

  // 開始建立 Signal 連線
  await startConnection();
  const connection = getConnection();

  // 監聽收到的訊息
  // ReceiveMessage 是後端 Hub 裡的自訂事件 ( SendMessage )
  // 也就是當今天一則訊息發送的順序是
  // 1. A 發送訊息 => 後端 SendMessage 觸發
  // 2. SendMessage 裡儲存訊息到資料庫順便觸發 ReceiveMessage
  // 3. B 接收到訊息 => 前端 connection.on('ReceiveMessage', ...) 觸發
  connection.on('ReceiveMessage', async (senderId, content, sendTime) => {
    // 確認傳送的訊息是不是當下的聊天對象
    if (Number(senderId) === Number(currentTargetId.value)) {
      messages.value.push({ senderId: Number(senderId), content, sendTime });
      scrollToBottom();

      await connection.invoke('MarkAsRead', Number(currentTargetId.value));
    }
  });

  // 監聽對方已讀事件
  connection.on('MessageRead', (readerUserId) => {
    // 把我傳給這個人的訊息全部標為已讀
    messages.value.forEach((msg) => {
      if (msg.senderId === myUserId && msg.receiverId === Number(readerUserId)) {
        msg.isRead = true;
      }
    });
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

      const connection = getConnection();
      if (connection) {
        await connection.invoke('MarkAsRead', Number(targetId));
      }
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
    receiverId: currentTargetId.value,
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
   根據搜尋關鍵字過濾聊天對象列表
*/
const filteredChatUserList = computed(() => {
  if (!searchKeyword.value) return chatUserList.value;
  return chatUserList.value.filter((user) => user.userName.includes(searchKeyword.value));
});

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
  <div class="container mx-auto max-w-5xl">
    <div class="flex gap-6 p-6 h-[calc(100vh-160px)]">
      <!--#region 左側：聊天對象列表 -->
      <aside class="w-60 rounded-card overflow-hidden flex flex-col border-2 border-surface-dark">
        <!--#region 標題 -->
        <div class="px-4 py-3 border-b border-border-soft">
          <span class="text-sm font-medium text-ink-900">訊息</span>
        </div>
        <!-- #endregion -->

        <!--#region 搜尋框 -->
        <div class="px-3 py-2 border-b border-border-soft">
          <input
            v-model="searchKeyword"
            placeholder="搜尋聯絡人..."
            class="w-full border border-border-soft rounded-full px-3 py-1.5 text-sm outline-none focus:border-brand-500"
          />
        </div>
        <!-- #endregion -->

        <!--#region 沒有對話時的提示 -->
        <div v-if="filteredChatUserList.length === 0" class="py-8 text-center text-xs text-ink-500">
          {{ searchKeyword ? '找不到相關聯絡人' : '還沒有任何對話' }}
        </div>
        <!-- #endregion -->

        <!--#region 聊天對象列表 -->
        <div class="overflow-y-auto flex-1">
          <div
            v-for="user in filteredChatUserList"
            :key="user.chatPartnerId"
            @click="selectUser(user)"
            class="px-4 py-3 cursor-pointer hover:bg-surface-muted border-b border-border-soft flex items-center gap-3"
            :class="currentTargetId === user.chatPartnerId ? 'bg-surface-muted' : ''"
          >
            <!--#region 頭貼 -->
            <div
              class="w-9 h-9 rounded-full bg-brand-100 flex items-center justify-center shrink-0"
            >
              <img :src="imgUrl(user)" class="rounded-full object-cover" />
            </div>
            <!-- #endregion -->

            <!--#region 用戶名稱 -->
            <span class="text-sm text-ink-900">{{ user.userName }}</span>
            <!-- #endregion -->
          </div>
        </div>
        <!-- #endregion -->
      </aside>
      <!-- #endregion -->

      <!--#region 右側：對話內容 -->
      <main class="flex-1 flex flex-col overflow-hidden rounded-card border border-border-soft">
        <!--#region 沒選對象時的提示 -->
        <div
          v-if="!currentTargetId"
          class="flex-1 flex items-center justify-center text-ink-500 text-sm"
        >
          請選擇一個聊天對象
        </div>
        <!-- #endregion -->

        <template v-else>
          <!--#region 訊息列表 -->
          <div class="flex-1 overflow-y-auto px-6 py-4 flex flex-col gap-3" ref="messageList">
            <div
              v-for="(msg, index) in messages"
              :key="index"
              class="flex flex-col"
              :class="msg.senderId === myUserId ? 'items-end' : 'items-start'"
            >
              <!--#region 訊息泡泡 + 時間 -->
              <div class="relative group">
                <!--#region 訊息泡泡 -->
                <div
                  class="max-w-xs px-4 py-2 rounded-2xl text-sm"
                  :class="
                    msg.senderId === myUserId
                      ? 'bg-brand-500 text-white rounded-br-sm'
                      : 'bg-surface-muted text-ink-900 rounded-bl-sm'
                  "
                >
                  <p class="m-0">{{ msg.content }}</p>
                </div>
                <!-- #endregion -->
                <!--#region 已讀狀態 -->
                <small
                  v-if="msg.senderId === myUserId"
                  class="text-[10px] text-ink-300 px-1 block text-right"
                >
                  {{ msg.isRead ? '已讀' : '未讀' }}
                </small>
                <!-- #endregion -->
                <!--#region 時間-->
                <small
                  class="text-[10px] text-ink-500 mt-1 px-1 opacity-0 group-hover:opacity-100 transition-opacity duration-200 block"
                  :class="msg.senderId === myUserId ? 'text-right' : 'text-left'"
                >
                  {{ formatDateTimeString(msg.sendTime) }}
                </small>
                <!-- #endregion -->
              </div>
              <!-- #endregion -->
            </div>
          </div>
          <!-- #endregion -->

          <!--#region 輸入框 -->
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
          <!-- #endregion -->
        </template>
      </main>
      <!-- #endregion -->
    </div>
  </div>
</template>
