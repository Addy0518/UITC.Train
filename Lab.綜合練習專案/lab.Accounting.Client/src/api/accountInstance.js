import axios from 'axios';
import { useAuthStore } from '@/stores/auth';
import router from '@/router';
import Swal from 'sweetalert2';

/*
   創建一個 axios ( VITE_TODO_BASE_URL 是放在環境設定 ( env ) 的 url )
*/
const instance = axios.create({
  baseURL: import.meta.env.VITE_TODO_BASE_URL,
});

/*
   Api 發送請求的攔截器
*/
instance.interceptors.request.use(
  function (config) {
    /*
      統一把所有 api header 加上 token
    */
    const authStore = useAuthStore();
    if (authStore.token) {
      config.headers.Authorization = `Bearer ${authStore.token}`;
    }
    /*
      記得 return
    */
    return config;
  },
  function (error) {
    /*
       用 Promise.reject 回傳 error 給每一個 Api 的 try-catch error
    */
    return Promise.reject(error);
  },
);

/*
   Api 回傳資料的攔截器
*/
instance.interceptors.response.use(
  function (response) {
    return response;
  },
  function (error) {
    const { status, data } = error.response;
    if (status === 400) {
      /*
         這裡是看 return Promise.reject(error); 發現 驗證錯誤都在 data 的 errors 裡 , 分成好幾個物件
         所以用 flatMap 把她攤平 (本來是 {[],[],[]}變成 "","","") , 再 join 串起來
      */
      const errorMsg = Object.values(data.errors)
        .flatMap((x) => x)
        .join('\r\n');
      Swal.fire({
        icon: 'error',
        title: errorMsg,
      });
    }
    if (status === 401) {
      alert('登入時間過期，請重新登入！');

      router.push({ name: 'login' });
    }
    if (status === 500) {
      const errorMsg = `狀況 : ${data.message} \r\n Url : ${data.error500.instance} \r\n 錯誤訊息 : ${data.error500.title}`;
      Swal.fire({
        icon: 'error',
        title: `連線錯誤 : ${errorMsg}`,
      });
    }

    return Promise.reject(error);
  },
);
/*
   記得 export 出去
*/
export default instance;
