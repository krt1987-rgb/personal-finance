import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatIconModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  stats = [
    { title: 'Total Wealth', value: '₹0.00', icon: 'account_balance', color: '#4caf50' },
    { title: 'Stock Portfolio', value: '₹0.00', icon: 'trending_up', color: '#2196f3' },
    { title: 'Mutual Funds', value: '₹0.00', icon: 'pie_chart', color: '#ff9800' },
    { title: 'Bank Balance', value: '₹0.00', icon: 'account_balance_wallet', color: '#9c27b0' }
  ];
}
