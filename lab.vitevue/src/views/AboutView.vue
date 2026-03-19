<script setup>
import { onMounted, ref } from 'vue';
// import HomeView from './HomeView.vue';
const props = defineProps(['foo']);
console.log(props.foo);
const x = ref('hello');

const sets = ref([
  [1, 2, 3, 4, 5, 6],
  [7, 8, 9, 10, 11],
]);

function even(number) {
  return number.filter((num) => num % 2 === 0);
}

const elTest = document.getElementById('test');
console.log('SetUP', elTest);

onMounted(() => {
  const elTest2 = document.getElementById('test');
  console.log('onMounted', elTest2);
});
</script>

<template>
  <!-- <HomeView /> -->
  <div class="about">
    <h1>{{ x }}</h1>
    <ul v-for="set in sets">
      <li v-for="n in even(set)">
        <p>{{ n }}</p>
      </li>
    </ul>
  </div>
  <span id="test  ">TEST</span>
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
s
