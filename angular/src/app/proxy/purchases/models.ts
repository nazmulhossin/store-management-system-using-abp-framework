import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdatePurchaseDto {
  purchaseNumber: string;
  purchaseDate: string;
  supplierName: string;
  description?: string;
  paidAmount: number;
  purchaseItems: CreateUpdatePurchaseItemDto[];
}

export interface CreateUpdatePurchaseItemDto {
  id?: string;
  productName: string;
  warehouseName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
}

export interface GetPurchaseListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
}

export interface PurchaseDto extends EntityDto<string> {
  purchaseNumber?: string;
  purchaseDate?: string;
  supplierName?: string;
  description?: string;
  totalAmount: number;
  overallDiscount: number;
  payableAmount: number;
  paidAmount: number;
  dueAmount: number;
  purchaseItems: PurchaseItemDto[];
}

export interface PurchaseItemDto extends EntityDto<string> {
  productName?: string;
  warehouseName?: string;
  quantity: number;
  unitPrice: number;
  subTotal: number;
  discount: number;
}

export interface PurchaseListDto extends EntityDto<string> {
  purchaseNumber?: string;
  purchaseDate?: string;
  supplierName?: string;
  description?: string;
  totalAmount: number;
  paidAmount: number;
}
