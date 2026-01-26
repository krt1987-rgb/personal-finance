import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api/api.service';
import { ImportResult } from '../../models/import.model';

@Injectable({
  providedIn: 'root'
})
export class ImportService {
  private readonly api = inject(ApiService);

  importStockHoldings(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('StockHoldings/import', file);
  }

  importBankAccounts(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('BankAccounts/import', file);
  }

  importFixedDeposits(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('FixedDeposits/import', file);
  }

  importMutualFundHoldings(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('MutualFundHoldings/import', file);
  }

  importProvidentFunds(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('ProvidentFunds/import', file);
  }

  importFamilyMembers(file: File): Observable<ImportResult> {
    return this.api.uploadFile<ImportResult>('FamilyMembers/import', file);
  }
}
