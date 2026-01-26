import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';

interface MenuItem {
  icon: string;
  label: string;
  route: string;
  children?: MenuItem[];
}

@Component({
  selector: 'app-sidebar',
  imports: [
    CommonModule,
    RouterModule,
    MatListModule,
    MatIconModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  menuItems: MenuItem[] = [
    {
      icon: 'dashboard',
      label: 'Dashboard',
      route: '/dashboard'
    },
    {
      icon: 'show_chart',
      label: 'Portfolio',
      route: '/portfolio',
      children: [
        { icon: 'trending_up', label: 'Stocks', route: '/portfolio/stocks' },
        { icon: 'account_balance', label: 'Mutual Funds', route: '/portfolio/mutual-funds' }
      ]
    },
    {
      icon: 'account_balance_wallet',
      label: 'Banking',
      route: '/banking',
      children: [
        { icon: 'account_balance', label: 'Bank Accounts', route: '/banking/accounts' },
        { icon: 'savings', label: 'Fixed Deposits', route: '/banking/fixed-deposits' }
      ]
    },
    {
      icon: 'money',
      label: 'Provident Funds',
      route: '/provident-funds'
    },
    {
      icon: 'people',
      label: 'Family',
      route: '/family'
    },
    {
      icon: 'assessment',
      label: 'Reports',
      route: '/reports'
    }
  ];
}
