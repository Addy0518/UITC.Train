import accountApiInstance from '@/api/accountInstance.js';

// 帳本相關 API ===========================================================

// 搜尋全部帳本
export const getAllLedger = (queryString = '') =>
  accountApiInstance.get(`Ledger/GetAllLedger${queryString}`);
// 搜尋單一帳本
export const getLedger = (ledgerId) =>
  accountApiInstance.get(`Ledger/GetLedger?ledgerId=${ledgerId}`);
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
