import accountApiInstance from '@/api/accountInstance.js';

// 管理員的類別相關 API ===========================================================

// 查看所有類別
export const getAllCategories = (request) =>
  accountApiInstance.get('admin/Category/GetAllCategories', { params: request });

// 新增類別
export const addCategory = (request) =>
  accountApiInstance.post('admin/Category/AddCategory', request);

// 刪除類別
export const deleteCategory = (categoryId) =>
  accountApiInstance.delete('admin/Category/DeleteCategory', { params: { categoryId } });
