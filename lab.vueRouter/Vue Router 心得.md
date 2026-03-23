# Vue Router 心得

1. 路由設定

```html
<!-- router => index.js -->
<!-- 建立一個名為 router 的路由實體  -->
const router = createRouter({ history: createWebHistory(import.meta.env.BASE_URL), routes: [ {
<!-- 路徑 -->
path: '/',
<!-- 名稱 -->
name: 'home',
<!-- 元件 -->
component: HomeView, }, ], })
<!-- 導出路由來引用 -->
export default router

<!-- main.js -->
<!-- 引用剛剛導出的路由 -->
import router from './router' const app = createApp(App) app.use(createPinia())
<!-- 使用他 -->
app.use(router)
```

2. 路由參數

```html
<!-- 可以給路由加上動態參數  -->
{ path: '/User/:id', name: 'User', component: UserView, }, 在元件裡就能用 route 抓到參數
<template>User{{ $route.params.id }}</template>
```

```html
<!-- 也能用 watch 直接在元件監聽參數變化 , 並抓出舊的跟新的參數 -->
watch( () => route.params.id, (newId, oldId) => { (console.log('新的', newId), console.log('舊的',
oldId)); }, );
```

```html
<!-- 也可以用路由守衛偵測當今天參數變化時 , 要做什麼動作 (比如導到其他網頁) -->
onBeforeRouteUpdate((to) => { value.value = to.params.id; });
```

```html
<!-- 也可以設定一個全域的捕捉錯誤路由 , 只要不是正常的網址就導向這裡 -->
{ path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
```

3. 路由匹配

```html
<!-- 可以給一個 component 設定好幾個路由 , 並規定當接收參數是數字或其他時要跳轉到哪 -->
{
<!-- 只接收數字 -->
path: '/User/:id(\\d+)', name: 'User', component: UserView, }, {
<!-- 除了數字以外 -->
path: '/User/:Name', name: 'UserName', component: UserView, },
```

4. 嵌套路由

```html
<!-- 可以在本來的 component 加上子路由 , 當今天網址是 User/id的時候 , 同時會顯示 UserProfile -->
{ path: '/User/:id(\\d+)', name: 'User', component: UserView, children: [ { path: 'profile',
component: UserProfile, }, ], },

<template>
  <span>{{ value }}</span>
  User{{ $route.params.id }}{{ $route.params.Name }}
  <div>
    <!-- 嵌套在裡面 -->
    <RouterView />
  </div>
</template>
```

```html
<!-- 這樣寫可以做到一層一層層遞式的路由 -->
const routes = [
  {
    path: '/admin',
    children: [
      { path: '', component: AdminOverview },
      { path: 'users', component: AdminUserList },
      { path: 'users/:id', component: AdminUserDetails },
    ],
  },
]
```

5. 命名路由

```html
<!-- 剛剛在 App.vue 設定的 RouteLink , 也可以改成用直接寫名稱跟參數 , 這樣就不會顯示我們剛剛的子路由 , 因為這是直接指定 -->
<template>
  <RouterLink :to="{ name: 'User', params: { id: 123 } }">前往用戶123</RouterLink>
  <RouterLink to="/User/999">前往用戶999</RouterLink>
  <RouterView />
</template>
```

6. 路由導航

```html
<!-- 先在組件拿到 router -->
const router = useRouter();
<!-- 寫一個方法把路由 push 上去 , 有點像是往下一頁走的感覺 -->
const gotoRouter = () => {
  router.push({
    name: 'UserName',
    params: { Name: 'Alex' },
  });
};
<!-- 直接綁在 click 事件上 -->
<button @click="gotoRouter">Alex</button>

<!-- 也可以直接寫在事件上 , 比較精簡 -->
<button @click="router.push('/User/Andy')">Andy</button>

```

