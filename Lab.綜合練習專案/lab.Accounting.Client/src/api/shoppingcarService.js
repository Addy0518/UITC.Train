import accountApiInstance from '@/api/accountInstance.js';

// 購物車相關 API ===========================================================

// 查看購物車裡的商品
export const getAllProductsInShoppingCar = () =>
  accountApiInstance.get(`ShoppingCar/GetAllProductsInShoppingCar`);
//新增購物車商品
export const addProductsInShoppingCar = (productsId,boughtquantity) =>
  accountApiInstance.post(`ShoppingCar/AddProductsInShoppingCar?productsId=${productsId}&boughtquantity=${boughtquantity}`);
// 刪除購物車商品
export const deleteProductsInShoppingCar = (productsId) =>
  accountApiInstance.delete(`ShoppingCar/DeleteProductsInShoppingCar?productsId=${productsId}`);
