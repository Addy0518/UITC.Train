<script setup></script>

<template>
  <RouterLink :to="{ name: 'UserById', params: { id: 123 } }">前往用戶123</RouterLink>
  <RouterLink to="/User/999">前往用戶999</RouterLink>
  <RouterView />
</template>

<style scoped></style>
