import accountApiInstance from '@/api/accountInstance.js';

// 物流相關 API ===========================================================

// 產生綠界超商門市地圖網址
export const getCvsMapUrl = (params) =>
  accountApiInstance.get('Logistics/GetCvsMapUrl', { params });

// 儲存物流暫存訂單收件人 ( 超商 )
export const saveCvsReceiver = (request) =>
  accountApiInstance.post('Logistics/SaveCvsReceiver', request);

// 儲存物流暫存訂單資料 ( 宅配 )
export const saveHomeLogisticsTemp = (request) =>
  accountApiInstance.post('Logistics/SaveHomeLogisticsTemp', request);

// 查看物流暫存訂單資料
export const getLogisticsTemp = (sessionKey) =>
  accountApiInstance.get(`Logistics/GetLogisticsTemp?sessionKey=${sessionKey}`);
