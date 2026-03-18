# Vue 心得

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
