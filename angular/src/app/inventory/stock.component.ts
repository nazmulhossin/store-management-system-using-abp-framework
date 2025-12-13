import {
  FormGroup,
  FormBuilder,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormArray,
  AbstractControl
} from '@angular/forms';
import { Component, inject, OnInit } from '@angular/core';
import { DatePipe, CurrencyPipe, formatDate, CommonModule } from '@angular/common';
import { NgbDatepickerModule, NgbDateStruct, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import {
  ListService,
  PagedResultDto,
  LocalizationPipe,
  PermissionDirective,
  AutofocusDirective
} from '@abp/ng.core';
import {
  ConfirmationService,
  Confirmation,
  NgxDatatableDefaultDirective,
  NgxDatatableListDirective,
  ModalCloseDirective,
  ModalComponent
} from '@abp/ng.theme.shared';
import { GetStockListDto, StockDto, StockService } from '../proxy/inventory';

@Component({
  selector: 'app-stock',
  templateUrl: './stock.component.html',
  styleUrl: './stock.component.scss',
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    NgbDatepickerModule,
    NgxDatatableModule,
    NgbDropdownModule,
    NgxDatatableListDirective,
    NgxDatatableDefaultDirective,
    LocalizationPipe
  ],
  providers: [ListService],
})

export class StockComponent implements OnInit {
  public readonly list = inject(ListService);
  private stockService = inject(StockService);

  stock = { items: [], totalCount: 0 } as PagedResultDto<StockDto>;

  filters = {
    productName: '',
    warehouseName: '',
  };

  ngOnInit() {
    const stockStreamCreator = query =>
      this.stockService.getList({
        ...query,
        ...this.filters,
      });

    this.list.hookToQuery(stockStreamCreator).subscribe(response => {
      this.stock = response;
    });
  }

  applyFilter() {
    this.list.get(); // reload list with filters
  }

  clearFilter() {
    this.filters.productName = undefined;
    this.filters.warehouseName = undefined;
    this.list.get();
  }
}