import accountApiInstance from '@/api/accountInstance.js';

// 使用者相關 API ===========================================================

// 使用者註冊
export const registerApi = (userRegister) =>
  accountApiInstance.post(`User/Register`, userRegister);
// 使用者登入
export const loginApi = (userLogin) => accountApiInstance.post(`User/Login`, userLogin);
// 使用者登出
export const logoutApi = () => accountApiInstance.post(`User/Logout`);

// 使用者大頭照新增編輯
export const userHeadShot = (userFile) =>
  accountApiInstance.post(`User/UserHeadShotUpload`, userFile);
