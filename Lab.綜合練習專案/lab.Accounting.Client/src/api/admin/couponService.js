import accountApiInstance from '@/api/accountInstance.js';

// 管理員的優惠卷相關 API ===========================================================

// 查看所有優惠卷
export const getAllCoupons = (request) =>
  accountApiInstance.get(`admin/Coupon/GetAllCoupons?request=${request}`);

// 新增優惠卷
export const createCoupons = (request) =>
  accountApiInstance.post('admin/Coupon/CreateCoupons', request);

// 編輯優惠卷
export const updateCoupons = (request) =>
  accountApiInstance.put('admin/Coupon/UpdateCoupons', request);
