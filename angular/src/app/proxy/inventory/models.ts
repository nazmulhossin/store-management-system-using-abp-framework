import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface GetStockListDto extends PagedAndSortedResultRequestDto {
  productName?: string;
  warehouseName?: string;
}

export interface StockDto {
  productName?: string;
  warehouseName?: string;
  currentStock: number;
}
