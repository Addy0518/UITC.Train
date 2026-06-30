<script setup>
/*
   變數名稱代表意義
   keyword : 搜尋關鍵字
   activeCategory : 分類標籤
   faqCategories : 所有 FAQ 資料，依分類整理成陣列
*/
const keyword = ref('');
const activeCategory = ref('account');

const faqCategories = ref([
  {
    key: 'account',
    title: '帳號與註冊',
    items: [
      {
        q: '忘記密碼該怎麼辦？',
        a: '請到登入頁面點選「忘記密碼」，輸入您註冊時使用的帳號（Email），系統會寄送 6 位數驗證碼到您的信箱。輸入驗證碼並設定新密碼即可完成重設。驗證碼有效時間為 10 分鐘，逾時請重新申請。',
      },
      {
        q: '一直收不到驗證碼信件，怎麼辦？',
        a: '請先確認垂圾信件資料夾、確認 Email 是否正確、等待 1～2 分鐘。若多次嘗試仍未收到，請聯絡客服協助查詢。',
      },
      {
        q: '如何修改個人資料？',
        a: '登入後進入「會員中心 → 用戶資料」，即可編輯暱稱、上傳大頭照、更新聯絡電話與收件地址。',
      },
      {
        q: '可以同時用一個 Email 註冊多個帳號嗎？',
        a: '不可以，每個 Email 僅能對應一個帳號，這是為了保障帳號安全與交易紀錄的完整性。',
      },
    ],
  },
  {
    key: 'order',
    title: '訂單與物流',
    items: [
      {
        q: '如何查詢我的訂單狀態？',
        a: '登入後進入「會員中心 → 購買清單」，可以看到訂單目前的狀態（待付款、待出貨、已出貨、已完成、已取消）。',
      },
      {
        q: '下單後多久會出貨？',
        a: '一般商品賣家會在付款成功後 1～3 個工作日內完成出貨，實際時間依各賣場公告為準。',
      },
      {
        q: '訂單成立後可以更改收件地址嗎？',
        a: '若訂單尚未出貨，可嘗試聯絡賣家協助修改；若已出貨，將無法再變更收件地址。',
      },
      {
        q: '包裹遺失或損壞該怎麼處理？',
        a: '請保留商品與包裝照片，並在「購買清單」中對該筆訂單提出申訴，客服將協助協調後續處理。',
      },
    ],
  },
  {
    key: 'payment',
    title: '付款方式',
    items: [
      {
        q: '平台支援哪些付款方式？',
        a: '目前支援信用卡、ATM 轉帳、超商代碼繳費等多種付款方式，詳細選項依結帳頁面顯示為準。',
      },
      {
        q: '付款失敗或扣款後訂單仍顯示未付款，怎麼辦？',
        a: '請不要重複付款，先聯絡客服並提供訂單編號與付款截圖，將由客服協助核對金流紀錄。',
      },
      { q: '可以開立發票嗎？', a: '可以，結帳時可選擇電子發票，訂單完成後系統會自動產生發票。' },
    ],
  },
  {
    key: 'return',
    title: '退換貨與退款',
    items: [
      {
        q: '商品到貨後可以退換貨嗎？期限多久？',
        a: '一般商品享有到貨後 7 天鑑賞期（不適用於生鮮、個人衛生用品等特殊商品），請於期限內在「購買清單」提出申請。',
      },
      {
        q: '退款多久會退回原付款方式？',
        a: '信用卡退款約 7～14 個工作日反映在帳單上，ATM／超商退款則約需 3～5 個工作日撥款。',
      },
      {
        q: '收到的商品跟描述不符，該怎麼辦？',
        a: '請先保留照片並聯絡賣家協商；若無法達成共識，可在訂單頁面提出客服申訴。',
      },
    ],
  },
  {
    key: 'coupon',
    title: '優惠券與促銷',
    items: [
      {
        q: '優惠券無法使用，是什麼原因？',
        a: '常見原因：優惠券已過期、訂單金額未達門檻、不適用該商品分類，或已被使用過。',
      },
      {
        q: '優惠券可以跟其他活動疊加使用嗎？',
        a: '除非活動頁面特別說明，一般優惠券不可疊加，結帳時系統會自動套用最優惠的方案。',
      },
    ],
  },
  {
    key: 'seller',
    title: '賣家相關',
    items: [
      {
        q: '如何申請成為賣家？',
        a: '登入後進入「會員中心 → 更多功能 → 賣家申請」，填寫賣場基本資料並送出申請，審核約 1～3 個工作日。',
      },
      {
        q: '上架的商品需要審核嗎？要等多久？',
        a: '是的，新上架或編輯後的商品都會經過平台審核，審核時間約 24 小時內，結果會以站內通知告知。',
      },
    ],
  },
  {
    key: 'security',
    title: '帳號安全',
    items: [
      {
        q: '如何保護我的帳號安全？',
        a: '建議定期更換密碼、避免在公共網路登入、發現異常登入紀錄請立即修改密碼並聯絡客服。',
      },
      {
        q: '我懷疑帳號被盜用，該怎麼辦？',
        a: '請立即透過「忘記密碼」流程重設密碼，並聯絡客服協助凍結帳號、排查異常交易紀錄。',
      },
    ],
  },
]);

