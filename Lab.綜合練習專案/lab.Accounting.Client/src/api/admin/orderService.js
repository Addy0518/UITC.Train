import accountApiInstance from '@/api/accountInstance.js';

// 管理員的訂單相關 API ===========================================================

// 查看所有訂單
export const getAllOrder = (request) =>
  accountApiInstance.get(`admin/Order/GetAllOrder`, { params: request });


