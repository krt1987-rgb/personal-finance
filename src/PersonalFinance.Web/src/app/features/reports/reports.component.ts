import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ApexOptions } from 'apexcharts';
import { WealthService, WealthTimeline } from '../../shared/services/wealth/wealth.service';

@Component({
  selector: 'app-reports',
  imports: [
    CommonModule, 
    MatCardModule, 
    MatButtonModule, 
    MatIconModule,
    MatTabsModule,
    NgApexchartsModule
  ],
  template: `
    <div class="page">
      <h1>Reports & Analytics</h1>

      <mat-tab-group>
        <mat-tab label="Net Worth Timeline">
          <div class="tab-content">
            <mat-card>
              <mat-card-header>
                <mat-card-title>Net Worth Over Time</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                @if (loading) {
                  <div class="loading">Loading timeline data...</div>
                }
                @if (!loading && timeline && timeline.snapshots.length > 0) {
                  <apx-chart
                    [series]="netWorthChartOptions.series!"
                    [chart]="netWorthChartOptions.chart!"
                    [xaxis]="netWorthChartOptions.xaxis!"
                    [yaxis]="netWorthChartOptions.yaxis!"
                    [stroke]="netWorthChartOptions.stroke!"
                    [colors]="netWorthChartOptions.colors!"
                    [dataLabels]="netWorthChartOptions.dataLabels!">
                  </apx-chart>
                  
                  @if (timeline.totalChange !== undefined && timeline.percentageChange !== undefined) {
                    <div class="summary">
                      <div class="summary-item">
                        <span class="label">Total Change:</span>
                        <span class="value" [class.positive]="timeline.totalChange >= 0" [class.negative]="timeline.totalChange < 0">
                          {{ formatCurrency(timeline.totalChange) }}
                        </span>
                      </div>
                      <div class="summary-item">
                        <span class="label">Percentage Change:</span>
                        <span class="value" [class.positive]="timeline.percentageChange >= 0" [class.negative]="timeline.percentageChange < 0">
                          {{ timeline.percentageChange.toFixed(2) }}%
                        </span>
                      </div>
                    </div>
                  }
                }
                @if (!loading && (!timeline || timeline.snapshots.length === 0)) {
                  <div class="no-data">
                    <mat-icon>timeline</mat-icon>
                    <p>No wealth snapshots available yet</p>
                    <p class="hint">Create snapshots from the dashboard to track your wealth over time</p>
                  </div>
                }
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>

        <mat-tab label="Snapshot History">
          <div class="tab-content">
            <mat-card>
              <mat-card-header>
                <mat-card-title>Wealth Snapshots</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                @if (timeline && timeline.snapshots.length > 0) {
                  <div class="snapshot-list">
                    @for (snapshot of timeline.snapshots; track snapshot.id) {
                      <div class="snapshot-item">
                        <div class="snapshot-date">
                          {{ snapshot.snapshotDate | date:'medium' }}
                        </div>
                        <div class="snapshot-values">
                          <div class="value-item">
                            <span class="label">Net Worth:</span>
                            <span class="value">{{ formatCurrency(snapshot.netWorth) }}</span>
                          </div>
                          <div class="value-item">
                            <span class="label">Assets:</span>
                            <span class="value">{{ formatCurrency(snapshot.totalAssets) }}</span>
                          </div>
                        </div>
                      </div>
                    }
                  </div>
                } @else {
                  <div class="no-data">
                    <p>No snapshots available</p>
                  </div>
                }
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `,
  styles: [`
    .page { 
      padding: 1.5rem;
    }
    .page h1 { 
      margin-bottom: 1.5rem; 
    }
    
    .tab-content {
      padding: 1.5rem 0;
    }

    .loading {
      text-align: center;
      padding: 3rem;
      color: #666;
    }

    .no-data { 
      text-align: center; 
      padding: 4rem 2rem; 
      color: #999; 
      
      mat-icon { 
        font-size: 64px; 
        width: 64px; 
        height: 64px; 
        margin-bottom: 1rem; 
      }

      p {
        margin: 0.5rem 0;
      }

      .hint {
        font-size: 0.9rem;
        color: #bbb;
      }
    }

    .summary {
      margin-top: 2rem;
      padding: 1.5rem;
      background: #f5f5f5;
      border-radius: 8px;
      display: flex;
      gap: 2rem;
      justify-content: center;

      .summary-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 0.5rem;

        .label {
          font-size: 0.9rem;
          color: #666;
        }

        .value {
          font-size: 1.5rem;
          font-weight: 600;

          &.positive {
            color: #4caf50;
          }

          &.negative {
            color: #f44336;
          }
        }
      }
    }

    .snapshot-list {
      .snapshot-item {
        padding: 1rem;
        border-bottom: 1px solid #eee;

        &:last-child {
          border-bottom: none;
        }

        .snapshot-date {
          font-weight: 600;
          color: #333;
          margin-bottom: 0.5rem;
        }

        .snapshot-values {
          display: flex;
          gap: 2rem;

          .value-item {
            display: flex;
            gap: 0.5rem;

            .label {
              color: #666;
            }

            .value {
              font-weight: 500;
              color: #333;
            }
          }
        }
      }
    }
  `]
})
export class ReportsComponent implements OnInit {
  private readonly wealthService = inject(WealthService);

  timeline: WealthTimeline | null = null;
  loading = true;

  netWorthChartOptions: any = {
    chart: { type: 'line', height: 350, toolbar: { show: true } },
    series: [],
    xaxis: { type: 'datetime', title: { text: 'Date' } },
    yaxis: { title: { text: 'Amount (₹)' } },
    stroke: { curve: 'smooth', width: 3 },
    colors: ['#4caf50', '#2196f3', '#f44336'],
    dataLabels: { enabled: false }
  };

  ngOnInit(): void {
    this.loadTimeline();
  }

  loadTimeline(): void {
    this.loading = true;
    this.wealthService.getTimeline().subscribe({
      next: (data) => {
        this.timeline = data;
        this.updateNetWorthChart();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading timeline:', err);
        this.loading = false;
      }
    });
  }

  updateNetWorthChart(): void {
    if (!this.timeline || this.timeline.snapshots.length === 0) return;

    const snapshots = this.timeline.snapshots;
    const dates = snapshots.map(s => new Date(s.snapshotDate).getTime());
    const netWorthData = snapshots.map(s => s.netWorth);
    const assetsData = snapshots.map(s => s.totalAssets);
    const liabilitiesData = snapshots.map(s => s.totalLiabilities);

    this.netWorthChartOptions = {
      ...this.netWorthChartOptions,
      series: [
        { name: 'Net Worth', data: dates.map((date, i) => ({ x: date, y: netWorthData[i] })) },
        { name: 'Total Assets', data: dates.map((date, i) => ({ x: date, y: assetsData[i] })) },
        { name: 'Total Liabilities', data: dates.map((date, i) => ({ x: date, y: liabilitiesData[i] })) }
      ]
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
}
