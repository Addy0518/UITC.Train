# Vue 心得

### 基礎語法

1. ref 是指響應式資料 , 將所有資料內容包起來 , 當今天資料更新了 , 他就會自動更新網頁對應的地方

```html
<!-- setup 是把設定自動對應到網頁的簡寫 -->
<script setup>
  import { ref } from 'vue';
  // 定義變數
  const e = 'Hello';
</script>

<template>
  <!-- {{}} 使用變數 , split 是把字串切開變陣列-->
  <h1>{{e.split('')}}</h1>
</template>
```

2. 可以對類別或 id 等等做綁定

```html
<script setup>
  // 先定義一個變數的值為類別
  const titleClass = ref('title');
</script>

<template>
  <!-- :class 是 v-bind 的縮寫 , 前面記得要空格 -->
  <h1 :class="titleClass">Make me red</h1>
</template>

<style>
  .title {
    color: red;
  }
</style>
```

3. 事件監聽 , 可以在標籤寫 @事件 =' 函式或變數等等.. '

```html
<template>
  <!-- 這裡我直接寫動作 count++ 寫在監聽裡 , 也可以先寫函式在放進去也行 -->
  <button @click="count++">Count is: {{ count }}</button>
</template>
```

4. 雙向繫結 , 兩邊同時指向同一個值

```html
<template>
  <!-- v-model 是簡寫 , 大部分都這樣寫 -->
  <input v-model="text" placeholder="Type here" />
  <p>{{ text }}</p>
</template>
```

5. if..else.. , Vue 的寫法

```html
<script setup>
  import { ref } from 'vue';

  const awesome = ref(true);
  // 先設定一個 true false 的切換開關函式 , 記得是要比對 value , 這是 Vue 規定的
  function toggle() {
    awesome.value = !awesome.value;
  }
</script>

<template>
  <button @click="toggle">Toggle</button>
  <!-- 使用 v-If 跟 v-else 來切換 -->
  <h1 v-if="awesome">Vue is awesome!</h1>
  <h1 v-else>Oh no 😢</h1>
</template>
```

6. v-for 迴圈

```html
<script setup>
  import { ref } from 'vue';

  // 給每個 todo 對象一個唯一的 id
  let id = 0;
  // 輸入框的值
  const newTodo = ref('');
  // 第一層 id 是 0 +1 , 第二層是 1+1 .. 以此類推
  const todos = ref([
    { id: id++, text: 'Learn HTML' },
    { id: id++, text: 'Learn JavaScript' },
    { id: id++, text: 'Learn Vue' },
  ]);

  // 新增一筆 , 用 push 的方式將同格式的物件推進去 , 值就是輸入框輸入的
  function addTodo() {
    todos.value.push({ id: id++, text: newTodo.value });
    // 推完再清空輸入框
    newTodo.value = '';
  }

  // 通常刪除會用 filter , 意思是把滿足條件的留下來
  // 這裡是直接創立一個新的 todos 陣列 , 當每一個在 todos 裡的 todo 進來時比對是否是我選的 , 不是就留下 , 是就篩選掉 , 最後把這新陣列覆蓋舊陣列
  function removeTodo(todo) {
    todos.value = todos.value.filter((t) => t !== todo);
  }
</script>

<template>
  <!-- @submit 是執行這個 function -->
  <!-- prevent 是防止頁面刷新 -->
  <form @submit.prevent="addTodo">
    <input v-model="newTodo" required placeholder="new todo" />
    <button>Add Todo</button>
  </form>
  <ul>
    <!-- for 迴圈遍歷 todos , 索引則是 id -->
    <li v-for="todo in todos" :key="todo.id">
      {{ todo.text }}
      <button @click="removeTodo(todo)">X</button>
    </li>
  </ul>
</template>
```

7. computed 即時監聽變化 , 當內部有值變動時重新計算 , 沒有就傳舊的 , 節省效能

