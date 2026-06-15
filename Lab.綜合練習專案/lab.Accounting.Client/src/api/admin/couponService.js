import accountApiInstance from '@/api/accountInstance.js';

// 管理員的優惠卷相關 API ===========================================================

// 查看用戶優惠卷
export const getUserCoupon = () => accountApiInstance.get('admin/Coupon/GetUserCoupon');

// 查看所有優惠卷
export const getAllCoupons = () => accountApiInstance.get('admin/Coupon/GetAllCoupons');

// 新增優惠卷
export const createCoupons = () => accountApiInstance.post('admin/Coupon/CreateCoupons');

// 編輯優惠卷
export const updateCoupons = () => accountApiInstance.put('admin/Coupon/UpdateCoupons');
