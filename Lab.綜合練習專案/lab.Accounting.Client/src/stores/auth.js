import { ref } from 'vue';
import { defineStore } from 'pinia';

/*
   定義 pinia 的設定
*/
export const useAuthStore = defineStore(
  'auth',
  () => {
    const token = ref(null);
    const userId = ref(null);
    const userName = ref(null);

    /*
      登入時存入 token 資料
    */
    function setAuth(data) {
      token.value = data.token;
      userId.value = data.userId;
      userName.value = data.userName;
    }
    /*
      登出時清除
    */
    function clearAuth() {
      token.value = null;
      userId.value = null;
      userName.value = null;
    }
    /*
      重要 => 記得回傳這些資料並 export 傳出去 , 供其他地方使用
    */
    return { token, userId, userName, setAuth, clearAuth };
  },
  /*
    使用 pinia-plugin-persistedstate , 自動把 pinia 的資料存到 localstorage
  */
  {
    persist: true,
  },
);
