export interface User {
  id: string;
  name: string;
  initials: string;
  color: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  error?: string;
}
