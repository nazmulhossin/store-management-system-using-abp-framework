import type { CreateUpdateSaleDto, GetSaleListDto, SaleDto, SaleListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SaleService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SaleDto>({
      method: 'POST',
      url: '/api/app/sale',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/sale/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SaleDto>({
      method: 'GET',
      url: `/api/app/sale/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetSaleListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SaleListDto>>({
      method: 'GET',
      url: '/api/app/sale',
      params: { customerName: input.customerName, startDate: input.startDate, endDate: input.endDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateSaleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SaleDto>({
      method: 'PUT',
      url: `/api/app/sale/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}