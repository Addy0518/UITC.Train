import accountApiInstance from '@/api/accountInstance.js';

// 聊天室相關 API ===========================================================

// 取得歷史訊息
export const GetMessageHistory = (targetUserId) =>
  accountApiInstance.get(`/Chat/GetMessageHistory`, {
    params: { targetUserId },
  });

// 取得聊天對象列表
export const getChatUserList = () => accountApiInstance.get(`/Chat/GetChatUserList`);

// 改變已讀狀態
export const updateReadStatus = (senderId) =>
  accountApiInstance.put(`/Chat/UpdateReadStatus`, null, { params: { senderId } });
