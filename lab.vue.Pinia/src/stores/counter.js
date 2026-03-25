import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
export const useCounterStore = defineStore('counter', () => {
  const count = ref(0)
  const doubleCount = computed(() => count.value * 4)
  function increment() {
    count.value--
  }
  function add() {
    count.value++
  }
  function $resert() {
    count.value = 0
  }
  const name = 'Andy'
  const items = ref([])

  return { items, name, count, doubleCount, increment, $resert, add }
})

export const userCounterStore2 = defineStore('counter2', {
  state: () => ({
    count: ref(1),
    User: [{ name: 'Andy', userId: 1 }],
    userData: null,
  }),

  actions: {
    async LoginUser(userId) {
      try {
        const user = await this.User.find((id) => id.userId === Number(userId))
        this.userData = user
        return `Welcome ${user.name}`
      } catch (err) {
        return err
      }
    },

    test() {
      console.log(this.hello)
      console.log(this.secret)
    },
  },

  getters: {
    doubleCount: (state) => state.count * 2,

    doubleCountPlusOne() {
      return this.doubleCount + 1
    },

    countUserId: (state) => {
      return (userId) => state.User.find((id) => userId === id.userId)
    },
  },
})
