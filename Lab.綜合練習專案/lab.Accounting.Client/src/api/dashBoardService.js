import accountApiInstance from '@/api/accountInstance.js';

// 賣家數據相關 API ===========================================================

// 查看賣家數據
export const getDashboard = () => accountApiInstance.get(`DashBoard/GetDashboard`);
