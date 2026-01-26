import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AIModelConfiguration,
  CreateAIModelConfiguration,
  StockAnalysis,
  CreateStockAnalysisRequest,
  StockResearchRequest,
  StockResearchResponse
} from '../../models';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AIService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api`;

  // AI Model Configuration endpoints
  getAIConfigurations(): Observable<AIModelConfiguration[]> {
    return this.http.get<AIModelConfiguration[]>(`${this.apiUrl}/aimodelconfigurations`);
  }

  getAIConfiguration(id: string): Observable<AIModelConfiguration> {
    return this.http.get<AIModelConfiguration>(`${this.apiUrl}/aimodelconfigurations/${id}`);
  }

  getDefaultAIConfiguration(): Observable<AIModelConfiguration> {
    return this.http.get<AIModelConfiguration>(`${this.apiUrl}/aimodelconfigurations/default`);
  }

  createAIConfiguration(config: CreateAIModelConfiguration): Observable<AIModelConfiguration> {
    return this.http.post<AIModelConfiguration>(`${this.apiUrl}/aimodelconfigurations`, config);
  }

  updateAIConfiguration(id: string, config: Partial<CreateAIModelConfiguration>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/aimodelconfigurations/${id}`, config);
  }

  deleteAIConfiguration(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/aimodelconfigurations/${id}`);
  }

  setDefaultAIConfiguration(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/aimodelconfigurations/${id}/set-default`, {});
  }

  // Stock Analysis endpoints
  createStockAnalysis(request: CreateStockAnalysisRequest): Observable<StockAnalysis> {
    return this.http.post<StockAnalysis>(`${this.apiUrl}/stockanalysis`, request);
  }

  getStockAnalysis(id: string): Observable<StockAnalysis> {
    return this.http.get<StockAnalysis>(`${this.apiUrl}/stockanalysis/${id}`);
  }

  getStockAnalysesBySymbol(symbol: string): Observable<StockAnalysis[]> {
    return this.http.get<StockAnalysis[]>(`${this.apiUrl}/stockanalysis/symbol/${symbol}`);
  }

  getUserAnalyses(limit: number = 50): Observable<StockAnalysis[]> {
    return this.http.get<StockAnalysis[]>(`${this.apiUrl}/stockanalysis?limit=${limit}`);
  }

  researchStock(request: StockResearchRequest): Observable<StockResearchResponse> {
    return this.http.post<StockResearchResponse>(`${this.apiUrl}/stockanalysis/research`, request);
  }

  getQuickOverview(symbol: string): Observable<StockAnalysis> {
    return this.http.get<StockAnalysis>(`${this.apiUrl}/stockanalysis/quick-overview/${symbol}`);
  }
}
