import accountApiInstance from '@/api/accountInstance.js';

// 使用者相關 API ===========================================================

// 使用者註冊
export const registerApi = (userRegister) => accountApiInstance.post(`User/Register`, userRegister);
// 使用者登入
export const loginApi = (userLogin) => accountApiInstance.post(`User/Login`, userLogin);
// Google 第三方登入
export const googleLoginApi = (request) =>
  accountApiInstance.post(`User/GoogleLogin`, request);
// 使用者登出
export const logoutApi = () => accountApiInstance.post(`User/Logout`);

// 使用者大頭照新增編輯
export const userHeadShot = (userFile) =>
  accountApiInstance.post(`User/UserHeadShotUpload`, userFile);

// 查看登入者資訊
export const getMyUser = () => accountApiInstance.get(`User/GetUser`);

// 查看指定使用者資訊
export const getOneUser = (id) => accountApiInstance.get(`User/GetOneUser?userId=${id}`);

// 更新使用者資訊
export const updateUser = (request) => accountApiInstance.put(`User/UpdateUser`, request);

// 更新使用者密碼 ( 已登入 )
export const updatePassword = (request) => accountApiInstance.put(`User/UpdatePassword`, request);

// 更新使用者密碼 ( 忘記密碼 )
export const forgetUpdatePassword = (request) =>
  accountApiInstance.post(`User/ForgetUpdatePassword`, request);

// 寄送忘記密碼的驗證碼
export const sendVerfiyCode = (request) => accountApiInstance.post(`User/SendVerfiyCode`, request);
