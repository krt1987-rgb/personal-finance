import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../api/api.service';

export interface NetWorth {
  totalAssets: number;
  totalLiabilities: number;
  netWorth: number;
  assetBreakdown: { [key: string]: number };
  liabilityBreakdown: { [key: string]: number };
  calculatedAt: Date;
}

export interface WealthSnapshot {
  id: string;
  userId: string;
  snapshotDate: Date;
  totalAssets: number;
  totalLiabilities: number;
  netWorth: number;
  stocksValue: number;
  mutualFundsValue: number;
  bankAccountsBalance: number;
  fixedDepositsValue: number;
  providentFundsBalance: number;
  realEstateValue: number;
  otherAssetsValue: number;
  homeLoanOutstanding: number;
  personalLoanOutstanding: number;
  creditCardOutstanding: number;
  otherLiabilitiesOutstanding: number;
  snapshotType: string;
  notes?: string;
  createdAt: Date;
}

export interface CreateWealthSnapshot {
  notes?: string;
  snapshotType?: string;
}

export interface WealthTimeline {
  snapshots: WealthSnapshot[];
  totalChange?: number;
  percentageChange?: number;
}

export interface WealthComparison {
  fromSnapshot?: WealthSnapshot;
  toSnapshot?: WealthSnapshot;
  netWorthChange: number;
  percentageChange: number;
  assetChanges: { [key: string]: number };
}

@Injectable({
  providedIn: 'root'
})
export class WealthService {
  private readonly apiService = inject(ApiService);

  getCurrentNetWorth(): Observable<NetWorth> {
    return this.apiService.get<NetWorth>('networth/current');
  }

  getAllSnapshots(): Observable<WealthSnapshot[]> {
    return this.apiService.get<WealthSnapshot[]>('wealthsnapshots');
  }

  getLatestSnapshot(): Observable<WealthSnapshot> {
    return this.apiService.get<WealthSnapshot>('wealthsnapshots/latest');
  }

  createSnapshot(data: CreateWealthSnapshot): Observable<WealthSnapshot> {
    return this.apiService.post<WealthSnapshot>('wealthsnapshots', data);
  }

  getTimeline(fromDate?: Date, toDate?: Date): Observable<WealthTimeline> {
    const params: any = {};
    if (fromDate) params.fromDate = fromDate.toISOString();
    if (toDate) params.toDate = toDate.toISOString();
    return this.apiService.get<WealthTimeline>('wealthsnapshots/timeline', params);
  }

  comparePeriods(fromDate: Date, toDate: Date): Observable<WealthComparison> {
    const params = {
      fromDate: fromDate.toISOString(),
      toDate: toDate.toISOString()
    };
    return this.apiService.get<WealthComparison>('wealthsnapshots/compare', params);
  }

  deleteSnapshot(id: string): Observable<void> {
    return this.apiService.delete<void>(`wealthsnapshots/${id}`);
  }
}
