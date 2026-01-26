import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-stocks',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './stocks.component.html',
  styleUrl: './stocks.component.scss'
})
export class StocksComponent {
  displayedColumns: string[] = ['symbol', 'name', 'quantity', 'avgPrice', 'currentPrice', 'value', 'profitLoss', 'actions'];
  dataSource: any[] = [];
  
  addStock(): void {
    // TODO: Open dialog to add stock
    console.log('Add stock');
  }
  
  editStock(stock: any): void {
    // TODO: Open dialog to edit stock
    console.log('Edit stock', stock);
  }
  
  deleteStock(stock: any): void {
    // TODO: Implement delete
    console.log('Delete stock', stock);
  }
}
