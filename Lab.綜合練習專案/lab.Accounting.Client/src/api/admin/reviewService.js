import accountApiInstance from '@/api/accountInstance.js';

// 管理員的審查相關 API ===========================================================

// 查看商品審核
export const getProductsReview = (reviewId) =>
  accountApiInstance.get(`admin/Review/GetProductsReview?reviewId=${reviewId}`);

// 查看所有商品審核
export const getAllProductsReview = (request) =>
  accountApiInstance.get(`admin/Review/GetAllProductsReview`, { params: request });

// 審核通過或駁回
export const approveOrRejectProductsReview = (request) =>
  accountApiInstance.put(`admin/Review/ApproveOrRejectProductsReview`, request);
