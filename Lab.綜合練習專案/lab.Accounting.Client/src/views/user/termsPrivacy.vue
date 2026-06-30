<script setup>
/*
   服務條款、隱私權政策內容，依章節整理成陣列
*/
const termsSections = ref([
  {
    title: '一、服務說明',
    paragraphs: [
      '歡迎使用本平台。本平台提供使用者進行商品買賣、瀏覽、下單、付款及相關交易服務。當您完成註冊並使用本平台服務時，即表示您已閱讀、瞭解並同意接受本服務條款之所有內容。',
    ],
  },
  {
    title: '二、會員帳號',
    paragraphs: [
      '1. 使用者註冊時應提供真實、正確、完整的個人資料，並隨時維持資料更新。',
      '2. 每個帳號僅限本人使用，不得轉讓、出借或與他人共用。',
      '3. 使用者應自行妥善保管帳號密碼，因密碼保管不當所生之損害，本平台不負賠償責任。',
      '4. 若發現帳號有異常使用情形，使用者應立即通知本平台。',
    ],
  },
  {
    title: '三、交易規範',
    paragraphs: [
      '1. 買賣雙方應遵守誠實交易原則，不得刊登虛假商品資訊、哄抬價格或進行欺詐行為。',
      '2. 賣家上架之商品內容，需經本平台審核後始得公開顯示。',
      '3. 交易過程中產生之金流，將透過第三方支付服務商（如綠界科技 ECPay）進行處理。',
      '4. 訂單成立後，買賣雙方應依約定時間完成出貨及收貨；如有糾紛，可申請平台客服介入處理。',
    ],
  },
  {
    title: '四、付款與計價',
    paragraphs: [
      '1. 商品價格、運費及相關費用，以結帳頁面顯示金額為準。',
      '2. 本平台保留隨時調整商品定價、優惠活動內容之權利，但不影響已成立訂單之約定價格。',
    ],
  },
  {
    title: '五、智慧財產權',
    paragraphs: [
      '本平台之網站內容、商標、程式碼及相關素材，其智慧財產權歸本平台或原權利人所有，使用者不得擅自複製、轉載、散布或用於商業用途。',
    ],
  },
  {
    title: '六、免責聲明',
    paragraphs: [
      '1. 本平台僅提供交易媒合服務，對於賣家所刊登商品之品質、數量、規格、合法性等，不負保證責任。',
      '2. 因不可抗力因素導致服務中斷，本平台不負任何賠償責任，但將盡力儘速恢復服務。',
    ],
  },
  {
    title: '七、服務變更與終止',
    paragraphs: [
      '本平台保留隨時修改、暫停或終止部分或全部服務之權利。如使用者違反本服務條款，本平台得不經事先通知終止其帳號使用權限。',
    ],
  },
]);

const privacySections = ref([
  {
    title: '一、資料蒐集範圍與目的',
    paragraphs: [
      '本平台於使用者註冊、下單、客服聯繫等過程中，可能蒐集基本資料（姓名、Email、聯絡電話、收件地址）、交易資料（訂單紀錄、付款紀錄）及技術資料（裝置資訊、IP 位址、Cookie）。',
      '蒐集目的僅限於：提供帳號驗證、處理訂單與客服支援、寄送交易通知及驗證碼信件、安全性維護用途。',
    ],
  },
  {
    title: '二、資料的使用與保護',
    paragraphs: [
      '1. 本平台對使用者個人資料採取合理之技術與管理措施（如密碼加密儲存），防止資料遭未經授權之存取或洩漏。',
      '2. 除法律規定或使用者同意之情形外，本平台不會將使用者個人資料提供予第三方。',
      '3. 為完成金流付款及物流配送之必要，本平台可能將部分資料提供予合作之金流服務商及物流業者。',
    ],
  },
  {
    title: '三、Cookie 之使用',
    paragraphs: [
      '本平台可能使用 Cookie 技術，以記錄使用者登入狀態、購物車內容及瀏覽偏好。使用者可透過瀏覽器設定停用 Cookie，但可能影響部分功能之正常使用。',
    ],
  },
  {
    title: '四、使用者權利',
    paragraphs: [
      '使用者就其個人資料，依法享有查詢、補充更正、停止蒐集處理利用、請求刪除個人資料之權利。',
      '如需行使上述權利，請透過「服務支援 → 聯絡客服」與我們聯繫。',
    ],
  },
]);
</script>

<template>
  <div class="bg-page-bg-soft py-10 px-4">
    <div class="max-w-3xl mx-auto">
      <!-- #region 標題 -->
      <div class="text-center mb-8">
        <h1 class="text-2xl font-bold text-ink-900 m-0">服務條款與隱私權政策</h1>
        <p class="text-sm text-ink-500 mt-2 mb-0">最後更新日期：2026 年 6 月</p>
      </div>
      <!-- #endregion -->

      <div class="bg-page-bg border border-border-soft rounded-card p-6">
        <!-- #region 分頁切換：服務條款 / 隱私權政策
             注意：PrimeVue v3 用 TabView/TabPanel，v4 改成 Tabs/TabList/TabPanels，
             請依你實際的 PrimeVue 版本確認語法是否一致 -->
        <TabView>
          <TabPanel header="服務條款">
            <Accordion class="mt-2">
              <AccordionPanel
                v-for="(section, idx) in termsSections"
                :key="section.title"
                :value="idx"
              >
                <AccordionHeader>{{ section.title }}</AccordionHeader>

                <AccordionContent>
                  <p
                    v-for="(p, pIdx) in section.paragraphs"
                    :key="pIdx"
                    class="text-sm text-ink-500 leading-relaxed mb-1 last:mb-0"
                  >
                    {{ p }}
                  </p>
                </AccordionContent>
              </AccordionPanel>
            </Accordion>
          </TabPanel>

          <TabPanel header="隱私權政策">
            <div class="flex flex-col gap-6 mt-2">
              <div v-for="section in privacySections" :key="section.title">
                <h2 class="text-base font-semibold text-ink-900 mb-2">{{ section.title }}</h2>
                <p
                  v-for="(p, idx) in section.paragraphs"
                  :key="idx"
                  class="text-sm text-ink-500 leading-relaxed mb-1"
                >
                  {{ p }}
                </p>
              </div>
            </div>
          </TabPanel>
        </TabView>
        <!-- #endregion -->
      </div>
    </div>
  </div>
</template>
