import accountApiInstance from '@/api/accountInstance.js';
import { isDeleteEnum } from '../common/enum';

// 商品相關 API ===========================================================

// 查看指定商品
export const getProduct = (productId) =>
  accountApiInstance.get(`Products/GetProducts?productId=${productId}`);
// 查看所有商品
export const getAllProduct = (pageIndex = 0, pageSize = 10) =>
  accountApiInstance.get(`Products/GetAllProducts?pageIndex=${pageIndex}&pageSize=${pageSize}`);
// 賣家查看賣場所有商品
export const getSellerAllProduct = (
  pageIndex = 0,
  pageSize = 10,
  isDelete = isDeleteEnum.Normal.value,
) =>
  accountApiInstance.get(
    `Products/GetSellerAllProducts?pageIndex=${pageIndex}&pageSize=${pageSize}&isDelete=${isDelete}`,
  );

// 買家查看賣場所有商品
export const userGetSellerAllProduct = (
  pageIndex = 0,
  pageSize = 10,
  isDelete = isDeleteEnum.Normal.value,
  userId,
) =>
  accountApiInstance.get(
    `Products/UserGetSellerAllProducts?pageIndex=${pageIndex}&pageSize=${pageSize}&isDelete=${isDelete}&userId=${userId}`,
  );
// 查看類別
export const getCategory = (categoryId = null) =>
  accountApiInstance.get(`Products/GetCategory`, { params: { productcategoryId: categoryId } });
// 新增單一商品 + 類別
export const createProducts = (product) =>
  accountApiInstance.post(`Products/CreateProducts`, product);
// 更新單一商品 + 類別
export const updateProducts = (product) =>
  accountApiInstance.put(`Products/UpdateProducts`, product);
// 軟刪除或硬刪除單一商品
export const deleteProducts = (productsId) =>
  accountApiInstance.delete(`Products/DeleteProducts?productsId=${productsId}`);
// 復原單一或全部商品刪除狀態
export const updateProductsDeleteStatus = (productsId) =>
  accountApiInstance.put(`Products/UpdateProductsDeleteStatus`, productsId);

// 商品圖片相關 API ===========================================================

// 商品圖片上傳
export const productsImgUpload = (ImgData) =>
  accountApiInstance.post(`Products/ProductsImgUpload`, ImgData);
// 商品圖片刪除
export const productsImgDelete = (productsImgId) =>
  accountApiInstance.delete(`Products/ProductsImgDelete?productsImgId=${productsImgId}`);

// 評價相關 API ===========================================================

// 新增單一商品評價
export const createProductRate = (request) =>
  accountApiInstance.post(`Products/CreateProductRate`, request);
