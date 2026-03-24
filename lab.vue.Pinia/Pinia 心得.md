# Pinia 心得

### Pinia 是類似前端的 " 全域資料庫 " , 能夠把要存在前端的資料都放在這 , 並傳給不同的 component , 這樣就不用父組件 => 子組件 => 子組件這樣一層一層 , 主要存一些資料的狀態等等 (比如會員登入狀態)

### 安裝跟設定 => https://pinia.vuejs.org/zh/getting-started.html

1. 用 definestore 定義 store 資料庫 , 並在裡面修改各種參數

```html
<!-- 第一種寫法 (Option Store) -->
<script setup>
  import { defineStore } from 'pinia'

  const useAlertsStore = defineStore('counter', {
    <!-- state 是 store 的數據 , 類似 ref()  -->
    state: () => ({ count: 0, name: 'Eduardo' }),
    <!-- getters 是類似 vue 的 computed() 計算屬性 -->
    <!-- 去修改 store 的參數的 -->
    getters: {
      doubleCount: (state) => state.count * 2,
    },
    <!-- actions 則像是方法 methods() -->
    actions: {
      increment() {
        this.count++
      },
    },
  })
</script>
```

```html
<!-- 第二種寫法 (Setup Store) -->
<script setup>
  export const useCounterStore = defineStore('counter', () => {
    const count = ref(0)
    const name = ref('Eduardo')
    const doubleCount = computed(() => count.value * 2)
    function increment() {
      count.value++
    }

    return { count, name, doubleCount, increment }
  })
</script>
```

```html
<!-- 使用他 -->
<script setup>
  import { useCounterStore } from '@/stores/counter'
  import { computed, watch } from 'vue'
  <!-- 直接注入就行 -->
  const store = useCounterStore()
  console.log(store)
  <!-- 可以使用方法 -->
  setTimeout(() => {
    store.increment()
  }, 1000)
  <!-- 也可以使用計算式 -->
  const doubleValue = computed(() => store.doubleCount)
  <!-- 順序是近來之後看到 settimeOut 要等 10 秒 , 就先執行 doubleCount , 再回去執行 increment , 最後 watch 監聽到 -->
  watch(doubleValue, () => console.log(doubleValue.value))
</script>
```

```html
<!-- 可以把 store 裡的方法解構 -->
<script setup>
  const { name, doubleCount } = useCounterStore(store)
</script>
```

2. State

```html
<!-- 要傳遞的資料就都放在 state -->
<script setup>
  const useStore = defineStore('storeId', {
    state: () => {
      return {
        count: 0,
        name: 'Andy',
        isAdmin: true,
      }
    },
  })
</script>
```

```html
<script setup>
  <!-- 可以直接讀取 -->
  store.count = store.count + 1 * 5
  console.log(store.count)
</script>
```

```html
<script setup>
  export const useCounterStore = defineStore('counter', () => {
    const count = ref(0)
    const doubleCount = computed(() => count.value * 4)
    function increment() {
      count.value--
    }
    <!-- 也可以在本來 store 的地方加 $resert 方法來重置 -->
    function $resert() {
      count.value = 0
    }
    return { count, doubleCount, increment, $resert }
  })
</script>

store.count = store.count + 1 * 5 console.log('重製前 ' + store.count) store.$resert()
console.log('重製後 ' + store.count)
```

```html
<script setup>
  <!-- 除了剛剛的改法以外 , 這兩種也可以改 state 的資料 , patch 能在同一時間改多個屬性 -->
  onMounted(() => {
    store.$patch({
      count: store.count * 2,
    })

    store.$patch((state) => {
      state.items.push({ a: 'ccc', b: 222222 })
    })
  })

  <!-- 或是初始整個 state -->
  pinia.state.value = {}
</script>
```

```html
<!-- $subscribe 則是跟 Vue 的 watch 差不多 , 好處是他在 patch 後只會觸發一次 -->
<script setup>
  // import { MutationType } from 'pinia'
  onMounted(() => {
    store.$subscribe((mutation, state) => {
      console.log('修改類型:', mutation.type)

      console.log('最新的 count:', state.count)
      console.log('最新的 items:', state.items)
    })
    store.$patch({
      count: store.count * 2,
    })

    store.$patch((state) => {
      state.items.push({ a: 'ccc', b: 222222 })
    })
  })
</script>
```

3. Getter

