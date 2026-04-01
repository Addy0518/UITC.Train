import accountApiInstance from '@/api/accountInstance.js';

export const getAllLedger = (queryString = '') =>
  accountApiInstance.get(`/Ledger/GetAllLedger${queryString}`);
// 使用者註冊
export const registerApi = (userRegister) =>
  accountApiInstance.post(`/User/Register`, userRegister);
// 使用者登入
export const loginApi = (userLogin) => accountApiInstance.post(`/User/Login`, userLogin);

// export const getAllLedger2 = async (cateId = [], selectdate = null) => {
//   let queryString = '';
//   if (cateId && cateId.length > 0) {
//     queryString += `?` + cateId.map((id) => `categoryId=${id}`).join(`&`);
//   }
//   if (selectdate) {
//     // 如果用 toString 的話怕格式會不一樣 , 而用 ISO 再把 t 後面的時間去掉也不行 , 因為時區傳患的關係 , 所以用 英文格式 en-CA 轉成 1990-01-01 的格式

//     const datestring = selectdate.toLocaleDateString('en-CA');
//     queryString += (url.includes('?') ? '&' : '?') + `date=${datestring}`;
//   }

//   const result = {
//     isSuccess: true,
//     data: null,
//     message: '',
//   };

//   try {
//     const response = await todoApiInstance.get(`/Ledger/GetAllLedger${queryString}`);
//     result.isSuccess = true;
//     result.data = response.data;
//     response.message = '';
//   } catch (error) {
//     console.log(error);
//     result.isSuccess = false;
//     result.data = error.response.data;
//   }

//   return result;
// };
