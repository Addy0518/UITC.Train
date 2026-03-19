<script setup>
import TheWelcome from '../components/TheWelcome.vue';

const x = '';
if (!x) {
  console.log('真值2');
} else {
  console.log('假值2');
}

const y = null;
if (y) {
  console.log('假值我不做');
}

if (0 && 'dog') {
  console.log('123');
}
</script>

<template>
  <main>
    <TheWelcome />
    <p>122222</p>
  </main>
</template>
