<script setup>
import { computed, ref } from 'vue'

// 因為 id 是要可變動的用 let
let id = 0
// 輸入框
const newBook = ref('')
// 管理畫面是新增書本還是租借
const borrow = ref(false)

// 管理租借畫面是以租借還是未租借
const borrowHave = ref(false)

const searchquery = ref('')

// 創建模擬資料
const books = ref([])

// 查閱書本
const searchBooks = computed(() => {
  return books.value.filter((b) => b.name.includes(searchquery.value))
})

// 顯示租借 or 未租借
const borrowBooks = computed(() => {
  return borrowHave.value ? searchBooks.value.filter((b) => b.borrowDone) : searchBooks.value
})

// 新增書本
function addBook() {
  id++
  books.value.push({ id: id, text: `第${id}本書`, name: newBook.value, borrowDone: false })
  newBook.value = ''
}

// 刪除書本
function deleteBooks(book) {
  window.alert('確定刪除?')
  books.value = books.value.filter((b) => b !== book)
}
</script>

<template>
  <h1>Todo練習專案</h1>
  <div style="display: flex; align-items: center; gap: 10px">
    <h2 v-if="borrow">書本租借系統</h2>
    <h2 v-else>書本管理系統</h2>
    <button style="height: 20px" @click="borrow = !borrow">切換</button>
  </div>

  <!-- 管理畫面 -->
  <template v-if="!borrow">
    <form @submit.prevent="addBook">
      <input type="text" placeholder="請輸入要加入的書本名稱" v-model="newBook" required />
      <button style="margin-left: 20px">新增</button>
    </form>

    <ul>
      <li v-for="book in books" :key="book.id">
        <span style="padding-right: 20px">{{ book.text }}</span>
        <span> <input type="text" v-model="book.name" /></span>
        <span style="padding-left: 20px"><button @click="deleteBooks(book)">刪除</button></span>
      </li>
    </ul>
  </template>

  <!-- 租借畫面 -->
  <template v-if="borrow">
    <form @submit.prevent>
      <input type="text" placeholder="請輸入要查閱的書本名稱" v-model="searchquery" />
      <button style="margin-left: 20px" @click="borrowHave = !borrowHave">
        {{ borrowHave ? '已租借' : '全部' }}
      </button>
    </form>

    <ul>
      <li v-for="book in borrowBooks" :key="book.id">
        <span style="padding-right: 20px"> {{ book.text }}</span>
        <span> {{ book.name }}</span>
        <span style="padding-left: 20px">
          <input type="checkbox" v-model="book.borrowDone" />租借
        </span>
      </li>
    </ul>
  </template>
</template>

<style></style>
