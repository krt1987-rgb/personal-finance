import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ImportDialogComponent } from '../../../shared/components/import-dialog/import-dialog.component';

@Component({
  selector: 'app-mutual-funds',
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatDialogModule, MatSnackBarModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Mutual Fund Holdings</h1>
        <div class="button-group">
          <button mat-raised-button color="accent" (click)="importMutualFunds()">
            <mat-icon>upload_file</mat-icon>
            Import
          </button>
          <button mat-raised-button color="primary">
            <mat-icon>add</mat-icon>
            Add Mutual Fund
          </button>
        </div>
      </div>
      <mat-card>
        <mat-card-content>
          <div class="no-data">
            <mat-icon>pie_chart</mat-icon>
            <p>No mutual fund holdings found</p>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .page { }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .button-group { display: flex; gap: 0.5rem; }
    .no-data { text-align: center; padding: 4rem 2rem; color: #999; }
    .no-data mat-icon { font-size: 64px; width: 64px; height: 64px; margin-bottom: 1rem; }
  `]
})
export class MutualFundsComponent {
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  importMutualFunds(): void {
    const dialogRef = this.dialog.open(ImportDialogComponent, {
      width: '600px',
      data: {
        moduleType: 'mutualFunds',
        title: 'Import Mutual Fund Holdings'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.successCount > 0) {
        this.snackBar.open('Import completed. Refresh the page to see imported data.', 'Close', { duration: 5000 });
      }
    });
  }
}
