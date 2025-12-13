import type { CreateUpdatePurchaseDto, GetPurchaseListDto, PurchaseDto, PurchaseListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PurchaseService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePurchaseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'POST',
      url: '/api/app/purchase',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'GET',
      url: `/api/app/purchase/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPurchaseListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PurchaseListDto>>({
      method: 'GET',
      url: '/api/app/purchase',
      params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePurchaseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'PUT',
      url: `/api/app/purchase/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}