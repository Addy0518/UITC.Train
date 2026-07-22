import accountApiInstance from '@/api/accountInstance.js';

// 管理員的審查相關 API ===========================================================

// 查看商品審核
export const getProductsReview = (reviewId) =>
  accountApiInstance.get(`admin/Review/GetProductsReview?reviewId=${reviewId}`);

// 查看所有商品審核
export const getAllProductsReview = (request) =>
  accountApiInstance.get(`admin/Review/GetAllProductsReview`, { params: request });

// 查看指定審核表的所有商品圖片
export const getReviewAllImg = (reviewId) =>
  accountApiInstance.get(`admin/Review/GetReviewAllImg?reviewId=${reviewId}`);

// 商品審核通過或駁回
export const approveOrRejectProductsReview = (request) =>
  accountApiInstance.put(`admin/Review/ApproveOrRejectProductsReview`, request);

// 查看商品審核
export const getStoreReview = (reviewId) =>
  accountApiInstance.get(`admin/Review/GetStoreReview?reviewId=${reviewId}`);

// 查看所有商品審核
export const getAllStoreReview = (request) =>
  accountApiInstance.get(`admin/Review/GetAllStoreReview`, { params: request });

// 賣場審核通過或駁回
export const approveOrRejectStoreReview = (request) =>
  accountApiInstance.put(`admin/Review/ApproveOrRejectStoreReview`, request);
