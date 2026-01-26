# Personal Finance - Angular Web Application

## Overview

This is the Angular 21 frontend for the Personal Finance Management System. The application provides a modern, Material Design-based user interface for managing personal finances including stocks, mutual funds, bank accounts, provident funds, and family member information.

## Features

### Implemented Components

#### Core Layout
- **Header Component**: Navigation bar with user menu and authentication controls
- **Sidebar Component**: Side navigation with links to all major features
- **Footer Component**: Application footer with copyright information
- **Main Layout**: Container that combines header, sidebar, and content area

#### Authentication
- **Login Page**: User authentication with email and password
- **Register Page**: New user registration
- **Auth Service**: JWT-based authentication service (ready for API integration)

#### Dashboard
- **Dashboard**: Overview with wealth statistics and summary cards
  - Total Wealth display
  - Stock Portfolio summary
  - Mutual Funds summary
  - Bank Balance summary
  - Placeholders for charts and recent transactions

#### Portfolio Management
- **Stocks Page**: View and manage stock holdings
  - Table view with stock details
  - Add/Edit/Delete functionality (ready for integration)
  - Profit/Loss calculations
- **Mutual Funds Page**: View and manage mutual fund investments
  - Placeholder for MF holdings list

#### Banking
- **Bank Accounts Page**: Manage bank accounts
  - Placeholder for account list
- **Fixed Deposits Page**: Track fixed deposits
  - Placeholder for FD list

#### Other Modules
- **Provident Funds**: Manage EPF, PPF, VPF, and NPS
- **Family Members**: Manage family member information
- **Reports**: Analytics and reporting (placeholder)

### Shared Services & Components

#### Services
- **AuthService**: Handles authentication, login, register, and token management
- **ApiService**: Base HTTP service for API calls with common methods (GET, POST, PUT, DELETE, PATCH)

#### Components
- **LoadingComponent**: Reusable loading spinner
- **ConfirmDialogComponent**: Reusable confirmation dialog

## Technology Stack

- **Angular**: 21.1.0
- **Angular Material**: 21.1.0
- **RxJS**: 7.8.0
- **TypeScript**: 5.9.2
- **SCSS**: For styling

## Project Structure

```
src/app/
├── core/
│   └── layout/
│       ├── header/           # Navigation header
│       ├── sidebar/          # Side navigation menu
│       ├── footer/           # Application footer
│       └── main-layout/      # Main layout container
├── features/
│   ├── auth/
│   │   ├── login/           # Login component
│   │   └── register/        # Registration component
│   ├── dashboard/           # Dashboard with statistics
│   ├── portfolio/
│   │   ├── stocks/          # Stock holdings management
│   │   └── mutual-funds/    # Mutual fund management
│   ├── banking/
│   │   ├── accounts/        # Bank accounts
│   │   └── fixed-deposits/  # Fixed deposits
│   ├── provident-fund/      # PF management (EPF, PPF, VPF, NPS)
│   ├── family/              # Family members
│   └── reports/             # Reports and analytics
├── shared/
│   ├── components/
│   │   ├── loading/         # Loading spinner
│   │   └── confirm-dialog/  # Confirmation dialog
│   ├── services/
│   │   ├── auth/           # Authentication service
│   │   └── api/            # Base API service
│   └── models/             # TypeScript interfaces/models
├── app.config.ts           # Application configuration
├── app.routes.ts           # Route definitions
└── app.ts                  # Root component
```

## Getting Started

### Prerequisites
- Node.js 20+
- npm 10+

### Installation

```bash
# Navigate to the Web project
cd src/PersonalFinance.Web

# Install dependencies
npm install
```

### Development Server

```bash
# Start the development server
npm start

# Application will be available at http://localhost:4200
```

### Build

```bash
# Build for production
npm run build

# Build output will be in dist/PersonalFinanceWeb
```

### Testing

```bash
# Run tests
npm test
```

## Configuration

### API Endpoint
The API endpoint is currently hardcoded in the services. For production, this should be moved to environment files:

- `ApiService`: `http://localhost:5000/api`
- `AuthService`: `/api`

TODO: Create environment configuration files for different environments.

## Routing

The application uses Angular's standalone routing with the following structure:

- `/auth/login` - Login page
- `/auth/register` - Registration page
- `/dashboard` - Dashboard (default route)
- `/portfolio/stocks` - Stock holdings
- `/portfolio/mutual-funds` - Mutual fund holdings
- `/banking/accounts` - Bank accounts
- `/banking/fixed-deposits` - Fixed deposits
- `/provident-funds` - Provident fund management
- `/family` - Family members
- `/reports` - Reports and analytics

## Styling

The application uses Angular Material with a custom theme defined in `src/styles.scss`:

- Primary color: Indigo
- Accent color: Pink
- Warn color: Red

Global styles and Material theme are configured in the main `styles.scss` file.

## Next Steps / TODO

### High Priority
1. **API Integration**: Connect all services to the backend API
2. **Auth Guard**: Implement route guards for protected pages
3. **HTTP Interceptor**: Add JWT token interceptor for authenticated requests
4. **Error Handling**: Implement global error handling
5. **Loading States**: Add loading indicators for async operations

### Feature Enhancements
1. **Stock Module**:
   - Add/Edit stock dialog
   - Stock detail view with transaction history
   - Real-time price updates
   
2. **Mutual Funds**:
   - Complete MF holdings table
   - Add/Edit MF dialog
   - SIP tracking
   
3. **Banking**:
   - Account list with balances
   - Transaction history
   - FD maturity tracking
   
4. **Reports**:
   - Charts with Chart.js or similar
   - Export functionality (PDF, Excel)
   - Custom date range filtering
   
5. **Dashboard**:
   - Recent transactions list
   - Interactive charts
   - Quick actions

### Code Quality
1. Add unit tests for all components
2. Add integration tests
3. Implement proper error boundaries
4. Add form validation
5. Accessibility improvements

## Architecture Decisions

### Standalone Components
The application uses Angular's modern standalone component architecture, eliminating the need for NgModules. This provides:
- Simpler component structure
- Better tree-shaking
- Improved lazy loading
- More explicit dependencies

### Signal-based State
Components use Angular signals for reactive state management where appropriate.

### Material Design
Angular Material provides a consistent, professional UI with minimal custom styling needed.

## Contributing

When adding new features:

1. Create components in appropriate feature folders
2. Use shared components and services where possible
3. Follow the existing naming conventions
4. Update this README with new features
5. Ensure components are properly typed with TypeScript
6. Use SCSS for styling

## License

This project is part of the Personal Finance Management System.
