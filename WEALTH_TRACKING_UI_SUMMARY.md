# Wealth Tracking - UI Components Summary

## Dashboard Component

### Overview
The dashboard now displays real-time wealth data with visualizations.

### Components Added:
1. **Header Section**
   - Page title: "Dashboard"
   - "Create Snapshot" button (Material raised button with camera icon)

2. **Stats Grid (4 cards)**
   - **Total Net Worth** (Green, account_balance icon)
     - Shows aggregated net worth from all assets
   - **Total Assets** (Blue, trending_up icon)
     - Sum of all asset values
   - **Stocks** (Orange, show_chart icon)
     - Current stock portfolio value
   - **Mutual Funds** (Purple, pie_chart icon)
     - Current mutual fund portfolio value

3. **Asset Allocation Card**
   - ApexCharts Pie Chart
   - Shows breakdown by:
     - Stocks
     - MutualFunds
     - BankAccounts
     - FixedDeposits
     - ProvidentFunds
   - Color-coded segments with legend

4. **Asset Breakdown Card**
   - List view showing each asset type with value
   - Format: "Asset Type: ₹X,XXX.XX"
   - Only shows assets with value > 0

### Data Flow:
```
Component OnInit
  ↓
WealthService.getCurrentNetWorth()
  ↓
Update Stats Cards
  ↓
Update Pie Chart
  ↓
Display Breakdown List
```

### Features:
- Loading state ("Loading wealth data...")
- Error handling with error message display
- Currency formatting (Indian Rupee)
- Responsive grid layout
- Real-time data refresh

---

## Reports Component

### Overview
Comprehensive reports page with tabbed interface for different views.

### Tab 1: Net Worth Timeline
**Components:**
1. **Chart Section**
   - ApexCharts Line Chart
   - 3 series:
     - Net Worth (Green line)
     - Total Assets (Blue line)
     - Total Liabilities (Red line)
   - X-axis: Date (datetime)
   - Y-axis: Amount in ₹
   - Interactive toolbar for zoom/pan

2. **Summary Statistics**
   - Total Change: Displayed with color (green=positive, red=negative)
   - Percentage Change: With 2 decimal precision
   - Styled summary boxes

3. **Empty State**
   - Timeline icon (64px)
   - Message: "No wealth snapshots available yet"
   - Hint text: "Create snapshots from the dashboard to track your wealth over time"

### Tab 2: Snapshot History
**Components:**
1. **Snapshot List**
   - Each item shows:
     - Snapshot date (formatted: medium)
     - Net Worth value
     - Total Assets value
   - Styled with borders and spacing

2. **Empty State**
   - Message: "No snapshots available"

### Data Flow:
```
Component OnInit
  ↓
WealthService.getTimeline()
  ↓
Process snapshots data
  ↓
Build chart series
  ↓
Calculate changes
  ↓
Render chart + summary
```

### Features:
- Loading indicator
- Error handling
- Empty state messages
- Material Design tabs
- Color-coded positive/negative changes
- Responsive layout

---

## API Integration

### Endpoints Used:

1. **GET /api/networth/current**
   - Used by: Dashboard
   - Returns: Current net worth with breakdown
   - Auth: Required (JWT)

2. **GET /api/wealthsnapshots/timeline**
   - Used by: Reports
   - Returns: Array of snapshots with change metrics
   - Auth: Required (JWT)
   - Optional params: fromDate, toDate

3. **POST /api/wealthsnapshots**
   - Used by: Dashboard "Create Snapshot" button
   - Body: { snapshotType: "Manual", notes?: string }
   - Auth: Required (JWT)

---

## Styling

### Color Scheme:
- **Green (#4caf50)**: Positive changes, net worth
- **Blue (#2196f3)**: Assets, general info
- **Orange (#ff9800)**: Stocks
- **Purple (#9c27b0)**: Mutual funds
- **Red (#f44336)**: Negative changes, liabilities

### Typography:
- Stats values: 1.5rem, font-weight 600
- Card titles: Material typography
- Currency: Indian Rupee format (₹1,234.56)

### Layout:
- Dashboard grid: 4 columns on desktop, responsive
- Cards: Material Design elevation and spacing
- Charts: 350px height, full width
- Padding: Consistent 1.5rem spacing

---

## User Workflows

### Workflow 1: View Current Wealth
1. User navigates to Dashboard
2. System loads net worth from API
3. Dashboard displays:
   - 4 stat cards with real values
   - Asset allocation pie chart
   - Breakdown list
4. User sees complete financial picture

### Workflow 2: Create Wealth Snapshot
1. User clicks "Create Snapshot" button
2. System calls POST /api/wealthsnapshots
3. Snapshot is created with current wealth data
4. Success message shown
5. Snapshot appears in Reports timeline

### Workflow 3: View Wealth Progression
1. User navigates to Reports
2. System loads timeline data
3. Reports displays:
   - Line chart showing net worth over time
   - Total change and % change
   - List of all snapshots
4. User can see wealth trends and growth

---

## Technical Notes

### Chart Configuration:
- Library: ng-apexcharts (ApexCharts wrapper for Angular)
- Chart types used:
  - Pie: Asset allocation
  - Line: Net worth timeline
- Features:
  - Smooth curves
  - Interactive legends
  - Zoom/pan toolbar
  - Responsive sizing
  - Custom colors

### State Management:
- Component-level state (no global store)
- RxJS observables for async data
- Error handling with try/catch
- Loading states for UX

### Performance:
- Lazy data loading
- No unnecessary re-renders
- Efficient change detection
- Optimized chart updates

---

## Accessibility

- Material Design components (ARIA compliant)
- Semantic HTML structure
- Color contrast ratios met
- Keyboard navigation support
- Screen reader friendly

---

## Mobile Responsiveness

- Grid adapts to screen size:
  - Desktop: 4 columns
  - Tablet: 2 columns
  - Mobile: 1 column
- Charts scale to container width
- Touch-friendly buttons and interactions
- Vertical scrolling on small screens

---

## Error Handling

1. **API Errors**
   - Network failures: Show error message
   - Unauthorized: Handled by auth guard
   - Server errors: Display user-friendly message

2. **Empty States**
   - No data: Show helpful message
   - No snapshots: Guide user to create one

3. **Loading States**
   - Show "Loading..." indicator
   - Disable buttons during loading
   - Prevent duplicate requests

---

## Future Enhancements (Not Implemented)

These could be added in future iterations:
- Date range picker for custom timeline periods
- Export charts as images/PDF
- Comparison view (year-over-year, month-over-month)
- Goal tracking overlays on charts
- Prediction/projection lines
- Benchmark comparisons
- Mobile app version
