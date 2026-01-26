import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { SafePipe } from '../../../../shared/pipes/safe.pipe';
import { AIService } from '../../../../shared/services/ai/ai.service';
import { StockAnalysisType, StockAnalysis } from '../../../../shared/models';

@Component({
  selector: 'app-ai-research-dialog',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatChipsModule,
    SafePipe
  ],
  templateUrl: './ai-research-dialog.component.html',
  styleUrl: './ai-research-dialog.component.scss'
})
export class AIResearchDialogComponent {
  private readonly aiService = inject(AIService);
  private readonly dialogRef = inject(MatDialogRef<AIResearchDialogComponent>);
  
  analysisTypes = [
    { value: StockAnalysisType.QuickOverview, label: 'Quick Overview' },
    { value: StockAnalysisType.Fundamental, label: 'Fundamental Analysis' },
    { value: StockAnalysisType.Technical, label: 'Technical Analysis' },
    { value: StockAnalysisType.Sentiment, label: 'Sentiment Analysis' },
    { value: StockAnalysisType.Valuation, label: 'Valuation' },
    { value: StockAnalysisType.RiskAssessment, label: 'Risk Assessment' },
    { value: StockAnalysisType.Comprehensive, label: 'Comprehensive Analysis' }
  ];

  selectedAnalysisType: StockAnalysisType = StockAnalysisType.QuickOverview;
  loading = false;
  analysis: StockAnalysis | null = null;
  error: string | null = null;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { symbol: string; companyName: string }) {}

  analyzeStock(): void {
    this.loading = true;
    this.error = null;
    this.analysis = null;

    this.aiService.createStockAnalysis({
      symbol: this.data.symbol,
      companyName: this.data.companyName,
      analysisType: this.selectedAnalysisType,
      useCache: true
    }).subscribe({
      next: (result) => {
        this.analysis = result;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to analyze stock. Please ensure you have configured an AI provider.';
        this.loading = false;
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 2: return 'primary'; // Completed
      case 3: return 'warn'; // Failed
      case 4: return 'accent'; // Cached
      default: return '';
    }
  }
}
