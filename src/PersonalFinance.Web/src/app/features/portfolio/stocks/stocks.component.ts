import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Stock } from '../../../shared/models';
import { StockDialogComponent } from './stock-dialog/stock-dialog.component';
import { AIResearchDialogComponent } from './ai-research-dialog/ai-research-dialog.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ImportDialogComponent } from '../../../shared/components/import-dialog/import-dialog.component';

@Component({
  selector: 'app-stocks',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './stocks.component.html',
  styleUrl: './stocks.component.scss'
})
export class StocksComponent {
  displayedColumns: string[] = ['symbol', 'name', 'quantity', 'avgPrice', 'currentPrice', 'value', 'profitLoss', 'actions'];
  dataSource: Stock[] = [];
  
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  
  addStock(): void {
    const dialogRef = this.dialog.open(StockDialogComponent, {
      width: '500px',
      data: { mode: 'add' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Generate a temporary ID (in a real app, this would come from the API)
        const newStock: Stock = {
          ...result,
          id: crypto.randomUUID()
        };
        
        this.dataSource = [...this.dataSource, newStock];
        this.snackBar.open('Stock added successfully!', 'Close', { duration: 3000 });
        
        // TODO: Call API service to save the stock
        console.log('Add stock to API:', newStock);
      }
    });
  }
  
  editStock(stock: Stock): void {
    const dialogRef = this.dialog.open(StockDialogComponent, {
      width: '500px',
      data: { stock, mode: 'edit' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const index = this.dataSource.findIndex(s => s.id === stock.id);
        if (index !== -1) {
          this.dataSource[index] = { ...result, id: stock.id };
          this.dataSource = [...this.dataSource]; // Trigger change detection
          this.snackBar.open('Stock updated successfully!', 'Close', { duration: 3000 });
          
          // TODO: Call API service to update the stock
          console.log('Update stock via API:', result);
        }
      }
    });
  }
  
  deleteStock(stock: Stock): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Stock',
        message: `Are you sure you want to delete ${stock.name} (${stock.symbol})?`
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.dataSource = this.dataSource.filter(s => s.id !== stock.id);
        this.snackBar.open('Stock deleted successfully!', 'Close', { duration: 3000 });
        
        // TODO: Call API service to delete the stock
        console.log('Delete stock via API:', stock.id);
      }
    });
  }
  
  importStocks(): void {
    const dialogRef = this.dialog.open(ImportDialogComponent, {
      width: '600px',
      data: {
        moduleType: 'stocks',
        title: 'Import Stock Holdings'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.successCount > 0) {
        // Reload data after successful import
        this.snackBar.open('Import completed. Refresh the page to see imported data.', 'Close', { duration: 5000 });
      }
    });
  }

  aiResearch(stock: Stock): void {
    this.dialog.open(AIResearchDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      data: {
        symbol: stock.symbol,
        companyName: stock.name
      }
    });
  }
}
