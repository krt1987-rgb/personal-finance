# How to Add Import Functionality to Other Modules

This guide explains how to add the import button to the remaining modules (Banking, Fixed Deposits, Mutual Funds, Provident Funds, and Family Members).

## The import functionality has already been implemented in:
- ✅ Backend API endpoints for all modules
- ✅ Import service for all modules  
- ✅ Shared components (FileUploadComponent, ImportDialogComponent)

## What's Left to Do

You only need to add the **Import button** to each module's component. The stocks module serves as a reference implementation.

## Step-by-Step Guide

### 1. Update the Component Template (HTML)

**Example for Bank Accounts** (`banking/accounts/accounts.component.html`):

```html
<!-- Add button group to header -->
<div class="page-header">
  <h1>Bank Accounts</h1>
  <div class="button-group">
    <button mat-raised-button color="accent" (click)="importAccounts()">
      <mat-icon>upload_file</mat-icon>
      Import
    </button>
    <button mat-raised-button color="primary" (click)="addAccount()">
      <mat-icon>add</mat-icon>
      Add Account
    </button>
  </div>
</div>
```

### 2. Update the Component TypeScript

**Example for Bank Accounts** (`banking/accounts/accounts.component.ts`):

```typescript
// Add import at the top
import { ImportDialogComponent } from '../../../shared/components/import-dialog/import-dialog.component';

// Add method to the component class
importAccounts(): void {
  const dialogRef = this.dialog.open(ImportDialogComponent, {
    width: '600px',
    data: {
      moduleType: 'bankAccounts',  // Use the correct module type
      title: 'Import Bank Accounts'
    }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result && result.successCount > 0) {
      // Reload data after successful import
      this.snackBar.open('Import completed. Refresh the page to see imported data.', 'Close', { duration: 5000 });
    }
  });
}
```

### 3. Update the Component Styles (SCSS)

Add button group styling:

```scss
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  
  .button-group {
    display: flex;
    gap: 0.5rem;
  }
}
```

## Module-Specific Configuration

### Banking - Bank Accounts
```typescript
moduleType: 'bankAccounts',
title: 'Import Bank Accounts'
```

### Banking - Fixed Deposits
```typescript
moduleType: 'fixedDeposits',
title: 'Import Fixed Deposits'
```

### Portfolio - Mutual Funds
```typescript
moduleType: 'mutualFunds',
title: 'Import Mutual Fund Holdings'
```

### Provident Funds
```typescript
moduleType: 'providentFunds',
title: 'Import Provident Funds'
```

### Family - Family Members
```typescript
moduleType: 'familyMembers',
title: 'Import Family Members'
```

## Complete Example: Fixed Deposits Component

### fixed-deposits.component.html
```html
<div class="fixed-deposits-page">
  <div class="page-header">
    <h1>Fixed Deposits</h1>
    <div class="button-group">
      <button mat-raised-button color="accent" (click)="importDeposits()">
        <mat-icon>upload_file</mat-icon>
        Import
      </button>
      <button mat-raised-button color="primary" (click)="addDeposit()">
        <mat-icon>add</mat-icon>
        Add Fixed Deposit
      </button>
    </div>
  </div>
  
  <!-- Rest of your existing component template -->
</div>
```

### fixed-deposits.component.ts
```typescript
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ImportDialogComponent } from '../../../shared/components/import-dialog/import-dialog.component';

@Component({
  selector: 'app-fixed-deposits',
  // ... existing component config
})
export class FixedDepositsComponent {
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  
  // ... existing methods (addDeposit, editDeposit, deleteDeposit)
  
  importDeposits(): void {
    const dialogRef = this.dialog.open(ImportDialogComponent, {
      width: '600px',
      data: {
        moduleType: 'fixedDeposits',
        title: 'Import Fixed Deposits'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.successCount > 0) {
        this.snackBar.open('Import completed. Refresh the page to see imported data.', 'Close', { duration: 5000 });
      }
    });
  }
}
```

### fixed-deposits.component.scss
```scss
.fixed-deposits-page {
  .page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
    
    .button-group {
      display: flex;
      gap: 0.5rem;
    }
  }
}
```

## Testing Your Implementation

After adding the import button:

1. Navigate to the module in the browser
2. Click the "Import" button
3. The import dialog should open showing:
   - Module-specific field information
   - File upload button
4. Select a CSV or Excel file from `/sample-import-templates/`
5. Click "Import"
6. Verify the import results are displayed

## Important Notes

- **MatDialog**: Make sure `MatDialog` is injected in your component
- **MatSnackBar**: Make sure `MatSnackBar` is injected for notifications
- **Import Statement**: Add the `ImportDialogComponent` import
- **Module Type**: Use the exact module type as defined in `ImportDialogData` interface
- **Sample Files**: Users can download sample files from `/sample-import-templates/` directory

## Troubleshooting

### Import button not showing
- Check if the button group HTML is added correctly
- Verify the styles are applied

### Dialog not opening
- Ensure `ImportDialogComponent` is imported
- Check if `MatDialog` is injected

### Import fails
- Verify the backend API endpoint is working
- Check the moduleType matches the ImportDialogData type
- Ensure the CSV/Excel file format matches the sample templates

## Summary

By following this guide, you can quickly add import functionality to all remaining modules. The pattern is consistent across all modules, making it easy to replicate. Each module only needs:

1. Import button in the template
2. Import method in the TypeScript file  
3. Button group styling in the SCSS file

All the complex logic (file parsing, validation, API calls, error handling) is already implemented in the shared components and services!
