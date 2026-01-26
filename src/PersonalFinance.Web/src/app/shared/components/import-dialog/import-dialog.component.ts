import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FileUploadComponent } from '../file-upload/file-upload.component';
import { ImportService } from '../../services/import/import.service';
import { ImportResult } from '../../models/import.model';

export interface ImportDialogData {
  moduleType: 'stocks' | 'bankAccounts' | 'fixedDeposits' | 'mutualFunds' | 'providentFunds' | 'familyMembers';
  title: string;
}

@Component({
  selector: 'app-import-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    FileUploadComponent
  ],
  templateUrl: './import-dialog.component.html',
  styleUrls: ['./import-dialog.component.css']
})
export class ImportDialogComponent {
  private readonly importService = inject(ImportService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialogRef = inject(MatDialogRef<ImportDialogComponent>);

  isUploading = false;
  importResult: ImportResult | null = null;
  selectedFile: File | null = null;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ImportDialogData) {}

  onFileSelected(file: File): void {
    this.selectedFile = file;
    this.importResult = null;
  }

  onImport(): void {
    if (!this.selectedFile) {
      this.snackBar.open('Please select a file first', 'Close', { duration: 3000 });
      return;
    }

    this.isUploading = true;
    this.importResult = null;

    let importObservable;
    switch (this.data.moduleType) {
      case 'stocks':
        importObservable = this.importService.importStockHoldings(this.selectedFile);
        break;
      case 'bankAccounts':
        importObservable = this.importService.importBankAccounts(this.selectedFile);
        break;
      case 'fixedDeposits':
        importObservable = this.importService.importFixedDeposits(this.selectedFile);
        break;
      case 'mutualFunds':
        importObservable = this.importService.importMutualFundHoldings(this.selectedFile);
        break;
      case 'providentFunds':
        importObservable = this.importService.importProvidentFunds(this.selectedFile);
        break;
      case 'familyMembers':
        importObservable = this.importService.importFamilyMembers(this.selectedFile);
        break;
      default:
        this.isUploading = false;
        this.snackBar.open('Unknown module type', 'Close', { duration: 3000 });
        return;
    }

    importObservable.subscribe({
      next: (result) => {
        this.isUploading = false;
        this.importResult = result;
        
        if (result.isSuccess) {
          this.snackBar.open(
            `Successfully imported ${result.successCount} records`,
            'Close',
            { duration: 5000 }
          );
        } else {
          this.snackBar.open(
            `Import completed with ${result.failureCount} errors`,
            'Close',
            { duration: 5000 }
          );
        }
      },
      error: (error) => {
        this.isUploading = false;
        this.snackBar.open(
          'Import failed: ' + (error.error?.message || error.message || 'Unknown error'),
          'Close',
          { duration: 5000 }
        );
      }
    });
  }

  onClose(): void {
    this.dialogRef.close(this.importResult);
  }
}
