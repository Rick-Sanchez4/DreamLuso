export interface Result<T> {
  value?: T;
  isSuccess: boolean;
  error?: Error;
}

export interface Error {
  code: string;
  description: string;
}

export interface Success {
  message?: string;
}

