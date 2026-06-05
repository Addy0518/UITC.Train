import accountApiInstance from '@/api/accountInstance.js';

// 管理員的用戶相關 API ===========================================================

// 取得所有使用者資訊
export const getAllUser = (request) =>
  accountApiInstance.get(`admin/User/GetAllUser`, { params: request });

// 取得使用者詳細資訊
export const getUserDetails = (userId) =>
  accountApiInstance.get(`admin/User/GetUserDetails?userId=${userId}`);

//  軟刪除單一用戶
export const deleteUser = (userId) =>
  accountApiInstance.put(`admin/User/DeleteUser?userId=${userId}`);
