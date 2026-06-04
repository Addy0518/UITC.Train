import accountApiInstance from '@/api/accountInstance.js';

// 審查相關 API ===========================================================

// 查看商品審核
export const getProductsReview = (reviewId) =>
  accountApiInstance.get(`Review/GetProductsReview?reviewId=${reviewId}`);

// 查看所有商品審核
export const getAllProductsReview = (request) =>
  accountApiInstance.get(`Review/GetAllProductsReview`, { params: request });

// 審核通過或駁回
export const approveOrRejectProductsReview = (request) =>
  accountApiInstance.put(`Review/ApproveOrRejectProductsReview`, request);
