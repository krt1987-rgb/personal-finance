import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-reports',
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="page">
      <h1>Reports & Analytics</h1>
      <mat-card>
        <mat-card-content>
          <div class="no-data">
            <mat-icon>assessment</mat-icon>
            <p>Reports and analytics will be displayed here</p>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page { }
    .page h1 { margin-bottom: 1.5rem; }
    .no-data { text-align: center; padding: 4rem 2rem; color: #999; }
    .no-data mat-icon { font-size: 64px; width: 64px; height: 64px; margin-bottom: 1rem; }
  `]
})
export class ReportsComponent {}