``` html
<!-- 剛剛那樣是往下一頁走 , 這次設定往上一頁 -->
<!-- -1就是上一步 , 1 則是往下一步 , 以此類推 -->
const goBack = () => {
  router.go(-1);
};

<button @click="goBack">回上一頁</button>
```

7. 命名視圖

```html
<!-- 我在元件上多放了一個 RouterView , 一次渲染兩個 component -->
 <div>
    <RouterView name="Left" />
    <RouterView name="Right" />
  </div>

  {
      path: '/User/:id(\\d+)?',

      name: 'User',
      component: UserView,
      children: [
        {
          path: '',
          name: 'userprofile',
          <!-- 在路徑這裡的 component 把兩個都加進去就行了 -->
          components: { Left: UserProfile, Right: UserRight },
        },
      ],
    },
```

8. prop 從路由傳送

```html
<!-- 剛剛在傳遞參數的時候是用 $route 去抓 , 現在改用 props -->
<!-- 定義一個 defineProps -->
defineProps({
  id: String,
});

<!-- 在路由這裡把 props 改 true 就可以了 -->
 path: '/User/:id(\\d+)?',
      props: true,
      name: 'User',
      component: UserView,
```

9. 路由守衛

```html

<!-- 路由守衛 , to 代表要去的網址 , from 則是來自哪個網址 -->
router.beforeEach((to, from) => {
  <!-- 可以 console.log 出來看看裡面都有啥 -->
  console.log(to);
  <!-- 簡單的攔截器 -->
  if (to.name === 'User' || to.name === 'UserName') {
    return { name: 'NotFound' };
  }
  console.log(`to=>${to.name}`);
  console.log(`from=>${from.name}`);
});

<!-- 還有一個叫做 beforeResolve , 在 beforeEach 後面執行 -->
router.beforeResolve(async (to) => {
  console.log(`我是beforeResolve=>${to}`);
});
```

```html
<!-- 在守衛內可以全域注入 -->
app.provide('global', 'hello injections');

router.beforeEach((to, from) => {
  <!-- 顯示 hello injections -->
  const global = inject('global');
  console.log(global);
});
```

```html
<!-- 也可以直接在路徑上用 -->
 {
      path: '/User/:id(\\d+)?',
      props: true,
      name: 'UserById',
      component: UserView,
      <!-- 要注意的是 , 當今天是套在父路由上時 , 如果我剛進來時會觸發  -->
      <!-- 但當我在子路由間切換 (比如現在的 Left 跟 Right ) , 就不會觸發  -->
      beforeEnter: (to, from) => {
        console.log('進來home');
      },
      children: [
        {
          path: '',
          name: 'userprofile',
          components: { Left: UserProfile, Right: UserRight },
        },
      ],
    },
```

```html
<!-- 其他的守衛 -->
<!-- 當今天網址餐被更新時 (比如剛剛的兩個子路由切換 , 就可以用這個偵測) -->
beforeRouteUpdate
<!-- 當要離開時 -->
beforeRouteLeave
```

10. meta 路由信息

```html
<!-- 在路由這裡可以加入 meta 讓路由攜帶訊息 -->
 {
      path: '/User/:Name',
      name: 'UserName',
      component: UserView,
      meta: { required: true },
    },

<!-- 在守衛這裡就可以驗證訊息  -->
router.beforeEach((to, from) => {
  if (to.meta.required) {
    console.log('可以登入');
    return { path: '/:pathMatch(.*)*' };
  } else {
    console.log('不能登入');
  }
});
```

11. 路遊懶加載

```html
<!-- 可以把本來的 import 改成這樣寫 , 再使用這個路由時才載入她 -->
<!-- import UserRight from '@/views/UserRight.vue'; -->
const UserRight = () => import('@/views/UserRight.vue');

<!-- 或者這樣寫 : -->
  {
      path: '/User/:Name',
      name: 'UserName',
      <!-- 寫在裡面 -->
      component: () => import('@/views/UserView.vue'),
      meta: { required: true },
    },

```