```html
<script setup>
  export const userCounterStore2 = defineStore('counter2', {
    state: () => ({
      count: ref(1),
    }),
    // 接收現在這個 store 的 state 並計算內部函數
    getters: {
      doubleCount: (state) => state.count * 2,
    },
  })
</script>
```

```html
<script setup>
  getters: {
     doubleCount: (state) => state.count * 2,

    <!-- getter 裡面可以多個方法 , 也可以用 this 在方法裡指定其他方法 -->
     doubleCountPlusOne() {
       return this.doubleCount + 1
     },
   },
</script>
```

```html
也可以向 getter 傳送參數
<script setup>
  export const userCounterStore2 = defineStore('counter2', {
    state: () => ({
      count: ref(1),
      <!-- 我先定義一個 User 物件 -->
      User: [{ name: 'Andy', userId: 1 }],
    }),

    getters: {
      <!-- 這裡定義接收一個 userId , 跟 User 比對並回傳 -->
      countUserId: (state) => {
        return (userId) => state.User.find((id) => userId === id.userId)
      },
    },
  })
</script>

<!-- 最後傳遞參數 , 比對成功就回傳 User -->
<p>User : {{ store2.countUserId(1) }}</p>
```

4. Action

```html
<script setup>
  export const userCounterStore2 = defineStore('counter2', {
    state: () => ({
      count: ref(1),
      User: [{ name: 'Andy', userId: 1 }],
    }),
    <!-- 定義業務邏輯 -->
    actions: {
      increment() {
        this.count++
      },
    },
  })
</script>
```

```html
<script setup>
    actions: {
      <!-- 跟 getter 不同的是它可以 await -->
      async LoginUser(userId) {
        try {
          const user = await this.User.find((id) => id.userId === Number(userId))
          this.userData = user
          return `Welcome ${user.name}`
        } catch (err) {
          return err
        }
      },
    },

  <!-- setup 這裡也非同步 -->
  const inputId = ref(0)
  const message = ref('')
  async function hendleLogin() {
    message.value = await store2.LoginUser(inputId.value)

  }


   <input type="text" v-model="inputId" placeholder="輸入id" />
   <button @click="hendleLogin()">登入</button>
   <p>登入成功 : {{ store2.userData?.name }}</p>
</script>
```

```html
<script setup>
  onMounted(() => {
    <!-- $onAction 跟 $subscribe 一樣是監聽式 , 他是監聽 Action 的 -->
    <!-- 建立在 onMouted , 一開始就監聽 -->
    const unsubscribe = store2.$onAction(
      ({
        name, // action 名稱
        args, // 傳給 action 的參數
        after, // 在 action 結束後可以做的事情
        onError, //抓到 action 錯誤的處理
      }) => {
        <!-- 訂一個時間點 , 這樣每次 action 進來就計時 -->
        const startTime = Date.now()
        <!-- 這行會在 action 執行前觸發 -->
        console.log(`Start "${name}" with params [${args.join(', ')}].`)

        <!-- after 則是在 action 結束後觸發 -->
        after((result) => {
          console.log(`Finished "${name}" after ${Date.now() - startTime}ms.\nResult: ${result}.`)
        })

        <!-- action 有錯誤則會在這觸發 -->
        onError((error) => {
          console.warn(`Failed "${name}" after ${Date.now() - startTime}ms.\nError: ${error}.`)
        })
      },
    )
  })

  <!-- 如果不想要監聽器了也可以手動關掉 -->
  unsubscribe()
</script>
```

5. 插件

```html
<!-- 可以自訂一些比如新增方法 , 靜態屬性等等 -->
<script setup>
  import { createApp } from 'vue'
  import { createPinia } from 'pinia'

  import App from './App.vue'
  import router from './router'

  const app = createApp(App)
  <!-- 先建立個靜態屬性 -->
  function SecretPiniaPlugin() {
    return { secret: 'the cake is a lie' }
  }
  const pinia = createPinia()
  // 把這個插件給 pinia
  pinia.use(SecretPiniaPlugin)
  // 使用他
  app.use(pinia)
  app.use(router)

  app.mount('#app')
</script>
```

```html
<script setup>
  <!-- 也可以這樣寫 -->
  pinia.use(({ store }) => {
    store.hello = 'world'
  })
</script>
```

```html
<script setup>
  <!-- 這樣全域的 pinia 都可以用 , 我拿剛剛得 store2 用 -->
   actions: {
    test() {
      console.log(this.hello)
      console.log(this.secret)
    },
  },
</script>
```
