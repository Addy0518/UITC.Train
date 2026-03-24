<script setup>
import { useCounterStore, userCounterStore2 } from '@/stores/counter'
import { onMounted, ref } from 'vue'
// import { computed, watch } from 'vue'
const store = useCounterStore()
const store2 = userCounterStore2()

// console.log(store)
console.log('aaa' + store2.doubleCountPlusOne)
// setTimeout(() => {
//   store.increment()
// }, 1000)

// const doubleValue = computed(() => store.doubleCount)
// watch(doubleValue, () => console.log(doubleValue.value))
// store.count = store.count + 1 * 5
// console.log('重製前 ' + store.count)
onMounted(() => {
  const unsubscribe = store2.$onAction(
    ({
      name, // action 名称
      args, // 传递给 action 的参数数组
      after, // 在 action 返回或解决后的钩子
      onError, // action 抛出或拒绝的钩子
    }) => {
      // 为这个特定的 action 调用提供一个共享变量
      const startTime = Date.now()
      // 这将在执行 "store "的 action 之前触发。
      console.log(`Start "${name}" with params [${args.join(', ')}].`)

      // 这将在 action 成功并完全运行后触发。
      // 它等待着任何返回的 promise
      after((result) => {
        console.log(`Finished "${name}" after ${Date.now() - startTime}ms.\nResult: ${result}.`)
      })

      // 如果 action 抛出或返回一个拒绝的 promise，这将触发
      onError((error) => {
        console.warn(`Failed "${name}" after ${Date.now() - startTime}ms.\nError: ${error}.`)
      })
    },
  )

  store.$subscribe((mutation, state) => {
    console.log('修改類型:', mutation.type)

    console.log('最新的 count:', state.count)
    console.log('最新的 items:', state.items)
  })
  store.$patch({
    count: store.count * 2,
  })

  store.$patch((state) => {
    state.items.push({ a: 'ccc', b: 222222 })
  })
})
const inputId = ref(0)
const message = ref('')
async function hendleLogin() {
  message.value = await store2.LoginUser(inputId.value)
  console.log(inputId.value)
}
</script>

<template>
  <h1>123</h1>
  <p>121121212</p>
  <input type="text" v-model="inputId" placeholder="輸入id" />
  <p>{{ store.items }}</p>
  <button @click="hendleLogin()">登入</button>
  <p>登入成功 : {{ store2.userData?.name }}</p>
  <button @click="store2.test">測試插件</button>
</template>