```html
<script setup>
  import { ref, computed } from 'vue';

  let id = 0;

  const newTodo = ref('');
  // hideCompleted 是開關決定要不要秀出以勾選的選項
  const hideCompleted = ref(false);
  const todos = ref([
    // done 代表以完成或未完成
    { id: id++, text: 'Learn HTML', done: true },
    { id: id++, text: 'Learn JavaScript', done: true },
    { id: id++, text: 'Learn Vue', done: false },
  ]);

  // 創建一個新陣列 filteredTodos , 用來計算要列出什麼物件
  const filteredTodos = computed(() => {
    // 判斷 hideCompleted 是 true 還是 false
    // filter((t) => !t.done) 篩選出當 done 反轉過來是 false(未完成) 時 , 就保留 , true (已完成) 就丟掉
    return hideCompleted.value ? todos.value.filter((t) => !t.done) : todos.value;
  });

  function addTodo() {
    todos.value.push({ id: id++, text: newTodo.value, done: false });
    newTodo.value = '';
  }

  function removeTodo(todo) {
    todos.value = todos.value.filter((t) => t !== todo);
  }
</script>

<template>
  <form @submit.prevent="addTodo">
    <input v-model="newTodo" required placeholder="new todo" />
    <button>Add Todo</button>
  </form>
  <ul>
    <!-- 表單 -->
    <li v-for="todo in filteredTodos" :key="todo.id">
      <input type="checkbox" v-model="todo.done" />
      <span :class="{ done: todo.done }">{{ todo.text }}</span>
      <button @click="removeTodo(todo)">X</button>
    </li>
  </ul>
  <!-- 開關決定要全秀出來還是以完成的就隱藏 -->
  <button @click="hideCompleted = !hideCompleted">
    {{ hideCompleted ? 'Show all' : 'Hide completed' }}
  </button>
</template>

<style>
  .done {
    text-decoration: line-through;
  }
</style>
```

8. onMounted 生命週期

```html
<script setup>
  import { ref, onMounted } from 'vue';

  const pElementRef = ref(null);
  // onMounted 能夠在標籤還沒生出來之前 , 先把值給他
  onMounted(() => {
    pElementRef.value.textContent = 'hrllo';
  });
</script>

<template>
  <!-- 這裡要引用 ref 的變數 , 但是在剛開始 <script setup> 的時候這個變數還是 null , 這時候就要用 onMounted 來 -->
  <p ref="pElementRef">Hello</p>
</template>
```

9. watch 偵聽器

```html
<script setup>
  import { ref, watch } from 'vue';

  const todoId = ref(1);
  const todoData = ref(null);

  // api 拿資料
  async function fetchData() {
    todoData.value = null;
    const res = await fetch(`https://jsonplaceholder.typicode.com/todos/${todoId.value}`);
    todoData.value = await res.json();
  }
  // 呼叫他
  fetchData();

  // 偵測 todoId 有無變動 , 有就執行一些比如改變變數的動作等等
  watch(todoId, fetchData);
</script>
```

### 深入組件

1. 局部註冊 (跟 Angular 的註冊 compoment 有點像)

```html
<script setup>
  // 先 import 組件進來
  import HomeView from './HomeView.vue';
</script>

<template>
  <!-- 在 template 引用就行 -->
  <HomeView />
</template>
```

2. props 父元件傳給子元件的單向傳遞方法

```html
<!-- 父元件 -->
<!-- 父元件先傳給子元件一個參數 => msg -->
<ChildCompoment v-model:light="light" v-model:swag="swag" msg="hello" />

<!-- 子元件 -->
<!--  要接參數的變數要用 {} 包起來 , 因為 defineProps(['msg']) 這裡面有所有父元件傳過來的參數 , 我們只要 msg 所以解構 props 確保只拿到這個參數 -->
<!-- defineProps(['msg']) 則要用 [] 起來 -->
<!-- defineProps 只能呼叫一次 , 所有父綁定的值都在這設定就好 -->
import { defineProps } from 'vue'; const { msg } = defineProps(['msg']);
```

```html
<!-- 也可以一次丟多個參數過去 , 但記得在子這裡改不了值 , 因為 prop 值是唯讀 -->
const post = { id: 1, title: 'HHHHHH', };
<!-- v-bind綁定 -->
<ChildCompoment v-model:light="light" v-model:swag="swag" v-bind="post" />
```

3. 組件 v-model , 使用 defineModel() 雙向綁定 , 讓父跟子能共同變化

```html
<!-- 父元件 -->
<script setup>
  import { ref } from 'vue';
  import ChildCompoment from './ChildCompoment.vue';

  const light = ref(false);
