import accountApiInstance from '@/api/accountInstance.js';

// 管理員的用戶相關 API ===========================================================

// 取得所有使用者資訊
export const getAllUser = (request) =>
  accountApiInstance.get(`admin/User/GetAllUser`, { params: request });

// 取得使用者詳細資訊
export const getUserDetails = (userId) =>
  accountApiInstance.get(`admin/User/GetUserDetails?userId=${userId}`);

//  軟刪除單一用戶
export const deleteUser = (userId, deleteReason) =>
  accountApiInstance.put(`admin/User/DeleteUser?userId=${userId}&deleteReason=${deleteReason}`);

//  復原已選取的用戶刪除狀態
export const updateUserDeleteStatus = (userId) =>
  accountApiInstance.put(`admin/User/UpdateUserDeleteStatus?userId=${userId}`);
