import accountApiInstance from '@/api/accountInstance.js';

// 通知相關 API ===========================================================

// 查看用戶的所有通知
export const getAllNotifications = () =>
  accountApiInstance.post('Notification/GetAllNotifications');

// 查看單一通知
export const getNotification = () => accountApiInstance.get('Notification/GetNotification');
