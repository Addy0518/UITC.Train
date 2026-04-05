import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useAuthStore = defineStore(
  'auth',
  () => {
    const token = ref(null);
    const userId = ref(null);
    const userName = ref(null);

    // 登入時存入
    function setAuth(data) {
      token.value = data.token;
      userId.value = data.userId;
      userName.value = data.userName;
    }

    // 登出時清除
    function clearAuth() {
      token.value = null;
      userId.value = null;
      userName.value = null;
    }

    return { token, userId, userName, setAuth, clearAuth };
  },
  {
    persist: true,
  },
);
