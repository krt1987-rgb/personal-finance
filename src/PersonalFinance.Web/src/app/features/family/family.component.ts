import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-family',
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Family Members</h1>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon>
          Add Member
        </button>
      </div>
      <mat-card>
        <mat-card-content>
          <div class="no-data">
            <mat-icon>people</mat-icon>
            <p>No family members found</p>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page { }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .no-data { text-align: center; padding: 4rem 2rem; color: #999; }
    .no-data mat-icon { font-size: 64px; width: 64px; height: 64px; margin-bottom: 1rem; }
  `]
})
export class FamilyComponent {}
