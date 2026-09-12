export type WorkMode = 'Remote' | 'Office' | 'Hybride';

export type EmploymentType = 'FullTime' | 'PartTime' | 'Internship';

export type SalaryCurrency = 'USD' | 'EUR' | 'UAH' | 'PLN';

export const WORK_MODE_LABELS: Record<WorkMode, string> = {
  Remote: 'Remote',
  Office: 'Office',
  Hybride: 'Hybrid',
};

export const EMPLOYMENT_TYPE_LABELS: Record<EmploymentType, string> = {
  FullTime: 'Full-time',
  PartTime: 'Part-time',
  Internship: 'Internship',
};

export const CURRENCY_SYMBOLS: Record<SalaryCurrency, string> = {
  USD: '$',
  EUR: '€',
  UAH: '₴',
  PLN: 'zł',
};

export function formatSalary(
  min?: number | null,
  max?: number | null,
  currency?: SalaryCurrency | null
): string | null {
  if (min == null && max == null) return null;

  const symbol = currency ? CURRENCY_SYMBOLS[currency] ?? currency : '';
  const formatNum = (num: number) => num.toLocaleString();

  if (min != null && max != null) {
    if (min === max) {
      return `${symbol}${formatNum(min)}`;
    }
    return `${symbol}${formatNum(min)} – ${symbol}${formatNum(max)}`;
  }

  if (min != null) {
    return `From ${symbol}${formatNum(min)}`;
  }

  if (max != null) {
    return `Up to ${symbol}${formatNum(max)}`;
  }

  return null;
}

export function formatWorkMode(mode?: WorkMode | null): string | null {
  if (!mode) return null;
  return WORK_MODE_LABELS[mode] ?? mode;
}

export function formatEmploymentType(type?: EmploymentType | null): string | null {
  if (!type) return null;
  return EMPLOYMENT_TYPE_LABELS[type] ?? type;
}

export interface VacancyDto {
  id: string;
  companyId: string;
  title: string;
  description?: string | null;
  responsibilities?: string | null;
  requirements?: string | null;
  niceToHave?: string | null;
  benefits?: string | null;
  url?: string | null;
  location?: string | null;
  workMode?: WorkMode | null;
  employmentType?: EmploymentType | null;
  salaryMin?: number | null;
  salaryMax?: number | null;
  salaryCurrency?: SalaryCurrency | null;
  postedAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateVacancyDto {
  companyId: string;
  title: string;
  description?: string | null;
  responsibilities?: string | null;
  requirements?: string | null;
  niceToHave?: string | null;
  benefits?: string | null;
  url?: string | null;
  location?: string | null;
  workMode?: WorkMode | null;
  employmentType?: EmploymentType | null;
  salaryMin?: number | null;
  salaryMax?: number | null;
  salaryCurrency?: SalaryCurrency | null;
  postedAt?: string | null;
}

export interface UpdateVacancyDto {
  title: string;
  description?: string | null;
  responsibilities?: string | null;
  requirements?: string | null;
  niceToHave?: string | null;
  benefits?: string | null;
  url?: string | null;
  location?: string | null;
  workMode?: WorkMode | null;
  employmentType?: EmploymentType | null;
  salaryMin?: number | null;
  salaryMax?: number | null;
  salaryCurrency?: SalaryCurrency | null;
}
