import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { WealthService, NetWorth } from '../../shared/services/wealth/wealth.service';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ApexOptions } from 'apexcharts';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatIconModule,
    MatButtonModule,
    NgApexchartsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly wealthService = inject(WealthService);
  
  stats = [
    { title: 'Total Net Worth', value: '₹0.00', icon: 'account_balance', color: '#4caf50' },
    { title: 'Total Assets', value: '₹0.00', icon: 'trending_up', color: '#2196f3' },
    { title: 'Stocks', value: '₹0.00', icon: 'show_chart', color: '#ff9800' },
    { title: 'Mutual Funds', value: '₹0.00', icon: 'pie_chart', color: '#9c27b0' }
  ];

  netWorth: NetWorth | null = null;
  loading = true;
  error: string | null = null;

  // Chart options for asset allocation
  assetAllocationChartOptions: any = {
    chart: { type: 'pie', height: 350 },
    labels: [],
    series: [],
    legend: { position: 'bottom' },
    colors: ['#2196f3', '#9c27b0', '#4caf50', '#ff9800', '#f44336']
  };

  ngOnInit(): void {
    this.loadNetWorth();
  }

  loadNetWorth(): void {
    this.loading = true;
    this.error = null;
    
    this.wealthService.getCurrentNetWorth().subscribe({
      next: (data) => {
        this.netWorth = data;
        this.updateStats();
        this.updateAssetAllocationChart();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading net worth:', err);
        this.error = 'Failed to load net worth data';
        this.loading = false;
      }
    });
  }

  updateStats(): void {
    if (!this.netWorth) return;

    this.stats = [
      { 
        title: 'Total Net Worth', 
        value: this.formatCurrency(this.netWorth.netWorth), 
        icon: 'account_balance', 
        color: '#4caf50' 
      },
      { 
        title: 'Total Assets', 
        value: this.formatCurrency(this.netWorth.totalAssets), 
        icon: 'trending_up', 
        color: '#2196f3' 
      },
      { 
        title: 'Stocks', 
        value: this.formatCurrency(this.netWorth.assetBreakdown['Stocks'] || 0), 
        icon: 'show_chart', 
        color: '#ff9800' 
      },
      { 
        title: 'Mutual Funds', 
        value: this.formatCurrency(this.netWorth.assetBreakdown['MutualFunds'] || 0), 
        icon: 'pie_chart', 
        color: '#9c27b0' 
      }
    ];
  }

  updateAssetAllocationChart(): void {
    if (!this.netWorth || !this.netWorth.assetBreakdown) return;

    const breakdown = this.netWorth.assetBreakdown;
    const labels = Object.keys(breakdown).filter(key => breakdown[key] > 0);
    const series = labels.map(key => breakdown[key]);

    this.assetAllocationChartOptions = {
      ...this.assetAllocationChartOptions,
      labels: labels,
      series: series
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  }

  createSnapshot(): void {
    this.wealthService.createSnapshot({ snapshotType: 'Manual' }).subscribe({
      next: () => {
        console.log('Snapshot created successfully');
      },
      error: (err) => {
        console.error('Error creating snapshot:', err);
      }
    });
  }
}
