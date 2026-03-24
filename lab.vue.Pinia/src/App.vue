<script setup>
// import { defineStore } from 'pinia'

// const useAlertsStore = defineStore('counter', {
//   state: () => ({ count: 0, name: 'Eduardo' }),
//   // getters 是類似 vue 的 computed 計算屬性
//   getters: {
//     doubleCount: (state) => state.count * 2,
//   },
//   // actions 則像是方法 methods
//   actions: {
//     increment() {
//       this.count++
//     },
//   },
// })
</script>

<template>
  <RouterLink :to="{ name: 'about' }">去 About me</RouterLink>
  <RouterView />
</template>
