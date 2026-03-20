<script setup>
import { capitalize, ref } from 'vue';
import ChildCompoment from './ChildCompoment.vue';

const childnumber = ref(100);
const name = ref('aaa');
function addchild(n) {
  console.log(n.one);
  console.log(n.two);
}
</script>

<template>
  <ChildCompoment>
    click
    <template v-slot:foo>
      <span>hhhh</span>
    </template>
  </ChildCompoment>
</template>

<style></style>
