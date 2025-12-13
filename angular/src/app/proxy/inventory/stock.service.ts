import type { GetStockListDto, StockDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetStockListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<StockDto>>({
      method: 'GET',
      url: '/api/app/stock',
      params: { productName: input.productName, warehouseName: input.warehouseName, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}