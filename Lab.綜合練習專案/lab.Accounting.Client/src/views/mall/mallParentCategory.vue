<script setup>
import { getOneFatherCategory, getOneSonCategory } from '@/api/categoryService';
import defaultImgurl from '@/img/預設圖片.jpg';

/*
   變數名稱代表意義
   groupedCategories : [{ parent: MallProductCategory, children: MallProductCategory[] }]
   searchKeyword     : 前端即時篩選關鍵字
   baseUrl           : 環境變數裡的圖片基底位址
*/
const router = useRouter();
const baseUrl = import.meta.env.VITE_IMG_URL;
const groupedCategories = ref([]);
const searchKeyword = ref('');

const showLoading = inject('showLoading');
const hideLoading = inject('hideLoading');

/*
   初始化：先拿所有父類別，再並行拿每個父類別的子類別
*/
onMounted(async () => {
  try {
    showLoading();
    const parentRes = await getOneFatherCategory();
    const { data: parentData } = parentRes;
    if (parentData.codeStatus !== 2000) return;

    const parents = parentData.returnData;

    const childResults = await Promise.all(
      parents.map((p) =>
        getOneSonCategory(p.productCategoryId).then((res) => ({
          parent: p,
          children: res.data.codeStatus === 2000 ? res.data.returnData : [],
        })),
      ),
    );

    groupedCategories.value = childResults;
  } catch (err) {
    console.log(err);
  } finally {
    hideLoading();
  }
});

/*
   讀取類別圖片，沒有就回傳預設
*/
const getCategoryImg = (category) => {
  if (category.productCategoryImg) {
    return `${baseUrl}/CategoryImg/${category.productCategoryImg}`;
  }
  return defaultImgurl;
};

/*
   前端即時篩選：關鍵字符合子類別名稱或父類別名稱時顯示
   父類別名稱符合時整組顯示，子類別名稱符合時只顯示符合的子類別
*/
const filteredGroups = computed(() => {
  const kw = searchKeyword.value.trim().toLowerCase();
  if (!kw) return groupedCategories.value;

  return groupedCategories.value
    .map((group) => {
      if (group.parent.productCategoryName.toLowerCase().includes(kw)) {
        return group;
      }
      const matchedChildren = group.children.filter((c) =>
        c.productCategoryName.toLowerCase().includes(kw),
      );
      return matchedChildren.length > 0 ? { ...group, children: matchedChildren } : null;
    })
    .filter(Boolean);
});

/*
   麵包屑首頁
*/
const home = ref({
  icon: 'pi pi-home',
  command: () => router.push({ name: 'mall' }),
});

const breadCrumbItem = ref([{ label: '全部分類' }]);
</script>

<template>
  <!--#region 整體容器 -->
  <div class="flex flex-col w-full items-center bg-page-bg">
    <div class="w-full max-w-screen-xl px-6 py-6">

      <!--#region 麵包屑 -->
      <Breadcrumb :home="home" :model="breadCrumbItem" class="mb-6" />
      <!-- #endregion -->

      <!--#region 搜尋欄 -->
      <div class="relative mb-6">
        <i class="pi pi-search absolute left-3 top-1/2 -translate-y-1/2 text-ink-300 text-sm" />
        <input
          v-model="searchKeyword"
          placeholder="搜尋分類名稱"
          class="w-full max-w-sm pl-9 pr-4 py-2 text-sm border border-border-soft rounded-card bg-page-bg text-ink-900 placeholder:text-ink-300 focus:outline-none focus:border-ink-300"
        />
      </div>
      <!-- #endregion -->

      <!--#region 查無結果 -->
      <div
        v-if="filteredGroups.length === 0"
        class="flex flex-col items-center justify-center py-20 text-ink-500"
      >
        <i class="pi pi-inbox text-4xl mb-3" />
        <span class="text-sm">找不到符合的分類</span>
      </div>
      <!-- #endregion -->

      <!--#region 分類群組列表 -->
      <div v-for="group in filteredGroups" :key="group.parent.productCategoryId" class="mb-8">

        <!--#region 父類別標題 -->
        <div class="flex items-center gap-3 mb-4">
          <img
            :src="getCategoryImg(group.parent)"
            class="w-8 h-8 rounded-card object-cover border border-border-soft"
          />
          <span class="text-base font-bold text-ink-900">{{ group.parent.productCategoryName }}</span>
          <div class="flex-1 h-px bg-border-soft"></div>
        </div>
        <!-- #endregion -->

        <!--#region 子類別卡片列表 -->
        <div class="grid grid-cols-5 md:grid-cols-7 lg:grid-cols-10 gap-3">
          <RouterLink
            v-for="child in group.children"
            :key="child.productCategoryId"
            :to="{ name: 'mall-category', params: { id: child.productCategoryId } }"
            class="bg-page-bg border border-border-soft rounded-card overflow-hidden hover:border-ink-300 transition-colors cursor-pointer flex flex-col items-center p-3 gap-2"
          >
            <div class="w-full aspect-square bg-surface-muted rounded-card overflow-hidden">
              <img :src="getCategoryImg(child)" alt="" class="w-full h-full object-cover" />
            </div>
            <span class="text-xs text-ink-900 text-center leading-tight line-clamp-2">
              {{ child.productCategoryName }}
            </span>
          </RouterLink>

          <!--#region 若無子類別，直接顯示父類別本身可點 -->
          <RouterLink
            v-if="group.children.length === 0"
            :to="{ name: 'mall-category', params: { id: group.parent.productCategoryId } }"
            class="bg-page-bg border border-border-soft rounded-card overflow-hidden hover:border-ink-300 transition-colors cursor-pointer flex flex-col items-center p-3 gap-2"
          >
            <div class="w-full aspect-square bg-surface-muted rounded-card overflow-hidden">
              <img :src="getCategoryImg(group.parent)" alt="" class="w-full h-full object-cover" />
            </div>
            <span class="text-xs text-ink-900 text-center">{{ group.parent.productCategoryName }}</span>
          </RouterLink>
          <!-- #endregion -->
        </div>
        <!-- #endregion -->

      </div>
      <!-- #endregion -->

    </div>
  </div>
  <!-- #endregion -->
</template>
