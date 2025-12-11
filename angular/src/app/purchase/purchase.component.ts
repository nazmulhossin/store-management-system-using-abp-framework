import {
  FormGroup,
  FormBuilder,
  Validators,
  FormsModule,
  ReactiveFormsModule,
  FormArray
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
import { PurchaseListDto, PurchaseService } from '../proxy/purchases';

@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrl: './purchase.component.scss',
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

export class PurchaseComponent implements OnInit {
  public readonly list = inject(ListService);
  private purchaseService = inject(PurchaseService);
  private fb = inject(FormBuilder);
  private confirmation = inject(ConfirmationService);
  totalAmount = 0;
  totalDiscount = 0;
  payableAmount = 0;

  purchase = { items: [], totalCount: 0 } as PagedResultDto<PurchaseListDto>;
  selectedPurchase = {} as PurchaseListDto;
  form: FormGroup;
  isModalOpen = false;

  ngOnInit() {
    const purchaseStreamCreator = query => this.purchaseService.getList(query);

    this.list.hookToQuery(purchaseStreamCreator).subscribe(response => {
      this.purchase = response;
    });
  }

  createPurchase() {
    this.totalAmount = 0;
    this.totalDiscount = 0;
    this.payableAmount = 0;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      purchaseNumber: ['', Validators.required],
      purchaseDate: [null, Validators.required],
      supplierName: ['', Validators.required],
      description: [''],
      paidAmount: [0, Validators.required],
      purchaseItems: this.fb.array([])
    });

    // Initialize summary values
    this.totalAmount = 0;
    this.totalDiscount = 0;
  }

  get purchaseItems(): FormArray {
    return this.form.get('purchaseItems') as FormArray;
  }

  addItem() {
    this.purchaseItems.push(this.buildItemForm());
    this.updateSummary();
  }

  removeItem(index: number) {
    this.purchaseItems.removeAt(index);
    this.updateSummary();
  }

  buildItemForm(): FormGroup {
    return this.fb.group({
      productName: ['', Validators.required],
      warehouseName: ['', Validators.required],
      quantity: [0, Validators.required],
      unitPrice: [0, Validators.required],
      discount: [0],
    });
  }

  updateSummary() {
    let amount = 0;
    let discount = 0;

    this.purchaseItems.controls.forEach(ctrl => {
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
}