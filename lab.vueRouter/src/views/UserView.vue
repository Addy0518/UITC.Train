<script setup>
import { watch, ref } from 'vue';
import { useRoute, onBeforeRouteUpdate, useRouter } from 'vue-router';

defineProps({
  id: String,
});

const router = useRouter();

const gotoRouter = () => {
  router.push({
    name: 'UserName',
    params: { Name: 'Alex' },
  });
};

const goBack = () => {
  router.go(-1);
};

const route = useRoute();
const value = ref(0);
watch(
  () => route.params.id,
  (newId, oldId) => {
    (console.log('新的', newId), console.log('舊的', oldId));
    if (newId) fetchData(newId);
  },
);

async function fetchData(id) {
  try {
    const Id = id;
    console.log(Id);
  } catch (err) {
    console.log(err);
  }
}

onBeforeRouteUpdate((to) => {
  value.value = to.params.id;
});
</script>

<template>
  <span>{{ value }}</span>
  User{{ id }}{{ $route.params.Name }}
  <button @click="gotoRouter">Alex</button>
  <button @click="router.push('/User/Andy')">Andy</button>
  <button @click="goBack">回上一頁</button>
  <div>
    <RouterView name="Left" />
    <RouterView name="Right" />
  </div>
</template>
