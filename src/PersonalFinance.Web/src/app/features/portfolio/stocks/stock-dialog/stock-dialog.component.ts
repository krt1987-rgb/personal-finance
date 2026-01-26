import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Stock } from '../../../../shared/models';

export interface StockDialogData {
  stock?: Stock;
  mode: 'add' | 'edit';
}

@Component({
  selector: 'app-stock-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './stock-dialog.component.html',
  styleUrl: './stock-dialog.component.scss'
})
export class StockDialogComponent {
  stockForm: FormGroup;
  mode: 'add' | 'edit';
  
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<StockDialogComponent>);

  constructor(@Inject(MAT_DIALOG_DATA) public data: StockDialogData) {
    this.mode = data.mode;
    
    this.stockForm = this.fb.group({
      symbol: [data.stock?.symbol || '', [Validators.required]],
      name: [data.stock?.name || '', [Validators.required]],
      quantity: [data.stock?.quantity || 0, [Validators.required, Validators.min(0)]],
      avgPrice: [data.stock?.avgPrice || 0, [Validators.required, Validators.min(0)]],
      currentPrice: [data.stock?.currentPrice || 0, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.stockForm.valid) {
      const formValue = this.stockForm.value;
      const result: Partial<Stock> = {
        ...formValue,
        value: formValue.quantity * formValue.currentPrice,
        profitLoss: (formValue.currentPrice - formValue.avgPrice) * formValue.quantity
      };
      
      if (this.mode === 'edit' && this.data.stock) {
        result.id = this.data.stock.id;
      }
      
      this.dialogRef.close(result);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
