export interface User {
  id: string;
  name: string;
  email: string;
}

export interface Stock {
  id: string;
  symbol: string;
  name: string;
  quantity: number;
  avgPrice: number;
  currentPrice: number;
  value: number;
  profitLoss: number;
}

export interface MutualFund {
  id: string;
  fundName: string;
  folioNumber: string;
  units: number;
  nav: number;
  investedValue: number;
  currentValue: number;
  returns: number;
}

export interface BankAccount {
  id: string;
  bankName: string;
  accountNumber: string;
  accountType: string;
  balance: number;
}

export interface FixedDeposit {
  id: string;
  bankName: string;
  accountNumber: string;
  principalAmount: number;
  interestRate: number;
  maturityDate: Date;
  maturityAmount: number;
}

export interface ProvidentFund {
  id: string;
  type: 'EPF' | 'PPF' | 'VPF' | 'NPS';
  accountNumber: string;
  balance: number;
  employeeContribution: number;
  employerContribution: number;
}

export interface FamilyMember {
  id: string;
  name: string;
  relationship: string;
  dateOfBirth: Date;
  occupation: string;
}

// AI Models
export * from './ai.model';
