export interface ImportResult {
  totalRows: number;
  successCount: number;
  failureCount: number;
  errors: ImportError[];
  isSuccess: boolean;
}

export interface ImportError {
  rowNumber: number;
  error: string;
  rowData: { [key: string]: string };
}
