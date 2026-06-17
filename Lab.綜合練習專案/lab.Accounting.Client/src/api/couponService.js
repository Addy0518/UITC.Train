import accountApiInstance from '@/api/accountInstance.js';

// 優惠卷相關 API ===========================================================

// 查看指定優惠卷
export const getCoupon = (couponId) =>
  accountApiInstance.get(`Coupon/GetCoupon?couponId=${couponId}`);

// 查看用戶優惠卷
export const getUserCoupon = () => accountApiInstance.get('Coupon/GetUserCoupon');

// 賣家新增優惠卷
export const sellerCreateCoupons = (request) =>
  accountApiInstance.post('Coupon/SellerCreateCoupons', request);

// 賣家編輯優惠卷
export const sellerUpdateCoupons = (request) =>
  accountApiInstance.put('Coupon/SellerUpdateCoupons', request);

// 用戶領取優惠卷
export const createUserCoupon = (request) =>
  accountApiInstance.post('Coupon/CreateUserCoupon', request);
