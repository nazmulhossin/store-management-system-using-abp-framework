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
import { CreateUpdateSaleDto, SaleDto, SaleListDto, SaleService } from '../proxy/sales';

@Component({
  selector: 'app-sale',
  templateUrl: './sale.component.html',
  styleUrl: './sale.component.scss',
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    NgbDatepickerModule,
    NgxDatatableModule,
    NgbDropdownModule,
    ModalComponent,
    NgxDatatableListDirective,
    NgxDatatableDefaultDirective,
    PermissionDirective,
    ModalCloseDirective,
    LocalizationPipe,
    DatePipe,
    CurrencyPipe
  ],
  providers: [ListService],
})

export class SaleComponent implements OnInit {
  public readonly list = inject(ListService);
  private saleService = inject(SaleService);
  private fb = inject(FormBuilder);
  private confirmation = inject(ConfirmationService);
  totalAmount = 0;
  totalDiscount = 0;
  payableAmount = 0;

  sales = { items: [], totalCount: 0 } as PagedResultDto<SaleListDto>;
  selectedSale = {} as SaleDto;

  form: FormGroup;
  isModalOpen = false;

  ngOnInit() {
    const saleStreamCreator = query => this.saleService.getList(query);

    this.list.hookToQuery(saleStreamCreator).subscribe(response => {
      this.sales = response;
    });
  }

  createSale() {
    this.selectedSale = {} as SaleDto;
    this.totalAmount = 0;
    this.totalDiscount = 0;
    this.payableAmount = 0;
    this.buildForm();
    this.isModalOpen = true;
  }

  editSale(id: string) {
    this.saleService.get(id).subscribe(sale => {
      this.selectedSale = sale;
      console.log(sale);
      this.buildForm();              // create empty form
      this.patchHeader(sale);        // set header values
      this.patchItems(sale);         // set items
      this.updateSummary();          // recalculate totals

      this.isModalOpen = true;
    });
  }

  deleteSale(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.saleService.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  buildForm() {
    this.form = this.fb.group({
      salesNumber: ['', Validators.required],
      salesDate: [null, Validators.required],
      customerName: ['', Validators.required],
      paidAmount: [0, Validators.required],
      saleItems: this.fb.array([], [this.minItems(1)])
    });

    this.totalAmount = 0;
    this.totalDiscount = 0;
    this.payableAmount = 0;
  }

  get saleItems(): FormArray {
    return this.form.get('saleItems') as FormArray;
  }

  addItem() {
    this.saleItems.push(this.buildItemForm());
    this.updateSummary();
  }

  removeItem(index: number) {
    this.saleItems.removeAt(index);
    this.updateSummary();
  }

  buildItemForm(): FormGroup {
    return this.fb.group({
      productName: ['', Validators.required],
      warehouseName: ['', Validators.required],
      quantity: [0, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0.0001)]],
      discount: [0],
    });
  }

  updateSummary() {
    let amount = 0;
    let discount = 0;

    this.saleItems.controls.forEach(ctrl => {
      const qty = Number(ctrl.get('quantity')?.value || 0);
      const price = Number(ctrl.get('unitPrice')?.value || 0);
      const disc = Number(ctrl.get('discount')?.value || 0);

      amount += qty * price;
      discount += disc;
    });

    this.totalAmount = amount;
    this.totalDiscount = discount;
    this.payableAmount = amount - discount;

    const paid = Number(this.form.get('paidAmount')?.value || 0);
  }

  save() {
    if (this.form.invalid) return;

    const value = this.form.value;

    const requestData: CreateUpdateSaleDto = {
      salesNumber: value.salesNumber,
      salesDate: this.formatDate(value.salesDate),
      customerName: value.customerName,
      paidAmount: Number(value.paidAmount),
      saleItems: value.saleItems.map(item => ({
        productName: item.productName,
        warehouseName: item.warehouseName,
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
        discount: Number(item.discount),
      })),
    };

    let request$;
    if (this.selectedSale?.id) {
      request$ = this.saleService.update(this.selectedSale.id, requestData);
    } else {
      request$ = this.saleService.create(requestData);
    }

    request$.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.selectedSale = {} as any;
      this.list.get();
    });
  }

  private patchHeader(sale: any) {
    this.form.patchValue({
      salesNumber: sale.salesNumber,
      salesDate: this.parseDate(sale.salesDate),
      customerName: sale.customerName,
      paidAmount: sale.paidAmount
    });
  }

  private patchItems(sale: SaleDto) {
    this.saleItems.clear();

    sale.saleItems.forEach(item => {
      this.saleItems.push(
        this.fb.group({
          productName: [item.productName, Validators.required],
          warehouseName: [item.warehouseName, Validators.required],
          quantity: [item.quantity, Validators.required],
          unitPrice: [item.unitPrice, Validators.required],
          discount: [item.discount || 0],
        })
      );
    });
  }

  minItems(min: number) {
    return (formArray: AbstractControl) => {
      return (formArray as FormArray).length >= min ? null : { minItems: true };
    };
  }

  private parseDate(value: string | Date): NgbDateStruct | null {
    if (!value) {
      return null;
    }

    const date = new Date(value);
    if (isNaN(date.getTime())) {
      return null;
    }

    return {
      year: date.getFullYear(),
      month: date.getMonth() + 1,
      day: date.getDate(),
    };
  }

  private formatDate(dateStruct: NgbDateStruct | null): string {
    if (!dateStruct) {
      return '';
    }

    const date = new Date(dateStruct.year, dateStruct.month - 1, dateStruct.day);
    return formatDate(date, 'yyyy-MM-dd', 'en');
  }
}