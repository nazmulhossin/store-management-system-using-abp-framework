import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateSaleDto {
  salesNumber: string;
  salesDate: string;
  customerName: string;
  paidAmount: number;
  saleItems: CreateUpdateSaleItemDto[];
}

export interface CreateUpdateSaleItemDto {
  id?: string;
  productName: string;
  warehouseName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
}

export interface GetSaleListDto extends PagedAndSortedResultRequestDto {
  customerName?: string;
  startDate?: string;
  endDate?: string;
}

export interface SaleDto extends EntityDto<string> {
  salesNumber?: string;
  salesDate?: string;
  customerName?: string;
  totalAmount: number;
  overallDiscount: number;
  payableAmount: number;
  paidAmount: number;
  dueAmount: number;
  saleItems: SaleItemDto[];
}

export interface SaleItemDto extends EntityDto<string> {
  productName?: string;
  warehouseName?: string;
  quantity: number;
  unitPrice: number;
  subTotal: number;
  discount: number;
}

export interface SaleListDto extends EntityDto<string> {
  salesNumber?: string;
  salesDate?: string;
  customerName?: string;
  totalAmount: number;
  paidAmount: number;
}