</script>

<template>
  <!-- 綁一個bool值到子元件上 -->
  <ChildCompoment v-model="light" />
</template>

<!-- 子元件 -->
<script setup>
  // 使用 defindModel 拿到父元件丟過來的值
  const model = defineModel();
</script>

<template>
  <button @click="model = !model">切換開關</button>
  <p>目前狀態：{{ model ? '開燈' : '關燈' }}</p>
</template>
```

```html
<!-- 也可以綁多個 v-model 傳遞給子元件 -->
<ChildCompoment v-model:light="light" v-model:swag="swag" />
```

4. 路由 , 依序從程式進入點 => 主畫面 => 子畫面 => 子元件.. 為順序

5. 觸發跟監聽事件

```html
<!-- 在父元件可以監聽子元件的動作 , 這裡我在子元件有一個按鈕事件 add1  -->
 <!-- 測試函式 -->
 function add1() {
  numberrr++;
}
</script>

<template>
 <!-- 用 emit 去觸發我定義的事件 (add1) -->
  <button
    type="button"
    @click="$emit('add1')"
    class="rounded-lg bg-blue-500 px-4 py-2 font-semibold text-white shadow-md transition duration-150 ease-in-out hover:bg-blue-600 active:scale-95"
  >
    Click me
  </button>
</template>

 <!-- 父元件一旦監聽到這個事件處發 , 就會觸發父元件這邊的事件 (addchild) -->
 <ChildCompoment @add1="addchild" />
```

```html
<!-- 也可以在事件後面帶參數 , 父元件監聽到後就會拿到這個值 -->
<!-- 子元件 -->
@click="$emit('add1', 10)"

<!-- 父元件 -->
<!-- 接到 n 然後賦值 -->
function addchild(n) { childnumber.value += n; }

<ChildCompoment @add1="addchild" />
```

```html
<!-- 可以在子元件先處理這個事件 , 預防一些狀況 , 父元件監聽到後就會拿到這個值 -->
<!-- 子元件 -->
<!-- 使用 defineEmits 處理監聽事件發生時的狀況 -->
const emit = defineEmits({ add1: ({ one, two }) => { if (one > 5 && two > 10) {
console.log('Yeeee'); return true; } else { console.log('NOOOOOO'); return false; } }, });
<!-- handleClick 函式則會丟參數進 emit 檢查  -->
function handleClick() { emit('add1', { one: 20, two: 20 }); }

<button @click="handleClick">Click me</button>

<!-- 父元件 -->
<!-- 接到 n 然後處理物件裡的 one 跟 two -->
function addchild(n) { console.log(n.one); console.log(n.two); }
```

6. 屬性繼承

```html
<!-- 在沒有指名類別的情況下 -->
<!-- 子元件有一個按鈕 -->
<button>Click meeeee</button>
<!-- 父元件放一個 class -->
<ChildCompoment class="large" />
<!-- 最後就會變成這樣 , 父的會默認給子的 -->
<button class="large">Click meeeee</button>
```

```html
<!-- 如果子已經有 class 了 , 他也會從父的繼承並合併 -->
<button class="btn">Click meeeee</button>
<!-- 變這樣 -->
<button class="btn large">Click meeeee</button>
```

```html
<!-- click 這種監聽事件也會繼承喔 -->
<ChildCompoment @click="childnumber++" />
```

```html
<!-- 想要禁用的話就在子元件這樣設定 -->
<script setup>
  defineOptions({
    inheritAttrs: false,
  });
  // ...setup 邏輯
</script>
```

7. Slot 插槽

```html
<!-- 在父元件插入要寫入的內容 -->
<ChildCompoment>click</ChildCompoment>

<!-- 子元件加上 slot , 就會把父的內容寫在這 , 包含 style 等等做綁定 -->
<button>
  <slot></slot>
</button>
```

```html
<!-- 可以給插槽上名字 -->
<!-- 父元件 -->
<ChildCompoment>
  click
  <!-- v-slot -->
  <template v-slot:foo>
    <span>hhhh</span>
  </template>
</ChildCompoment>
<!-- 子元件 -->
<span style="color: blue">
  <slot name="foo"></slot>
</span>
```
