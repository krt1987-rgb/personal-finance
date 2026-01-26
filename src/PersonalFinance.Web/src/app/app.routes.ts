import { Routes } from '@angular/router';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { StocksComponent } from './features/portfolio/stocks/stocks.component';
import { MutualFundsComponent } from './features/portfolio/mutual-funds/mutual-funds.component';
import { BankAccountsComponent } from './features/banking/accounts/bank-accounts.component';
import { FixedDepositsComponent } from './features/banking/fixed-deposits/fixed-deposits.component';
import { ProvidentFundComponent } from './features/provident-fund/provident-fund.component';
import { FamilyComponent } from './features/family/family.component';
import { ReportsComponent } from './features/reports/reports.component';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { 
        path: 'portfolio',
        children: [
          { path: 'stocks', component: StocksComponent },
          { path: 'mutual-funds', component: MutualFundsComponent },
          { path: '', redirectTo: 'stocks', pathMatch: 'full' }
        ]
      },
      {
        path: 'banking',
        children: [
          { path: 'accounts', component: BankAccountsComponent },
          { path: 'fixed-deposits', component: FixedDepositsComponent },
          { path: '', redirectTo: 'accounts', pathMatch: 'full' }
        ]
      },
      { path: 'provident-funds', component: ProvidentFundComponent },
      { path: 'family', component: FamilyComponent },
      { path: 'reports', component: ReportsComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
