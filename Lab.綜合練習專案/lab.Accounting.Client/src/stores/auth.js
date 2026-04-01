import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', () => {
   const token=ref(localStorage.getItem('token') || null);
   const userId = ref(Number(localStorage.getItem('userId')) || null);
   const userName = ref(localStorage.getItem('userName') || null);


  // 登入時存入
  function setAuth(data) {
    token.value = data.token;
    userId.value = data.userId;
    userName.value = data.userName;
    // 存 localStorage 讓重新整理後不會消失
    localStorage.setItem('token', data.token);
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('userName', data.userName);
  }

   // 登出時清除
  function clearAuth() {
    token.value = null;
    userId.value = null;
    userName.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('userName');
  }

  return { token, userId, userName, setAuth, clearAuth };
});
