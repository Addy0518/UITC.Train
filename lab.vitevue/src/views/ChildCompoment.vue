<script setup>
const [firstName, modifiers] = defineModel('firstName');
console.log(modifiers);
// const numberrr = 0;

// const emit = defineEmits({
//   add1: ({ one, two }) => {
//     if (one > 5 && two > 10) {
//       console.log('Yeeee');
//       return true;
//     } else {
//       console.log('NOOOOOO');
//       return false;
//     }
//   },
// });

// function handleClick() {
//   emit('add1', { one: 20, two: 20 });
// }
</script>

<template>
  <button
    class="rounded-lg bg-blue-500 px-4 py-2 font-semibold text-white shadow-md transition duration-150 ease-in-out hover:bg-blue-600 active:scale-95"
  >
    <slot></slot>
  </button>

  <span style="color: blue">
    <slot name="foo"></slot>
  </span>
  <!-- <input type="text" v-model="firstName" /> -->
  <!-- <button
    type="button"
    @click="handleClick"
    class="rounded-lg bg-blue-500 px-4 py-2 font-semibold text-white shadow-md transition duration-150 ease-in-out hover:bg-blue-600 active:scale-95"
  >
    Click me
  </button> -->
</template>
