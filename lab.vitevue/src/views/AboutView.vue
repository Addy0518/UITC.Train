<script setup>
import { ref } from 'vue';
const x = ref(0);
</script>

<template>
  <div class="about">
    <h1>This is an about7778 page</h1>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
