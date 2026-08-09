export type ApplicationStatus =
  | 'Draft'
  | 'Applied'
  | 'Viewed'
  | 'HRInterview'
  | 'TechnicalInterview'
  | 'Offer'
  | 'Rejected'
  | 'NoResponse';

export interface ApplicationDto {
  id: string;
  companyId: string;
  companyName: string;
  vacancyId?: string | null;
  vacancyTitle?: string | null;
  resumeRevisionId: string;
  status: ApplicationStatus;
  jobSource: string;
  notes?: string | null;
  appliedAt?: string | null;
  createdAt: string;
  updatedAt: string;
  allowedTransitions: ApplicationStatus[];
}

export interface SubmitApplicationDto {
  companyId: string;
  resumeRevisionId: string;
  vacancyId?: string | null;
  jobSource: string;
  notes?: string | null;
  submitImmediately: boolean;
}

export interface ChangeApplicationStatusDto {
  newStatus: ApplicationStatus;
  notes?: string | null;
}