/*
   依關鍵字篩選問題（同時比對問題跟答案文字），找不到符合的分類就自動從畫面上消失
*/
const filteredCategories = computed(() => {
  if (!keyword.value.trim()) return faqCategories.value;

  const kw = keyword.value.trim().toLowerCase();
  return faqCategories.value
    .map((c) => ({
      ...c,
      items: c.items.filter(
        (item) => item.q.toLowerCase().includes(kw) || item.a.toLowerCase().includes(kw),
      ),
    }))
    .filter((c) => c.items.length > 0);
});
</script>

<template>
  <div class="bg-page-bg-soft py-10 px-4">
    <div class="max-w-3xl mx-auto">
      <!-- #region 標題 + 搜尋 -->
      <div class="text-center mb-8">
        <h1 class="text-2xl font-bold text-ink-900 m-0">常見問題</h1>
        <p class="text-sm text-ink-500 mt-2 mb-6">快速找到您需要的解答</p>

        <InputGroup class="max-w-md mx-auto">
          <InputGroupAddon>
            <i class="pi pi-search"></i>
          </InputGroupAddon>
          <InputText v-model="keyword" placeholder="輸入關鍵字搜尋問題" />
        </InputGroup>
      </div>
      <!-- #endregion -->

      <!-- #region 沒有符合結果 -->
      <div v-if="filteredCategories.length === 0" class="text-center text-ink-500 py-10">
        找不到符合「{{ keyword }}」的問題，您可以聯絡客服協助查詢。
      </div>
      <!-- #endregion -->

      <!-- #region 有搜尋關鍵字時：不顯示分頁，直接把所有符合結果全部列出 -->
      <div v-else-if="keyword.trim()" class="bg-page-bg border border-border-soft rounded-card p-4">
        <p class="text-sm text-ink-500 mb-3 px-2">
          共找到 {{ filteredCategories.reduce((acc, c) => acc + c.items.length, 0) }} 筆結果
        </p>
        <Accordion>
          <AccordionPanel
            v-for="(item, idx) in filteredCategories.flatMap((c) => c.items)"
            :key="idx"
            :value="idx"
          >
            <AccordionHeader>{{ item.q }}</AccordionHeader>
            <AccordionContent>
              <p class="text-sm text-ink-500 leading-relaxed m-0">{{ item.a }}</p>
            </AccordionContent>
          </AccordionPanel>
        </Accordion>
      </div>
      <!-- #endregion -->

      <!-- #region 沒有搜尋時：分類頁籤 + 手風琴 -->
      <div v-else>
        <!-- 分類頁籤（橫向捲動，手機也能用） -->
        <div class="overflow-x-auto mb-4">
          <div class="flex gap-2 min-w-max">
            <button
              v-for="category in filteredCategories"
              :key="category.key"
              @click="activeCategory = category.key"
              :class="[
                'py-1.5 px-4 rounded-full text-sm border cursor-pointer transition-colors',
                activeCategory === category.key
                  ? 'bg-brand-500 text-white border-brand-500'
                  : 'bg-page-bg text-ink-500 border-border-soft hover:border-ink-300',
              ]"
            >
              {{ category.title }}
            </button>
          </div>
        </div>

        <!-- 目前選中的分類問答 -->
        <div class="bg-page-bg border border-border-soft rounded-card p-4">
          <template v-for="category in filteredCategories" :key="category.key">
            <div v-if="activeCategory === category.key">
              <Accordion>
                <AccordionPanel v-for="(item, idx) in category.items" :key="idx" :value="idx">
                  <AccordionHeader>{{ item.q }}</AccordionHeader>
                  <AccordionContent>
                    <p class="text-sm text-ink-500 leading-relaxed m-0">{{ item.a }}</p>
                  </AccordionContent>
                </AccordionPanel>
              </Accordion>
            </div>
          </template>
        </div>
      </div>
      <!-- #endregion -->

      <!-- #region 底部聯絡客服導引 -->
      <div class="mt-10 text-center bg-page-bg border border-border-soft rounded-card p-6">
        <p class="text-sm text-ink-900 mb-3">沒有找到您需要的答案嗎？</p>
        <RouterLink
          class="inline-block py-2 px-5 rounded-card bg-brand-500 text-white text-sm font-medium cursor-pointer transition-opacity hover:opacity-90"
        >
          聯絡客服
        </RouterLink>
      </div>
      <!-- #endregion -->
    </div>
  </div>
</template>
