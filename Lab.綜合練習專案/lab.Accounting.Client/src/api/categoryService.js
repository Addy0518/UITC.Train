import accountApiInstance from '@/api/accountInstance.js';

// 類別相關 API ===========================================================

// 查看最頂層父類別
export const getOneFatherCategory = () => accountApiInstance.get('Category/GetOneFatherCategory');

// 查看指定類別的直屬子類別（下一層）
export const getOneSonCategory = (fatherCategoryId) =>
  accountApiInstance.get('Category/GetOneSonCategory', { params: { fatherCategoryId } });

// 查看指定類別底下所有層級類別
export const getSonCategories = (fatherCategoryId) =>
  accountApiInstance.get('Category/GetSonCategories', { params: { fatherCategoryId } });

// 查看指定類別往上所有層級類別（麵包屑用）
export const getFatherCategories = (sonCategoryId) =>
  accountApiInstance.get('Category/GetFatherCategories', { params: { sonCategoryId } });

// 新增類別
export const addCategory = (categoryName, parentId = null) =>
  accountApiInstance.post('Category/AddCategory', { categoryName, parentId });

// 刪除類別
export const deleteCategory = (categoryId) =>
  accountApiInstance.delete('Category/DeleteCategory', { params: { categoryId } });
