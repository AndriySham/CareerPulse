export interface CompanyDto {
  id: string;
  name: string;
  website?: string | null;
  industry?: string | null;
  notes?: string | null;
  isArchived: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateCompanyDto {
  name: string;
  website?: string | null;
  industry?: string | null;
}

export interface UpdateCompanyDto {
  name: string;
  website?: string | null;
  industry?: string | null;
  notes?: string | null;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
