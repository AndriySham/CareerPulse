export interface RecruiterDto {
  id: string;
  companyId: string;
  name: string;
  phone?: string | null;
  email?: string | null;
  linkedInUrl?: string | null;
  telegramUrl?: string | null;
  notes?: string | null;
}

export interface CreateRecruiterDto {
  name: string;
  phone?: string | null;
  email?: string | null;
  linkedInUrl?: string | null;
  telegramUrl?: string | null;
  notes?: string | null;
}

export interface UpdateRecruiterDto {
  name: string;
  phone?: string | null;
  email?: string | null;
  linkedInUrl?: string | null;
  telegramUrl?: string | null;
  notes?: string | null;
}
