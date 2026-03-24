<script setup>
import { RouterView } from 'vue-router';
</script>

<template>
  <RouterLink :to="{ name: 'test' }">test</RouterLink>
  <RouterView />
</template>

<style scoped></style>
