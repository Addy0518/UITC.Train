import accountApiInstance from '@/api/accountInstance.js';

// 管理員的類別相關 API ===========================================================
// 新增類別
export const addCategory = (categoryName, parentId = null) =>
  accountApiInstance.post('admin/Category/AddCategory', { categoryName, parentId });

// 刪除類別
export const deleteCategory = (categoryId) =>
  accountApiInstance.delete('admin/Category/DeleteCategory', { params: { categoryId } });
