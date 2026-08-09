export interface VacancyDto {
  id: string;
  companyId: string;
  title: string;
  description?: string | null;
  url?: string | null;
  postedAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateVacancyDto {
  companyId: string;
  title: string;
  description?: string | null;
  url?: string | null;
  postedAt?: string | null;
}

export interface UpdateVacancyDto {
  title: string;
  description?: string | null;
  url?: string | null;
}
