import accountApiInstance from '@/api/accountInstance.js';

// 物流相關 API ===========================================================

// 取得超商門市地圖網址
export const getCvsMapUrl = (params) =>
  accountApiInstance.get('Logistics/GetCvsMapUrl', { params });

// 買家填完配送資訊後送出，存進暫存表
export const saveHomeLogisticsTemp = (request) =>
  accountApiInstance.post('Logistics/SaveHomeLogisticsTemp', request);

// 查看物流暫存訂單資料
export const getLogisticsTemp = (sessionKey) =>
  accountApiInstance.get(`Logistics/GetLogisticsTemp?sessionKey=${sessionKey}`);
