import accountApiInstance from '@/api/accountInstance.js';
import { isDeleteEnum } from '../common/enum';

/*
   從 accountInstance 匯入 axios 設定 , 開始串接後端 api
*/
/*
   使用者相關 API ===========================================================
*/
// 使用者註冊
export const registerApi = (userRegister) =>
  accountApiInstance.post(`/User/Register`, userRegister);
// 使用者登入
export const loginApi = (userLogin) => accountApiInstance.post(`/User/Login`, userLogin);
// 使用者登出
export const logoutApi = () => accountApiInstance.post(`/User/Logout`);

// 使用者大頭照新增編輯
export const userHeadShot = (userFile) =>
  accountApiInstance.post(`/User/UserHeadShotUpload`, userFile);

/*
   帳本相關 API ===========================================================
*/
// 搜尋全部帳本
export const getAllLedger = (queryString = '') =>
  accountApiInstance.get(`/Ledger/GetAllLedger${queryString}`);
// 搜尋單一帳本
export const getLedger = (ledgerId) =>
  accountApiInstance.get(`/Ledger/GetLedger?ledgerId=${ledgerId}`);
// 新增帳本
export const createLedger = (ledgerCreate) =>
  accountApiInstance.post(`Ledger/CreateLedger`, ledgerCreate);
// 編輯帳本
export const updateLedger = (ledgerUpdate) =>
  accountApiInstance.put(`Ledger/UpdateLedger`, ledgerUpdate);
// 刪除帳本
export const deleteLedger = (ledgerDelete = null) =>
  accountApiInstance.delete(`Ledger/DeleteLedger/${ledgerDelete}`);
// 刪除所有軟刪除狀態帳本
export const deleteAllSoftDeleteLedger = () =>
  accountApiInstance.delete(`Ledger/DeleteAllSoftDeleteLedger`);

/*
   商城相關 API ===========================================================
*/
// 查看指定商品
export const getProduct = (productId) =>
  accountApiInstance.get(`Mall/GetProducts?productId=${productId}`);
// 查看所有商品
export const getAllProduct = (pageIndex = 0, pageSize = 10) =>
  accountApiInstance.get(`Mall/GetAllProducts?pageIndex=${pageIndex}&pageSize=${pageSize}`);
// 查看賣家所有商品
export const getSellerAllProduct = (
  pageIndex = 0,
  pageSize = 10,
  isDelete = isDeleteEnum.Normal.value,
) =>
  accountApiInstance.get(
    `Mall/GetSellerAllProducts?pageIndex=${pageIndex}&pageSize=${pageSize}&isDelete=${isDelete}`,
  );
// 查看類別
export const getCategory = (categoryId = null) =>
  accountApiInstance.get(`Mall/GetCategory`, { params: { productcategoryId: categoryId } });
// 新增單一商品 + 類別
export const createProducts = (product) => accountApiInstance.post(`Mall/CreateProducts`, product);
// 更新單一商品 + 類別
export const updateProducts = (product) => accountApiInstance.put(`Mall/UpdateProducts`, product);
// 軟刪除或硬刪除單一商品
export const deleteProducts = (productsId) =>
  accountApiInstance.delete(`Mall/DeleteProducts?productsId=${productsId}`);
// 復原單一或全部商品刪除狀態
export const updateProductsDeleteStatus = (productsId) =>
  accountApiInstance.put(`Mall/UpdateProductsDeleteStatus`, productsId);
// 商品圖片上傳
export const productsImgUpload = (ImgData) =>
  accountApiInstance.post(`Mall/ProductsImgUpload`, ImgData);
// 商品圖片刪除
export const productsImgDelete = (productsImgId) =>
  accountApiInstance.delete(`Mall/ProductsImgDelete?productsImgId=${productsImgId}`);
// 查看購物車裡的商品
export const getAllProductsInShoppingCar = () =>
  accountApiInstance.get(`Mall/GetAllProductsInShoppingCar`);
//新增購物車商品
export const addProductsInShoppingCar = (productsId) =>
  accountApiInstance.post(`Mall/AddProductsInShoppingCar?productsId=${productsId}`);
// 刪除購物車商品
export const deleteProductsInShoppingCar = (productsId) =>
  accountApiInstance.delete(`Mall/DeleteProductsInShoppingCar?productsId=${productsId}`);
// 使用者購買商品
export const userBuyProduct = (buyRequest) =>
  accountApiInstance.post(`Mall/UserBuyProduct`, buyRequest);
