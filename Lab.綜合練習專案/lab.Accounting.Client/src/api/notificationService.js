import accountApiInstance from '@/api/accountInstance.js';

// 通知相關 API ===========================================================

// 查看用戶的所有通知
export const getAllNotifications = (request) =>
  accountApiInstance.post('Notification/GetAllNotifications', request);

// 查看單一通知
export const getNotification = (notificationId) =>
  accountApiInstance.get('Notification/GetNotification', { params: { notificationId } });

// 改變所有通知已讀狀態
export const updateAllNotificationReadStatus = () =>
  accountApiInstance.put('Notification/UpdateAllNotificationReadStatus');
